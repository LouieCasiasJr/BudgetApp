export interface ITransactionFetchResponse {
  isSuccess: boolean;
  data: ITransaction[];
  message: string;
  pageIndex: number;
  pageSize: number;
  totalCount: number;
  totalPageCount: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface ITransactionChangeResponse {
  errorMessage: string;
  successCount: number;
  failureCount: number;
  successSet: ITransaction[];
  failureSet: ITransaction[];
}

export interface ITransaction {
  transactionId?: number;
  transactionDate: string;
  card: string;
  description: string;
  debit: boolean;
  amount: number;
  category?: string;
  reference?: string;
  bucketId?: number;
  priority?: number;
  message?: string;
  currency: string;
}
