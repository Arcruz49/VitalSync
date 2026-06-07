using MassTransit;
using System.Text.Json;
using VitalSync.Contracts;
using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Enums;
using VitalSyncAPI.Domain.Interfaces;

namespace VitalSyncAPI.Consumers;

public class WeeklyReportGeneratedConsumer(
    IWeeklyReportRepository weeklyReportRepository,
    IUnitOfWork unitOfWork,
    ILogger<WeeklyReportGeneratedConsumer> logger
) : IConsumer<WeeklyReportGeneratedEvent>
{
    public async Task Consume(ConsumeContext<WeeklyReportGeneratedEvent> context)
    {
        var message = context.Message;

        var report = await weeklyReportRepository.GetById(message.ReportId);
        if (report is null)
        {
            logger.LogWarning("WeeklyReport {Id} não encontrado.", message.ReportId);
            return;
        }

        if (message.Success)
        {
            report.Summary          = message.Summary;
            report.MetricsAnalysis  = message.MetricsAnalysis;
            report.Patterns         = message.Patterns;
            report.Recommendations  = message.Recommendations;
            report.NutritionSummary = message.NutritionSummary;
            report.Disclaimer       = message.Disclaimer;
            report.Status           = ReportStatus.Completed;
        }
        else
        {
            report.Status = ReportStatus.Failed;
            logger.LogError("Falha ao gerar relatório {Id}: {Error}",
                message.ReportId, message.ErrorMessage);
        }

        weeklyReportRepository.UpdateWeeklyReport(report);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation(
            "WeeklyReport {Id} atualizado com status {Status}",
            message.ReportId, report.Status);
    }
}