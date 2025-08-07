<template>
  <div class="language-switcher">
    <button 
      @click="toggleDropdown"
      class="language-button"
      :class="{ 'active': isDropdownOpen }"
    >
      <span class="current-lang">{{ currentLanguage.nativeName }}</span>
      <svg 
        class="chevron" 
        :class="{ 'rotated': isDropdownOpen }"
        width="16" 
        height="16" 
        viewBox="0 0 24 24" 
        fill="none" 
        stroke="currentColor" 
        stroke-width="2"
      >
        <polyline points="6,9 12,15 18,9"></polyline>
      </svg>
    </button>
    
    <div 
      v-if="isDropdownOpen" 
      class="language-dropdown"
      @click.stop
    >
      <button
        v-for="lang in availableLanguages"
        :key="lang.code"
        @click="selectLanguage(lang.code)"
        class="language-option"
        :class="{ 'selected': lang.code === currentLocale }"
      >
        <span class="lang-name">{{ lang.nativeName }}</span>
        <span class="lang-code">{{ lang.code.toUpperCase() }}</span>
        <svg 
          v-if="lang.code === currentLocale"
          class="check-icon"
          width="16" 
          height="16" 
          viewBox="0 0 24 24" 
          fill="none" 
          stroke="currentColor" 
          stroke-width="2"
        >
          <polyline points="20,6 9,17 4,12"></polyline>
        </svg>
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { changeLanguage, getCurrentLanguage, getAvailableLanguages } from '../i18n';

const { locale } = useI18n();
const isDropdownOpen = ref(false);
const availableLanguages = getAvailableLanguages();

const currentLocale = computed(() => getCurrentLanguage());
const currentLanguage = computed(() => {
  return availableLanguages.find(lang => lang.code === currentLocale.value) || availableLanguages[0];
});

const toggleDropdown = () => {
  isDropdownOpen.value = !isDropdownOpen.value;
};

const selectLanguage = (langCode) => {
  changeLanguage(langCode);
  isDropdownOpen.value = false;
};

const closeDropdown = (event) => {
  if (!event.target.closest('.language-switcher')) {
    isDropdownOpen.value = false;
  }
};

onMounted(() => {
  document.addEventListener('click', closeDropdown);
});

onUnmounted(() => {
  document.removeEventListener('click', closeDropdown);
});
</script>

<style scoped>
.language-switcher {
  position: relative;
  display: inline-block;
}

.language-button {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 12px;
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s ease;
  font-size: 14px;
  color: #374151;
  min-width: 80px;
}

.language-button:hover {
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.language-button.active {
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.current-lang {
  font-weight: 500;
}

.chevron {
  transition: transform 0.2s ease;
  color: #6b7280;
}

.chevron.rotated {
  transform: rotate(180deg);
}

.language-dropdown {
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  margin-top: 4px;
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05);
  z-index: 50;
  overflow: hidden;
}

.language-option {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
  padding: 12px 16px;
  background: none;
  border: none;
  cursor: pointer;
  transition: background-color 0.2s ease;
  font-size: 14px;
  text-align: left;
}

.language-option:hover {
  background-color: #f3f4f6;
}

.language-option.selected {
  background-color: #eff6ff;
  color: #3b82f6;
}

.lang-name {
  font-weight: 500;
}

.lang-code {
  font-size: 12px;
  color: #6b7280;
  margin-left: 8px;
}

.language-option.selected .lang-code {
  color: #3b82f6;
}

.check-icon {
  color: #3b82f6;
  margin-left: 8px;
}

/* RTL Support */
[dir="rtl"] .language-button {
  flex-direction: row-reverse;
}

[dir="rtl"] .language-dropdown {
  left: auto;
  right: 0;
}

[dir="rtl"] .language-option {
  text-align: right;
  flex-direction: row-reverse;
}

[dir="rtl"] .lang-code {
  margin-left: 0;
  margin-right: 8px;
}

[dir="rtl"] .check-icon {
  margin-left: 0;
  margin-right: 8px;
}
</style>