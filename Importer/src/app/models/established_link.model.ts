export interface IEstablishedLinkFetchResponse {
  isSuccess: boolean;
  data: IEstablishedLink[];
  message: string;
  totalCount: number;
}

export interface IEstablishedLinkChangeResponse {
  errorMessage: string;
  successCount: number;
  failureCount: number;
  successSet: IEstablishedLink[];
  failureSet: IEstablishedLink[];
}

export interface IEstablishedLink {
  linkId: number;
  containsText: string;
  category: string;
  bucketId: number;
}
