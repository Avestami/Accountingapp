# Travel Accounting System - Vue 3 Implementation Guide

## Chosen Option: Vue 3 + Vite + Pinia + Vue Router

This option provides modern development experience, excellent RTL support, and aligns with the UI_PLAN.md specifications.

## Project Scaffolding Commands

```bash
# Navigate to project directory
cd d:\work\Karlancer\Accountingproject1

# Create Vue 3 project with Vite
npm create vue@latest ClientApp -- --typescript --router --pinia --eslint

# Navigate to ClientApp
cd ClientApp

# Install additional dependencies
npm install
npm install @vueuse/core vue-persian-datetime-picker chart.js vue-chartjs
npm install @headlessui/vue @heroicons/vue lucide-vue-next
npm install jspdf xlsx file-saver

# Development server
npm run dev
```

## Folder Structure

```
ClientApp/
├── public/
│   ├── favicon.ico
│   └── captcha-sample.png
├── src/
│   ├── assets/
│   │   ├── styles/
│   │   │   ├── main.css
│   │   │   ├── rtl.css
│   │   │   └── components.css
│   │   └── images/
│   ├── components/
│   │   ├── base/
│   │   │   ├── AppButton.vue
│   │   │   ├── AppCard.vue
│   │   │   ├── AppIcon.vue
│   │   │   ├── AppModal.vue
│   │   │   └── AppSpinner.vue
│   │   ├── forms/
│   │   │   ├── TextInput.vue
│   │   │   ├── CurrencyInput.vue
│   │   │   ├── PersianDatePicker.vue
│   │   │   ├── TypeaheadSelect.vue
│   │   │   ├── CaptchaInput.vue
│   │   │   ├── FileUpload.vue
│   │   │   └── ToggleSwitch.vue
│   │   ├── layout/
│   │   │   ├── AppHeader.vue
│   │   │   ├── AppSidebar.vue
│   │   │   └── MainLayout.vue
│   │   └── data/
│   │       ├── DataTable.vue
│   │       ├── AuditTrail.vue
│   │       ├── StatCard.vue
│   │       ├── BarChart.vue
│   │       └── LineChart.vue
│   ├── views/
│   │   ├── auth/
│   │   │   └── LoginView.vue
│   │   ├── dashboard/
│   │   │   └── DashboardView.vue
│   │   ├── sales/
│   │   │   ├── CreateDocumentView.vue
│   │   │   ├── UnissuedTicketsView.vue
│   │   │   ├── IssuedTicketsView.vue
│   │   │   └── CanceledTicketsView.vue
│   │   ├── finance/
│   │   │   ├── VoucherView.vue
│   │   │   ├── CostsView.vue
│   │   │   ├── IncomesView.vue
│   │   │   └── TransferView.vue
│   │   ├── reports/
│   │   │   ├── ProfitLossView.vue
│   │   │   ├── AccountsReceivableView.vue
│   │   │   └── ReportsShellView.vue
│   │   └── settings/
│   │       ├── UsersView.vue
│   │       ├── RolesView.vue
│   │       ├── AirlinesView.vue
│   │       ├── BanksView.vue
│   │       └── CounterpartiesView.vue
│   ├── stores/
│   │   ├── auth.js
│   │   ├── sales.js
│   │   ├── finance.js
│   │   ├── reports.js
│   │   └── settings.js
│   ├── services/
│   │   ├── api.js
│   │   ├── mockData.js
│   │   └── exportService.js
│   ├── utils/
│   │   ├── formatters.js
│   │   ├── validators.js
│   │   └── dateHelpers.js
│   ├── router/
│   │   └── index.js
│   ├── App.vue
│   └── main.js
├── package.json
├── vite.config.js
└── README.md
```

## Core Configuration Files

### vite.config.js
```javascript
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { fileURLToPath, URL } from 'node:url'

export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  },
  server: {
    port: 3000,
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true
      }
    }
  }
})
```

### src/main.js
```javascript
import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router'
import './assets/styles/main.css'
import './assets/styles/rtl.css'

const app = createApp(App)

app.use(createPinia())
app.use(router)

app.mount('#app')
```

### src/assets/styles/main.css
```css
@import url('https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap');

:root {
  --primary-color: #3b82f6;
  --secondary-color: #64748b;
  --success-color: #10b981;
  --warning-color: #f59e0b;
  --error-color: #ef4444;
  --background-color: #f8fafc;
  --surface-color: #ffffff;
  --text-primary: #1e293b;
  --text-secondary: #64748b;
  --border-color: #e2e8f0;
}

* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: 'Inter', sans-serif;
  background-color: var(--background-color);
  color: var(--text-primary);
  line-height: 1.6;
}

.btn {
  @apply px-4 py-2 rounded-md font-medium transition-colors duration-200;
}

.btn-primary {
  @apply bg-blue-600 text-white hover:bg-blue-700;
}

.btn-secondary {
  @apply bg-gray-200 text-gray-800 hover:bg-gray-300;
}

.card {
  @apply bg-white rounded-lg shadow-sm border border-gray-200 p-6;
}

.form-group {
  @apply mb-4;
}

.form-label {
  @apply block text-sm font-medium text-gray-700 mb-2;
}

.form-input {
  @apply w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent;
}

.table {
  @apply w-full border-collapse;
}

.table th,
.table td {
  @apply px-4 py-3 text-left border-b border-gray-200;
}

.table th {
  @apply bg-gray-50 font-medium text-gray-900;
}

.status-red {
  @apply border-l-4 border-red-500;
}

.status-blue {
  @apply border-l-4 border-blue-500;
}
```

### src/assets/styles/rtl.css
```css
[dir="rtl"] {
  direction: rtl;
  text-align: right;
}

[dir="rtl"] .sidebar {
  right: 0;
  left: auto;
}

[dir="rtl"] .main-content {
  margin-right: 250px;
  margin-left: 0;
}

[dir="rtl"] .form-input {
  text-align: right;
}

[dir="rtl"] .table th,
[dir="rtl"] .table td {
  text-align: right;
}

[dir="rtl"] .dropdown-menu {
  right: 0;
  left: auto;
}
```

## Reusable UI Components

### src/components/base/AppButton.vue
```vue
<template>
  <button
    :class="[
      'btn',
      `btn-${variant}`,
      {
        'opacity-50 cursor-not-allowed': disabled,
        'flex items-center gap-2': icon
      }
    ]"
    :disabled="disabled"
    @click="$emit('click', $event)"
  >
    <component v-if="icon" :is="icon" class="w-4 h-4" />
    <slot />
  </button>
</template>

<script setup>
defineProps({
  variant: {
    type: String,
    default: 'primary',
    validator: (value) => ['primary', 'secondary', 'success', 'warning', 'error'].includes(value)
  },
  disabled: {
    type: Boolean,
    default: false
  },
  icon: {
    type: [String, Object],
    default: null
  }
})

defineEmits(['click'])
</script>
```

