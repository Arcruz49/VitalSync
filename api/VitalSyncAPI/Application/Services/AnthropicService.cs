using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;
using System.Text.Json;
using VitalSync.Contracts;
using VitalSyncAI.Models;
using VitalSyncAPI.Application.Models;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Domain.Interfaces;

namespace VitalSyncAPI.Application.Services;

public class AnthropicService(IConfiguration config) : IAIAnalysisService
{
    private readonly string _insightModel = config["Anthropic:InsightModel"] ?? AnthropicModels.Claude45Haiku;
    private readonly AnthropicClient _client = new(config["Anthropic:ApiKey"]);

    public async Task<AIAnalysisResult> AnalyzeAsync(UserProfile profile, BodyMetrics metrics, List<HealthRecord> records)
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

        var raw = ((TextContent)response.Content[0]).Text.Trim();

        // o retorno da ia estava vindo com markdown então precisa validar se possui ``` no json
        if (raw.StartsWith("```"))
        {
            var firstNewline = raw.IndexOf('\n');
            var lastFence = raw.LastIndexOf("```");
            if (firstNewline >= 0 && lastFence > firstNewline)
                raw = raw[(firstNewline + 1)..lastFence].Trim();
        }

        return JsonSerializer.Deserialize<AIAnalysisResult>(raw, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new AIAnalysisResult();
    }

