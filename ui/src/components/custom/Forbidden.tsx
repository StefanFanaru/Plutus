// make a page for 403 forbidden
// using material ui icons and components
//

import React from "react";
import { Box, Button, Typography } from "@mui/material";
import BlockIcon from "@mui/icons-material/Block";

export default function Forbidden() {
  function onRetry() {
    const clientId = import.meta.env.VITE_OIDC_CLIENT_ID;
    localStorage.removeItem(
      `oidc.user:https://auth.stefanaru.com/realms/stefanaru:${clientId}`,
    );
    window.location.reload();
  }
  return (
    <Box
      display="flex"
      flexDirection="column"
      justifyContent="center"
      alignItems="center"
      height="100%"
      paddingTop="5rem"
    >
      <BlockIcon color="error" sx={{ fontSize: 100 }} />
      <Typography variant="h4">403 Forbidden</Typography>
      <Typography variant="body1" sx={{ mb: "1rem" }}>
        You are not allowed to access this page
      </Typography>
      <Button variant="contained" onClick={onRetry}>
        Retry
      </Button>
    </Box>
  );
}
