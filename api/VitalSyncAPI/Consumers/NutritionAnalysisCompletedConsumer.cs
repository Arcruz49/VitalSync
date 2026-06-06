using MassTransit;
using VitalSync.Contracts;
using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Enums;
using VitalSyncAPI.Domain.Interfaces;

namespace VitalSyncAPI.Consumers;

public class NutritionAnalysisCompletedConsumer(
    INutritionRecordRepository nutritionRecordRepository,
    IUnitOfWork unitOfWork,
    ILogger<NutritionAnalysisCompletedConsumer> logger
) : IConsumer<NutritionAnalysisCompletedEvent>
{
    public async Task Consume(ConsumeContext<NutritionAnalysisCompletedEvent> context)
    {
        var message = context.Message;

        var record = await nutritionRecordRepository.GetById(message.NutritionRecordId);
        if (record is null)
        {
            logger.LogWarning(
                "NutritionRecord {Id} não encontrado ao processar análise concluída",
                message.NutritionRecordId);
            return;
        }

        if (message.Success)
        {
            record.FoodDescription = message.FoodDescription;
            record.CaloriesKcal    = message.CaloriesKcal;
            record.ProteinG        = message.ProteinG;
            record.CarbsG          = message.CarbsG;
            record.FatG            = message.FatG;
            record.Confidence      = message.Confidence;
            record.Status          = NutritionStatus.Completed;
        }
        else
        {
            record.FoodDescription = "Não foi possível analisar a imagem.";
            record.Status          = NutritionStatus.Failed;
        }

        nutritionRecordRepository.UpdateNutritionRecord(record);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation(
            "NutritionRecord {Id} atualizado com status {Status}",
            message.NutritionRecordId, record.Status);
    }
}