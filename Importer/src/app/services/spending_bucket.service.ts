import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, BehaviorSubject, Subscription, of, firstValueFrom, shareReplay } from 'rxjs';

import { ISpendingBucket, ISpendingBucketFetchResponse,ISpendingBucketChangeResponse } from '../models/spending_bucket.model';


@Injectable({
  providedIn: 'root'
})
export class SpendingBucketService {
  private appUrl = 'SpendingBuckets';

  private SpendingBucketSource = new BehaviorSubject<ISpendingBucket[]>([]);
  BucketList$ = this.SpendingBucketSource.asObservable();

  constructor(private http: HttpClient) {
    this.refreshObservables();
  }

  private getBucketList() {
    return new Promise(resolve => {
      firstValueFrom(this.http.get<ISpendingBucketFetchResponse>(this.appUrl, { withCredentials: false })).then(resp => {
        this.SpendingBucketSource.next(resp.data);
        resolve(undefined);
      })
    })
  }

  refreshObservables() {
    return new Promise(resolve => {
      this.getBucketList().then(() => {
            resolve(undefined);
      })
    })
  }

  submitLinks(links: ISpendingBucket[]): Observable<ISpendingBucketChangeResponse> {
    return this.http.post<any>(this.appUrl+'/Add', { links });
  }

  updateLinks(links: ISpendingBucket[]): Observable<ISpendingBucketChangeResponse> {
    return this.http.patch<any>(this.appUrl, links, { withCredentials: false });
  }
}
