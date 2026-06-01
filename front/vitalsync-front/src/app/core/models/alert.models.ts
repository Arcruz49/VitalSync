export interface AlertResponse {
  id: string;
  healthRecordId: string;
  metricTypeId: number;
  metricTypeName?: string;
  severity: string;
  message: string;
  triggeredAt: string;
  acknowledgedAt: string | null;
}
