import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, BehaviorSubject, Subscription, of, firstValueFrom, shareReplay } from 'rxjs';

import { IEstablishedLink, IEstablishedLinkFetchResponse, IEstablishedLinkChangeResponse } from '../models/established_link.model';


@Injectable({
  providedIn: 'root'
})
export class EstablishedLinkService {
  private appUrl = 'EstablishedLinks';

  private EstablishedLinksSource = new BehaviorSubject<IEstablishedLink[]>([]);
  LinkList$ = this.EstablishedLinksSource.asObservable();

  constructor(private http: HttpClient) {
    this.refreshObservables();
  }

  private getLinkList() {
    return new Promise(resolve => {
      firstValueFrom(this.http.get<IEstablishedLinkFetchResponse>(this.appUrl, { withCredentials: false })).then(resp => {
        this.EstablishedLinksSource.next(resp.data);
        resolve(undefined);
      })
    })
  }

  refreshObservables() {
    return new Promise(resolve => {
      this.getLinkList().then(() => {
            resolve(undefined);
      })
    })
  }

  submitLink(link: IEstablishedLink): Observable<IEstablishedLinkChangeResponse> {
    return this.http.post<any>(this.appUrl+'/Add', link);
  }

  submitLinks(links: IEstablishedLink[]): Observable<IEstablishedLinkChangeResponse> {
    return this.http.post<any>(this.appUrl+'/AddList', links);
  }

  updateLinks(links: IEstablishedLink[]): Observable<IEstablishedLinkChangeResponse> {
    return this.http.patch<any>(this.appUrl, links, { withCredentials: false });
  }
}