### src/components/layout/AppLayout.vue
```vue
<template>
  <div class="app-layout" dir="rtl">
    <!-- Header -->
    <header class="bg-white shadow-sm border-b">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center h-16">
          <div class="flex items-center">
            <h1 class="text-xl font-semibold text-gray-900">سیستم حسابداری سفر</h1>
          </div>
          
          <!-- Global Search -->
          <div class="flex-1 max-w-lg mx-8">
            <div class="relative">
              <input
                type="text"
                placeholder="جستجو بر اساس شماره سند، سریال، مرجع..."
                class="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                v-model="searchQuery"
                @keyup.enter="performSearch"
              />
              <div class="absolute inset-y-0 right-0 pr-3 flex items-center">
                <svg class="h-5 w-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="m21 21-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
                </svg>
              </div>
            </div>
          </div>
          
          <!-- User Menu -->
          <div class="flex items-center gap-4">
            <span class="text-sm text-gray-700">{{ user?.fullName }}</span>
            <button
              @click="logout"
              class="text-sm text-red-600 hover:text-red-800"
            >
              خروج
            </button>
          </div>
        </div>
      </div>
    </header>

    <!-- Navigation -->
    <nav class="bg-gray-50 border-b">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex space-x-8 rtl:space-x-reverse">
          <router-link
            v-for="item in navigation"
            :key="item.name"
            :to="item.to"
            class="px-3 py-4 text-sm font-medium text-gray-700 hover:text-gray-900 border-b-2 border-transparent hover:border-gray-300"
            active-class="text-blue-600 border-blue-600"
          >
            {{ item.name }}
          </router-link>
        </div>
      </div>
    </nav>

    <!-- Main Content -->
    <main class="flex-1 overflow-auto">
      <router-view />
    </main>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const searchQuery = ref('')

const user = computed(() => authStore.user)

const navigation = [
  { name: 'داشبورد', to: '/dashboard' },
  { name: 'فروش', to: '/sales' },
  { name: 'مالی', to: '/finance' },
  { name: 'گزارشات', to: '/reports' },
  { name: 'تنظیمات', to: '/settings' }
]

const performSearch = () => {
  if (searchQuery.value.trim()) {
    console.log('Searching for:', searchQuery.value)
    // Implement global search logic
  }
}

const logout = () => {
  authStore.logout()
  router.push('/login')
}
</script>
```

### src/components/charts/DashboardChart.vue
```vue
<template>
  <div class="bg-white p-6 rounded-lg shadow">
    <h3 class="text-lg font-medium text-gray-900 mb-4">{{ title }}</h3>
    <div class="h-64 flex items-center justify-center bg-gray-50 rounded">
      <!-- Placeholder for chart -->
      <div class="text-center">
        <div class="text-4xl text-gray-400 mb-2">📊</div>
        <p class="text-gray-500">نمودار {{ title }}</p>
        <p class="text-sm text-gray-400 mt-2">{{ description }}</p>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  title: {
    type: String,
    required: true
  },
  description: {
    type: String,
    default: ''
  }
})
</script>
```

## Package.json Dependencies

```json
{
  "name": "travel-accounting-ui",
  "private": true,
  "version": "0.0.0",
  "type": "module",
  "scripts": {
    "dev": "vite",
    "build": "vite build",
    "preview": "vite preview"
  },
  "dependencies": {
    "vue": "^3.3.4",
    "vue-router": "^4.2.4",
    "pinia": "^2.1.6",
    "jspdf": "^2.5.1",
    "xlsx": "^0.18.5"
  },
  "devDependencies": {
    "@vitejs/plugin-vue": "^4.2.3",
    "vite": "^4.4.5",
    "tailwindcss": "^3.3.0",
    "autoprefixer": "^10.4.14",
    "postcss": "^8.4.24"
  }
}
```

## Installation & Setup Commands

```bash
# Create project
npm create vue@latest travel-accounting-ui
cd travel-accounting-ui

# Install dependencies
npm install
npm install vue-router pinia jspdf xlsx
npm install -D tailwindcss autoprefixer postcss

# Initialize Tailwind CSS
npx tailwindcss init -p

# Start development server
npm run dev
```

## Tailwind Configuration

### tailwind.config.js
```javascript
/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      fontFamily: {
        'persian': ['Tahoma', 'Arial', 'sans-serif']
      }
    },
  },
  plugins: [],
}
```

## UI Acceptance Checklist

### ✅ Authentication & Layout
- [ ] Login form displays correctly with RTL layout
- [ ] Company dropdown, username, password, and captcha fields work
- [ ] Captcha refreshes when clicked
- [ ] Login redirects to dashboard on success
- [ ] Logout functionality works and redirects to login
- [ ] Global search box is visible in header
- [ ] Navigation menu shows all required sections

### ✅ Dashboard
- [ ] Dashboard cards display income/expense summaries
- [ ] A/R and A/P cards show correct mock data
- [ ] Chart placeholders are visible and labeled
- [ ] All numbers display with Persian/Farsi formatting
- [ ] RTL layout is consistent throughout

### ✅ Sales Module
- [ ] Unissued tickets list loads and displays
- [ ] Color coding works (red for first 5 days, blue for rest)
- [ ] Sorting by flight date functions correctly
- [ ] Row actions (edit/cancel/delete) are available per status
- [ ] Create Document wizard opens and navigates between steps
- [ ] Buyer, passenger, supplier, and finance sections display
- [ ] Adult/child/infant counters work
- [ ] Document number appears only after save (mock)
- [ ] Sale date defaults to last document date

### ✅ Finance Module
- [ ] Voucher form displays all required fields
- [ ] Costs and Incomes forms are accessible
- [ ] Transfer A→B form works
- [ ] All forms post to mock store
- [ ] Statement previews are shown

### ✅ Reports Module
- [ ] Report shell pages load
- [ ] Filter bars are complete and functional
- [ ] PDF export button triggers download (dummy)
- [ ] Excel export button triggers download (dummy)

### ✅ Settings Module
- [ ] Airlines list displays with mock data
- [ ] Banks/accounts show opening balances (Rial/FX)
- [ ] Counterparties (buyers/suppliers) with balances
- [ ] Users/roles management interface
- [ ] Card readers and gateways settings
- [ ] Origins/destinations management
- [ ] Audit history widgets on detail pages

### ✅ UI/UX Requirements
- [ ] All text displays in Persian/Farsi
- [ ] RTL layout is consistent across all pages
- [ ] Responsive design works on mobile/tablet
- [ ] Currency inputs show proper separators
- [ ] Persian date picker functions correctly
- [ ] Typeahead selects work with mock data
- [ ] Loading spinners appear during async operations
- [ ] Error messages display appropriately
- [ ] Export buttons are present on all list pages
- [ ] Keyboard navigation works properly
- [ ] Number formatting follows Persian conventions

