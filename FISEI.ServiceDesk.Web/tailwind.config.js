/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./**/*.razor",
    "./**/*.cshtml",
    "./**/*.html",
    "./wwwroot/**/*.js"
  ],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        brand: {
          50: "#eef2ff",
          100: "#e0e7ff",
          500: "#4f46e5",
          600: "#4338ca",
          700: "#3730a3"
        }
      },
      boxShadow: {
        card: "0 4px 24px -6px rgba(0,0,0,.4)"
      }
    }
  },
  plugins: [
    require('@tailwindcss/forms')
  ]
};