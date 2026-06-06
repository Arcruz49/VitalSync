using MassTransit;
using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Domain.Interfaces;
using VitalSync.Contracts;

namespace VitalSyncAPI.Consumers;

public class InsightGeneratedConsumer(
    IAIInsightRepository aiInsightRepository,
    IUnitOfWork unitOfWork,
    ILogger<InsightGeneratedConsumer> logger
) : IConsumer<InsightGeneratedEvent>
{
    public async Task Consume(ConsumeContext<InsightGeneratedEvent> context)
    {
        var message = context.Message;

        logger.LogInformation(
            "Insight recebido para HealthRecord {HealthRecordId}",
            message.HealthRecordId);

        var insight = new AIInsight
        {
            Id = Guid.NewGuid(),
            UserId = message.UserId,
            HealthRecordId = message.HealthRecordId,
            Insights = System.Text.Json.JsonSerializer.Serialize(message.Insights),
            Tips = System.Text.Json.JsonSerializer.Serialize(message.Tips),
            OverallAssessment = message.OverallAssessment,
            Disclaimer = message.Disclaimer,
            CreatedAt = DateTime.UtcNow
        };

        await aiInsightRepository.AddAsync(insight);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation(
            "Insight salvo com sucesso para HealthRecord {HealthRecordId}",
            message.HealthRecordId);
    }
}