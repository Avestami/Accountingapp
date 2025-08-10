<template>
  <div v-if="isOpen" class="fixed inset-0 z-50 overflow-y-auto">
    <div class="flex items-center justify-center min-h-screen pt-4 px-4 pb-20 text-center sm:block sm:p-0">
      <!-- Background overlay -->
      <div class="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity" @click="$emit('close')"></div>

      <!-- Modal panel -->
      <div class="inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-lg sm:w-full">
        <form @submit.prevent="handleSubmit">
          <div class="bg-white px-4 pt-5 pb-4 sm:p-6 sm:pb-4">
            <div class="sm:flex sm:items-start">
              <div class="w-full">
                <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">
                  {{ transfer ? 'ویرایش انتقال' : 'ثبت انتقال جدید' }}
                </h3>

                <!-- Date -->
                <div class="mb-4">
                  <label for="date" class="block text-sm font-medium text-gray-700 mb-1">
                    تاریخ <span class="text-red-500">*</span>
                  </label>
                  <input
                    id="date"
                    v-model="form.date"
                    type="date"
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                    :class="{ 'border-red-500': errors.date }"
                    required
                  />
                  <p v-if="errors.date" class="mt-1 text-sm text-red-600">{{ errors.date }}</p>
                </div>

                <!-- Amount -->
                <div class="mb-4">
                  <label for="amount" class="block text-sm font-medium text-gray-700 mb-1">
                    مبلغ (ریال) <span class="text-red-500">*</span>
                  </label>
                  <input
                    id="amount"
                    v-model.number="form.amount"
                    type="number"
                    min="0"
                    step="1000"
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                    :class="{ 'border-red-500': errors.amount }"
                    placeholder="مبلغ انتقال را وارد کنید"
                    required
                  />
                  <p v-if="errors.amount" class="mt-1 text-sm text-red-600">{{ errors.amount }}</p>
                </div>

                <!-- From Account -->
                <div class="mb-4">
                  <label for="fromAccount" class="block text-sm font-medium text-gray-700 mb-1">
                    از حساب <span class="text-red-500">*</span>
                  </label>
                  <select
                    id="fromAccount"
                    v-model="form.fromAccountId"
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                    :class="{ 'border-red-500': errors.fromAccountId }"
                    required
                  >
                    <option value="">انتخاب حساب مبدأ</option>
                    <option v-for="account in accounts" :key="account.id" :value="account.id">
                      {{ account.name }} - {{ account.accountNumber }}
                    </option>
                  </select>
                  <p v-if="errors.fromAccountId" class="mt-1 text-sm text-red-600">{{ errors.fromAccountId }}</p>
                </div>

                <!-- To Account -->
                <div class="mb-4">
                  <label for="toAccount" class="block text-sm font-medium text-gray-700 mb-1">
                    به حساب <span class="text-red-500">*</span>
                  </label>
                  <select
                    id="toAccount"
                    v-model="form.toAccountId"
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                    :class="{ 'border-red-500': errors.toAccountId }"
                    required
                  >
                    <option value="">انتخاب حساب مقصد</option>
                    <option v-for="account in accounts" :key="account.id" :value="account.id">
                      {{ account.name }} - {{ account.accountNumber }}
                    </option>
                  </select>
                  <p v-if="errors.toAccountId" class="mt-1 text-sm text-red-600">{{ errors.toAccountId }}</p>
                </div>

                <!-- Transfer Type -->
                <div class="mb-4">
                  <label for="type" class="block text-sm font-medium text-gray-700 mb-1">
                    نوع انتقال <span class="text-red-500">*</span>
                  </label>
                  <select
                    id="type"
                    v-model="form.type"
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                    :class="{ 'border-red-500': errors.type }"
                    required
                  >
                    <option value="">انتخاب نوع انتقال</option>
                    <option value="internal">انتقال داخلی</option>
                    <option value="external">انتقال خارجی</option>
                    <option value="bank">انتقال بانکی</option>
                    <option value="cash">انتقال نقدی</option>
                  </select>
                  <p v-if="errors.type" class="mt-1 text-sm text-red-600">{{ errors.type }}</p>
                </div>

                <!-- Status -->
                <div class="mb-4">
                  <label for="status" class="block text-sm font-medium text-gray-700 mb-1">
                    وضعیت <span class="text-red-500">*</span>
                  </label>
                  <select
                    id="status"
                    v-model="form.status"
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                    :class="{ 'border-red-500': errors.status }"
                    required
                  >
                    <option value="">انتخاب وضعیت</option>
                    <option value="pending">در انتظار</option>
                    <option value="processing">در حال پردازش</option>
                    <option value="completed">تکمیل شده</option>
                    <option value="failed">ناموفق</option>
                    <option value="cancelled">لغو شده</option>
                  </select>
                  <p v-if="errors.status" class="mt-1 text-sm text-red-600">{{ errors.status }}</p>
                </div>

                <!-- Reference Number -->
                <div class="mb-4">
                  <label for="referenceNumber" class="block text-sm font-medium text-gray-700 mb-1">
                    شماره مرجع
                  </label>
                  <input
                    id="referenceNumber"
                    v-model="form.referenceNumber"
                    type="text"
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                    :class="{ 'border-red-500': errors.referenceNumber }"
                    placeholder="شماره مرجع انتقال"
                  />
                  <p v-if="errors.referenceNumber" class="mt-1 text-sm text-red-600">{{ errors.referenceNumber }}</p>
                </div>

                <!-- Description -->
                <div class="mb-4">
                  <label for="description" class="block text-sm font-medium text-gray-700 mb-1">
                    توضیحات
                  </label>
                  <textarea
                    id="description"
                    v-model="form.description"
                    rows="3"
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                    :class="{ 'border-red-500': errors.description }"
                    placeholder="توضیحات انتقال"
                  ></textarea>
                  <p v-if="errors.description" class="mt-1 text-sm text-red-600">{{ errors.description }}</p>
                </div>

                <!-- Fee Amount -->
                <div class="mb-4">
                  <label for="feeAmount" class="block text-sm font-medium text-gray-700 mb-1">
                    مبلغ کارمزد (ریال)
                  </label>
                  <input
                    id="feeAmount"
                    v-model.number="form.feeAmount"
                    type="number"
                    min="0"
                    step="1000"
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                    :class="{ 'border-red-500': errors.feeAmount }"
                    placeholder="مبلغ کارمزد"
                  />
                  <p v-if="errors.feeAmount" class="mt-1 text-sm text-red-600">{{ errors.feeAmount }}</p>
                </div>

                <!-- Attachments -->
                <div class="mb-4">
                  <label for="attachments" class="block text-sm font-medium text-gray-700 mb-1">
                    فایل‌های پیوست
                  </label>
                  <input
                    id="attachments"
                    type="file"
                    multiple
                    accept=".pdf,.jpg,.jpeg,.png,.doc,.docx"
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                    @change="handleFileUpload"
                  />
                  <p class="mt-1 text-sm text-gray-500">
                    فرمت‌های مجاز: PDF, JPG, PNG, DOC, DOCX (حداکثر 5 فایل)
                  </p>
                  <div v-if="form.attachments.length > 0" class="mt-2">
                    <div v-for="(file, index) in form.attachments" :key="index" class="flex items-center justify-between bg-gray-50 p-2 rounded">
                      <span class="text-sm">{{ file.name }}</span>
                      <button type="button" @click="removeFile(index)" class="text-red-600 hover:text-red-800">
                        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
                        </svg>
                      </button>
                    </div>
                  </div>
                </div>

                <!-- Error message -->
                <div v-if="generalError" class="mb-4 p-3 bg-red-100 border border-red-400 text-red-700 rounded">
                  {{ generalError }}
                </div>
              </div>
            </div>
          </div>

          <div class="bg-gray-50 px-4 py-3 sm:px-6 sm:flex sm:flex-row-reverse">
            <button
              type="submit"
              :disabled="loading"
              class="w-full inline-flex justify-center rounded-md border border-transparent shadow-sm px-4 py-2 bg-indigo-600 text-base font-medium text-white hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 sm:ml-3 sm:w-auto sm:text-sm disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <svg v-if="loading" class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              {{ loading ? 'در حال ذخیره...' : (transfer ? 'ویرایش' : 'ثبت') }}
            </button>
            <button
              type="button"
              @click="$emit('close')"
              class="mt-3 w-full inline-flex justify-center rounded-md border border-gray-300 shadow-sm px-4 py-2 bg-white text-base font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 sm:mt-0 sm:ml-3 sm:w-auto sm:text-sm"
            >
              انصراف
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, reactive, watch, onMounted } from 'vue'
import { financeApi } from '@/services/api'