### ✅ Technical Requirements
- [ ] Vue 3 Composition API is used consistently
- [ ] Pinia stores manage state correctly
- [ ] Vue Router handles navigation
- [ ] Mock data service provides realistic data
- [ ] Components are reusable and well-structured
- [ ] Code follows Vue 3 best practices
- [ ] No console errors in browser
- [ ] Development server runs without issues

## Next Steps

1. **Setup Project**: Run the installation commands above
2. **Copy Files**: Copy all the provided code into the respective files
3. **Test Functionality**: Go through the acceptance checklist
4. **Customize Styling**: Adjust colors, fonts, and spacing as needed
5. **Add Real Data**: Replace mock data with API calls when backend is ready
6. **Enhance Components**: Add more sophisticated chart libraries, date pickers, etc.
7. **Performance**: Optimize bundle size and loading times
8. **Accessibility**: Add ARIA labels and keyboard navigation

This implementation provides a solid foundation for the travel accounting system UI with all the requested features and follows Vue 3 best practices with proper RTL support and Persian localization.

### src/components/forms/CurrencyInput.vue
```vue
<template>
  <div class="form-group">
    <label v-if="label" class="form-label">{{ label }}</label>
    <div class="relative">
      <input
        ref="input"
        v-model="displayValue"
        type="text"
        :class="[
          'form-input',
          currency === 'IRR' ? 'text-right' : 'text-left'
        ]"
        :placeholder="placeholder"
        @input="handleInput"
        @blur="handleBlur"
        @focus="handleFocus"
      />
      <span class="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-500 text-sm">
        {{ currency }}
      </span>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'

const props = defineProps({
  modelValue: {
    type: [Number, String],
    default: 0
  },
  label: {
    type: String,
    default: ''
  },
  placeholder: {
    type: String,
    default: '0'
  },
  currency: {
    type: String,
    default: 'IRR',
    validator: (value) => ['IRR', 'USD', 'EUR'].includes(value)
  }
})

const emit = defineEmits(['update:modelValue'])

const input = ref(null)
const displayValue = ref('')
const isFocused = ref(false)

const formatNumber = (value) => {
  if (!value) return ''
  const num = parseFloat(value.toString().replace(/,/g, ''))
  if (isNaN(num)) return ''
  return new Intl.NumberFormat('en-US').format(num)
}

const parseNumber = (value) => {
  if (!value) return 0
  return parseFloat(value.toString().replace(/,/g, '')) || 0
}

const handleInput = (event) => {
  const value = event.target.value.replace(/[^0-9.]/g, '')
  const numValue = parseNumber(value)
  emit('update:modelValue', numValue)
}

const handleFocus = () => {
  isFocused.value = true
  displayValue.value = props.modelValue.toString()
}

const handleBlur = () => {
  isFocused.value = false
  displayValue.value = formatNumber(props.modelValue)
}

watch(() => props.modelValue, (newValue) => {
  if (!isFocused.value) {
    displayValue.value = formatNumber(newValue)
  }
}, { immediate: true })
</script>
```

### src/components/forms/PersianDatePicker.vue
```vue
<template>
  <div class="form-group">
    <label v-if="label" class="form-label">{{ label }}</label>
    <div class="relative">
      <input
        v-model="displayValue"
        type="text"
        class="form-input text-right"
        :placeholder="placeholder"
        readonly
        @click="showPicker = true"
      />
      <div class="absolute left-3 top-1/2 transform -translate-y-1/2">
        <CalendarIcon class="w-5 h-5 text-gray-400" />
      </div>
    </div>
    
    <!-- Simple Persian Date Picker Modal -->
    <div v-if="showPicker" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white rounded-lg p-6 w-80">
        <div class="flex justify-between items-center mb-4">
          <h3 class="text-lg font-medium">انتخاب تاریخ</h3>
          <button @click="showPicker = false" class="text-gray-400 hover:text-gray-600">
            <XMarkIcon class="w-6 h-6" />
          </button>
        </div>
        
        <div class="grid grid-cols-3 gap-2 mb-4">
          <select v-model="selectedYear" class="form-input">
            <option v-for="year in years" :key="year" :value="year">{{ year }}</option>
          </select>
          <select v-model="selectedMonth" class="form-input">
            <option v-for="(month, index) in persianMonths" :key="index" :value="index + 1">{{ month }}</option>
          </select>
          <select v-model="selectedDay" class="form-input">
            <option v-for="day in daysInMonth" :key="day" :value="day">{{ day }}</option>
          </select>
        </div>
        
        <div class="flex gap-2">
          <button @click="selectDate" class="btn btn-primary flex-1">تأیید</button>
          <button @click="showPicker = false" class="btn btn-secondary flex-1">لغو</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'
import { CalendarIcon, XMarkIcon } from '@heroicons/vue/24/outline'

const props = defineProps({
  modelValue: {
    type: String,
    default: ''
  },
  label: {
    type: String,
    default: ''
  },
  placeholder: {
    type: String,
    default: '1403/01/01'
  }
})

const emit = defineEmits(['update:modelValue'])

const showPicker = ref(false)
const displayValue = ref('')
const selectedYear = ref(1403)
const selectedMonth = ref(1)
const selectedDay = ref(1)

const persianMonths = [
  'فروردین', 'اردیبهشت', 'خرداد', 'تیر', 'مرداد', 'شهریور',
  'مهر', 'آبان', 'آذر', 'دی', 'بهمن', 'اسفند'
]

const years = computed(() => {
  const currentYear = 1403
  return Array.from({ length: 10 }, (_, i) => currentYear - 5 + i)
})

const daysInMonth = computed(() => {
  const daysCount = selectedMonth.value <= 6 ? 31 : selectedMonth.value <= 11 ? 30 : 29
  return Array.from({ length: daysCount }, (_, i) => i + 1)
})

const selectDate = () => {
  const dateString = `${selectedYear.value}/${selectedMonth.value.toString().padStart(2, '0')}/${selectedDay.value.toString().padStart(2, '0')}`
  emit('update:modelValue', dateString)
  showPicker.value = false
}

watch(() => props.modelValue, (newValue) => {
  displayValue.value = newValue
  if (newValue) {
    const [year, month, day] = newValue.split('/').map(Number)
    selectedYear.value = year
    selectedMonth.value = month
    selectedDay.value = day
  }
}, { immediate: true })
</script>
```

