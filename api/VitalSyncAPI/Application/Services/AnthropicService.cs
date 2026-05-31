using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Domain.Interfaces;

namespace VitalSyncAPI.Application.Services;

public class AnthropicService(IConfiguration config) : IAIAnalysisService
{
    private readonly string _insightModel = config["Anthropic:InsightModel"] ?? AnthropicModels.Claude45Haiku;
    private readonly AnthropicClient _client = new(config["Anthropic:ApiKey"]);

    public async Task<string> AnalyzeAsync(UserProfile profile, BodyMetrics metrics, List<HealthRecord> records)
    {
        var response = await _client.Messages.GetClaudeMessageAsync(
            new MessageParameters
            {
                Model = _insightModel,
                MaxTokens = 1024,
                Messages =
                [
                    new Message
                    {
                        Role = RoleType.User,
                        Content = new List<ContentBase>
                        {
                            new TextContent
                            {
                                Text = BuildPrompt(profile, metrics, records)
                            }
                        }
                    }
                ]
            }
        );

        return ((TextContent)response.Content[0]).Text;
    }

    private static string BuildPrompt(UserProfile profile, BodyMetrics metrics, List<HealthRecord> records)
    {
        return $"""
            Você é um assistente de saúde personalizado. Analise os dados abaixo e gere até 3 insights 
            específicos, acionáveis e personalizados. Seja direto e prático.

            IMPORTANTE: Sempre inclua ao final: "Este conteúdo é informativo e não substitui 
            avaliação médica profissional."

            ## Perfil do usuário
            - Objetivo: {profile.Goal}
            - Nível de atividade: {profile.ActivityLevel}
            - IMC: {metrics.BMI}
            - TDEE: {metrics.TDEE} kcal
            - Meta calórica diária: {metrics.CalorieGoal} kcal

            ## Métricas recentes (últimos 30 dias)
            {BuildMetricsSection(records)}

            """;
    }

    private static string BuildMetricsSection(List<HealthRecord> records)
    {
        if (records.Count == 0) return "Nenhuma métrica registrada.";

        return string.Join("\n", records
            .GroupBy(r => r.MetricType.Name)
            .Select(g =>
                $"- {g.Key}: último valor {g.OrderByDescending(r => r.MeasuredAt).First().Value} " +
                $"{g.First().MetricType.Unit} (média: {g.Average(r => r.Value):F1})")
        );
    }
}