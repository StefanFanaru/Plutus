import {
  Alert,
  Box,
  Button,
  Card,
  CardContent,
  Link,
  Typography,
} from "@mui/material";
import Grid from "@mui/material/Grid2";
import axiosClient from "../../axiosClient";
import { useEffect, useState } from "react";

function Setup() {
  const [requisitionUrl, setRequisitionUrl] = useState<string | null>(null);
  async function fetchRequisitionUrl() {
    const response = await axiosClient.get<{ link: string }>(
      "/api/Setup/requisition-url",
    );
    setRequisitionUrl(response.data.link);
  }

  useEffect(() => {
    fetchRequisitionUrl();
  }, []);

  function onAuthorizeWithRevolut() {
    window.location.href = requisitionUrl!;
  }

  return (
    <Box sx={{ width: "100%", maxWidth: { sm: "100%", md: "1700px" } }}>
      <Grid
        container
        spacing={2}
        columns={5}
        alignItems="center"
        justifyContent="center"
      >
        <Grid size={{ xs: 12, lg: 9, xl: 3 }}>
          {/* */}
          <Card variant="outlined">
            <CardContent>
              <Typography variant="h5" component="div">
                Welcome to Plutus!
              </Typography>
              <Typography sx={{ color: "text.secondary", mb: 1.5 }}>
                Let's get started by setting up your account.
              </Typography>
              <Typography variant="body2">
                To be able to use Plutus, you need to set up your account. This
                process is simple and will only take a few seconds.
                <br />
                <br />
                Plutus uses{" "}
                <Link href="https://gocardless.com" color="inherit">
                  GoCardless
                </Link>{" "}
                to mediate the access to your Revolut information.
                <br />
                In order to allow Plutus to access your transactions history and
                balance, you need to authorize with Revolut.
              </Typography>
              <Alert severity="error" sx={{ mt: "1rem" }}>
                Plutus is not able to make transactions on your behalf. It can
                only read some of you Revolut account's data.
              </Alert>
            </CardContent>
            <Button
              sx={{ mt: "1rem" }}
              onClick={onAuthorizeWithRevolut}
              variant="contained"
              color="secondary"
              component="label"
            >
              Authorize with Revolut
            </Button>
          </Card>
        </Grid>
      </Grid>
    </Box>
  );
}

export default Setup;