### src/components/forms/TypeaheadSelect.vue
```vue
<template>
  <div class="form-group">
    <label v-if="label" class="form-label">{{ label }}</label>
    <div class="relative">
      <input
        v-model="searchTerm"
        type="text"
        class="form-input"
        :placeholder="placeholder"
        @input="handleInput"
        @focus="showDropdown = true"
        @blur="handleBlur"
      />
      
      <div v-if="showDropdown && filteredOptions.length" class="absolute z-10 w-full mt-1 bg-white border border-gray-300 rounded-md shadow-lg max-h-60 overflow-auto">
        <div
          v-for="option in filteredOptions"
          :key="option.id"
          class="px-4 py-2 hover:bg-gray-100 cursor-pointer"
          @mousedown="selectOption(option)"
        >
          {{ option.name }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'

const props = defineProps({
  modelValue: {
    type: [String, Number, Object],
    default: null
  },
  options: {
    type: Array,
    default: () => []
  },
  label: {
    type: String,
    default: ''
  },
  placeholder: {
    type: String,
    default: 'جستجو...'
  },
  valueKey: {
    type: String,
    default: 'id'
  },
  textKey: {
    type: String,
    default: 'name'
  }
})

const emit = defineEmits(['update:modelValue'])

const searchTerm = ref('')
const showDropdown = ref(false)

const filteredOptions = computed(() => {
  if (!searchTerm.value) return props.options
  return props.options.filter(option => 
    option[props.textKey].toLowerCase().includes(searchTerm.value.toLowerCase())
  )
})

const selectOption = (option) => {
  searchTerm.value = option[props.textKey]
  emit('update:modelValue', option[props.valueKey])
  showDropdown.value = false
}

const handleInput = () => {
  showDropdown.value = true
}

const handleBlur = () => {
  setTimeout(() => {
    showDropdown.value = false
  }, 200)
}

watch(() => props.modelValue, (newValue) => {
  const selectedOption = props.options.find(option => option[props.valueKey] === newValue)
  if (selectedOption) {
    searchTerm.value = selectedOption[props.textKey]
  }
})
</script>
```

## Static JSON Mock Data Store

### src/services/mockData.js
```javascript
// Mock data for the entire application
export const mockData = {
  // Authentication
  users: [
    {
      id: 1,
      username: 'admin',
      password: '123456',
      company: 'شرکت مسافرتی آسمان',
      role: 'admin',
      fullName: 'مدیر سیستم'
    }
  ],

  // Dashboard stats
  dashboardStats: {
    totalIncome: 2500000000,
    totalExpense: 1800000000,
    accountsReceivable: 450000000,
    accountsPayable: 320000000,
    incomeChart: [
      { month: 'فروردین', amount: 400000000 },
      { month: 'اردیبهشت', amount: 450000000 },
      { month: 'خرداد', amount: 380000000 },
      { month: 'تیر', amount: 520000000 },
      { month: 'مرداد', amount: 480000000 },
      { month: 'شهریور', amount: 270000000 }
    ]
  },

  // Sales documents
  salesDocuments: [
    {
      id: 1052,
      docNumber: 'SL-2024-1052',
      saleDate: '1403/05/01',
      passengerName: 'احمد محمدی',
      itinerary: 'تهران -> دبی',
      flightDate: '1403/05/08',
      status: 'Unissued',
      amount: 15000000,
      currency: 'IRR',
      supplier: 'ایران ایر',
      serviceType: 'AirTicket'
    },
    {
      id: 1053,
      docNumber: 'SL-2024-1053',
      saleDate: '1403/05/02',
      passengerName: 'فاطمه احمدی',
      itinerary: 'تهران -> استانبول',
      flightDate: '1403/05/15',
      status: 'Issued',
      amount: 18500000,
      currency: 'IRR',
      supplier: 'ترکیش ایرلاینز',
      serviceType: 'AirTicket'
    },
    {
      id: 1054,
      docNumber: 'SL-2024-1054',
      saleDate: '1403/05/03',
      passengerName: 'علی رضایی',
      itinerary: 'تهران -> مشهد',
      flightDate: '1403/05/06',
      status: 'Canceled',
      amount: 3200000,
      currency: 'IRR',
      supplier: 'آسمان',
      serviceType: 'AirTicket'
    }
  ],

  // Airlines
  airlines: [
    { id: 1, name: 'ایران ایر', code: 'IR', active: true },
    { id: 2, name: 'آسمان', code: 'EP', active: true },
    { id: 3, name: 'ماهان', code: 'W5', active: true },
    { id: 4, name: 'ترکیش ایرلاینز', code: 'TK', active: true },
    { id: 5, name: 'امارات', code: 'EK', active: true }
  ],

  // Counterparties
  counterparties: [
    {
      id: 1,
      name: 'شرکت گردشگری پارس',
      type: 'Supplier',
      phone: '021-88776655',
      email: 'info@pars-travel.com',
      openingBalanceIRR: 25000000,
      openingBalanceUSD: 1500
    },
    {
      id: 2,
      name: 'آقای محمد احمدی',
      type: 'Customer',
      phone: '0912-3456789',
      email: 'ahmad@email.com',
      openingBalanceIRR: -5000000,
      openingBalanceUSD: 0
    }
  ],

  // Banks and accounts
  banks: [
    {
      id: 1,
      name: 'بانک ملی',
      accountNumber: '0123456789',
      iban: 'IR123456789012345678901234',
      currency: 'IRR',
      openingBalance: 150000000
    },
    {
      id: 2,
      name: 'بانک پاسارگاد',
      accountNumber: '9876543210',
      iban: 'IR987654321098765432109876',
      currency: 'USD',
      openingBalance: 25000
    }
  ],

  // Vouchers
  vouchers: [
    {
      id: 1,
      voucherNumber: 'V-2024-001',
      date: '1403/05/01',
      counterpartyId: 1,
      amount: 50000000,
      currency: 'IRR',
      bankAccountId: 1,
      trackingNumber: 'TRK123456',
      description: 'پرداخت بابت بلیط هواپیما'
    }
  ],

  // Origins and destinations
  locations: [
    { id: 1, name: 'تهران', code: 'THR', type: 'City' },
    { id: 2, name: 'مشهد', code: 'MHD', type: 'City' },
    { id: 3, name: 'اصفهان', code: 'IFN', type: 'City' },
    { id: 4, name: 'دبی', code: 'DXB', type: 'City' },
    { id: 5, name: 'استانبول', code: 'IST', type: 'City' }
  ],

  // Document number counter
  documentCounters: {
    sales: 1054,
    voucher: 1,
    cost: 1,
    income: 1,
    transfer: 1
  },

  // Last document date for default
  lastDocumentDate: '1403/05/03'
}

// Helper functions
export const getNextDocumentNumber = (type) => {
  const counter = mockData.documentCounters[type]
  mockData.documentCounters[type] = counter + 1
  
  const prefixes = {
    sales: 'SL',
    voucher: 'V',
    cost: 'C',
    income: 'I',
    transfer: 'T'
  }
  
  return `${prefixes[type]}-2024-${(counter + 1).toString().padStart(4, '0')}`
}

export const getDocumentsByStatus = (status) => {
  return mockData.salesDocuments.filter(doc => doc.status === status)
}

export const addSalesDocument = (document) => {
  const newDoc = {
    ...document,
    id: Math.max(...mockData.salesDocuments.map(d => d.id)) + 1,
    docNumber: getNextDocumentNumber('sales'),
    saleDate: mockData.lastDocumentDate
  }
  mockData.salesDocuments.push(newDoc)
  return newDoc
}

export const updateDocumentStatus = (id, status) => {
  const doc = mockData.salesDocuments.find(d => d.id === id)
  if (doc) {
    doc.status = status
  }
  return doc
}

export const deleteDocument = (id) => {
  const index = mockData.salesDocuments.findIndex(d => d.id === id)
  if (index > -1) {
    mockData.salesDocuments.splice(index, 1)
    return true
  }
  return false
}
```

