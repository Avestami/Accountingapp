<template>
  <div class="tickets-view">
    <!-- Header -->
    <div class="flex justify-between items-center mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">Tickets Management</h1>
        <p class="text-gray-600">Manage flight tickets and bookings</p>
      </div>
      <button
        @click="showCreateModal = true"
        class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg flex items-center gap-2"
      >
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"></path>
        </svg>
        New Ticket
      </button>
    </div>

    <!-- Filters -->
    <div class="bg-white rounded-lg shadow-sm border p-4 mb-6">
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Search</label>
          <input
            v-model="filters.searchTerm"
            type="text"
            placeholder="Search tickets..."
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            @input="debouncedSearch"
          />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Status</label>
          <select
            v-model="filters.status"
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            @change="loadTickets"
          >
            <option value="">All Statuses</option>
            <option value="0">Unissued</option>
            <option value="1">Issued</option>
            <option value="2">Cancelled</option>
            <option value="3">Refunded</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Type</label>
          <select
            v-model="filters.type"
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            @change="loadTickets"
          >
            <option value="">All Types</option>
            <option value="0">Domestic</option>
            <option value="1">International</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Within 5 Days</label>
          <select
            v-model="filters.isWithinFiveDays"
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            @change="loadTickets"
          >
            <option value="">All</option>
            <option value="true">Yes</option>
            <option value="false">No</option>
          </select>
        </div>
      </div>
    </div>

    <!-- Tickets Table -->
    <div class="bg-white rounded-lg shadow-sm border overflow-hidden">
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Ticket Number
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Title
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Customer
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Amount
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Status
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Type
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Created
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Actions
              </th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr
              v-for="ticket in tickets"
              :key="ticket.id"
              :class="{ 'bg-red-50': ticket.isWithinFiveDays }"
            >
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                {{ ticket.ticketNumber }}
                <span v-if="ticket.isWithinFiveDays" class="ml-2 inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-red-100 text-red-800">
                  {{ ticket.daysToTravel }}d
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ ticket.title }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ ticket.counterpartyName }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ formatCurrency(ticket.amount, ticket.currency) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span :class="getStatusClass(ticket.status)" class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium">
                  {{ getStatusText(ticket.status) }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ getTypeText(ticket.type) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ formatDate(ticket.createdAt) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                <div class="flex items-center space-x-2">
                  <button
                    @click="viewTicket(ticket)"
                    class="text-blue-600 hover:text-blue-900"
                  >
                    View
                  </button>
                  <button
                    v-if="ticket.status === 0"
                    @click="editTicket(ticket)"
                    class="text-green-600 hover:text-green-900"
                  >
                    Edit
                  </button>
                  <button
                    v-if="ticket.status === 0"
                    @click="issueTicket(ticket)"
                    class="text-purple-600 hover:text-purple-900"
                  >
                    Issue
                  </button>
                  <button
                    v-if="ticket.status !== 2 && ticket.status !== 3"
                    @click="cancelTicket(ticket)"
                    class="text-red-600 hover:text-red-900"
                  >
                    Cancel
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      <div class="bg-white px-4 py-3 flex items-center justify-between border-t border-gray-200 sm:px-6">
        <div class="flex-1 flex justify-between sm:hidden">
          <button
            @click="previousPage"
            :disabled="currentPage === 1"
            class="relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50"
          >
            Previous
          </button>
          <button
            @click="nextPage"
            :disabled="currentPage === totalPages"
            class="ml-3 relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50"
          >
            Next
          </button>
        </div>
        <div class="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
          <div>
            <p class="text-sm text-gray-700">
              Showing <span class="font-medium">{{ startItem }}</span> to <span class="font-medium">{{ endItem }}</span> of
              <span class="font-medium">{{ totalCount }}</span> results
            </p>
          </div>
          <div>
            <nav class="relative z-0 inline-flex rounded-md shadow-sm -space-x-px" aria-label="Pagination">
              <button
                @click="previousPage"
                :disabled="currentPage === 1"
                class="relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50"
              >
                Previous
              </button>
              <button
                v-for="page in visiblePages"
                :key="page"
                @click="goToPage(page)"
                :class="[
                  page === currentPage
                    ? 'z-10 bg-blue-50 border-blue-500 text-blue-600'
                    : 'bg-white border-gray-300 text-gray-500 hover:bg-gray-50',
                  'relative inline-flex items-center px-4 py-2 border text-sm font-medium'
                ]"
              >
                {{ page }}
              </button>
              <button
                @click="nextPage"
                :disabled="currentPage === totalPages"
                class="relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50"
              >
                Next
              </button>
            </nav>
          </div>
        </div>
      </div>
    </div>

    <!-- Create/Edit Modal -->
    <div v-if="showCreateModal || showEditModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-20 mx-auto p-5 border w-11/12 max-w-4xl shadow-lg rounded-md bg-white">
        <div class="mt-3">
          <h3 class="text-lg font-medium text-gray-900 mb-4">
            {{ showCreateModal ? 'Create New Ticket' : 'Edit Ticket' }}
          </h3>
          <TicketForm
            :ticket="selectedTicket"
            :is-edit="showEditModal"
            @save="handleSaveTicket"
            @cancel="closeModals"
          />
        </div>
      </div>
    </div>

    <!-- View Modal -->
    <div v-if="showViewModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-20 mx-auto p-5 border w-11/12 max-w-4xl shadow-lg rounded-md bg-white">
        <div class="mt-3">
          <div class="flex justify-between items-center mb-4">
            <h3 class="text-lg font-medium text-gray-900">Ticket Details</h3>
            <button @click="showViewModal = false" class="text-gray-400 hover:text-gray-600">
              <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
              </svg>
            </button>
          </div>
          <TicketDetails :ticket="selectedTicket" />
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import { useTicketStore } from '@/stores/ticketStore'
import { debounce } from '@/utils/debounce'
import TicketForm from '@/components/tickets/TicketForm.vue'
import TicketDetails from '@/components/tickets/TicketDetails.vue'

