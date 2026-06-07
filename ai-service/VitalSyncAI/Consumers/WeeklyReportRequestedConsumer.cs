using MassTransit;
using VitalSync.Contracts;
using VitalSyncAI.Services;

namespace VitalSyncAI.Consumers;

public class WeeklyReportRequestedConsumer(
    AnthropicService anthropicService,
    IPublishEndpoint publishEndpoint,
    ILogger<WeeklyReportRequestedConsumer> logger
) : IConsumer<WeeklyReportRequestedEvent>
{
    public async Task Consume(ConsumeContext<WeeklyReportRequestedEvent> context)
    {
        var request = context.Message;

        logger.LogInformation(
            "Gerando relatório semanal para usuário {UserId} semana {WeekStart}",
            request.UserId, request.WeekStart);

        try
        {
            var result = await anthropicService.GenerateWeeklyReportAsync(request);

            await publishEndpoint.Publish(new WeeklyReportGeneratedEvent(
                request.ReportId,
                request.UserId,
                result.Summary,
                result.MetricsAnalysis,
                result.Patterns,
                result.Recommendations,
                result.NutritionSummary,
                result.Disclaimer,
                Success: true,
                ErrorMessage: null
            ));
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Erro ao gerar relatório semanal para usuário {UserId}",
                request.UserId);

            await publishEndpoint.Publish(new WeeklyReportGeneratedEvent(
                request.ReportId,
                request.UserId,
                string.Empty,
                "[]",
                "[]",
                "[]",
                null,
                string.Empty,
                Success: false,
                ErrorMessage: ex.Message
            ));
        }
    }
}