## Template Pages

### src/views/auth/LoginView.vue
```vue
<template>
  <div class="min-h-screen bg-gray-50 flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8" dir="rtl">
    <div class="max-w-md w-full space-y-8">
      <div class="text-center">
        <h2 class="mt-6 text-3xl font-extrabold text-gray-900">
          ورود به سیستم حسابداری مسافرتی
        </h2>
      </div>
      
      <form class="mt-8 space-y-6" @submit.prevent="handleLogin">
        <div class="card">
          <div class="space-y-4">
            <div>
              <label class="form-label">شرکت/پنل</label>
              <input
                v-model="form.company"
                type="text"
                class="form-input"
                placeholder="نام شرکت"
                required
              />
            </div>
            
            <div>
              <label class="form-label">نام کاربری</label>
              <input
                v-model="form.username"
                type="text"
                class="form-input"
                placeholder="نام کاربری"
                required
              />
            </div>
            
            <div>
              <label class="form-label">رمز عبور</label>
              <input
                v-model="form.password"
                type="password"
                class="form-input"
                placeholder="رمز عبور"
                required
              />
            </div>
            
            <CaptchaInput v-model="form.captcha" />
          </div>
          
          <div class="mt-6">
            <AppButton type="submit" class="w-full" :disabled="loading">
              <AppSpinner v-if="loading" class="w-4 h-4 ml-2" />
              ورود
            </AppButton>
          </div>
          
          <div v-if="error" class="mt-4 text-red-600 text-sm text-center">
            {{ error }}
          </div>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import AppButton from '@/components/base/AppButton.vue'
import AppSpinner from '@/components/base/AppSpinner.vue'
import CaptchaInput from '@/components/forms/CaptchaInput.vue'

const router = useRouter()
const authStore = useAuthStore()

const loading = ref(false)
const error = ref('')

const form = reactive({
  company: 'شرکت مسافرتی آسمان',
  username: 'admin',
  password: '123456',
  captcha: ''
})

const handleLogin = async () => {
  loading.value = true
  error.value = ''
  
  try {
    await authStore.login(form)
    router.push('/')
  } catch (err) {
    error.value = err.message || 'خطا در ورود به سیستم'
  } finally {
    loading.value = false
  }
}
</script>
```

### src/views/sales/UnissuedTicketsView.vue
```vue
<template>
  <div class="p-6" dir="rtl">
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold text-gray-900">بلیط‌های صادر نشده</h1>
      <AppButton @click="$router.push('/sales/new')">
        <PlusIcon class="w-4 h-4 ml-2" />
        سند جدید
      </AppButton>
    </div>
    
    <!-- Filters -->
    <div class="card mb-6">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div>
          <label class="form-label">از تاریخ</label>
          <PersianDatePicker v-model="filters.fromDate" />
        </div>
        <div>
          <label class="form-label">تا تاریخ</label>
          <PersianDatePicker v-model="filters.toDate" />
        </div>
        <div>
          <label class="form-label">طرف حساب</label>
          <TypeaheadSelect
            v-model="filters.counterpartyId"
            :options="counterparties"
            placeholder="انتخاب طرف حساب"
          />
        </div>
        <div class="flex items-end">
          <AppButton @click="applyFilters" class="ml-2">اعمال فیلتر</AppButton>
          <AppButton variant="secondary" @click="clearFilters">پاک کردن</AppButton>
        </div>
      </div>
    </div>
    
    <!-- Data Table -->
    <div class="card">
      <DataTable
        :data="filteredDocuments"
        :columns="columns"
        :loading="loading"
        @row-action="handleRowAction"
      />
    </div>
    
    <!-- Export Buttons -->
    <div class="mt-4 flex gap-2">
      <AppButton variant="secondary" @click="exportToPDF">
        <DocumentArrowDownIcon class="w-4 h-4 ml-2" />
        خروجی PDF
      </AppButton>
      <AppButton variant="secondary" @click="exportToExcel">
        <TableCellsIcon class="w-4 h-4 ml-2" />
        خروجی Excel
      </AppButton>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useSalesStore } from '@/stores/sales'
import { useSettingsStore } from '@/stores/settings'
import { PlusIcon, DocumentArrowDownIcon, TableCellsIcon } from '@heroicons/vue/24/outline'
import AppButton from '@/components/base/AppButton.vue'
import DataTable from '@/components/data/DataTable.vue'
import PersianDatePicker from '@/components/forms/PersianDatePicker.vue'
import TypeaheadSelect from '@/components/forms/TypeaheadSelect.vue'
import { exportToPDF, exportToExcel } from '@/services/exportService'

const router = useRouter()
const salesStore = useSalesStore()
const settingsStore = useSettingsStore()

const loading = ref(false)
const filters = ref({
  fromDate: '',
  toDate: '',
  counterpartyId: null
})

const columns = [
  { key: 'docNumber', title: 'شماره سند', sortable: true },
  { key: 'saleDate', title: 'تاریخ فروش', sortable: true },
  { key: 'passengerName', title: 'نام مسافر', sortable: true },
  { key: 'itinerary', title: 'مسیر', sortable: false },
  { key: 'flightDate', title: 'تاریخ پرواز', sortable: true },
  { key: 'amount', title: 'مبلغ', sortable: true, format: 'currency' },
  { key: 'supplier', title: 'تأمین‌کننده', sortable: true },
  { 
    key: 'actions', 
    title: 'عملیات', 
    sortable: false,
    actions: [
      { key: 'edit', title: 'ویرایش', variant: 'primary' },
      { key: 'issue', title: 'صدور', variant: 'success' },
      { key: 'cancel', title: 'لغو', variant: 'error' },
      { key: 'delete', title: 'حذف', variant: 'error' }
    ]
  }
]

const counterparties = computed(() => settingsStore.counterparties)
const unissuedDocuments = computed(() => salesStore.getDocumentsByStatus('Unissued'))

const filteredDocuments = computed(() => {
  let docs = unissuedDocuments.value
  
  // Apply filters
  if (filters.value.counterpartyId) {
    docs = docs.filter(doc => doc.counterpartyId === filters.value.counterpartyId)
  }
  
  // Sort by flight date (nearest first) and apply color rules
  return docs.map(doc => {
    const today = new Date()
    const flightDate = new Date(doc.flightDate.replace(/\//g, '-'))
    const daysDiff = Math.ceil((flightDate - today) / (1000 * 60 * 60 * 24))
    
    return {
      ...doc,
      _rowClass: daysDiff <= 5 ? 'status-red' : 'status-blue',
      _sortKey: daysDiff
    }
  }).sort((a, b) => a._sortKey - b._sortKey)
})

const handleRowAction = (action, row) => {
  switch (action) {
    case 'edit':
      router.push(`/sales/edit/${row.id}`)
      break
    case 'issue':
      salesStore.updateDocumentStatus(row.id, 'Issued')
      break
    case 'cancel':
      salesStore.updateDocumentStatus(row.id, 'Canceled')
      break
    case 'delete':
      if (confirm('آیا از حذف این سند اطمینان دارید؟')) {
        salesStore.deleteDocument(row.id)
      }
      break
  }
}

const applyFilters = () => {
  // Filters are reactive, so they're automatically applied
}

const clearFilters = () => {
  filters.value = {
    fromDate: '',
    toDate: '',
    counterpartyId: null
  }
}

const exportToPDF = () => {
  exportToPDF(filteredDocuments.value, 'unissued-tickets')
}

const exportToExcel = () => {
  exportToExcel(filteredDocuments.value, 'unissued-tickets')
}

onMounted(() => {
  salesStore.loadDocuments()
  settingsStore.loadCounterparties()
})
</script>
```

