<template>
  <div v-if="isOpen" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50" @click.self="closeModal">
    <div class="bg-white rounded-lg shadow-xl w-full max-w-2xl mx-4 max-h-[90vh] overflow-y-auto">
      <div class="flex items-center justify-between p-6 border-b">
        <h3 class="text-lg font-semibold text-gray-900">
          {{ isEdit ? 'ویرایش درآمد' : 'افزودن درآمد جدید' }}
        </h3>
        <button @click="closeModal" class="text-gray-400 hover:text-gray-600">
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
          </svg>
        </button>
      </div>

      <form @submit.prevent="handleSubmit" class="p-6">
        <!-- Error Display -->
        <div v-if="error" class="mb-4 p-4 bg-red-50 border border-red-200 rounded-md">
          <div class="flex">
            <svg class="w-5 h-5 text-red-400" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"></path>
            </svg>
            <div class="mr-3">
              <h3 class="text-sm font-medium text-red-800">خطا در پردازش</h3>
              <div class="mt-2 text-sm text-red-700">{{ error }}</div>
            </div>
          </div>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <!-- Date -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              تاریخ <span class="text-red-500">*</span>
            </label>
            <input
              v-model="form.date"
              type="date"
              required
              :class="[
                'w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500',
                errors.date ? 'border-red-300 focus:ring-red-500' : 'border-gray-300'
              ]"
            />
            <p v-if="errors.date" class="mt-1 text-sm text-red-600">{{ errors.date }}</p>
          </div>

          <!-- Amount -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              مبلغ (ریال) <span class="text-red-500">*</span>
            </label>
            <input
              v-model.number="form.amount"
              type="number"
              min="0"
              step="1000"
              required
              placeholder="مثال: 1500000"
              :class="[
                'w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500',
                errors.amount ? 'border-red-300 focus:ring-red-500' : 'border-gray-300'
              ]"
            />
            <p v-if="errors.amount" class="mt-1 text-sm text-red-600">{{ errors.amount }}</p>
          </div>

          <!-- Type -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              نوع درآمد <span class="text-red-500">*</span>
            </label>
            <select
              v-model="form.type"
              required
              :class="[
                'w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500',
                errors.type ? 'border-red-300 focus:ring-red-500' : 'border-gray-300'
              ]"
            >
              <option value="">انتخاب کنید</option>
              <option value="sales">فروش</option>
              <option value="commission">کمیسیون</option>
              <option value="service">خدمات</option>
              <option value="rental">اجاره</option>
              <option value="interest">سود</option>
              <option value="other">سایر</option>
            </select>
            <p v-if="errors.type" class="mt-1 text-sm text-red-600">{{ errors.type }}</p>
          </div>

          <!-- Status -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              وضعیت <span class="text-red-500">*</span>
            </label>
            <select
              v-model="form.status"
              required
              :class="[
                'w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500',
                errors.status ? 'border-red-300 focus:ring-red-500' : 'border-gray-300'
              ]"
            >
              <option value="">انتخاب کنید</option>
              <option value="pending">در انتظار تایید</option>
              <option value="confirmed">تایید شده</option>
              <option value="received">دریافت شده</option>
              <option value="cancelled">لغو شده</option>
            </select>
            <p v-if="errors.status" class="mt-1 text-sm text-red-600">{{ errors.status }}</p>
          </div>

          <!-- Account -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              حساب <span class="text-red-500">*</span>
            </label>
            <select
              v-model="form.accountId"
              required
              :class="[
                'w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500',
                errors.accountId ? 'border-red-300 focus:ring-red-500' : 'border-gray-300'
              ]"
            >
              <option value="">انتخاب حساب</option>
              <option v-for="account in accounts" :key="account.id" :value="account.id">
                {{ account.name }} - {{ account.code }}
              </option>
            </select>
            <p v-if="errors.accountId" class="mt-1 text-sm text-red-600">{{ errors.accountId }}</p>
          </div>

          <!-- Customer/Source -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              مشتری/منبع درآمد
            </label>
            <input
              v-model="form.source"
              type="text"
              placeholder="نام مشتری یا منبع درآمد"
              :class="[
                'w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500',
                errors.source ? 'border-red-300 focus:ring-red-500' : 'border-gray-300'
              ]"
            />
            <p v-if="errors.source" class="mt-1 text-sm text-red-600">{{ errors.source }}</p>
          </div>

          <!-- Reference Number -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              شماره مرجع
            </label>
            <input
              v-model="form.referenceNumber"
              type="text"
              placeholder="شماره فاکتور، قرارداد و غیره"
              :class="[
                'w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500',
                errors.referenceNumber ? 'border-red-300 focus:ring-red-500' : 'border-gray-300'
              ]"
            />
            <p v-if="errors.referenceNumber" class="mt-1 text-sm text-red-600">{{ errors.referenceNumber }}</p>
          </div>

          <!-- Tax Amount -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              مبلغ مالیات (ریال)
            </label>
            <input
              v-model.number="form.taxAmount"
              type="number"
              min="0"
              step="1000"
              placeholder="مبلغ مالیات (اختیاری)"
              :class="[
                'w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500',
                errors.taxAmount ? 'border-red-300 focus:ring-red-500' : 'border-gray-300'
              ]"
            />
            <p v-if="errors.taxAmount" class="mt-1 text-sm text-red-600">{{ errors.taxAmount }}</p>
          </div>
        </div>

        <!-- Description -->
        <div class="mt-6">
          <label class="block text-sm font-medium text-gray-700 mb-2">
            شرح <span class="text-red-500">*</span>
          </label>
          <textarea
            v-model="form.description"
            rows="3"
            required
            placeholder="توضیحات کامل درآمد را وارد کنید"
            :class="[
              'w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500',
              errors.description ? 'border-red-300 focus:ring-red-500' : 'border-gray-300'
            ]"
          ></textarea>
          <p v-if="errors.description" class="mt-1 text-sm text-red-600">{{ errors.description }}</p>
        </div>

        <!-- Payment Method -->
        <div class="mt-6">
          <label class="block text-sm font-medium text-gray-700 mb-2">
            روش پرداخت
          </label>
          <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
            <label class="flex items-center">
              <input
                v-model="form.paymentMethod"
                type="radio"
                value="cash"
                class="form-radio h-4 w-4 text-blue-600"
              />
              <span class="mr-2 text-sm">نقدی</span>
            </label>
            <label class="flex items-center">
              <input
                v-model="form.paymentMethod"
                type="radio"
                value="bank_transfer"
                class="form-radio h-4 w-4 text-blue-600"
              />
              <span class="mr-2 text-sm">انتقال بانکی</span>
            </label>
            <label class="flex items-center">
              <input
                v-model="form.paymentMethod"
                type="radio"
                value="check"
                class="form-radio h-4 w-4 text-blue-600"
              />
              <span class="mr-2 text-sm">چک</span>
            </label>
            <label class="flex items-center">
              <input
                v-model="form.paymentMethod"
                type="radio"
                value="credit"
                class="form-radio h-4 w-4 text-blue-600"
              />
              <span class="mr-2 text-sm">اعتباری</span>
            </label>
          </div>
        </div>

        <!-- Attachments -->
        <div class="mt-6">
          <label class="block text-sm font-medium text-gray-700 mb-2">
            پیوست‌ها
          </label>
          <input
            ref="fileInput"
            type="file"
            multiple
            accept=".pdf,.jpg,.jpeg,.png,.doc,.docx"
            @change="handleFileUpload"
            class="hidden"
          />
          <div class="border-2 border-dashed border-gray-300 rounded-lg p-4">
            <div class="text-center">
              <svg class="mx-auto h-12 w-12 text-gray-400" stroke="currentColor" fill="none" viewBox="0 0 48 48">
                <path d="M28 8H12a4 4 0 00-4 4v20m32-12v8m0 0v8a4 4 0 01-4 4H12a4 4 0 01-4-4v-4m32-4l-3.172-3.172a4 4 0 00-5.656 0L28 28M8 32l9.172-9.172a4 4 0 015.656 0L28 28m0 0l4 4m4-24h8m-4-4v8m-12 4h.02" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
              </svg>
              <div class="mt-4">
                <button type="button" @click="$refs.fileInput.click()" class="text-blue-600 hover:text-blue-500">
                  انتخاب فایل
                </button>
                <p class="text-sm text-gray-500">یا فایل‌ها را اینجا بکشید</p>
              </div>
              <p class="text-xs text-gray-500 mt-2">
                PDF, JPG, PNG, DOC تا 5MB
              </p>
            </div>
          </div>
          
          <!-- File List -->
          <div v-if="form.attachments.length > 0" class="mt-3">
            <div v-for="(file, index) in form.attachments" :key="index" class="flex items-center justify-between py-2 px-3 bg-gray-50 rounded-md mb-2">
              <span class="text-sm text-gray-700">{{ file.name }}</span>
              <button type="button" @click="removeFile(index)" class="text-red-500 hover:text-red-700">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
                </svg>
              </button>
            </div>
          </div>
        </div>

        <!-- Form Actions -->
        <div class="flex justify-end space-x-3 space-x-reverse mt-8 pt-6 border-t">
          <button
            type="button"
            @click="closeModal"
            class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
          >
            انصراف
          </button>
          <button
            type="submit"
            :disabled="loading"
            class="px-4 py-2 text-sm font-medium text-white bg-blue-600 border border-transparent rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <span v-if="loading" class="flex items-center">
              <svg class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              در حال پردازش...
            </span>
            <span v-else>
              {{ isEdit ? 'ویرایش' : 'ثبت' }}
            </span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
