import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NotificationStateService {
  private refreshSubject = new Subject<void>();

  // Observable that components can subscribe to
  refresh$ = this.refreshSubject.asObservable();

  // Method to trigger a refresh
  triggerRefresh() {
    this.refreshSubject.next();
  }
}
