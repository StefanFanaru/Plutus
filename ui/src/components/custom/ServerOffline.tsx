import { Button, Typography } from "@mui/material";
import { ErrorOutline } from "@mui/icons-material";

const ServiceOffline = () => {
  function onRetry() {
    window.location.reload();
  }
  return (
    <div
      style={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "start",
        paddingTop: "2rem",
        height: "100vh",
      }}
    >
      <ErrorOutline color="error" sx={{ fontSize: 100 }} />
      <Typography variant="h4" gutterBottom>
        Temporary unavailable
      </Typography>
      <Typography variant="body1" gutterBottom sx={{ mb: "1rem" }}>
        Something went wrong. Please try again later.
      </Typography>
      <Button variant="contained" onClick={onRetry}>
        Retry
      </Button>
    </div>
  );
};

export default ServiceOffline;
