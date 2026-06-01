import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  UserProfileRequest, UserProfileResponse,
  UserConditionRequest, UserConditionResponse,
  UserMedicationRequest, UserMedicationResponse,
  PersonalRangeResponse
} from '../models/profile.models';

@Injectable({ providedIn: 'root' })
export class ProfileService {
  constructor(private http: HttpClient) {}

  getProfile() {
    return this.http.get<UserProfileResponse>('/profile', { withCredentials: true });
  }

  saveProfile(request: UserProfileRequest) {
    return this.http.post<UserProfileResponse>('/profile', request, { withCredentials: true });
  }

  getConditions() {
    return this.http.get<UserConditionResponse[]>('/profile/conditions', { withCredentials: true });
  }

  saveConditions(conditions: UserConditionRequest[]) {
    return this.http.post<void>('/profile/conditions', conditions, { withCredentials: true });
  }

  getMedications() {
    return this.http.get<UserMedicationResponse[]>('/profile/medications', { withCredentials: true });
  }

  saveMedications(medications: UserMedicationRequest[]) {
    return this.http.post<void>('/profile/medications', medications, { withCredentials: true });
  }

  getPersonalRanges() {
    return this.http.get<PersonalRangeResponse[]>('/profile/personal-range', { withCredentials: true });
  }

  recalculateRanges() {
    return this.http.post<void>('/profile/personal-range/recalculate', {}, { withCredentials: true });
  }
}
