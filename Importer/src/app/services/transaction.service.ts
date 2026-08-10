import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, BehaviorSubject, Subscription, of, firstValueFrom, shareReplay } from 'rxjs';

import { ITransaction, ITransactionFetchResponse, ITransactionChangeResponse } from '../models/transaction.model';


@Injectable({
  providedIn: 'root'
})
export class TransactionService {
  private appUrl = 'Transaction';

  constructor(private http: HttpClient) {
    this.refreshObservables();
  }



  refreshObservables() {
    return new Promise(resolve => {
        resolve(undefined);
    })
  }

  submitTransactions(transactions: ITransaction[]): Observable<ITransactionChangeResponse> {
    return this.http.post<any>(this.appUrl + '/Add', transactions);
  }

  updateTransactions(transactions: ITransaction[]): Observable<ITransactionChangeResponse> {
    return this.http.patch<any>(this.appUrl, transactions, { withCredentials: true });
  }
}
