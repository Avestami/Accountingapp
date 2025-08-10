import { defineStore } from 'pinia'
import { ref } from 'vue'
import { counterpartiesApi } from '@/services/api'

export const useCounterpartyStore = defineStore('counterparty', () => {
  const counterparties = ref([])
  const currentCounterparty = ref(null)
  const loading = ref(false)
  const error = ref(null)

  // Get all counterparties with filtering and pagination
  const getCounterparties = async (query = {}) => {
    loading.value = true
    error.value = null
    
    try {
      const params = new URLSearchParams()
      
      // Add query parameters
      if (query.page) params.append('page', query.page)
      if (query.pageSize) params.append('pageSize', query.pageSize)
      if (query.searchTerm) params.append('searchTerm', query.searchTerm)
      if (query.isCustomer !== undefined) params.append('isCustomer', query.isCustomer)
      if (query.isSupplier !== undefined) params.append('isSupplier', query.isSupplier)
      if (query.isActive !== undefined) params.append('isActive', query.isActive)
      if (query.currency) params.append('currency', query.currency)
      if (query.minBalance !== undefined) params.append('minBalance', query.minBalance)
      if (query.maxBalance !== undefined) params.append('maxBalance', query.maxBalance)
      if (query.sortBy) params.append('sortBy', query.sortBy)
      if (query.sortDirection) params.append('sortDirection', query.sortDirection)

      const response = await counterpartiesApi.getCounterparties(Object.fromEntries(params))
      counterparties.value = response.data?.items || response.data || response
      return response.data || response
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to fetch counterparties'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Get counterparty by ID
  const getCounterpartyById = async (id) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await counterpartiesApi.getCounterparty(id)
      currentCounterparty.value = response.data || response
      return response.data || response
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to fetch counterparty'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Create new counterparty
  const createCounterparty = async (counterpartyData) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await counterpartiesApi.createCounterparty(counterpartyData)
      const newCounterparty = response.data || response
      
      // Add to local counterparties array if it exists
      if (counterparties.value) {
        counterparties.value.unshift(newCounterparty)
      }
      
      return newCounterparty
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to create counterparty'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Update counterparty
  const updateCounterparty = async (id, counterpartyData) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await counterpartiesApi.updateCounterparty(id, counterpartyData)
      const updatedCounterparty = response.data || response
      
      // Update in local counterparties array if it exists
      if (counterparties.value) {
        const index = counterparties.value.findIndex(c => c.id === id)
        if (index !== -1) {
          counterparties.value[index] = updatedCounterparty
        }
      }
      
      // Update current counterparty if it's the same
      if (currentCounterparty.value && currentCounterparty.value.id === id) {
        currentCounterparty.value = updatedCounterparty
      }
      
      return updatedCounterparty
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to update counterparty'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Delete counterparty
  const deleteCounterparty = async (id) => {
    loading.value = true
    error.value = null
    
    try {
      await counterpartiesApi.deleteCounterparty(id)
      
      // Remove from local counterparties array if it exists
      if (counterparties.value) {
        const index = counterparties.value.findIndex(c => c.id === id)
        if (index !== -1) {
          counterparties.value.splice(index, 1)
        }
      }
      
      // Clear current counterparty if it's the same
      if (currentCounterparty.value && currentCounterparty.value.id === id) {
        currentCounterparty.value = null
      }
      
      return true
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to delete counterparty'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Get customers only
  const getCustomers = async (query = {}) => {
    return await getCounterparties({ ...query, isCustomer: true })
  }

  // Get suppliers only
  const getSuppliers = async (query = {}) => {
    return await getCounterparties({ ...query, isSupplier: true })
  }

  // Get active counterparties only
  const getActiveCounterparties = async (query = {}) => {
    return await getCounterparties({ ...query, isActive: true })
  }

  // Get counterparties summary/statistics
  const getCounterpartiesSummary = async () => {
    try {
      const response = await counterpartiesApi.getCounterpartiesSummary()
      return response.data || response
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to fetch counterparties summary'
      throw err
    }
  }

  // Clear error
  const clearError = () => {
    error.value = null
  }

  // Clear current counterparty
  const clearCurrentCounterparty = () => {
    currentCounterparty.value = null
  }

  // Reset store
  const reset = () => {
    counterparties.value = []
    currentCounterparty.value = null
    loading.value = false
    error.value = null
  }

  return {
    // State
    counterparties,
    currentCounterparty,
    loading,
    error,
    
    // Actions
    getCounterparties,
    getCounterpartyById,
    createCounterparty,
    updateCounterparty,
    deleteCounterparty,
    getCustomers,
    getSuppliers,
    getActiveCounterparties,
    getCounterpartiesSummary,
    clearError,
    clearCurrentCounterparty,
    reset
  }
})