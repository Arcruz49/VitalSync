using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;
using System.Text.Json;
using VitalSync.Contracts;
using VitalSyncAI.Models;
namespace VitalSyncAI.Services;

public class AnthropicService(IConfiguration config)
{
    private readonly string _model = config["Anthropic:InsightModel"] ?? AnthropicModels.Claude45Haiku;
    private readonly AnthropicClient _client = new(config["Anthropic:ApiKey"]);

    public async Task<InsightGeneratedEvent> AnalyzeAsync(InsightRequestedEvent request)
    {
        var jsonSchema = """
            {
                "insights": ["insight 1", "insight 2"],
                "tips": ["dica 1", "dica 2"],
                "overallAssessment": "avaliação geral",
                "disclaimer": "Este conteúdo é informativo e não substitui avaliação médica profissional."
            }
            """;

        var prompt = $"""
            Você é um assistente de saúde personalizado. Analise os dados abaixo e responda
            EXCLUSIVAMENTE com um JSON válido, sem texto adicional, sem markdown.

            O JSON deve seguir exatamente esta estrutura:
            {jsonSchema}

            ## Registro atual
            - Métrica: {request.MetricTypeName}
            - Valor: {request.Value} {request.Unit}
            - Registrado em: {request.MeasuredAt:dd/MM/yyyy HH:mm}

            ## Perfil do usuário
            - Objetivo: {request.Goal}
            - Nível de atividade: {request.ActivityLevel}
            - IMC: {request.BMI}
            - TDEE: {request.TDEE} kcal
            - Meta calórica: {request.CalorieGoal} kcal
            - Condições: {string.Join(", ", request.Conditions.DefaultIfEmpty("Nenhuma"))}
            - Medicamentos: {string.Join(", ", request.Medications.DefaultIfEmpty("Nenhum"))}

            ## Métricas recentes (últimos 30 dias)
            {BuildMetricsSection(request.RecentMetrics)}
            """;

        var response = await _client.Messages.GetClaudeMessageAsync(
            new MessageParameters
            {
                Model = _model,
                MaxTokens = 1024,
                Messages =
                [
                    new Message
                    {
                        Role = RoleType.User,
                        Content = new List<ContentBase>
                        {
                            new TextContent { Text = prompt }
                        }
                    }
                ]
            }
        );

        var raw = ((TextContent)response.Content[0]).Text.Trim();
        var json = raw.StartsWith("```")
            ? raw[(raw.IndexOf('\n') + 1)..raw.LastIndexOf("```")].Trim()
            : raw;

        var result = JsonSerializer.Deserialize<InsightResult>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new InsightResult();

        return new InsightGeneratedEvent(
            request.HealthRecordId,
            request.UserId,
            result.Insights,
            result.Tips,
            result.OverallAssessment,
            result.Disclaimer
        );
    }

    private static string BuildMetricsSection(List<MetricSummary> metrics)
    {
        if (metrics.Count == 0) return "Nenhuma métrica registrada.";

        return string.Join("\n", metrics.Select(m =>
            $"- {m.MetricName}: último valor {m.LastValue} {m.Unit} (média: {m.Average:F1})"));
    }
}

