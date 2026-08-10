export interface ISpendingBucketFetchResponse {
  isSuccess: boolean;
  data: ISpendingBucket[];
  message: string;
  totalCount: number;
}

export interface ISpendingBucketChangeResponse {
  successCount: number;
  failureCount: number;
  failureSet: ISpendingBucket[];
}

export interface ISpendingBucket {
  bucketId: number;
  bucketLabel: string;
  defaultPriority: number;
  displayOrder: number;
}