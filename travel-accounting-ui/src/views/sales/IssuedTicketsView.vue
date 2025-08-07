<template>
  <div class="min-h-screen bg-gray-50 rtl">
    <div class="container mx-auto px-4 py-8">
      <div class="bg-white rounded-lg shadow-sm p-6">
        <div class="flex justify-between items-center mb-6">
          <h1 class="text-2xl font-bold text-gray-900">بلیت‌های صادر شده</h1>
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
          <div class="bg-blue-50 p-4 rounded-lg">
            <div class="flex items-center">
              <div class="p-2 bg-blue-100 rounded-lg">
                <svg class="w-6 h-6 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                </svg>
              </div>
              <div class="mr-4">
                <p class="text-sm font-medium text-blue-600">کل بلیت‌های صادر شده</p>
                <p class="text-2xl font-bold text-blue-900">{{ totalTickets }}</p>
              </div>
            </div>
          </div>
          <div class="bg-green-50 p-4 rounded-lg">
            <div class="flex items-center">
              <div class="p-2 bg-green-100 rounded-lg">
                <svg class="w-6 h-6 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1"></path>
                </svg>
              </div>
              <div class="mr-4">
                <p class="text-sm font-medium text-green-600">کل فروش</p>
                <p class="text-2xl font-bold text-green-900">{{ formatCurrency(totalSales) }}</p>
              </div>
            </div>
          </div>
          <div class="bg-yellow-50 p-4 rounded-lg">
            <div class="flex items-center">
              <div class="p-2 bg-yellow-100 rounded-lg">
                <svg class="w-6 h-6 text-yellow-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6"></path>
                </svg>
              </div>
              <div class="mr-4">
                <p class="text-sm font-medium text-yellow-600">میانگین قیمت</p>
                <p class="text-2xl font-bold text-yellow-900">{{ formatCurrency(averagePrice) }}</p>
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
                <p class="text-2xl font-bold text-purple-900">{{ todayTickets }}</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Filters -->
        <div class="grid grid-cols-1 md:grid-cols-5 gap-4 mb-6">
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
                  نام مسافران
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  مسیر
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  ایرلاین
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  تاریخ پرواز
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  قیمت
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  تاریخ صدور
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
                  <ul>
                    <li v-for="passenger in ticket.passengers" :key="passenger.id">{{ passenger.name }}</li>
                  </ul>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ ticket.route }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ ticket.airline }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ formatDate(ticket.flightDate) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ formatCurrency(ticket.price) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ formatDate(ticket.issueDate) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <div class="flex space-x-2 space-x-reverse">
                    <button @click="viewTicket(ticket)" class="text-blue-600 hover:text-blue-900">
                      مشاهده
                    </button>
                    <button @click="printTicket(ticket)" class="text-green-600 hover:text-green-900">
                      چاپ
                    </button>
                    <button @click="refundTicket(ticket)" class="text-red-600 hover:text-red-900">
                      استرداد
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Empty State -->
        <div v-if="!loading && filteredTickets.length === 0" class="text-center py-8">
          <p class="text-gray-500">هیچ بلیت صادر شده‌ای یافت نشد.</p>
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
  name: 'IssuedTicketsView',
  components: {
    AppButton,
    AppSpinner
  },
  setup() {
    const loading = ref(false)
    const searchTerm = ref('')
    const selectedAirline = ref('')
    const fromDate = ref('')
    const toDate = ref('')
    const tickets = ref([])

    // Mock data
    const mockTickets = [
      {
        id: 1,
        ticketNumber: 'TK001234',
        passengers: [{ id: 1, name: 'احمد محمدی' }],
        route: 'تهران - اصفهان',
        airline: 'ایران ایر',
        flightDate: '2024-02-15',
        price: 2500000,
        issueDate: '2024-02-10'
      },
      {
        id: 2,
        ticketNumber: 'TK001235',
        passengers: [{ id: 2, name: 'فاطمه احمدی' }],
        route: 'تهران - شیراز',
        airline: 'ماهان',
        flightDate: '2024-02-16',
        price: 3200000,
        issueDate: '2024-02-11'
      },
      {
        id: 3,
        ticketNumber: 'TK001236',
        passengers: [{ id: 3, name: 'علی رضایی' }],
        route: 'تهران - مشهد',
        airline: 'آسمان',
        flightDate: '2024-02-17',
        price: 2800000,
        issueDate: '2024-02-12'
      },
      {
        id: 4,
        ticketNumber: 'TK001237',
        passengers: [{ id: 4, name: 'مریم حسینی' }],
        route: 'اصفهان - تهران',
        airline: 'ایران ایر',
        flightDate: '2024-02-18',
        price: 2600000,
        issueDate: new Date().toISOString().split('T')[0] // Today
      },
      {
        id: 5,
        ticketNumber: 'TK001238',
        passengers: [{ id: 5, name: 'حسن کریمی' }, { id: 6, name: 'زهرا کریمی' }],
        route: 'شیراز - تهران',
        airline: 'ماهان',
        flightDate: '2024-02-19',
        price: 3100000,
        issueDate: new Date().toISOString().split('T')[0] // Today
      }
    ]

    const filteredTickets = computed(() => {
      return tickets.value.filter(ticket => {
        const matchesSearch = !searchTerm.value || 
          ticket.ticketNumber.toLowerCase().includes(searchTerm.value.toLowerCase()) ||
          ticket.passengers.some(p => p.name.includes(searchTerm.value))
        const matchesAirline = !selectedAirline.value || ticket.airline === selectedAirline.value
        const matchesFromDate = !fromDate.value || ticket.issueDate >= fromDate.value
        const matchesToDate = !toDate.value || ticket.issueDate <= toDate.value
        
        return matchesSearch && matchesAirline && matchesFromDate && matchesToDate
      })
    })

    const totalTickets = computed(() => tickets.value.length)
    const totalSales = computed(() => tickets.value.reduce((sum, ticket) => sum + ticket.price, 0))
    const averagePrice = computed(() => totalTickets.value > 0 ? totalSales.value / totalTickets.value : 0)
    const todayTickets = computed(() => {
      const today = new Date().toISOString().split('T')[0]
      return tickets.value.filter(ticket => ticket.issueDate === today).length
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
      fromDate.value = ''
      toDate.value = ''
    }

    const exportTickets = () => {
      console.log('Export tickets to Excel')
    }

    const viewTicket = (ticket) => {
      console.log('View ticket:', ticket)
    }

    const printTicket = (ticket) => {
      console.log('Print ticket:', ticket)
    }

    const refundTicket = (ticket) => {
      if (confirm('آیا از استرداد این بلیت اطمینان دارید؟')) {
        console.log('Refund ticket:', ticket)
      }
    }

    const formatDate = (date) => {
      return new Date(date).toLocaleDateString('fa-IR')
    }

    const formatCurrency = (amount) => {
      return new Intl.NumberFormat('fa-IR').format(amount) + ' ریال'
    }

    onMounted(() => {
      loadTickets()
    })

    return {
      loading,
      searchTerm,
      selectedAirline,
      fromDate,
      toDate,
      tickets,
      filteredTickets,
      totalTickets,
      totalSales,
      averagePrice,
      todayTickets,
      clearFilters,
      exportTickets,
      viewTicket,
      printTicket,
      refundTicket,
      formatDate,
      formatCurrency
    }
  }
}
</script>