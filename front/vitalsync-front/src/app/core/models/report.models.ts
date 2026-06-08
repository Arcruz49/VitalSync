export interface WeeklyReportResponse {
  id: string;
  weekStart: string;
  weekEnd: string;
  summary: string;
  patterns: string[];
  recommendations: string[];
  nutritionSummary: string | null;
  disclaimer: string;
  status: 'Pending' | 'Completed' | 'Failed';
  createdAt: string;
  metricsAnalysis: WeeklyMetricAnalysisResponse[] | null;
}

export interface WeeklyMetricAnalysisResponse {
  metricName: string;
  unit: string;
  average: number;
  min: number;
  max: number;
  trend: 'improving' | 'stable' | 'worsening';
  analysis: string;
}

export interface WeeklyReportRequest {
  weekStart?: string;
}
