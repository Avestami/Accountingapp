<template>
  <div class="p-6">
    <div class="mb-8">
      <h1 class="text-2xl font-bold text-gray-900 mb-2">تنظیمات سیستم</h1>
      <p class="text-gray-600">در این بخش می‌توانید تنظیمات کلی سیستم را مدیریت کنید.</p>
    </div>
    
    <div v-if="loading" class="flex justify-center items-center p-12">
      <AppSpinner size="lg" />
    </div>
    
    <div v-else class="bg-white rounded-lg shadow p-6">
      <div class="mb-6">
        <h2 class="text-xl font-semibold mb-4">تنظیمات عمومی</h2>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">نام شرکت</label>
            <input 
              type="text" 
              v-model="settings.companyName" 
              class="w-full border rounded p-2 text-right" 
              placeholder="نام شرکت را وارد کنید"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">واحد پول پیش‌فرض</label>
            <select v-model="settings.currency" class="w-full border rounded p-2 text-right">
              <option value="IRR">ریال ایران (IRR)</option>
              <option value="USD">دلار آمریکا (USD)</option>
              <option value="EUR">یورو (EUR)</option>
            </select>
          </div>
        </div>
      </div>
      
      <div class="mb-6">
        <h2 class="text-xl font-semibold mb-4">تنظیمات امنیتی</h2>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">مهلت نشست (دقیقه)</label>
            <input 
              type="number" 
              v-model="settings.sessionTimeout" 
              min="5" 
              class="w-full border rounded p-2 text-right" 
              placeholder="مهلت نشست به دقیقه"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">سیاست رمز عبور</label>
            <select v-model="settings.passwordPolicy" class="w-full border rounded p-2 text-right">
              <option value="low">ساده (حداقل ۶ کاراکتر)</option>
              <option value="medium">متوسط (حداقل ۸ کاراکتر، ۱ عدد)</option>
              <option value="high">پیشرفته (حداقل ۱۰ کاراکتر، حروف بزرگ و کوچک، اعداد، نمادها)</option>
            </select>
          </div>
        </div>
      </div>
      
      <div class="flex justify-end">
        <button 
          @click="saveSettings" 
          :disabled="saving"
          class="bg-primary-600 hover:bg-primary-700 disabled:opacity-50 text-white px-4 py-2 rounded flex items-center"
        >
          <svg v-if="saving" class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
          </svg>
          {{ saving ? 'در حال ذخیره...' : 'ذخیره تنظیمات' }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useSettingsStore } from '@/stores/settings'
import AppSpinner from '@/components/AppSpinner.vue'

const settingsStore = useSettingsStore()

// State
const loading = ref(true)
const saving = ref(false)
const settings = ref({
  companyName: 'سیستم حسابداری سفر',
  currency: 'IRR',
  sessionTimeout: 60,
  passwordPolicy: 'medium'
})

// Load settings on component mount
onMounted(async () => {
  try {
    await settingsStore.loadAllSettings()
    // Load system settings from store if available
    if (settingsStore.systemSettings) {
      settings.value = { ...settingsStore.systemSettings }
    }
  } finally {
    loading.value = false
  }
})

// Methods
const saveSettings = async () => {
  saving.value = true
  try {
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 1000))
    
    // Update store with new settings
    settingsStore.systemSettings = { ...settings.value }
    
    alert('تنظیمات با موفقیت ذخیره شد!')
  } catch (error) {
    alert('خطا در ذخیره تنظیمات: ' + error.message)
  } finally {
    saving.value = false
  }
}
</script>