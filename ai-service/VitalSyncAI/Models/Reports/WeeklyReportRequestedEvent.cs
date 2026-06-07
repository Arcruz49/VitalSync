namespace VitalSync.Contracts;

public record WeeklyReportRequestedEvent(
    Guid ReportId,
    Guid UserId,
    DateTime WeekStart,
    DateTime WeekEnd,
    string Goal,
    string ActivityLevel,
    decimal BMI,
    decimal CalorieGoal,
    List<string> Conditions,
    List<string> Medications,
    List<WeeklyMetricData> WeekMetrics,
    decimal TotalCalories,
    decimal TotalProtein,
    decimal TotalCarbs,
    decimal TotalFat,
    int MealsLogged,
    List<WeeklyMetricData> PreviousWeekMetrics
);