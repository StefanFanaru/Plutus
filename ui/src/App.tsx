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
import { AppUser, UserStatus } from "./common/dtos/User";
import { useNavigate } from "react-router";
import Setup from "./pages/Setup/Setup";
import Globals from "./common/globals";
import RequistionConfirmed from "./pages/Setup/RequistionConfirmed";
import SelectAccount from "./pages/Setup/SelectAccount";
import ReAuthorize from "./pages/Setup/ReAuthorize";
import FixedExpenses from "./pages/FixedExpenses/FixedExpenses";

const xThemeComponents = {
  ...chartsCustomizations,
  ...dataGridCustomizations,
  ...datePickersCustomizations,
  ...treeViewCustomizations,
};

function App() {
  const auth = useAuth();
  const navigate = useNavigate();
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
      const pathAndQuery = window.location.pathname + window.location.search;
      localStorage.setItem("authRedirectPath", pathAndQuery);
      auth.signinRedirect();
      setHasTriedSignin(true);
    } else if (auth.isAuthenticated && !auth.isLoading) {
      const pathAndQuery = localStorage.getItem("authRedirectPath");
      if (pathAndQuery) {
        localStorage.removeItem("authRedirectPath");
        navigate(pathAndQuery);
      }
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [auth, hasTriedSignin]);

  const props = {
    disableCustomTheme: false,
  };

  function checkServerStatus() {
    setIsServerOnlineLoading(true);
    axiosClient
      .get<AppUser>("/api/misc/app-user", {
        timeout: 5000,
      })
      .then((response) => {
        if (response.status === 200) {
          setIsServerOnline(true);
          setIsServerOnlineLoading(false);
          setIsForbidden(false);

          Globals.appUser = response.data;
          if (
            response.data.status === UserStatus.New &&
            window.location.pathname !== "/requisition-confirmed"
          ) {
            navigate("/setup");
          }

          if (
            response.data.status === UserStatus.RequisitionExpired &&
            window.location.pathname !== "/requisition-confirmed"
          ) {
            navigate("/re-authorize");
          }

          if (response.data.status === UserStatus.RequisitionConfirmed) {
            navigate("/select-account");
          }
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
                    <Route path="fixed-expenses" element={<FixedExpenses />} />
                    <Route path="setup" element={<Setup />} />
                    <Route path="re-authorize" element={<ReAuthorize />} />
                    <Route path="select-account" element={<SelectAccount />} />
                    <Route
                      path="requisition-confirmed"
                      element={<RequistionConfirmed />}
                    />
                    <Route path="*" element={<Dashboard />} />
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
