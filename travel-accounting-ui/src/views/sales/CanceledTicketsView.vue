<template>
  <div class="min-h-screen bg-gray-50 rtl">
    <div class="container mx-auto px-4 py-8">
      <div class="bg-white rounded-lg shadow-sm p-6">
        <div class="flex justify-between items-center mb-6">
          <h1 class="text-2xl font-bold text-gray-900">بلیت‌های لغو شده</h1>
          <div class="flex space-x-2 space-x-reverse">
            <AppButton variant="secondary" @click="exportTickets">
              <svg class="w-5 h-5 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 10v6m0 0l-3-3m3 3l3-3m2 8H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
              </svg>
              خروجی Excel
            </AppButton>
          </div>
        </div>

        <!-- Statistics Cards -->
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
          <div class="bg-red-50 p-4 rounded-lg">
            <div class="flex items-center">
              <div class="p-2 bg-red-100 rounded-lg">
                <svg class="w-6 h-6 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
                </svg>
              </div>
              <div class="mr-4">
                <p class="text-sm font-medium text-red-600">کل بلیت‌های لغو شده</p>
                <p class="text-2xl font-bold text-red-900">{{ totalCanceledTickets }}</p>
              </div>
            </div>
          </div>
          <div class="bg-orange-50 p-4 rounded-lg">
            <div class="flex items-center">
              <div class="p-2 bg-orange-100 rounded-lg">
                <svg class="w-6 h-6 text-orange-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1"></path>
                </svg>
              </div>
              <div class="mr-4">
                <p class="text-sm font-medium text-orange-600">کل مبلغ لغو شده</p>
                <p class="text-2xl font-bold text-orange-900">{{ formatCurrency(totalCanceledAmount) }}</p>
              </div>
            </div>
          </div>
          <div class="bg-blue-50 p-4 rounded-lg">
            <div class="flex items-center">
              <div class="p-2 bg-blue-100 rounded-lg">
                <svg class="w-6 h-6 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                </svg>
              </div>
              <div class="mr-4">
                <p class="text-sm font-medium text-blue-600">مبلغ بازگشتی</p>
                <p class="text-2xl font-bold text-blue-900">{{ formatCurrency(totalRefundedAmount) }}</p>
              </div>
            </div>
          </div>
          <div class="bg-purple-50 p-4 rounded-lg">
            <div class="flex items-center">
              <div class="p-2 bg-purple-100 rounded-lg">
                <svg class="w-6 h-6 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"></path>
                </svg>
              </div>
              <div class="mr-4">
                <p class="text-sm font-medium text-purple-600">امروز</p>
                <p class="text-2xl font-bold text-purple-900">{{ todayCanceledTickets }}</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Filters -->
        <div class="grid grid-cols-1 md:grid-cols-6 gap-4 mb-6">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">جستجو</label>
            <input
              v-model="searchTerm"
              type="text"
              placeholder="شماره بلیت، نام مسافر..."
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">ایرلاین</label>
            <select
              v-model="selectedAirline"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="">همه ایرلاین‌ها</option>
              <option value="ایران ایر">ایران ایر</option>
              <option value="ماهان">ماهان</option>
              <option value="آسمان">آسمان</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">دلیل لغو</label>
            <select
              v-model="selectedCancelReason"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="">همه دلایل</option>
              <option value="customer_request">درخواست مشتری</option>
              <option value="flight_canceled">لغو پرواز</option>
              <option value="schedule_change">تغییر برنامه</option>
              <option value="other">سایر</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">از تاریخ</label>
            <input
              v-model="fromDate"
              type="date"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">تا تاریخ</label>
            <input
              v-model="toDate"
              type="date"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <div class="flex items-end">
            <AppButton variant="secondary" @click="clearFilters" class="w-full">
              پاک کردن فیلترها
            </AppButton>
          </div>
        </div>

        <!-- Loading State -->
        <div v-if="loading" class="flex justify-center py-8">
          <AppSpinner />
        </div>

        <!-- Tickets Table -->
        <div v-else class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  شماره بلیت
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  نام مسافر
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  مسیر
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  ایرلاین
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  قیمت اصلی
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  مبلغ بازگشتی
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  دلیل لغو
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  تاریخ لغو
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  عملیات
                </th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr v-for="ticket in filteredTickets" :key="ticket.id" class="hover:bg-gray-50">
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                  {{ ticket.ticketNumber }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ ticket.passengerName }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ ticket.route }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ ticket.airline }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ formatCurrency(ticket.originalPrice) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ formatCurrency(ticket.refundAmount) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span :class="getCancelReasonBadgeClass(ticket.cancelReason)" class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full">
                    {{ getCancelReasonLabel(ticket.cancelReason) }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ formatDate(ticket.cancelDate) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <div class="flex space-x-2 space-x-reverse">
                    <button @click="viewTicket(ticket)" class="text-blue-600 hover:text-blue-900">
                      مشاهده
                    </button>
                    <button @click="printCancelReport(ticket)" class="text-green-600 hover:text-green-900">
                      گزارش لغو
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Empty State -->
        <div v-if="!loading && filteredTickets.length === 0" class="text-center py-8">
          <p class="text-gray-500">هیچ بلیت لغو شده‌ای یافت نشد.</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue'
import AppButton from '@/components/AppButton.vue'
import AppSpinner from '@/components/AppSpinner.vue'

export default {
  name: 'CanceledTicketsView',
  components: {
    AppButton,
    AppSpinner
  },
  setup() {
    const loading = ref(false)
    const searchTerm = ref('')
    const selectedAirline = ref('')
    const selectedCancelReason = ref('')
    const fromDate = ref('')
    const toDate = ref('')
    const tickets = ref([])

    // Mock data
    const mockTickets = [
      {
        id: 1,
        ticketNumber: 'TK001234',
        passengerName: 'احمد محمدی',
        route: 'تهران - اصفهان',
        airline: 'ایران ایر',
        originalPrice: 2500000,
        refundAmount: 2200000,
        cancelReason: 'customer_request',
        cancelDate: '2024-02-10'
      },
      {
        id: 2,
        ticketNumber: 'TK001235',
        passengerName: 'فاطمه احمدی',
        route: 'تهران - شیراز',
        airline: 'ماهان',
        originalPrice: 3200000,
        refundAmount: 0,
        cancelReason: 'flight_canceled',
        cancelDate: '2024-02-11'
      },
      {
        id: 3,
        ticketNumber: 'TK001236',
        passengerName: 'علی رضایی',
        route: 'تهران - مشهد',
        airline: 'آسمان',
        originalPrice: 2800000,
        refundAmount: 2500000,
        cancelReason: 'schedule_change',
        cancelDate: '2024-02-12'
      },
      {
        id: 4,
        ticketNumber: 'TK001237',
        passengerName: 'مریم حسینی',
        route: 'اصفهان - تهران',
        airline: 'ایران ایر',
        originalPrice: 2600000,
        refundAmount: 2300000,
        cancelReason: 'other',
        cancelDate: new Date().toISOString().split('T')[0] // Today
      }
    ]

    const filteredTickets = computed(() => {
      return tickets.value.filter(ticket => {
        const matchesSearch = !searchTerm.value || 
          ticket.ticketNumber.toLowerCase().includes(searchTerm.value.toLowerCase()) ||
          ticket.passengerName.includes(searchTerm.value)
        const matchesAirline = !selectedAirline.value || ticket.airline === selectedAirline.value
        const matchesCancelReason = !selectedCancelReason.value || ticket.cancelReason === selectedCancelReason.value
        const matchesFromDate = !fromDate.value || ticket.cancelDate >= fromDate.value
        const matchesToDate = !toDate.value || ticket.cancelDate <= toDate.value
        
        return matchesSearch && matchesAirline && matchesCancelReason && matchesFromDate && matchesToDate
      })
    })

    const totalCanceledTickets = computed(() => tickets.value.length)
    const totalCanceledAmount = computed(() => tickets.value.reduce((sum, ticket) => sum + ticket.originalPrice, 0))
    const totalRefundedAmount = computed(() => tickets.value.reduce((sum, ticket) => sum + ticket.refundAmount, 0))
    const todayCanceledTickets = computed(() => {
      const today = new Date().toISOString().split('T')[0]
      return tickets.value.filter(ticket => ticket.cancelDate === today).length
    })

    const loadTickets = async () => {
      loading.value = true
      // Simulate API call
      setTimeout(() => {
        tickets.value = mockTickets
        loading.value = false
      }, 1000)
    }

    const clearFilters = () => {
      searchTerm.value = ''
      selectedAirline.value = ''
      selectedCancelReason.value = ''
      fromDate.value = ''
      toDate.value = ''
    }

    const exportTickets = () => {
      console.log('Export canceled tickets to Excel')
    }

    const viewTicket = (ticket) => {
      console.log('View canceled ticket:', ticket)
    }

    const printCancelReport = (ticket) => {
      console.log('Print cancel report for ticket:', ticket)
    }

    const formatDate = (date) => {
      return new Date(date).toLocaleDateString('fa-IR')
    }

    const formatCurrency = (amount) => {
      return new Intl.NumberFormat('fa-IR').format(amount) + ' ریال'
    }

    const getCancelReasonLabel = (reason) => {
      const labels = {
        customer_request: 'درخواست مشتری',
        flight_canceled: 'لغو پرواز',
        schedule_change: 'تغییر برنامه',
        other: 'سایر'
      }
      return labels[reason] || reason
    }

    const getCancelReasonBadgeClass = (reason) => {
      const classes = {
        customer_request: 'bg-blue-100 text-blue-800',
        flight_canceled: 'bg-red-100 text-red-800',
        schedule_change: 'bg-yellow-100 text-yellow-800',
        other: 'bg-gray-100 text-gray-800'
      }
      return classes[reason] || 'bg-gray-100 text-gray-800'
    }

    onMounted(() => {
      loadTickets()
    })

    return {
      loading,
      searchTerm,
      selectedAirline,
      selectedCancelReason,
      fromDate,
      toDate,
      tickets,
      filteredTickets,
      totalCanceledTickets,
      totalCanceledAmount,
      totalRefundedAmount,
      todayCanceledTickets,
      clearFilters,
      exportTickets,
      viewTicket,
      printCancelReport,
      formatDate,
      formatCurrency,
      getCancelReasonLabel,
      getCancelReasonBadgeClass
    }
  }
}
</script>