### src/views/sales/CreateDocumentView.vue
```vue
<template>
  <div class="p-6" dir="rtl">
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold text-gray-900">ایجاد سند فروش جدید</h1>
      <div class="text-sm text-gray-600">
        شماره سند: <span class="font-medium">خودکار</span>
      </div>
    </div>
    
    <form @submit.prevent="handleSubmit">
      <!-- Buyer Section -->
      <div class="card mb-6">
        <h3 class="text-lg font-medium mb-4">اطلاعات خریدار</h3>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <TypeaheadSelect
            v-model="form.buyerId"
            :options="counterparties"
            label="خریدار"
            placeholder="انتخاب خریدار"
          />
          <div class="flex items-center">
            <ToggleSwitch
              v-model="form.isBuyerTraveler"
              label="خریدار همان مسافر اصلی است"
            />
          </div>
        </div>
      </div>
      
      <!-- Passengers Section -->
      <div class="card mb-6">
        <div class="flex justify-between items-center mb-4">
          <h3 class="text-lg font-medium">اطلاعات مسافران</h3>
          <AppButton type="button" variant="secondary" @click="addPassenger">
            <PlusIcon class="w-4 h-4 ml-2" />
            افزودن مسافر
          </AppButton>
        </div>
        
        <div v-for="(passenger, index) in form.passengers" :key="index" class="border rounded-lg p-4 mb-4">
          <div class="flex justify-between items-center mb-3">
            <h4 class="font-medium">مسافر {{ index + 1 }}</h4>
            <AppButton
              v-if="form.passengers.length > 1"
              type="button"
              variant="error"
              @click="removePassenger(index)"
            >
              <TrashIcon class="w-4 h-4" />
            </AppButton>
          </div>
          
          <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
            <TextInput
              v-model="passenger.name"
              label="نام و نام خانوادگی"
              placeholder="نام مسافر"
              required
            />
            <div>
              <label class="form-label">گروه سنی</label>
              <select v-model="passenger.ageGroup" class="form-input">
                <option value="Adult">بزرگسال</option>
                <option value="Child">کودک</option>
                <option value="Infant">نوزاد</option>
              </select>
            </div>
            <TextInput
              v-model="passenger.nationalId"
              label="کد ملی/پاسپورت"
              placeholder="کد ملی یا شماره پاسپورت"
            />
          </div>
        </div>
      </div>
      
      <!-- Services Section -->
      <div class="card mb-6">
        <h3 class="text-lg font-medium mb-4">خدمات</h3>
        
        <!-- Service Type Tabs -->
        <div class="border-b border-gray-200 mb-4">
          <nav class="-mb-px flex space-x-8">
            <button
              v-for="serviceType in serviceTypes"
              :key="serviceType.key"
              type="button"
              :class="[
                'py-2 px-1 border-b-2 font-medium text-sm',
                activeServiceType === serviceType.key
                  ? 'border-blue-500 text-blue-600'
                  : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
              ]"
              @click="activeServiceType = serviceType.key"
            >
              {{ serviceType.title }}
            </button>
          </nav>
        </div>
        
        <!-- Air Ticket Form -->
        <div v-if="activeServiceType === 'AirTicket'" class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <TypeaheadSelect
            v-model="form.services[0].airlineId"
            :options="airlines"
            label="ایرلاین"
            placeholder="انتخاب ایرلاین"
          />
          <TypeaheadSelect
            v-model="form.services[0].originId"
            :options="locations"
            label="مبدأ"
            placeholder="انتخاب مبدأ"
          />
          <TypeaheadSelect
            v-model="form.services[0].destinationId"
            :options="locations"
            label="مقصد"
            placeholder="انتخاب مقصد"
          />
          <PersianDatePicker
            v-model="form.services[0].flightDate"
            label="تاریخ پرواز"
          />
          <CurrencyInput
            v-model="form.services[0].price"
            :currency="form.services[0].currency"
            label="قیمت"
          />
          <div>
            <label class="form-label">ارز</label>
            <select v-model="form.services[0].currency" class="form-input">
              <option value="IRR">ریال</option>
              <option value="USD">دلار</option>
              <option value="EUR">یورو</option>
            </select>
          </div>
        </div>
      </div>
      
      <!-- Finance Section -->
      <div class="card mb-6">
        <h3 class="text-lg font-medium mb-4">اطلاعات مالی</h3>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <CurrencyInput
            v-model="form.totalAmount"
            label="مبلغ کل"
            currency="IRR"
          />
          <CurrencyInput
            v-model="form.paidAmount"
            label="مبلغ پرداختی"
            currency="IRR"
          />
        </div>
      </div>
      
      <!-- Action Buttons -->
      <div class="flex gap-4">
        <AppButton type="submit" :disabled="loading">
          <AppSpinner v-if="loading" class="w-4 h-4 ml-2" />
          ذخیره سند
        </AppButton>
        <AppButton type="button" variant="secondary" @click="$router.go(-1)">
          انصراف
        </AppButton>
      </div>
    </form>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useSalesStore } from '@/stores/sales'
import { useSettingsStore } from '@/stores/settings'
import { PlusIcon, TrashIcon } from '@heroicons/vue/24/outline'
import AppButton from '@/components/base/AppButton.vue'
import AppSpinner from '@/components/base/AppSpinner.vue'
import TextInput from '@/components/forms/TextInput.vue'
import CurrencyInput from '@/components/forms/CurrencyInput.vue'
import PersianDatePicker from '@/components/forms/PersianDatePicker.vue'
import TypeaheadSelect from '@/components/forms/TypeaheadSelect.vue'
import ToggleSwitch from '@/components/forms/ToggleSwitch.vue'

const router = useRouter()
const salesStore = useSalesStore()
const settingsStore = useSettingsStore()

const loading = ref(false)
const activeServiceType = ref('AirTicket')

const serviceTypes = [
  { key: 'AirTicket', title: 'بلیط هواپیما' },
  { key: 'TrainTicket', title: 'بلیط قطار/اتوبوس' },
  { key: 'Hotel', title: 'هتل' },
  { key: 'Tour', title: 'تور' },
  { key: 'Mixed', title: 'ترکیبی' }
]

const form = reactive({
  buyerId: null,
  isBuyerTraveler: true,
  passengers: [
    { name: '', ageGroup: 'Adult', nationalId: '' }
  ],
  services: [
    {
      type: 'AirTicket',
      airlineId: null,
      originId: null,
      destinationId: null,
      flightDate: '',
      price: 0,
      currency: 'IRR'
    }
  ],
  totalAmount: 0,
  paidAmount: 0
})

const counterparties = computed(() => settingsStore.counterparties.filter(c => c.type === 'Customer'))
const airlines = computed(() => settingsStore.airlines)
const locations = computed(() => settingsStore.locations)

const addPassenger = () => {
  form.passengers.push({ name: '', ageGroup: 'Adult', nationalId: '' })
}

const removePassenger = (index) => {
  form.passengers.splice(index, 1)
}

const handleSubmit = async () => {
  loading.value = true
  
  try {
    const document = {
      ...form,
      passengerName: form.passengers[0].name,
      itinerary: getItinerary(),
      flightDate: form.services[0].flightDate,
      amount: form.totalAmount,
      currency: form.services[0].currency,
      supplier: getSupplierName(),
      serviceType: activeServiceType.value,
      status: 'Unissued'
    }
    
    await salesStore.createDocument(document)
    router.push('/sales/tickets/unissued')
  } catch (error) {
    console.error('Error creating document:', error)
  } finally {
    loading.value = false
  }
}

const getItinerary = () => {
  const service = form.services[0]
  const origin = locations.value.find(l => l.id === service.originId)?.name || ''
  const destination = locations.value.find(l => l.id === service.destinationId)?.name || ''
  return `${origin} -> ${destination}`
}

const getSupplierName = () => {
  const service = form.services[0]
  return airlines.value.find(a => a.id === service.airlineId)?.name || ''
}

onMounted(() => {
  settingsStore.loadCounterparties()
  settingsStore.loadAirlines()
  settingsStore.loadLocations()
})
</script>
```

