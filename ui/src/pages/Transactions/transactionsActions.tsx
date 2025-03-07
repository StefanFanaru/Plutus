import axiosClient from "../../axiosClient";

export async function splitTransaction(
  transactionId: string,
  splits: { amount: number; categoryId: string }[],
) {
  const response = await axiosClient.post("/api/Transactions/split", {
    transactionId,
    splits,
  });

  return response.status === 200;
}

export async function changeCategory(
  transactionId: string,
  categoryId: string,
) {
  const response = await axiosClient.post(
    "/api/Transactions/change-category",
    {},
    {
      params: {
        transactionId,
        categoryId,
      },
    },
  );

  return response.status === 200;
}
export async function excludeFromAnalysis(id: string, value: boolean) {
  const response = await axiosClient.post(
    "/api/Transactions/exclude",
    {},
    {
      params: {
        id,
        value,
      },
    },
  );

  return response.status === 200;
}
