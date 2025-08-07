<template>
  <div class="min-h-screen bg-gray-50 rtl">
    <div class="container mx-auto px-4 py-8">
      <div class="bg-white rounded-lg shadow-sm p-6">
        <div class="flex justify-between items-center mb-6">
          <h1 class="text-2xl font-bold text-gray-900">بلیت‌های صادر نشده</h1>
          <AppButton variant="primary" @click="createTicket">
            <svg class="w-5 h-5 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"></path>
            </svg>
            ایجاد بلیت جدید
          </AppButton>
        </div>

        <!-- Filters -->
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
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
              <option value="iran-air">ایران ایر</option>
              <option value="mahan">ماهان</option>
              <option value="aseman">آسمان</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">وضعیت</label>
            <select
              v-model="selectedStatus"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="">همه وضعیت‌ها</option>
              <option value="pending">در انتظار</option>
              <option value="processing">در حال پردازش</option>
              <option value="hold">نگهداری</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">تاریخ</label>
            <input
              v-model="selectedDate"
              type="date"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
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
                  تاریخ پرواز
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  قیمت
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  وضعیت
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
                  {{ formatDate(ticket.flightDate) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ formatCurrency(ticket.price) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span :class="getStatusBadgeClass(ticket.status)" class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full">
                    {{ getStatusLabel(ticket.status) }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <div class="flex space-x-2 space-x-reverse">
                    <button @click="editTicket(ticket)" class="text-blue-600 hover:text-blue-900">
                      ویرایش
                    </button>
                    <button @click="issueTicket(ticket)" class="text-green-600 hover:text-green-900">
                      صدور
                    </button>
                    <button @click="deleteTicket(ticket)" class="text-red-600 hover:text-red-900">
                      حذف
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Empty State -->
        <div v-if="!loading && filteredTickets.length === 0" class="text-center py-8">
          <p class="text-gray-500">هیچ بلیت صادر نشده‌ای یافت نشد.</p>
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
  name: 'UnissuedTicketsView',
  components: {
    AppButton,
    AppSpinner
  },
  setup() {
    const loading = ref(false)
    const searchTerm = ref('')
    const selectedAirline = ref('')
    const selectedStatus = ref('')
    const selectedDate = ref('')
    const tickets = ref([])

    // Mock data
    const mockTickets = [
      {
        id: 1,
        ticketNumber: 'TK001234',
        passengerName: 'احمد محمدی',
        route: 'تهران - اصفهان',
        airline: 'ایران ایر',
        flightDate: '2024-02-15',
        price: 2500000,
        status: 'pending'
      },
      {
        id: 2,
        ticketNumber: 'TK001235',
        passengerName: 'فاطمه احمدی',
        route: 'تهران - شیراز',
        airline: 'ماهان',
        flightDate: '2024-02-16',
        price: 3200000,
        status: 'processing'
      },
      {
        id: 3,
        ticketNumber: 'TK001236',
        passengerName: 'علی رضایی',
        route: 'تهران - مشهد',
        airline: 'آسمان',
        flightDate: '2024-02-17',
        price: 2800000,
        status: 'hold'
      }
    ]

    const filteredTickets = computed(() => {
      return tickets.value.filter(ticket => {
        const matchesSearch = !searchTerm.value || 
          ticket.ticketNumber.toLowerCase().includes(searchTerm.value.toLowerCase()) ||
          ticket.passengerName.includes(searchTerm.value)
        const matchesAirline = !selectedAirline.value || ticket.airline === selectedAirline.value
        const matchesStatus = !selectedStatus.value || ticket.status === selectedStatus.value
        const matchesDate = !selectedDate.value || ticket.flightDate === selectedDate.value
        
        return matchesSearch && matchesAirline && matchesStatus && matchesDate
      })
    })

    const loadTickets = async () => {
      loading.value = true
      // Simulate API call
      setTimeout(() => {
        tickets.value = mockTickets
        loading.value = false
      }, 1000)
    }

    const createTicket = () => {
      // Navigate to create ticket page
      console.log('Create new ticket')
    }

    const editTicket = (ticket) => {
      console.log('Edit ticket:', ticket)
    }

    const issueTicket = (ticket) => {
      console.log('Issue ticket:', ticket)
    }

    const deleteTicket = (ticket) => {
      if (confirm('آیا از حذف این بلیت اطمینان دارید؟')) {
        tickets.value = tickets.value.filter(t => t.id !== ticket.id)
      }
    }

    const formatDate = (date) => {
      return new Date(date).toLocaleDateString('fa-IR')
    }

    const formatCurrency = (amount) => {
      return new Intl.NumberFormat('fa-IR').format(amount) + ' ریال'
    }

    const getStatusLabel = (status) => {
      const labels = {
        pending: 'در انتظار',
        processing: 'در حال پردازش',
        hold: 'نگهداری'
      }
      return labels[status] || status
    }

    const getStatusBadgeClass = (status) => {
      const classes = {
        pending: 'bg-yellow-100 text-yellow-800',
        processing: 'bg-blue-100 text-blue-800',
        hold: 'bg-gray-100 text-gray-800'
      }
      return classes[status] || 'bg-gray-100 text-gray-800'
    }

    onMounted(() => {
      loadTickets()
    })

    return {
      loading,
      searchTerm,
      selectedAirline,
      selectedStatus,
      selectedDate,
      tickets,
      filteredTickets,
      createTicket,
      editTicket,
      issueTicket,
      deleteTicket,
      formatDate,
      formatCurrency,
      getStatusLabel,
      getStatusBadgeClass
    }
  }
}
</script>