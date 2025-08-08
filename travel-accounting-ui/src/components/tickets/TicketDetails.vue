<template>
  <div class="ticket-details">
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- Main Ticket Information -->
      <div class="lg:col-span-2">
        <div class="bg-white border rounded-lg p-6">
          <div class="flex justify-between items-start mb-6">
            <div>
              <h2 class="text-xl font-semibold text-gray-900">{{ ticket.title }}</h2>
              <p class="text-sm text-gray-500 mt-1">Ticket #{{ ticket.ticketNumber }}</p>
            </div>
            <div class="text-right">
              <span :class="getStatusClass(ticket.status)" class="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium">
                {{ getStatusText(ticket.status) }}
              </span>
              <p class="text-sm text-gray-500 mt-1">{{ getTypeText(ticket.type) }}</p>
            </div>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
            <div>
              <h3 class="text-sm font-medium text-gray-500 uppercase tracking-wide mb-2">Customer Information</h3>
              <p class="text-lg font-medium text-gray-900">{{ ticket.counterpartyName }}</p>
            </div>
            <div>
              <h3 class="text-sm font-medium text-gray-500 uppercase tracking-wide mb-2">Total Amount</h3>
              <p class="text-lg font-medium text-gray-900">{{ formatCurrency(ticket.amount, ticket.currency) }}</p>
            </div>
          </div>

          <div v-if="ticket.description" class="mb-6">
            <h3 class="text-sm font-medium text-gray-500 uppercase tracking-wide mb-2">Description</h3>
            <p class="text-gray-900">{{ ticket.description }}</p>
          </div>

          <!-- Ticket Items -->
          <div>
            <h3 class="text-lg font-medium text-gray-900 mb-4">Flight Details</h3>
            <div class="space-y-4">
              <div
                v-for="(item, index) in ticket.items"
                :key="item.id || index"
                class="border border-gray-200 rounded-lg p-4"
              >
                <div class="flex justify-between items-start mb-3">
                  <h4 class="font-medium text-gray-900">{{ item.passengerName }}</h4>
                  <div class="text-right">
                    <p class="font-medium text-gray-900">{{ formatCurrency(item.amount, item.currency) }}</p>
                    <p v-if="item.taxAmount > 0" class="text-sm text-gray-500">
                      Tax: {{ formatCurrency(item.taxAmount, item.currency) }}
                    </p>
                  </div>
                </div>

                <div class="grid grid-cols-1 md:grid-cols-2 gap-4 text-sm">
                  <div>
                    <span class="text-gray-500">Airline:</span>
                    <span class="ml-2 text-gray-900">{{ item.airlineName }}</span>
                  </div>
                  <div>
                    <span class="text-gray-500">Flight:</span>
                    <span class="ml-2 text-gray-900">{{ item.flightNumber || 'N/A' }}</span>
                  </div>
                  <div>
                    <span class="text-gray-500">Route:</span>
                    <span class="ml-2 text-gray-900">{{ item.originName }} → {{ item.destinationName }}</span>
                  </div>
                  <div>
                    <span class="text-gray-500">Service Date:</span>
                    <span class="ml-2 text-gray-900">{{ formatDate(item.serviceDate) }}</span>
                    <span v-if="isWithinFiveDays(item.serviceDate)" class="ml-2 inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-red-100 text-red-800">
                      {{ getDaysToTravel(item.serviceDate) }}d
                    </span>
                  </div>
                </div>

                <div v-if="item.notes" class="mt-3 pt-3 border-t border-gray-100">
                  <span class="text-gray-500 text-sm">Notes:</span>
                  <p class="text-gray-900 text-sm mt-1">{{ item.notes }}</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Sidebar Information -->
      <div class="space-y-6">
        <!-- Status Card -->
        <div class="bg-white border rounded-lg p-6">
          <h3 class="text-lg font-medium text-gray-900 mb-4">Status Information</h3>
          <div class="space-y-3">
            <div class="flex justify-between">
              <span class="text-gray-500">Status:</span>
              <span :class="getStatusClass(ticket.status)" class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium">
                {{ getStatusText(ticket.status) }}
              </span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-500">Type:</span>
              <span class="text-gray-900">{{ getTypeText(ticket.type) }}</span>
            </div>
            <div v-if="ticket.isWithinFiveDays" class="flex justify-between">
              <span class="text-gray-500">Travel Alert:</span>
              <span class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-red-100 text-red-800">
                {{ ticket.daysToTravel }}d to travel
              </span>
            </div>
          </div>
        </div>

        <!-- Dates Card -->
        <div class="bg-white border rounded-lg p-6">
          <h3 class="text-lg font-medium text-gray-900 mb-4">Important Dates</h3>
          <div class="space-y-3">
            <div class="flex justify-between">
              <span class="text-gray-500">Created:</span>
              <span class="text-gray-900">{{ formatDateTime(ticket.createdAt) }}</span>
            </div>
            <div v-if="ticket.modifiedAt && ticket.modifiedAt !== ticket.createdAt" class="flex justify-between">
              <span class="text-gray-500">Modified:</span>
              <span class="text-gray-900">{{ formatDateTime(ticket.modifiedAt) }}</span>
            </div>
            <div v-if="ticket.items && ticket.items.length > 0" class="flex justify-between">
              <span class="text-gray-500">Service Date:</span>
              <span class="text-gray-900">{{ formatDate(getEarliestServiceDate()) }}</span>
            </div>
          </div>
        </div>

        <!-- Summary Card -->
        <div class="bg-white border rounded-lg p-6">
          <h3 class="text-lg font-medium text-gray-900 mb-4">Summary</h3>
          <div class="space-y-3">
            <div class="flex justify-between">
              <span class="text-gray-500">Total Items:</span>
              <span class="text-gray-900">{{ ticket.items?.length || 0 }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-500">Total Amount:</span>
              <span class="text-gray-900 font-medium">{{ formatCurrency(ticket.amount, ticket.currency) }}</span>
            </div>
            <div v-if="getTotalTax() > 0" class="flex justify-between">
              <span class="text-gray-500">Total Tax:</span>
              <span class="text-gray-900">{{ formatCurrency(getTotalTax(), ticket.currency) }}</span>
            </div>
          </div>
        </div>

        <!-- Actions Card -->
        <div v-if="showActions" class="bg-white border rounded-lg p-6">
          <h3 class="text-lg font-medium text-gray-900 mb-4">Actions</h3>
          <div class="space-y-2">
            <button
              v-if="ticket.status === 0"
              @click="$emit('edit', ticket)"
              class="w-full bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md text-sm font-medium"
            >
              Edit Ticket
            </button>
            <button
              v-if="ticket.status === 0"
              @click="$emit('issue', ticket)"
              class="w-full bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded-md text-sm font-medium"
            >
              Issue Ticket
            </button>
            <button
              v-if="ticket.status !== 2 && ticket.status !== 3"
              @click="$emit('cancel', ticket)"
              class="w-full bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded-md text-sm font-medium"
            >
              Cancel Ticket
            </button>
            <button
              @click="$emit('print', ticket)"
              class="w-full bg-gray-600 hover:bg-gray-700 text-white px-4 py-2 rounded-md text-sm font-medium"
            >
              Print Ticket
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { computed } from 'vue'

export default {
  name: 'TicketDetails',
  props: {
    ticket: {
      type: Object,
      required: true
    },
    showActions: {
      type: Boolean,
      default: false
    }
  },
  emits: ['edit', 'issue', 'cancel', 'print'],
  setup(props) {
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
      if (!dateString) return 'N/A'
      return new Date(dateString).toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
      })
    }

    const formatDateTime = (dateString) => {
      if (!dateString) return 'N/A'
      return new Date(dateString).toLocaleString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      })
    }

    const isWithinFiveDays = (serviceDate) => {
      if (!serviceDate) return false
      const today = new Date()
      const service = new Date(serviceDate)
      const diffTime = service.getTime() - today.getTime()
      const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24))
      return diffDays >= 0 && diffDays <= 5
    }

    const getDaysToTravel = (serviceDate) => {
      if (!serviceDate) return 0
      const today = new Date()
      const service = new Date(serviceDate)
      const diffTime = service.getTime() - today.getTime()
      const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24))
      return Math.max(0, diffDays)
    }

    const getEarliestServiceDate = () => {
      if (!props.ticket.items || props.ticket.items.length === 0) return null
      
      const dates = props.ticket.items
        .map(item => item.serviceDate)
        .filter(date => date)
        .sort()
      
      return dates.length > 0 ? dates[0] : null
    }

    const getTotalTax = () => {
      if (!props.ticket.items) return 0
      return props.ticket.items.reduce((total, item) => total + (item.taxAmount || 0), 0)
    }

    return {
      getStatusClass,
      getStatusText,
      getTypeText,
      formatCurrency,
      formatDate,
      formatDateTime,
      isWithinFiveDays,
      getDaysToTravel,
      getEarliestServiceDate,
      getTotalTax
    }
  }
}
</script>

<style scoped>
.ticket-details {
  max-height: 80vh;
  overflow-y: auto;
}
</style>