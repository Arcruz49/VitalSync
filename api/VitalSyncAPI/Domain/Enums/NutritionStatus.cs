namespace VitalSyncAPI.Domain.Enums;
public enum NutritionStatus
{
    Pending = 1, //aguardando o processo da ia
    Completed = 2, //concluido
    Failed = 3 //erro
}