export default {
  name: 'TicketsView',
  components: {
    TicketForm,
    TicketDetails
  },
  setup() {
    const ticketStore = useTicketStore()
    
    const tickets = ref([])
    const loading = ref(false)
    const currentPage = ref(1)
    const pageSize = ref(10)
    const totalCount = ref(0)
    const totalPages = ref(0)
    
    const showCreateModal = ref(false)
    const showEditModal = ref(false)
    const showViewModal = ref(false)
    const selectedTicket = ref(null)
    
    const filters = reactive({
      searchTerm: '',
      status: '',
      type: '',
      isWithinFiveDays: ''
    })

    const startItem = computed(() => {
      return (currentPage.value - 1) * pageSize.value + 1
    })

    const endItem = computed(() => {
      return Math.min(currentPage.value * pageSize.value, totalCount.value)
    })

    const visiblePages = computed(() => {
      const pages = []
      const start = Math.max(1, currentPage.value - 2)
      const end = Math.min(totalPages.value, currentPage.value + 2)
      
      for (let i = start; i <= end; i++) {
        pages.push(i)
      }
      
      return pages
    })

    const loadTickets = async () => {
      loading.value = true
      try {
        const query = {
          page: currentPage.value,
          pageSize: pageSize.value,
          searchTerm: filters.searchTerm || undefined,
          status: filters.status ? parseInt(filters.status) : undefined,
          type: filters.type ? parseInt(filters.type) : undefined,
          isWithinFiveDays: filters.isWithinFiveDays ? filters.isWithinFiveDays === 'true' : undefined
        }

        const result = await ticketStore.getTickets(query)
        tickets.value = result.items
        totalCount.value = result.totalCount
        totalPages.value = result.totalPages
      } catch (error) {
        console.error('Error loading tickets:', error)
      } finally {
        loading.value = false
      }
    }

    const debouncedSearch = debounce(() => {
      currentPage.value = 1
      loadTickets()
    }, 300)

    const previousPage = () => {
      if (currentPage.value > 1) {
        currentPage.value--
        loadTickets()
      }
    }

    const nextPage = () => {
      if (currentPage.value < totalPages.value) {
        currentPage.value++
        loadTickets()
      }
    }

    const goToPage = (page) => {
      currentPage.value = page
      loadTickets()
    }

    const viewTicket = (ticket) => {
      selectedTicket.value = ticket
      showViewModal.value = true
    }

    const editTicket = (ticket) => {
      selectedTicket.value = { ...ticket }
      showEditModal.value = true
    }

    const issueTicket = async (ticket) => {
      if (confirm('Are you sure you want to issue this ticket?')) {
        try {
          await ticketStore.issueTicket(ticket.id)
          loadTickets()
        } catch (error) {
          console.error('Error issuing ticket:', error)
        }
      }
    }

    const cancelTicket = async (ticket) => {
      const reason = prompt('Please enter cancellation reason:')
      if (reason) {
        try {
          await ticketStore.cancelTicket(ticket.id, reason)
          loadTickets()
        } catch (error) {
          console.error('Error cancelling ticket:', error)
        }
      }
    }

    const handleSaveTicket = async (ticketData) => {
      try {
        if (showEditModal.value) {
          await ticketStore.updateTicket(selectedTicket.value.id, ticketData)
        } else {
          await ticketStore.createTicket(ticketData)
        }
        closeModals()
        loadTickets()
      } catch (error) {
        console.error('Error saving ticket:', error)
      }
    }

    const closeModals = () => {
      showCreateModal.value = false
      showEditModal.value = false
      showViewModal.value = false
      selectedTicket.value = null
    }

    const getStatusClass = (status) => {
      const classes = {
        0: 'bg-yellow-100 text-yellow-800', // Unissued
        1: 'bg-green-100 text-green-800',   // Issued
        2: 'bg-red-100 text-red-800',       // Cancelled
        3: 'bg-blue-100 text-blue-800'      // Refunded
      }
      return classes[status] || 'bg-gray-100 text-gray-800'
    }

    const getStatusText = (status) => {
      const texts = {
        0: 'Unissued',
        1: 'Issued',
        2: 'Cancelled',
        3: 'Refunded'
      }
      return texts[status] || 'Unknown'
    }

    const getTypeText = (type) => {
      const texts = {
        0: 'Domestic',
        1: 'International'
      }
      return texts[type] || 'Unknown'
    }

    const formatCurrency = (amount, currency) => {
      return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: currency || 'IRR',
        minimumFractionDigits: 0
      }).format(amount)
    }

    const formatDate = (dateString) => {
      return new Date(dateString).toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
      })
    }

    onMounted(() => {
      loadTickets()
    })

    return {
      tickets,
      loading,
      currentPage,
      totalCount,
      totalPages,
      startItem,
      endItem,
      visiblePages,
      filters,
      showCreateModal,
      showEditModal,
      showViewModal,
      selectedTicket,
      loadTickets,
      debouncedSearch,
      previousPage,
      nextPage,
      goToPage,
      viewTicket,
      editTicket,
      issueTicket,
      cancelTicket,
      handleSaveTicket,
      closeModals,
      getStatusClass,
      getStatusText,
      getTypeText,
      formatCurrency,
      formatDate
    }
  }
}
</script>

<style scoped>
.tickets-view {
  padding: 1.5rem;
}
</style>