import { ref, reactive, computed, watch, onMounted } from 'vue'
import { financeApi } from '@/services/api'

export default {
  name: 'IncomeModal',
  props: {
    isOpen: {
      type: Boolean,
      default: false
    },
    income: {
      type: Object,
      default: null
    }
  },
  emits: ['close', 'save'],
  setup(props, { emit }) {
    const loading = ref(false)
    const error = ref('')
    const accounts = ref([])
    
    const form = reactive({
      date: '',
      amount: null,
      type: '',
      status: 'pending',
      accountId: '',
      source: '',
      referenceNumber: '',
      description: '',
      taxAmount: null,
      paymentMethod: 'cash',
      attachments: []
    })
    
    const errors = reactive({
      date: '',
      amount: '',
      type: '',
      status: '',
      accountId: '',
      source: '',
      referenceNumber: '',
      description: '',
      taxAmount: ''
    })
    
    const isEdit = computed(() => !!props.income)
    
    // Validation rules
    const validateForm = () => {
      // Clear previous errors
      Object.keys(errors).forEach(key => {
        errors[key] = ''
      })
      
      let isValid = true
      
      // Date validation
      if (!form.date) {
        errors.date = 'تاریخ الزامی است'
        isValid = false
      } else {
        const selectedDate = new Date(form.date)
        const today = new Date()
        if (selectedDate > today) {
          errors.date = 'تاریخ نمی‌تواند از امروز بیشتر باشد'
          isValid = false
        }
      }
      
      // Amount validation
      if (!form.amount || form.amount <= 0) {
        errors.amount = 'مبلغ باید بیشتر از صفر باشد'
        isValid = false
      } else if (form.amount > 999999999) {
        errors.amount = 'مبلغ خیلی زیاد است'
        isValid = false
      }
      
      // Type validation
      if (!form.type) {
        errors.type = 'نوع درآمد الزامی است'
        isValid = false
      }
      
      // Status validation
      if (!form.status) {
        errors.status = 'وضعیت الزامی است'
        isValid = false
      }
      
      // Account validation
      if (!form.accountId) {
        errors.accountId = 'انتخاب حساب الزامی است'
        isValid = false
      }
      
      // Description validation
      if (!form.description || form.description.trim().length < 5) {
        errors.description = 'شرح باید حداقل 5 کاراکتر باشد'
        isValid = false
      } else if (form.description.length > 500) {
        errors.description = 'شرح نمی‌تواند بیشتر از 500 کاراکتر باشد'
        isValid = false
      }
      
      // Tax amount validation (optional but if provided should be valid)
      if (form.taxAmount !== null && form.taxAmount < 0) {
        errors.taxAmount = 'مبلغ مالیات نمی‌تواند منفی باشد'
        isValid = false
      } else if (form.taxAmount > form.amount) {
        errors.taxAmount = 'مبلغ مالیات نمی‌تواند بیشتر از مبلغ کل باشد'
        isValid = false
      }
      
      return isValid
    }
    
    const resetForm = () => {
      form.date = ''
      form.amount = null
      form.type = ''
      form.status = 'pending'
      form.accountId = ''
      form.source = ''
      form.referenceNumber = ''
      form.description = ''
      form.taxAmount = null
      form.paymentMethod = 'cash'
      form.attachments = []
      
      Object.keys(errors).forEach(key => {
        errors[key] = ''
      })
      
      error.value = ''
    }
    
    const loadAccounts = async () => {
      try {
        // Mock accounts for now - replace with actual API call
        accounts.value = [
          { id: '1', name: 'صندوق', code: '1001' },
          { id: '2', name: 'بانک ملی', code: '1002' },
          { id: '3', name: 'درآمد فروش', code: '3001' },
          { id: '4', name: 'درآمد خدمات', code: '3002' }
        ]
      } catch (err) {
        console.error('Error loading accounts:', err)
      }
    }
    
    const handleFileUpload = (event) => {
      const files = Array.from(event.target.files)
      const maxSize = 5 * 1024 * 1024 // 5MB
      const allowedTypes = ['application/pdf', 'image/jpeg', 'image/jpg', 'image/png', 'application/msword', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document']
      
      files.forEach(file => {
        if (file.size > maxSize) {
          error.value = `فایل ${file.name} بیشتر از 5MB است`
          return
        }
        
        if (!allowedTypes.includes(file.type)) {
          error.value = `نوع فایل ${file.name} مجاز نیست`
          return
        }
        
        form.attachments.push(file)
      })
      
      // Clear file input
      event.target.value = ''
    }
    
    const removeFile = (index) => {
      form.attachments.splice(index, 1)
    }
    
    const handleSubmit = async () => {
      if (!validateForm()) {
        return
      }
      
      try {
        loading.value = true
        error.value = ''
        
        const incomeData = {
          date: form.date,
          amount: form.amount,
          type: form.type,
          status: form.status,
          accountId: form.accountId,
          source: form.source || null,
          referenceNumber: form.referenceNumber || null,
          description: form.description.trim(),
          taxAmount: form.taxAmount || 0,
          paymentMethod: form.paymentMethod,
          attachments: form.attachments
        }
        
        let response
        if (isEdit.value) {
          response = await financeApi.updateIncome(props.income.id, incomeData)
        } else {
          response = await financeApi.createIncome(incomeData)
        }
        
        if (response.success) {
          emit('save', response.data)
          closeModal()
        } else {
          throw new Error(response.message || 'خطا در ثبت درآمد')
        }
      } catch (err) {
        error.value = err.message || 'خطا در ثبت درآمد'
        console.error('Error saving income:', err)
      } finally {
        loading.value = false
      }
    }
    
    const closeModal = () => {
      resetForm()
      emit('close')
    }
    
    // Watch for income prop changes (edit mode)
    watch(() => props.income, (newIncome) => {
      if (newIncome) {
        form.date = newIncome.date
        form.amount = newIncome.amount
        form.type = newIncome.type
        form.status = newIncome.status
        form.accountId = newIncome.accountId || ''
        form.source = newIncome.source || ''
        form.referenceNumber = newIncome.referenceNumber || ''
        form.description = newIncome.description
        form.taxAmount = newIncome.taxAmount || null
        form.paymentMethod = newIncome.paymentMethod || 'cash'
        form.attachments = newIncome.attachments || []
      }
    }, { immediate: true })
    
    // Watch for modal open/close
    watch(() => props.isOpen, (isOpen) => {
      if (isOpen) {
        loadAccounts()
        if (!props.income) {
          resetForm()
          // Set default date to today
          form.date = new Date().toISOString().split('T')[0]
        }
      }
    })
    
    onMounted(() => {
      loadAccounts()
    })
    
    return {
      loading,
      error,
      accounts,
      form,
      errors,
      isEdit,
      handleSubmit,
      closeModal,
      handleFileUpload,
      removeFile
    }
  }
}
</script>

<style scoped>
.rtl {
  direction: rtl;
}
</style>