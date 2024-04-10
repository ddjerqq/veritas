const defaultTheme = require("tailwindcss/defaultTheme");
const colors = require("tailwindcss/colors");

/** @type {import("tailwindcss").Config} */
export default {
  content: [
    "./**/*.{razor,js,cs,css}",
    "./wwwroot/index.html"
  ],
  darkMode: "class",
  theme: {
    extend: {
      colors: {
        background: "#252525",
        glass: "#ffffff07",
        "party-card-name": "#303030",
        "party-card": "#505050",

        unm: "#ce2121",
        lelo: "#d4a700",
        girchi: "#317e38",
        gd: "#0b6abe",

        ...colors,
      },
      keyframes: {
        "slideDown": {
          "0%": {
            transform: "translateY(-100%)",
            opacity: "0"
          },
          "100%": {
            transform: "translateY(0)",
            opacity: "1"
          }
        }
      },
      transitionTimingFunction: {
        "sweet": "cubic-bezier(0.34, 1.56, 0.64, 1)",
        "jumpy": "cubic-bezier(0.68, -0.6, 0.32, 1.6)",
      },
      boxShadow: {
        "party-card": "0 0 20px 0",
        "party-card-hover": "0 0 50px 5px",
      },
      fontFamily: {
        archyedt: "archyedt",
      },
    },
  },
  plugins: [],
};