export default {
  name: 'TransferModal',
  props: {
    isOpen: {
      type: Boolean,
      default: false
    },
    transfer: {
      type: Object,
      default: null
    }
  },
  emits: ['close', 'save'],
  setup(props, { emit }) {
    const loading = ref(false)
    const generalError = ref('')
    const accounts = ref([])

    const form = reactive({
      date: '',
      amount: null,
      fromAccountId: '',
      toAccountId: '',
      type: '',
      status: 'pending',
      referenceNumber: '',
      description: '',
      feeAmount: null,
      attachments: []
    })

    const errors = reactive({
      date: '',
      amount: '',
      fromAccountId: '',
      toAccountId: '',
      type: '',
      status: '',
      referenceNumber: '',
      description: '',
      feeAmount: ''
    })

    // Load accounts for dropdowns
    const loadAccounts = async () => {
      try {
        // Mock accounts data - replace with actual API call
        accounts.value = [
          { id: 1, name: 'حساب جاری', accountNumber: '1001' },
          { id: 2, name: 'حساب پس‌انداز', accountNumber: '1002' },
          { id: 3, name: 'حساب سرمایه‌گذاری', accountNumber: '1003' },
          { id: 4, name: 'حساب نقدی', accountNumber: '1004' }
        ]
      } catch (error) {
        console.error('Error loading accounts:', error)
      }
    }

    // Reset form
    const resetForm = () => {
      Object.keys(form).forEach(key => {
        if (key === 'attachments') {
          form[key] = []
        } else if (key === 'status') {
          form[key] = 'pending'
        } else {
          form[key] = key === 'amount' || key === 'feeAmount' ? null : ''
        }
      })
      Object.keys(errors).forEach(key => {
        errors[key] = ''
      })
      generalError.value = ''
    }

    // Populate form with transfer data
    const populateForm = () => {
      if (props.transfer) {
        Object.keys(form).forEach(key => {
          if (props.transfer[key] !== undefined) {
            form[key] = props.transfer[key]
          }
        })
      }
    }

    // Validate form
    const validateForm = () => {
      let isValid = true
      Object.keys(errors).forEach(key => {
        errors[key] = ''
      })

      if (!form.date) {
        errors.date = 'تاریخ الزامی است'
        isValid = false
      }

      if (!form.amount || form.amount <= 0) {
        errors.amount = 'مبلغ باید بیشتر از صفر باشد'
        isValid = false
      }

      if (!form.fromAccountId) {
        errors.fromAccountId = 'انتخاب حساب مبدأ الزامی است'
        isValid = false
      }

      if (!form.toAccountId) {
        errors.toAccountId = 'انتخاب حساب مقصد الزامی است'
        isValid = false
      }

      if (form.fromAccountId === form.toAccountId) {
        errors.toAccountId = 'حساب مقصد نمی‌تواند با حساب مبدأ یکسان باشد'
        isValid = false
      }

      if (!form.type) {
        errors.type = 'انتخاب نوع انتقال الزامی است'
        isValid = false
      }

      if (!form.status) {
        errors.status = 'انتخاب وضعیت الزامی است'
        isValid = false
      }

      if (form.feeAmount && form.feeAmount < 0) {
        errors.feeAmount = 'مبلغ کارمزد نمی‌تواند منفی باشد'
        isValid = false
      }

      return isValid
    }

    // Handle file upload
    const handleFileUpload = (event) => {
      const files = Array.from(event.target.files)
      if (files.length + form.attachments.length > 5) {
        alert('حداکثر 5 فایل می‌توانید انتخاب کنید')
        return
      }

      const allowedTypes = ['application/pdf', 'image/jpeg', 'image/jpg', 'image/png', 'application/msword', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document']
      const validFiles = files.filter(file => {
        if (!allowedTypes.includes(file.type)) {
          alert(`فرمت فایل ${file.name} مجاز نیست`)
          return false
        }
        if (file.size > 5 * 1024 * 1024) { // 5MB
          alert(`حجم فایل ${file.name} بیش از حد مجاز است`)
          return false
        }
        return true
      })

      form.attachments.push(...validFiles)
    }

    // Remove file
    const removeFile = (index) => {
      form.attachments.splice(index, 1)
    }

    // Handle form submission
    const handleSubmit = async () => {
      if (!validateForm()) {
        return
      }

      loading.value = true
      generalError.value = ''

      try {
        const transferData = { ...form }
        
        // Handle file uploads if any
        if (transferData.attachments.length > 0) {
          // In a real app, you would upload files to a file service
          // For now, we'll just store file names
          transferData.attachments = transferData.attachments.map(file => ({
            name: file.name,
            size: file.size,
            type: file.type
          }))
        }

        if (props.transfer) {
          // Update existing transfer
          await financeApi.updateTransfer(props.transfer.id, transferData)
        } else {
          // Create new transfer
          await financeApi.createTransfer(transferData)
        }

        emit('save', transferData)
      } catch (error) {
        console.error('Error saving transfer:', error)
        generalError.value = error.response?.data?.message || 'خطا در ذخیره انتقال'
      } finally {
        loading.value = false
      }
    }

    // Watch for modal open/close
    watch(() => props.isOpen, (newValue) => {
      if (newValue) {
        resetForm()
        populateForm()
        loadAccounts()
      }
    })

    onMounted(() => {
      loadAccounts()
    })

    return {
      loading,
      generalError,
      accounts,
      form,
      errors,
      handleSubmit,
      handleFileUpload,
      removeFile
    }
  }
}
</script>