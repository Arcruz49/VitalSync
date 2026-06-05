export interface NutritionRequest {
  mealType: number;
  notes?: string;
  measuredAt: string;
  imageBase64: string;
}

export interface NutritionResponse {
  id: string;
  mealType: string;
  foodDescription: string;
  caloriesKcal: number;
  proteinG: number;
  carbsG: number;
  fatG: number;
  confidence: number;
  notes?: string;
  measuredAt: string;
  createdAt: string;
}

export interface NutritionSummaryResponse {
  totalCaloriesKcal: number;
  totalProteinG: number;
  totalCarbsG: number;
  totalFatG: number;
  calorieGoal: number;
  proteinGoalG: number;
  carbsGoalG: number;
  fatGoalG: number;
  records: NutritionResponse[];
}

export const MEAL_TYPES = [
  { id: 1, key: 'Breakfast',   label: 'Café da manhã' },
  { id: 2, key: 'Lunch',       label: 'Almoço' },
  { id: 3, key: 'Dinner',      label: 'Jantar' },
  { id: 4, key: 'Snack',       label: 'Lanche' },
  { id: 5, key: 'PreWorkout',  label: 'Pré-treino' },
  { id: 6, key: 'PostWorkout', label: 'Pós-treino' },
];