## Router Configuration

### src/router/index.js
```javascript
import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

// Layouts
import MainLayout from '@/components/layout/MainLayout.vue'

// Views
import LoginView from '@/views/auth/LoginView.vue'
import DashboardView from '@/views/dashboard/DashboardView.vue'

// Sales
import CreateDocumentView from '@/views/sales/CreateDocumentView.vue'
import UnissuedTicketsView from '@/views/sales/UnissuedTicketsView.vue'
import IssuedTicketsView from '@/views/sales/IssuedTicketsView.vue'
import CanceledTicketsView from '@/views/sales/CanceledTicketsView.vue'

// Finance
import VoucherView from '@/views/finance/VoucherView.vue'
import CostsView from '@/views/finance/CostsView.vue'
import IncomesView from '@/views/finance/IncomesView.vue'
import TransferView from '@/views/finance/TransferView.vue'

// Reports
import ProfitLossView from '@/views/reports/ProfitLossView.vue'
import AccountsReceivableView from '@/views/reports/AccountsReceivableView.vue'

// Settings
import UsersView from '@/views/settings/UsersView.vue'
import AirlinesView from '@/views/settings/AirlinesView.vue'
import BanksView from '@/views/settings/BanksView.vue'
import CounterpartiesView from '@/views/settings/CounterpartiesView.vue'

const routes = [
  {
    path: '/login',
    name: 'Login',
    component: LoginView,
    meta: { requiresAuth: false }
  },
  {
    path: '/',
    component: MainLayout,
    meta: { requiresAuth: true },
    children: [
      {
        path: '',
        name: 'Dashboard',
        component: DashboardView
      },
      // Sales routes
      {
        path: 'sales/new',
        name: 'CreateDocument',
        component: CreateDocumentView
      },
      {
        path: 'sales/tickets/unissued',
        name: 'UnissuedTickets',
        component: UnissuedTicketsView
      },
      {
        path: 'sales/tickets/issued',
        name: 'IssuedTickets',
        component: IssuedTicketsView
      },
      {
        path: 'sales/tickets/canceled',
        name: 'CanceledTickets',
        component: CanceledTicketsView
      },
      // Finance routes
      {
        path: 'finance/vouchers/new',
        name: 'NewVoucher',
        component: VoucherView
      },
      {
        path: 'finance/transactions/costs',
        name: 'Costs',
        component: CostsView
      },
      {
        path: 'finance/transactions/incomes',
        name: 'Incomes',
        component: IncomesView
      },
      {
        path: 'finance/transfers/new',
        name: 'Transfer',
        component: TransferView
      },
      // Reports routes
      {
        path: 'reports/pl',
        name: 'ProfitLoss',
        component: ProfitLossView
      },
      {
        path: 'reports/ar',
        name: 'AccountsReceivable',
        component: AccountsReceivableView
      },
      // Settings routes
      {
        path: 'settings/users',
        name: 'Users',
        component: UsersView
      },
      {
        path: 'settings/airlines',
        name: 'Airlines',
        component: AirlinesView
      },
      {
        path: 'settings/banks',
        name: 'Banks',
        component: BanksView
      },
      {
        path: 'settings/counterparties',
        name: 'Counterparties',
        component: CounterpartiesView
      }
    ]
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// Navigation guard
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next('/login')
  } else if (to.path === '/login' && authStore.isAuthenticated) {
    next('/')
  } else {
    next()
  }
})

export default router
```

## Pinia Stores

