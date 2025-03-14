import { useNavigate } from "react-router";
import axiosClient from "../../axiosClient";
import { useEffect } from "react";
import Globals from "../../common/globals";
import { UserStatus } from "../../common/dtos/User";
import Loader from "../../components/custom/loader/Loader";

function RequistionConfirmed() {
  const navigate = useNavigate();
  async function confirmRequisition(requisitionId: string) {
    const response = await axiosClient.get<{
      isRevolutConfirmed: boolean;
    }>("/api/Setup/confirm-requisition", {
      params: { requisitionId },
    });

    if (response.data.isRevolutConfirmed) {
      Globals.appUser!.status = UserStatus.RevolutConfirmed;
      navigate("/");
    } else {
      navigate("/select-account");
    }
  }

  useEffect(() => {
    const requisitionId = new URLSearchParams(window.location.search).get("id");
    if (requisitionId) {
      confirmRequisition(requisitionId);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return (
    <div>
      <Loader />
    </div>
  );
}

export default RequistionConfirmed;
