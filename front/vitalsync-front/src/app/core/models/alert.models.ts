export interface AlertResponse {
  id: string;
  healthRecordId: string;
  metricTypeId: number;
  metricTypeName: string;
  unit: string;
  value: number;
  severity: string;
  message: string;
  triggeredAt: string;
  acknowledgedAt: string | null;
}