### src/stores/auth.js
```javascript
import { defineStore } from 'pinia'
import { mockData } from '@/services/mockData'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null,
    token: null,
    isAuthenticated: false
  }),

  actions: {
    async login(credentials) {
      // Mock authentication
      const user = mockData.users.find(
        u => u.username === credentials.username && 
             u.password === credentials.password &&
             u.company === credentials.company
      )

      if (!user) {
        throw new Error('نام کاربری یا رمز عبور اشتباه است')
      }

      if (!credentials.captcha) {
        throw new Error('لطفاً کد امنیتی را وارد کنید')
      }

      // Simulate API delay
      await new Promise(resolve => setTimeout(resolve, 1000))

      this.user = user
      this.token = 'mock-jwt-token'
      this.isAuthenticated = true

      // Store in localStorage
      localStorage.setItem('auth_token', this.token)
      localStorage.setItem('auth_user', JSON.stringify(user))
    },

    logout() {
      this.user = null
      this.token = null
      this.isAuthenticated = false
      
      localStorage.removeItem('auth_token')
      localStorage.removeItem('auth_user')
    },

    initializeAuth() {
      const token = localStorage.getItem('auth_token')
      const user = localStorage.getItem('auth_user')
      
      if (token && user) {
        this.token = token
        this.user = JSON.parse(user)
        this.isAuthenticated = true
      }
    }
  }
})
```

### src/stores/sales.js
```javascript
import { defineStore } from 'pinia'
import { mockData, getDocumentsByStatus, addSalesDocument, updateDocumentStatus, deleteDocument } from '@/services/mockData'

export const useSalesStore = defineStore('sales', {
  state: () => ({
    documents: [],
    loading: false,
    error: null
  }),

  getters: {
    getDocumentsByStatus: (state) => (status) => {
      return state.documents.filter(doc => doc.status === status)
    }
  },

  actions: {
    async loadDocuments() {
      this.loading = true
      try {
        // Simulate API call
        await new Promise(resolve => setTimeout(resolve, 500))
        this.documents = mockData.salesDocuments
      } catch (error) {
        this.error = error.message
      } finally {
        this.loading = false
      }
    },

    async createDocument(document) {
      this.loading = true
      try {
        const newDoc = addSalesDocument(document)
        this.documents.push(newDoc)
        return newDoc
      } catch (error) {
        this.error = error.message
        throw error
      } finally {
        this.loading = false
      }
    },

    updateDocumentStatus(id, status) {
      const doc = updateDocumentStatus(id, status)
      if (doc) {
        const index = this.documents.findIndex(d => d.id === id)
        if (index > -1) {
          this.documents[index] = doc
        }
      }
    },

    deleteDocument(id) {
      if (deleteDocument(id)) {
        const index = this.documents.findIndex(d => d.id === id)
        if (index > -1) {
          this.documents.splice(index, 1)
        }
      }
    }
  }
})
```

### src/stores/settings.js
```javascript
import { defineStore } from 'pinia'
import { mockData } from '@/services/mockData'

export const useSettingsStore = defineStore('settings', {
  state: () => ({
    airlines: [],
    counterparties: [],
    banks: [],
    locations: [],
    loading: false
  }),

  actions: {
    async loadAirlines() {
      this.loading = true
      try {
        await new Promise(resolve => setTimeout(resolve, 300))
        this.airlines = mockData.airlines
      } finally {
        this.loading = false
      }
    },

    async loadCounterparties() {
      this.loading = true
      try {
        await new Promise(resolve => setTimeout(resolve, 300))
        this.counterparties = mockData.counterparties
      } finally {
        this.loading = false
      }
    },

    async loadBanks() {
      this.loading = true
      try {
        await new Promise(resolve => setTimeout(resolve, 300))
        this.banks = mockData.banks
      } finally {
        this.loading = false
      }
    },

    async loadLocations() {
      this.loading = true
      try {
        await new Promise(resolve => setTimeout(resolve, 300))
        this.locations = mockData.locations
      } finally {
        this.loading = false
      }
    }
  }
})
```

## Export Service

### src/services/exportService.js
```javascript
import jsPDF from 'jspdf'
import * as XLSX from 'xlsx'

export const exportToPDF = (data, filename) => {
  // Mock PDF export - placeholder implementation
  console.log('Exporting to PDF:', filename, data)
  
  // Create a simple PDF with jsPDF
  const doc = new jsPDF()
  doc.text('Travel Accounting System Report', 20, 20)
  doc.text(`Generated on: ${new Date().toLocaleDateString('fa-IR')}`, 20, 30)
  doc.text(`Total Records: ${data.length}`, 20, 40)
  
  // Add table headers and data (simplified)
  let yPosition = 60
  data.slice(0, 10).forEach((item, index) => {
    doc.text(`${index + 1}. ${item.docNumber || item.name || 'N/A'}`, 20, yPosition)
    yPosition += 10
  })
  
  doc.save(`${filename}.pdf`)
}

export const exportToExcel = (data, filename) => {
  // Mock Excel export
  console.log('Exporting to Excel:', filename, data)
  
  const worksheet = XLSX.utils.json_to_sheet(data)
  const workbook = XLSX.utils.book_new()
  XLSX.utils.book_append_sheet(workbook, worksheet, 'Data')
  
  XLSX.writeFile(workbook, `${filename}.xlsx`)
}
```

## Additional Components

### src/components/base/AppSpinner.vue
```vue
<template>
  <div class="inline-block animate-spin rounded-full border-2 border-solid border-current border-r-transparent align-[-0.125em] motion-reduce:animate-[spin_1.5s_linear_infinite]" :class="sizeClass">
    <span class="!absolute !-m-px !h-px !w-px !overflow-hidden !whitespace-nowrap !border-0 !p-0 ![clip:rect(0,0,0,0)]">
      Loading...
    </span>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  size: {
    type: String,
    default: 'md',
    validator: (value) => ['sm', 'md', 'lg'].includes(value)
  }
})

const sizeClass = computed(() => {
  const sizes = {
    sm: 'h-4 w-4',
    md: 'h-6 w-6',
    lg: 'h-8 w-8'
  }
  return sizes[props.size]
})
</script>
```

### src/components/forms/CaptchaInput.vue
```vue
<template>
  <div class="form-group">
    <label class="form-label">کد امنیتی</label>
    <div class="flex gap-3">
      <div class="flex-1">
        <input
          :value="modelValue"
          type="text"
          class="form-input"
          placeholder="کد امنیتی را وارد کنید"
          @input="$emit('update:modelValue', $event.target.value)"
        />
      </div>
      <div class="w-24 h-10 bg-gray-200 rounded border flex items-center justify-center text-lg font-bold text-gray-700">
        {{ captchaCode }}
      </div>
      <button
        type="button"
        class="px-3 py-2 text-sm bg-gray-100 hover:bg-gray-200 rounded border"
        @click="refreshCaptcha"
      >
        🔄
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

defineProps({
  modelValue: {
    type: String,
    default: ''
  }
})

defineEmits(['update:modelValue'])

const captchaCode = ref('')

const generateCaptcha = () => {
  const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789'
  let result = ''
  for (let i = 0; i < 5; i++) {
    result += chars.charAt(Math.floor(Math.random() * chars.length))
  }
  return result
}

const refreshCaptcha = () => {
  captchaCode.value = generateCaptcha()
}

onMounted(() => {
  refreshCaptcha()
})
</script>
```