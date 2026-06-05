using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;
using System.Text.Json;
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
}