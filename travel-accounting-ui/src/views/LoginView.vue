<template>
  <div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-blue-50 to-indigo-100 py-12 px-4 sm:px-6 lg:px-8" dir="rtl">
    <div class="max-w-md w-full space-y-8">
      <!-- Header -->
      <div class="text-center">
        <div class="mx-auto h-10 w-10 bg-primary-600 rounded-full flex items-center justify-center mb-4">
          <svg class="h-5 w-5 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1" />
          </svg>
        </div>
        <h2 class="text-3xl font-bold text-gray-900 mb-2">
          سیستم حسابداری سفر
        </h2>
        <p class="text-gray-600">
          لطفاً برای ورود به سیستم اطلاعات خود را وارد کنید
        </p>
      </div>

      <!-- Login Form -->
      <form @submit.prevent="handleLogin" class="mt-8 space-y-6">
        <div class="bg-white rounded-lg shadow-md p-6 space-y-4">
          <!-- Company Selection -->
          <div>
            <label for="company" class="block text-sm font-medium text-gray-700 mb-1">
              شرکت
            </label>
            <TypeaheadSelect
              v-model="form.company"
              :options="companies"
              :search-fields="['name', 'code']"
              option-label="name"
              option-value="code"
              placeholder="انتخاب شرکت..."
              :error="errors.company"
            />
          </div>

          <!-- Username -->
          <div>
            <label for="username" class="block text-sm font-medium text-gray-700 mb-1">
              نام کاربری
            </label>
            <input
              id="username"
              v-model="form.username"
              type="text"
              required
              class="input-field"
              :class="{ 'border-red-300': errors.username }"
              placeholder="نام کاربری خود را وارد کنید"
              autocomplete="username"
            />
            <p v-if="errors.username" class="mt-1 text-sm text-red-600">
              {{ errors.username }}
            </p>
          </div>

          <!-- Password -->
          <div>
            <label for="password" class="block text-sm font-medium text-gray-700 mb-1">
              رمز عبور
            </label>
            <div class="relative">
              <input
                id="password"
                v-model="form.password"
                :type="showPassword ? 'text' : 'password'"
                required
                class="input-field pl-10"
                :class="{ 'border-red-300': errors.password }"
                placeholder="رمز عبور خود را وارد کنید"
                autocomplete="current-password"
              />
              <button
                type="button"
                @click="showPassword = !showPassword"
                class="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-gray-600"
              >
                <svg v-if="showPassword" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                </svg>
                <svg v-else class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.878 9.878L3 3m6.878 6.878L21 21" />
                </svg>
              </button>
            </div>
            <p v-if="errors.password" class="mt-1 text-sm text-red-600">
              {{ errors.password }}
            </p>
          </div>



          <!-- Remember Me -->
          <div class="flex items-center">
            <input
              id="remember-me"
              v-model="form.rememberMe"
              type="checkbox"
              class="h-4 w-4 text-primary-600 focus:ring-primary-500 border-gray-300 rounded"
            />
            <label for="remember-me" class="mr-2 block text-sm text-gray-900">
              مرا به خاطر بسپار
            </label>
          </div>
        </div>

        <!-- Error Message -->
        <div v-if="loginError" class="bg-red-50 border border-red-200 rounded-md p-4">
          <div class="flex">
            <div class="flex-shrink-0">
              <svg class="h-5 w-5 text-red-400" viewBox="0 0 20 20" fill="currentColor">
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd" />
            </svg>
            </div>
            <div class="mr-3">
              <p class="text-sm text-red-800">
                {{ loginError }}
              </p>
            </div>
          </div>
        </div>

        <!-- Login Button -->
        <div>
          <AppButton
            type="submit"
            variant="primary"
            size="lg"
            :loading="isLoading"
            :disabled="!isFormValid"
            full-width
          >
            ورود به سیستم
          </AppButton>
        </div>

        <!-- Additional Links -->
        <div class="text-center space-y-2">
          <a href="#" class="text-sm text-blue-600 hover:text-blue-500">
            رمز عبور خود را فراموش کرده‌اید؟
          </a>
        </div>
      </form>

      <!-- Footer -->
      <div class="text-center text-xs text-gray-500 mt-8">
        <p>© ۱۴۰۳ سیستم حسابداری سفر. تمامی حقوق محفوظ است.</p>
        <p class="mt-1">نسخه ۱.۰.۰</p>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import AppButton from '../components/AppButton.vue'
