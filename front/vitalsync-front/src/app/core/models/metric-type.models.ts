export interface MetricType {
  id: number;
  name: string;
  unit: string;
  minNormal: number | null;
  maxNormal: number | null;
  icon: string;
  sortOrder: number;
}