    private static string BuildPrompt(UserProfile profile, BodyMetrics metrics, List<HealthRecord> records)
    {
        var jsonSchema = """
            {
                "insights": ["insight 1", "insight 2", "insight 3"],
                "tips": ["dica 1", "dica 2"],
                "overallAssessment": "avaliação geral em uma frase",
                "disclaimer": "Este conteúdo é informativo e não substitui avaliação médica profissional."
            }
            """;

        return $"""
            Você é um assistente de saúde personalizado. Analise os dados abaixo e responda 
            EXCLUSIVAMENTE com um JSON válido, sem texto adicional, sem markdown, sem ```json.

            O JSON deve seguir exatamente esta estrutura:
            {jsonSchema}

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

    public async Task<NutritionAnalysisResult> AnalyzeFoodImageAsync(string imageBase64)
    {
        var base64Data = imageBase64.Contains(",") 
            ? imageBase64.Split(",")[1] 
            : imageBase64;

        var response = await _client.Messages.GetClaudeMessageAsync(
            new MessageParameters
            {
                Model = AnthropicModels.Claude46Sonnet,
                MaxTokens = 1024,
                Messages =
                [
                    new Message
                    {
                        Role = RoleType.User,
                        Content = new List<ContentBase>
                        {
                            new ImageContent
                            {
                                Source = new ImageSource
                                {
                                    Type = SourceType.base64,
                                    MediaType = "image/jpeg",
                                    Data = base64Data
                                }
                            },
                            new TextContent
                            {
                                Text = BuildNutritionPrompt()
                            }
                        }
                    }
                ]
            }
        );

        var json = ((TextContent)response.Content[0]).Text;
        return JsonSerializer.Deserialize<NutritionAnalysisResult>(json, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? new NutritionAnalysisResult();
    }

    private static string BuildNutritionPrompt()
    {
        return """
            Analise a imagem deste prato/alimento e estime os macronutrientes.
            Responda EXCLUSIVAMENTE com JSON válido, sem markdown, sem texto adicional.
            
            {
                "foodDescription": "descrição do que foi identificado",
                "caloriesKcal": 0,
                "proteinG": 0,
                "carbsG": 0,
                "fatG": 0,
                "confidence": 0.0,
                "disclaimer": "Valores estimados. Podem variar conforme preparo e porção."
            }
            
            confidence deve ser entre 0 e 1.
            Se não conseguir identificar alimento na imagem, retorne confidence: 0 e zeros nos macros.
            """;
    }

    public async Task<WeeklyReportResult> GenerateWeeklyReportAsync(WeeklyReportRequestedEvent request)
    {
        var jsonSchema = """
            {
                "summary": "resumo geral da semana em 2-3 frases",
                "metricsAnalysis": [
                    {
                        "metricName": "nome da métrica",
                        "unit": "unidade",
                        "average": 0,
                        "min": 0,
                        "max": 0,
                        "trend": "improving|stable|worsening",
                        "analysis": "análise específica desta métrica"
                    }
                ],
                "patterns": ["padrão identificado 1", "padrão identificado 2"],
                "recommendations": ["recomendação prática 1", "recomendação prática 2", "recomendação prática 3"],
                "nutritionSummary": "resumo nutricional da semana ou null se não houver dados",
                "disclaimer": "Este relatório é informativo e não substitui avaliação médica profissional."
            }
            """;

        var metricsSection = BuildWeeklyMetricsSection(request.WeekMetrics, request.PreviousWeekMetrics);
        var nutritionSection = request.MealsLogged > 0
            ? $"""
            ## Nutrição da semana
            - Refeições registradas: {request.MealsLogged}
            - Calorias totais: {request.TotalCalories:F0} kcal (meta diária: {request.CalorieGoal} kcal)
            - Proteína total: {request.TotalProtein:F0}g
            - Carboidratos total: {request.TotalCarbs:F0}g
            - Gordura total: {request.TotalFat:F0}g
            """
            : "## Nutrição\nNenhuma refeição registrada esta semana.";

        var prompt = $"""
            Você é um assistente de saúde especializado em análise de tendências semanais.
            Analise os dados abaixo e gere um relatório semanal completo e personalizado.
            Responda EXCLUSIVAMENTE com JSON válido, sem markdown, sem texto adicional.

            O JSON deve seguir exatamente esta estrutura:
            {jsonSchema}

            ## Perfil do usuário
            - Objetivo: {request.Goal}
            - Nível de atividade: {request.ActivityLevel}
            - IMC: {request.BMI}
            - Meta calórica diária: {request.CalorieGoal} kcal
            - Condições de saúde: {string.Join(", ", request.Conditions.DefaultIfEmpty("Nenhuma"))}
            - Medicamentos: {string.Join(", ", request.Medications.DefaultIfEmpty("Nenhum"))}

            ## Período analisado
            - Semana: {request.WeekStart:dd/MM/yyyy} a {request.WeekEnd:dd/MM/yyyy}

            {metricsSection}

            {nutritionSection}

            ## Instruções
            - Seja específico e cite valores reais dos dados
            - Compare com a semana anterior quando houver dados
            - Identifique padrões concretos (ex: pressão alta às finais de semana)
            - Recomendações devem ser práticas e acionáveis
            - Tom encorajador mas honesto
            - Se poucos dados foram registrados, mencione a importância de registrar mais
            """;

        var response = await _client.Messages.GetClaudeMessageAsync(
            new MessageParameters
            {
                Model = config["Anthropic:ReportModel"] ?? AnthropicModels.Claude46Sonnet,
                MaxTokens = 2048,
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

        var result = JsonSerializer.Deserialize<WeeklyReportRawResult>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new WeeklyReportRawResult();

        return new WeeklyReportResult
        {
            Summary          = result.Summary,
            MetricsAnalysis  = JsonSerializer.Serialize(result.MetricsAnalysis),
            Patterns         = JsonSerializer.Serialize(result.Patterns),
            Recommendations  = JsonSerializer.Serialize(result.Recommendations),
            NutritionSummary = result.NutritionSummary,
            Disclaimer       = result.Disclaimer
        };
    }

    private static string BuildWeeklyMetricsSection(
        List<WeeklyMetricData> weekMetrics,
        List<WeeklyMetricData> prevMetrics)
    {
        if (weekMetrics.Count == 0) return "## Métricas\nNenhuma métrica registrada esta semana.";

        var lines = weekMetrics.Select(m =>
        {
            var prev = prevMetrics.FirstOrDefault(p => p.MetricName == m.MetricName);
            var prevInfo = prev is not null
                ? $" (semana anterior: média {prev.Average} {m.Unit})"
                : string.Empty;

            return $"- {m.MetricName}: média {m.Average} {m.Unit}, " +
                $"min {m.Min}, max {m.Max}, " +
                $"tendência: {m.Trend}, {m.RecordCount} registros{prevInfo}";
        });

        return $"## Métricas da semana\n{string.Join("\n", lines)}";
    }
}