import TypeaheadSelect from '../components/TypeaheadSelect.vue'
export default {
  name: 'LoginView',
  components: {
    AppButton,
    TypeaheadSelect
  },
  setup() {
    // Router and stores
    const router = useRouter()
    const authStore = useAuthStore()
    
    // Reactive data
    const isLoading = ref(false)
    const showPassword = ref(false)
    const loginError = ref('')
    
    // Form data
    const form = ref({
      company: null,
      username: '',
      password: '',
      rememberMe: false
    })
    
    // Form errors
    const errors = ref({
      company: '',
      username: '',
      password: ''
    })
    
    // Companies list
    const companies = ref([
      { id: 1, name: 'شرکت دمو', code: 'demo' },
      { id: 2, name: 'شرکت گردشگری آسمان', code: 'ASM001' },
      { id: 3, name: 'آژانس مسافرتی پارس', code: 'PRS002' },
      { id: 4, name: 'دفتر خدمات مسافرتی ایران', code: 'IRN003' }
    ])
    
    // Computed properties
    const isFormValid = computed(() => {
      return form.value.company &&
             form.value.username.trim() &&
             form.value.password.trim() &&
             !Object.values(errors.value).some(error => error)
    })
    
    // Methods
    const validateForm = () => {
      // Reset errors
      Object.keys(errors.value).forEach(key => {
        errors.value[key] = ''
      })
      
      let isValid = true
      
      // Company validation
      if (!form.value.company) {
        errors.value.company = 'انتخاب شرکت الزامی است'
        isValid = false
      }
      
      // Username validation
      if (!form.value.username.trim()) {
        errors.value.username = 'نام کاربری الزامی است'
        isValid = false
      } else if (form.value.username.length < 3) {
        errors.value.username = 'نام کاربری باید حداقل ۳ کاراکتر باشد'
        isValid = false
      }
      
      // Password validation
      if (!form.value.password.trim()) {
        errors.value.password = 'رمز عبور الزامی است'
        isValid = false
      } else if (form.value.password.length < 6) {
        errors.value.password = 'رمز عبور باید حداقل ۶ کاراکتر باشد'
        isValid = false
      }
      
      return isValid
    }
    
    const handleLogin = async () => {
      if (!validateForm()) {
        return
      }
      
      isLoading.value = true
      loginError.value = ''
      
      try {
        const loginData = {
          company: form.value.company,
          username: form.value.username,
          password: form.value.password,
          rememberMe: form.value.rememberMe
        }
        
        await authStore.login(loginData)
        
        // Redirect to dashboard
        router.push('/')
        
      } catch (error) {
        loginError.value = error.message || 'خطا در ورود به سیستم'
        
        // Clear sensitive data on error
        form.value.password = ''
        
      } finally {
        isLoading.value = false
      }
    }
    


    
    // Lifecycle
    onMounted(() => {
      // Focus on first input
      setTimeout(() => {
        const firstInput = document.querySelector('input[type="text"]')
        if (firstInput) {
          firstInput.focus()
        }
      }, 100)
    })
    
    return {
      // Data
      isLoading,
      showPassword,
      loginError,
      form,
      errors,
      companies,
      
      // Computed
      isFormValid,
      
      // Methods
      handleLogin
    }
  }
}
</script>

<style scoped>
.input-field {
  width: 100%;
  padding: 0.5rem 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
  outline: none;
}

.input-field::placeholder {
  color: #9ca3af;
}

.input-field:focus {
  outline: none;
  box-shadow: 0 0 0 2px rgba(59, 130, 246, 0.5);
  border-color: #3b82f6;
}
</style>