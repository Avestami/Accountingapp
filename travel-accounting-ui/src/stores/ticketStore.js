import { defineStore } from 'pinia'
import { ref } from 'vue'
import api from '@/services/api'

export const useTicketStore = defineStore('ticket', () => {
  const tickets = ref([])
  const currentTicket = ref(null)
  const loading = ref(false)
  const error = ref(null)

  // Get all tickets with filtering and pagination
  const getTickets = async (query = {}) => {
    loading.value = true
    error.value = null
    
    try {
      const params = new URLSearchParams()
      
      // Add query parameters
      if (query.page) params.append('page', query.page)
      if (query.pageSize) params.append('pageSize', query.pageSize)
      if (query.searchTerm) params.append('searchTerm', query.searchTerm)
      if (query.status !== undefined) params.append('status', query.status)
      if (query.type !== undefined) params.append('type', query.type)
      if (query.counterpartyId) params.append('counterpartyId', query.counterpartyId)
      if (query.currency) params.append('currency', query.currency)
      if (query.minAmount !== undefined) params.append('minAmount', query.minAmount)
      if (query.maxAmount !== undefined) params.append('maxAmount', query.maxAmount)
      if (query.startDate) params.append('startDate', query.startDate)
      if (query.endDate) params.append('endDate', query.endDate)
      if (query.isWithinFiveDays !== undefined) params.append('isWithinFiveDays', query.isWithinFiveDays)
      if (query.sortBy) params.append('sortBy', query.sortBy)
      if (query.sortDirection) params.append('sortDirection', query.sortDirection)

      const response = await api.get(`/tickets?${params.toString()}`)
      tickets.value = response.data.items
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to fetch tickets'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Get ticket by ID
  const getTicketById = async (id) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.get(`/tickets/${id}`)
      currentTicket.value = response.data
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to fetch ticket'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Create new ticket
  const createTicket = async (ticketData) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.post('/tickets', ticketData)
      
      // Add to local tickets array if it exists
      if (tickets.value) {
        tickets.value.unshift(response.data)
      }
      
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to create ticket'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Update ticket
  const updateTicket = async (id, ticketData) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.put(`/tickets/${id}`, ticketData)
      
      // Update in local tickets array if it exists
      if (tickets.value) {
        const index = tickets.value.findIndex(t => t.id === id)
        if (index !== -1) {
          tickets.value[index] = response.data
        }
      }
      
      // Update current ticket if it's the same
      if (currentTicket.value && currentTicket.value.id === id) {
        currentTicket.value = response.data
      }
      
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to update ticket'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Delete ticket
  const deleteTicket = async (id) => {
    loading.value = true
    error.value = null
    
    try {
      await api.delete(`/tickets/${id}`)
      
      // Remove from local tickets array if it exists
      if (tickets.value) {
        const index = tickets.value.findIndex(t => t.id === id)
        if (index !== -1) {
          tickets.value.splice(index, 1)
        }
      }
      
      // Clear current ticket if it's the same
      if (currentTicket.value && currentTicket.value.id === id) {
        currentTicket.value = null
      }
      
      return true
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to delete ticket'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Issue ticket
  const issueTicket = async (id, notes = '') => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.post(`/tickets/${id}/issue`, { notes })
      
      // Update in local tickets array if it exists
      if (tickets.value) {
        const index = tickets.value.findIndex(t => t.id === id)
        if (index !== -1) {
          tickets.value[index] = response.data
        }
      }
      
      // Update current ticket if it's the same
      if (currentTicket.value && currentTicket.value.id === id) {
        currentTicket.value = response.data
      }
      
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to issue ticket'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Cancel ticket
  const cancelTicket = async (id, reason, notes = '') => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.post(`/tickets/${id}/cancel`, { reason, notes })
      
      // Update in local tickets array if it exists
      if (tickets.value) {
        const index = tickets.value.findIndex(t => t.id === id)
        if (index !== -1) {
          tickets.value[index] = response.data
        }
      }
      
      // Update current ticket if it's the same
      if (currentTicket.value && currentTicket.value.id === id) {
        currentTicket.value = response.data
      }
      
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to cancel ticket'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Get tickets summary/statistics
  const getTicketsSummary = async () => {
    try {
      const response = await api.get('/tickets/summary')
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to fetch tickets summary'
      throw err
    }
  }

  // Get tickets within 5 days
  const getTicketsWithinFiveDays = async () => {
    try {
      const response = await getTickets({ isWithinFiveDays: true })
      return response
    } catch (err) {
      throw err
    }
  }

  // Clear error
  const clearError = () => {
    error.value = null
  }

  // Clear current ticket
  const clearCurrentTicket = () => {
    currentTicket.value = null
  }

  // Reset store
  const reset = () => {
    tickets.value = []
    currentTicket.value = null
    loading.value = false
    error.value = null
  }

  return {
    // State
    tickets,
    currentTicket,
    loading,
    error,
    
    // Actions
    getTickets,
    getTicketById,
    createTicket,
    updateTicket,
    deleteTicket,
    issueTicket,
    cancelTicket,
    getTicketsSummary,
    getTicketsWithinFiveDays,
    clearError,
    clearCurrentTicket,
    reset
  }
})