import { defineStore } from 'pinia'
import { ref } from 'vue'
import api from '@/services/api'

export const useBankStore = defineStore('bank', () => {
  const banks = ref([])
  const currentBank = ref(null)
  const loading = ref(false)
  const error = ref(null)

  // Get all banks with filtering and pagination
  const getBanks = async (query = {}) => {
    loading.value = true
    error.value = null
    
    try {
      const params = new URLSearchParams()
      
      // Add query parameters
      if (query.page) params.append('page', query.page)
      if (query.pageSize) params.append('pageSize', query.pageSize)
      if (query.searchTerm) params.append('searchTerm', query.searchTerm)
      if (query.isActive !== undefined) params.append('isActive', query.isActive)
      if (query.sortBy) params.append('sortBy', query.sortBy)
      if (query.sortDirection) params.append('sortDirection', query.sortDirection)

      const response = await api.get(`/banks?${params.toString()}`)
      banks.value = response.data.items
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to fetch banks'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Get bank by ID
  const getBankById = async (id) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.get(`/banks/${id}`)
      currentBank.value = response.data
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to fetch bank'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Create new bank
  const createBank = async (bankData) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.post('/banks', bankData)
      
      // Add to local banks array if it exists
      if (banks.value) {
        banks.value.unshift(response.data)
      }
      
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to create bank'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Update bank
  const updateBank = async (id, bankData) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.put(`/banks/${id}`, bankData)
      
      // Update in local banks array if it exists
      if (banks.value) {
        const index = banks.value.findIndex(b => b.id === id)
        if (index !== -1) {
          banks.value[index] = response.data
        }
      }
      
      // Update current bank if it's the same
      if (currentBank.value && currentBank.value.id === id) {
        currentBank.value = response.data
      }
      
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to update bank'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Delete bank
  const deleteBank = async (id) => {
    loading.value = true
    error.value = null
    
    try {
      await api.delete(`/banks/${id}`)
      
      // Remove from local banks array if it exists
      if (banks.value) {
        const index = banks.value.findIndex(b => b.id === id)
        if (index !== -1) {
          banks.value.splice(index, 1)
        }
      }
      
      // Clear current bank if it's the same
      if (currentBank.value && currentBank.value.id === id) {
        currentBank.value = null
      }
      
      return true
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to delete bank'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Get active banks only
  const getActiveBanks = async (query = {}) => {
    return await getBanks({ ...query, isActive: true })
  }

  // Clear error
  const clearError = () => {
    error.value = null
  }

  // Clear current bank
  const clearCurrentBank = () => {
    currentBank.value = null
  }

  // Reset store
  const reset = () => {
    banks.value = []
    currentBank.value = null
    loading.value = false
    error.value = null
  }

  return {
    // State
    banks,
    currentBank,
    loading,
    error,
    
    // Actions
    getBanks,
    getBankById,
    createBank,
    updateBank,
    deleteBank,
    getActiveBanks,
    clearError,
    clearCurrentBank,
    reset
  }
})