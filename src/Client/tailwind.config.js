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
    colors: {
      "agmashenebeli": "#ff0000",
      "euro-georgia": "#0477cc",
      "unm": "#ce2121",
      "euro-democrats": "#6a3be1",
      "citizens": "#8bc43f",
      // "law-and-order": "#4040dc",
      "patriot": "#e7b031",
      "lelo": "#d4a700",
      "girchi-iago": "#317e38",
      "gd": "#0b6abe",
      "girchi-zurab": "#317e38",

      ...colors,

      "gray": colors.neutral,
    },
    fontFamily: {
      archyedt: "archyedt",
    },
    extend: {
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
        },
        "slideLeft": {
          "0%": {
            transform: "translateX(200%)",
            opacity: "0"
          },
          "100%": {
            transform: "translateX(0)",
            opacity: "1"
          }
        }
      },
      transitionTimingFunction: {
        "sweet": "cubic-bezier(0.34, 1.56, 0.64, 1)",
        "jumpy": "cubic-bezier(0.68, -0.6, 0.32, 1.6)",
      },
      boxShadow: {
        "party-card": "0 5px 10px 0",
        "party-card-hover": "0 0 50px 5px",
      },
    },
  },
  plugins: [],
};