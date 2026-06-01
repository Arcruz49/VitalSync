export interface AIAnalysisResult {
  insights: string[];
  tips: string[];
  overallAssessment: string;
  disclaimer: string;
}

export interface HealthRecordResponse {
  id: string;
  metricTypeId: number;
  metricTypeName: string;
  unit: string;
  value: number;
  measuredAt: string;
  notes: string | null;
  createdAt: string;
  insight?: AIAnalysisResult | null;
}

export interface HealthRecordRequest {
  metricTypeId: number;
  value: number;
  measuredAt: string;
  notes: string | null;
}

export interface SearchHealthRecordRequest {
  metricTypeId?: number;
  from?: string;
  to?: string;
}
