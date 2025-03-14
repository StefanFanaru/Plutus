export interface TransactionListItem {
  id: string;
  bookedAt: string;
  amount: number;
  type: number;
  obligorId: string;
  obligorName: string;
  obligorDisplayName: string;
  categoryId: string;
  categoryName: string;
  isExcluded: boolean;
  isSplitItem: boolean;
  isFixedExpense: boolean;
}
