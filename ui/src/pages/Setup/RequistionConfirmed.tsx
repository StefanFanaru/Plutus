import { useNavigate } from "react-router";
import axiosClient from "../../axiosClient";
import { useEffect } from "react";

function RequistionConfirmed() {
  const navigate = useNavigate();
  async function confirmRequisition(requisitionId: string) {
    await axiosClient.get<{ link: string }>("/api/Setup/confirm-requisition", {
      params: { requisitionId },
    });
    navigate("/select-account");
  }

  useEffect(() => {
    const requisitionId = new URLSearchParams(window.location.search).get("id");
    console.log(requisitionId);
    if (requisitionId) {
      confirmRequisition(requisitionId);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return <div />;
}

export default RequistionConfirmed;
