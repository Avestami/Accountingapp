import { createI18n } from 'vue-i18n';
import fa from '../locales/fa.js';
import en from '../locales/en.js';

// Get the user's preferred language from localStorage or browser
const getDefaultLocale = () => {
  const savedLocale = localStorage.getItem('locale');
  if (savedLocale && ['fa', 'en'].includes(savedLocale)) {
    return savedLocale;
  }
  
  // Check browser language
  const browserLang = navigator.language.split('-')[0];
  if (browserLang === 'fa') {
    return 'fa';
  }
  
  // Default to Persian for this Iranian travel accounting system
  return 'fa';
};

const i18n = createI18n({
  legacy: false, // Use Composition API mode
  locale: getDefaultLocale(),
  fallbackLocale: 'en',
  messages: {
    fa,
    en
  },
  // Enable missing translation warnings in development
  missingWarn: process.env.NODE_ENV === 'development',
  fallbackWarn: process.env.NODE_ENV === 'development'
});

// Function to change language
export const changeLanguage = (locale) => {
  if (['fa', 'en'].includes(locale)) {
    i18n.global.locale.value = locale;
    localStorage.setItem('locale', locale);
    
    // Update document direction for RTL/LTR
    document.documentElement.dir = locale === 'fa' ? 'rtl' : 'ltr';
    document.documentElement.lang = locale;
  }
};

// Function to get current language
export const getCurrentLanguage = () => {
  return i18n.global.locale.value;
};

// Function to get available languages
export const getAvailableLanguages = () => {
  return [
    { code: 'fa', name: 'فارسی', nativeName: 'فارسی' },
    { code: 'en', name: 'English', nativeName: 'English' }
  ];
};

// Initialize document direction on load
changeLanguage(getDefaultLocale());

export default i18n;