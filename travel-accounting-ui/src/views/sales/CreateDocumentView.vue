<template>
  <div class="min-h-screen bg-gray-50 rtl">
    <div class="container mx-auto px-4 py-8">
      <div class="bg-white rounded-lg shadow-sm p-6">
        <div class="flex justify-between items-center mb-6">
          <h1 class="text-2xl font-bold text-gray-900">ایجاد سند فروش</h1>
          <div class="flex space-x-2 space-x-reverse">
            <AppButton variant="secondary" @click="goBack">
              بازگشت
            </AppButton>
            <AppButton @click="saveDocument" :disabled="!isFormValid">
              ذخیره سند
            </AppButton>
          </div>
        </div>

        <!-- Document Form -->
        <form @submit.prevent="saveDocument" class="space-y-6">
          <!-- Basic Information -->
          <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">شماره سند</label>
              <input
                v-model="document.documentNumber"
                type="text"
                readonly
                class="w-full px-3 py-2 border border-gray-300 rounded-md bg-gray-50 focus:outline-none"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">تاریخ سند *</label>
              <input
                v-model="document.date"
                type="date"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">نوع سند *</label>
              <select
                v-model="document.type"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value="">انتخاب کنید</option>
                <option value="ticket_sale">فروش بلیت</option>
                <option value="hotel_booking">رزرو هتل</option>
                <option value="tour_package">پکیج تور</option>
                <option value="visa_service">خدمات ویزا</option>
                <option value="other">سایر</option>
              </select>
            </div>
          </div>

          <!-- Customer Information -->
          <div class="border-t pt-6">
            <h3 class="text-lg font-medium text-gray-900 mb-4">اطلاعات مشتری</h3>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">نام مشتری *</label>
                <input
                  v-model="document.customerName"
                  type="text"
                  required
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">شماره تماس</label>
                <input
                  v-model="document.customerPhone"
                  type="tel"
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>
              <div class="md:col-span-2">
                <label class="block text-sm font-medium text-gray-700 mb-2">آدرس</label>
                <textarea
                  v-model="document.customerAddress"
                  rows="3"
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                ></textarea>
              </div>
            </div>
          </div>

          <!-- Passenger Information -->
          <div v-if="document.type === 'ticket_sale'" class="border-t pt-6">
            <div class="flex justify-between items-center mb-4">
              <h3 class="text-lg font-medium text-gray-900">اطلاعات مسافران</h3>
              <AppButton variant="outline" size="sm" @click="addPassenger">
                افزودن مسافر
              </AppButton>
            </div>
            <div class="space-y-4">
              <div v-for="(passenger, index) in document.passengers" :key="index" class="border rounded-lg p-4 bg-gray-50">
                <div class="flex justify-between items-center mb-4">
                  <h4 class="font-semibold">مسافر {{ index + 1 }}</h4>
                  <button type="button" @click="removePassenger(index)" class="text-red-500 hover:text-red-700">
                    حذف
                  </button>
                </div>
                <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">نام *</label>
                    <input v-model="passenger.firstName" type="text" required class="w-full px-3 py-2 border border-gray-300 rounded-md" />
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">نام خانوادگی *</label>
                    <input v-model="passenger.lastName" type="text" required class="w-full px-3 py-2 border border-gray-300 rounded-md" />
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">جنسیت *</label>
                    <select v-model="passenger.gender" required class="w-full px-3 py-2 border border-gray-300 rounded-md">
                      <option value="male">مرد</option>
                      <option value="female">زن</option>
                    </select>
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">تاریخ تولد *</label>
                    <PersianDatePicker v-model="passenger.birthDate" required />
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">شماره پاسپورت</label>
                    <input v-model="passenger.passportNumber" type="text" class="w-full px-3 py-2 border border-gray-300 rounded-md" />
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">انقضای پاسپورت</label>
                    <PersianDatePicker v-model="passenger.passportExpiry" />
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Document Items -->
          <div class="border-t pt-6">
            <div class="flex justify-between items-center mb-4">
              <h3 class="text-lg font-medium text-gray-900">اقلام سند</h3>
              <AppButton variant="secondary" @click="addItem">
                <svg class="w-5 h-5 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path>
                </svg>
                افزودن قلم
              </AppButton>
            </div>
            
            <div class="space-y-4">
              <div v-for="(item, index) in document.items" :key="index" class="border rounded-lg p-4">
                <div class="grid grid-cols-1 md:grid-cols-5 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">شرح *</label>
                    <input
                      v-model="item.description"
                      type="text"
                      required
                      class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">تعداد *</label>
                    <input
                      v-model.number="item.quantity"
                      type="number"
                      min="1"
                      required
                      class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">قیمت واحد *</label>
                    <input
                      v-model.number="item.unitPrice"
                      type="number"
                      min="0"
                      required
                      class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">مجموع</label>
                    <input
                      :value="formatCurrency(item.quantity * item.unitPrice)"
                      type="text"
                      readonly
                      class="w-full px-3 py-2 border border-gray-300 rounded-md bg-gray-50"
                    />
                  </div>
                  <div class="flex items-end">
                    <button
                      type="button"
                      @click="removeItem(index)"
                      class="w-full px-3 py-2 text-red-600 hover:text-red-800 border border-red-300 rounded-md hover:bg-red-50"
                    >
                      حذف
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Totals -->
          <div class="border-t pt-6">
            <div class="flex justify-end">
              <div class="w-full md:w-1/3 space-y-2">
                <div class="flex justify-between">
                  <span class="text-sm font-medium text-gray-700">جمع کل:</span>
                  <span class="text-sm font-bold text-gray-900">{{ formatCurrency(totalAmount) }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-sm font-medium text-gray-700">تخفیف:</span>
                  <input
                    v-model.number="document.discount"
                    type="number"
                    min="0"
                    :max="totalAmount"
                    class="w-24 px-2 py-1 text-sm border border-gray-300 rounded text-left"
                  />
                </div>
                <div class="flex justify-between border-t pt-2">
                  <span class="text-base font-bold text-gray-900">مبلغ نهایی:</span>
                  <span class="text-base font-bold text-blue-600">{{ formatCurrency(finalAmount) }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Notes -->
          <div class="border-t pt-6">
            <label class="block text-sm font-medium text-gray-700 mb-2">توضیحات</label>
            <textarea
              v-model="document.notes"
              rows="4"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="توضیحات اضافی در مورد سند..."
            ></textarea>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import AppButton from '@/components/AppButton.vue'
import PersianDatePicker from '@/components/PersianDatePicker.vue'

export default {
  name: 'CreateDocumentView',
  components: {
    AppButton,
    PersianDatePicker
  },
  setup() {
    const router = useRouter()
    
    const document = ref({
      passengers: [],
      documentNumber: '',
      date: new Date().toISOString().split('T')[0],
      type: 'ticket_sale',
      customerName: '',
      customerPhone: '',
      customerAddress: '',
      items: [
        {
          description: '',
          quantity: 1,
          unitPrice: 0
        }
      ],
      discount: 0,
      notes: ''
    })

    const generateDocumentNumber = () => {
      const now = new Date()
      const year = now.getFullYear().toString().slice(-2)
      const month = (now.getMonth() + 1).toString().padStart(2, '0')
      const day = now.getDate().toString().padStart(2, '0')
      const random = Math.floor(Math.random() * 1000).toString().padStart(3, '0')
      return `DOC${year}${month}${day}${random}`
    }

    const totalAmount = computed(() => {
      return document.value.items.reduce((sum, item) => {
        return sum + (item.quantity * item.unitPrice)
      }, 0)
    })

    const finalAmount = computed(() => {
      return Math.max(0, totalAmount.value - (document.value.discount || 0))
    })

    const isFormValid = computed(() => {
      return document.value.date &&
             document.value.type &&
             document.value.customerName &&
             document.value.items.length > 0 &&
             document.value.items.every(item => item.description && item.quantity > 0 && item.unitPrice >= 0)
    })

    const addItem = () => {
      document.value.items.push({
        description: '',
        quantity: 1,
        unitPrice: 0
      })
    }

    const removeItem = (index) => {
      if (document.value.items.length > 1) {
        document.value.items.splice(index, 1)
      }
    }

    const saveDocument = async () => {
      if (!isFormValid.value) {
        alert('لطفاً تمام فیلدهای الزامی را پر کنید.')
        return
      }

      try {
        // Simulate API call
        console.log('Saving document:', document.value)
        alert('سند با موفقیت ذخیره شد.')
        router.push('/sales/documents')
      } catch (error) {
        console.error('Error saving document:', error)
        alert('خطا در ذخیره سند')
      }
    }

    const goBack = () => {
      router.go(-1)
    }

    const formatCurrency = (amount) => {
      return new Intl.NumberFormat('fa-IR').format(amount) + ' ریال'
    }

    const addPassenger = () => {
      document.value.passengers.push({
        firstName: '',
        lastName: '',
        gender: 'male',
        birthDate: '',
        passportNumber: '',
        passportExpiry: '',
      });
    };

    const removePassenger = (index) => {
      document.value.passengers.splice(index, 1);
    };

    onMounted(() => {
      document.value.documentNumber = generateDocumentNumber();
      if (document.value.type === 'ticket_sale' && document.value.passengers.length === 0) {
        addPassenger();
      }
    });

    return {
      document,
      totalAmount,
      finalAmount,
      isFormValid,
      addItem,
      removeItem,
      saveDocument,
      goBack,
      formatCurrency,
      addPassenger,
      removePassenger
    };
  }
}
</script>