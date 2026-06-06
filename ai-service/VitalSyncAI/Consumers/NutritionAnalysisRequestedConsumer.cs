using MassTransit;
using VitalSync.Contracts;
using VitalSyncAI.Services;

namespace VitalSyncAI.Consumers;
public class NutritionAnalysisRequestedConsumer(
    AnthropicService anthropicService,
    IPublishEndpoint publishEndpoint,
    ILogger<NutritionAnalysisRequestedConsumer> logger
) : IConsumer<NutritionAnalysisRequestedEvent>
{
    public async Task Consume(ConsumeContext<NutritionAnalysisRequestedEvent> context)
    {
        var request = context.Message;

        logger.LogInformation(
            "Processando análise nutricional para NutritionRecord {Id}",
            request.NutritionRecordId);

        try
        {
            var result = await anthropicService.AnalyzeFoodImageAsync(request.ImageBase64);

            await publishEndpoint.Publish(new NutritionAnalysisCompletedEvent(
                request.NutritionRecordId,
                request.UserId,
                result.FoodDescription,
                result.CaloriesKcal,
                result.ProteinG,
                result.CarbsG,
                result.FatG,
                result.Confidence,
                Success: true,
                ErrorMessage: null
            ));
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Erro ao analisar imagem para NutritionRecord {Id}",
                request.NutritionRecordId);

            await publishEndpoint.Publish(new NutritionAnalysisCompletedEvent(
                request.NutritionRecordId,
                request.UserId,
                string.Empty, 0, 0, 0, 0, 0,
                Success: false,
                ErrorMessage: ex.Message
            ));
        }
    }
}