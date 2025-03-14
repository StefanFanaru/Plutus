export interface ListRequest {
  sortField?: string | null;
  sortOrder?: string | null;
  pageNumber: number;
  pageSize: number;
  filter?: {
    field: string;
    operator: string;
    value: string;
  };
}
