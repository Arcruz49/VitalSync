export interface UserProfileRequest {
  heightCm: number;
  weightKg: number;
  targetWeightKg?: number;
  waistCircumferenceCm?: number;
  bodyFatPercent?: number;
  activityLevel: number;
  goal: number;
  trainingFrequencyDays: number;
  trainingTypes: string[];
  hoursSeated: number;
  habitualSleepHours: number;
  sleepQuality: number;
}

export interface UserProfileResponse {
  id: string;
  userId: string;
  heightCm: number;
  weightKg: number;
  targetWeightKg: number | null;
  waistCircumferenceCm: number | null;
  bodyFatPercent: number | null;
  activityLevel: string;
  goal: string;
  trainingFrequencyDays: number;
  trainingTypes: string[];
  hoursSeated: string;
  habitualSleepHours: number;
  sleepQuality: number;
  bmi: number;
  idealWeightMinKg: number;
  idealWeightMaxKg: number;
  bmr: number;
  tdee: number;
  calorieGoal: number;
  proteinGoalG: number;
  carbsGoalG: number;
  fatGoalG: number;
  createdAt: string;
  updatedAt: string;
}

export interface HealthCondition {
  id: number;
  name: string;
}

export interface UserConditionRequest {
  conditionId: number;
}

export interface UserConditionResponse {
  conditionId: number;
  conditionName: string;
}

export interface UserMedicationRequest {
  medicationClass: number;
  notes?: string;
}

export interface UserMedicationResponse {
  id: string;
  medicationClass: string;
  notes: string | null;
}

export interface PersonalRangeResponse {
  id: string;
  metricTypeId: number;
  metricTypeName: string;
  unit: string;
  minNormal: number;
  maxNormal: number;
  method: string;
  calculatedAt: string;
}

export const HEALTH_CONDITIONS: HealthCondition[] = [
  { id: 1, name: 'Hipertensão' },
  { id: 2, name: 'Diabetes tipo 1' },
  { id: 3, name: 'Diabetes tipo 2' },
  { id: 4, name: 'Pré-diabetes' },
  { id: 5, name: 'Obesidade' },
  { id: 6, name: 'Doença cardíaca' },
  { id: 7, name: 'Colesterol alto' },
  { id: 8, name: 'Hipotireoidismo' },
  { id: 9, name: 'Hipertireoidismo' },
  { id: 10, name: 'Asma' },
  { id: 11, name: 'DPOC' },
  { id: 12, name: 'Doença renal crônica' },
  { id: 13, name: 'Apneia do sono' },
  { id: 14, name: 'Ansiedade' },
  { id: 15, name: 'Depressão' },
  { id: 16, name: 'Fibromialgia' },
  { id: 17, name: 'Ovário policístico' },
];

export const MEDICATION_CLASSES = [
  { id: 0, name: 'Nenhum', description: 'Não uso medicamentos regularmente' },
  { id: 1, name: 'Beta-bloqueadores', description: 'Reduz frequência cardíaca em repouso' },
  { id: 2, name: 'Anti-hipertensivos', description: 'Controla pressão arterial' },
  { id: 3, name: 'Insulina / Antidiabéticos', description: 'Controla glicemia' },
  { id: 4, name: 'Corticoides', description: 'Pode elevar glicemia e pressão' },
  { id: 5, name: 'Antidepressivos / Ansiolíticos', description: 'Impacta humor e sono' },
  { id: 6, name: 'Hormônios tireoidianos', description: 'Afeta metabolismo e FC' },
  { id: 7, name: 'Diuréticos', description: 'Afeta peso e pressão' },
];

export const ACTIVITY_LEVELS = [
  { id: 1, name: 'Sedentário', description: 'Pouca ou nenhuma atividade' },
  { id: 2, name: 'Leve', description: 'Exercício leve 1–3 dias/semana' },
  { id: 3, name: 'Moderado', description: 'Exercício moderado 3–5 dias/semana' },
  { id: 4, name: 'Ativo', description: 'Exercício intenso 6–7 dias/semana' },
  { id: 5, name: 'Intenso', description: 'Atleta ou trabalho físico pesado' },
];

export const HEALTH_GOALS = [
  { id: 1, name: 'Perda de peso', description: 'Reduzir gordura corporal' },
  { id: 2, name: 'Ganho de massa', description: 'Aumentar músculo' },
  { id: 3, name: 'Manutenção', description: 'Manter peso atual' },
  { id: 4, name: 'Condicionamento', description: 'Melhorar performance' },
  { id: 5, name: 'Condição crônica', description: 'Controlar doença' },
  { id: 6, name: 'Saúde geral', description: 'Melhorar bem-estar' },
];

export const TRAINING_TYPES = [
  'Musculação', 'Corrida', 'Ciclismo', 'Natação', 'HIIT', 'Yoga', 'Pilates', 'Funcional'
];

export const HOURS_SEATED = [
  { id: 1, label: '< 4h' },
  { id: 2, label: '4–6h' },
  { id: 3, label: '6–8h' },
  { id: 4, label: '> 8h' },
];
