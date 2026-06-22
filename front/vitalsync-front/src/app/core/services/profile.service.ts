import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  UserProfileRequest, UserProfileResponse,
  UserConditionRequest, UserConditionResponse,
  UserMedicationRequest, UserMedicationResponse,
  PersonalRangeResponse, PROFILE_PICS
} from '../models/profile.models';

@Injectable({ providedIn: 'root' })
export class ProfileService {
  profilePicSrc = signal<string | null>(null);

  constructor(private http: HttpClient) {}

  updatePicFromKey(picKey: string) {
    const pic = PROFILE_PICS.find(p => p.key === picKey);
    this.profilePicSrc.set(pic?.src ?? null);
  }

  loadProfilePic() {
    this.getProfile().subscribe({
      next: p => this.updatePicFromKey(p.profilePic),
      error: () => {},
    });
  }

  getProfile() {
    return this.http.get<UserProfileResponse>('/api/profile', { withCredentials: true });
  }

  saveProfile(request: UserProfileRequest) {
    return this.http.post<UserProfileResponse>('/api/profile', request, { withCredentials: true });
  }

  getConditions() {
    return this.http.get<UserConditionResponse[]>('/api/profile/conditions', { withCredentials: true });
  }

  saveConditions(conditions: UserConditionRequest[]) {
    return this.http.post<void>('/api/profile/conditions', conditions, { withCredentials: true });
  }

  getMedications() {
    return this.http.get<UserMedicationResponse[]>('/api/profile/medications', { withCredentials: true });
  }

  saveMedications(medications: UserMedicationRequest[]) {
    return this.http.post<void>('/api/profile/medications', medications, { withCredentials: true });
  }

  getPersonalRanges() {
    return this.http.get<PersonalRangeResponse[]>('/api/profile/personal-range', { withCredentials: true });
  }

  recalculateRanges() {
    return this.http.post<void>('/api/profile/personal-range/recalculate', {}, { withCredentials: true });
  }
}
