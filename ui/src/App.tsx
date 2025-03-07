import type {} from "@mui/x-date-pickers/themeAugmentation";
import type {} from "@mui/x-charts/themeAugmentation";
import type {} from "@mui/x-tree-view/themeAugmentation";
import CssBaseline from "@mui/material/CssBaseline";
import Box from "@mui/material/Box";
import Stack from "@mui/material/Stack";
import {
  chartsCustomizations,
  dataGridCustomizations,
  datePickersCustomizations,
  treeViewCustomizations,
} from "./components/customizations";
import AppTheme from "./components/mui/AppTheme";
import { useEffect, useState } from "react";
import { hasAuthParams, useAuth } from "react-oidc-context";
import { Routes } from "react-router";
import { Route } from "react-router";
import Analytics from "./pages/Analytics";
import SideMenu from "./components/mui/SideMenu";
import Header from "./components/mui/Header";
import AppNavbar from "./components/mui/AppNavbar";
import Dashboard from "./pages/Dashboard";
import Obligors from "./pages/Obligors/Obligors";
import Transactions from "./pages/Transactions/Transactions";
import Categories from "./pages/Categories/Categories";
import "./App.css";
import axiosClient from "./axiosClient";
import ServiceOffline from "./components/custom/ServerOffline";
import Forbidden from "./components/custom/Forbidden";

const xThemeComponents = {
  ...chartsCustomizations,
  ...dataGridCustomizations,
  ...datePickersCustomizations,
  ...treeViewCustomizations,
};

function App() {
  const auth = useAuth();
  const [hasTriedSignin, setHasTriedSignin] = useState(false);
  const [isServerOnline, setIsServerOnline] = useState(false);
  const [isServerOnlineLoading, setIsServerOnlineLoading] = useState(true);
  const [isForbidden, setIsForbidden] = useState(true);

  // automatically sign-in
  useEffect(() => {
    if (
      !hasAuthParams() &&
      !auth.isAuthenticated &&
      !auth.activeNavigator &&
      !auth.isLoading &&
      !hasTriedSignin
    ) {
      auth.signinRedirect();
      setHasTriedSignin(true);
    }
  }, [auth, hasTriedSignin]);

  const props = {
    disableCustomTheme: false,
  };

  function checkServerStatus() {
    setIsServerOnlineLoading(true);
    axiosClient
      .get("/api/misc/authenticated-status", {
        timeout: 1000,
      })
      .then((response) => {
        if (response.status === 200) {
          setIsServerOnline(true);
          setIsServerOnlineLoading(false);
          setIsForbidden(false);
        }
      })
      .catch((error) => {
        if (error.response?.status === 403 || error.response?.status === 401) {
          setIsServerOnline(true);
        } else {
          setIsForbidden(false);
          setIsServerOnline(false);
        }
        setIsServerOnlineLoading(false);
      });
  }

  useEffect(() => {
    if (auth.isLoading) {
      return;
    }
    checkServerStatus();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [auth.isLoading]);

  return (
    <AppTheme {...props} themeComponents={xThemeComponents}>
      {/* <Loader />*/}
      <CssBaseline enableColorScheme />
      {auth.isLoading || isServerOnlineLoading ? (
        <div />
      ) : isForbidden ? (
        <Forbidden />
      ) : (
        <Box
          sx={{
            display: "flex",
          }}
        >
          <SideMenu />
          <AppNavbar />
          {/* Main content */}
          <Box
            component="main"
            sx={(theme) => ({
              flexGrow: 1,
              backgroundColor: `rgba(${theme.palette.background.default} / 1)`,
              overflow: "auto",
            })}
          >
            <Stack
              spacing={2}
              sx={{
                alignItems: "center",
                mx: 3,
                pb: 5,
                mt: { xs: 8, md: 0 },
              }}
            >
              <Header />
              {isServerOnline ? (
                !auth.isLoading &&
                auth.isAuthenticated && (
                  <Routes>
                    <Route index element={<Dashboard />} />
                    <Route path="obligors" element={<Obligors />} />
                    <Route path="transactions" element={<Transactions />} />
                    <Route path="categories" element={<Categories />} />
                    <Route path="analytics" element={<Analytics />} />
                  </Routes>
                )
              ) : (
                <ServiceOffline />
              )}
            </Stack>
          </Box>
        </Box>
      )}
    </AppTheme>
  );
}

export default App;
