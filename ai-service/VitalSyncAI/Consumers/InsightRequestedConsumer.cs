using MassTransit;
using VitalSync.Contracts;
using VitalSyncAI.Services;

namespace VitalSyncAI.Consumers;

public class InsightRequestedConsumer(
    AnthropicService anthropicService,
    IPublishEndpoint publishEndpoint,
    ILogger<InsightRequestedConsumer> logger
) : IConsumer<InsightRequestedEvent>
{
    public async Task Consume(ConsumeContext<InsightRequestedEvent> context)
    {
        var request = context.Message;

        logger.LogInformation(
            "Processando insight para HealthRecord {HealthRecordId} do usuário {UserId}",
            request.HealthRecordId, request.UserId);

        try
        {
            var result = await anthropicService.AnalyzeAsync(request);

            await publishEndpoint.Publish(result);

            logger.LogInformation(
                "Insight gerado com sucesso para HealthRecord {HealthRecordId}",
                request.HealthRecordId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Erro ao gerar insight para HealthRecord {HealthRecordId}",
                request.HealthRecordId);

            throw; // MassTransit
        }
    }
}