import { defineConfig } from "vite";
import mkcert from "vite-plugin-mkcert";
import react from "@vitejs/plugin-react";
import eslint from "vite-plugin-eslint2";

// const fullReloadAlways: PluginOption = {
//   name: "full-reload-always",
//   handleHotUpdate({ server }) {
//     server.ws.send({ type: "full-reload" });
//     return [];
//   },
// } as PluginOption;

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    react(),
    mkcert(),
    eslint({
      // Configure the ESLint plugin options
      include: ["src/**/*.ts", "src/**/*.tsx"], // Specify the files to lint
      cache: false, // Disable ESLint caching
    }),
  ],
  server: {
    https: true,
    port: 3101,
    host: "0.0.0.0",
  },
});
