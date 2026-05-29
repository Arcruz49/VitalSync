namespace VitalSyncAPI.Domain.Enums;

public enum ActivityLevel
{
    Sedentary = 1,        // fator 1.2
    LightlyActive = 2,    // fator 1.375
    ModeratelyActive = 3, // fator 1.55
    VeryActive = 4,       // fator 1.725
    ExtremelyActive = 5   // fator 1.9
}