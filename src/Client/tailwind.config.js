const defaultTheme = require('tailwindcss/defaultTheme');
const colors = require('tailwindcss/colors');

/** @type {import('tailwindcss').Config} */
export default {
  content: [
    './**/*.{razor,js,css}',
    './wwwroot/index.html'
  ],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        bg: '#252525',

        'party-card': '#505050',
        'party-card-name': '#303030',

        gd: {
          50: '#f1f7fe',
          100: '#e1eefd',
          200: '#bdddfa',
          300: '#83c2f6',
          400: '#41a4ef',
          500: '#1887df',
          600: '#0b6abe',
          700: '#0b5aa4',
          800: '#0d487f',
          900: '#113e69',
          950: '#0b2646',
        },
        unm: {
          50: '#fef2f2',
          100: '#fee2e2',
          200: '#fecaca',
          300: '#fca5a5',
          400: '#f87171',
          500: '#ef4444',
          600: '#ce2121',
          700: '#b91c1c',
          800: '#991b1b',
          900: '#7f1d1d',
          950: '#450a0a',
        },
        lelo: {
          50: '#fefee8',
          100: '#fdffc2',
          200: '#ffff88',
          300: '#fff843',
          400: '#ffea10',
          500: '#efcf03',
          600: '#d4a700',
          700: '#a47504',
          800: '#875b0c',
          900: '#734a10',
          950: '#432705',
        },
        girchi: {
          50: '#f3faf3',
          100: '#e3f5e4',
          200: '#c8eacb',
          300: '#9cd9a1',
          400: '#69bf70',
          500: '#44a34d',
          600: '#317e38',
          700: '#2c6932',
          800: '#27542b',
          900: '#214626',
          950: '#0e2511',
        },

        ...colors,
      },
      transitionTimingFunction: {
        'sweet': 'cubic-bezier(0.34, 1.56, 0.64, 1)',
        'jumpy': 'cubic-bezier(0.68, -0.6, 0.32, 1.6)',
      },
      boxShadow: {
        'party-card': '0 0 20px 0',
        'party-card-hover': '0 0 50px 5px',
      },
      fontFamily: {
        archyedt: 'archyedt',
      },
    },
  },
  plugins: [],
};