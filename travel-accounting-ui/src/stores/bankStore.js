import { defineStore } from 'pinia'
import { ref } from 'vue'
import { banksApi } from '@/services/api'

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

      const response = await banksApi.getBanks(Object.fromEntries(params))
      banks.value = response.data?.items || response.data || response
      return response.data || response
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
      const response = await banksApi.getBank(id)
      currentBank.value = response.data || response
      return response.data || response
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
      const response = await banksApi.createBank(bankData)
      const newBank = response.data || response
      
      // Add to local banks array if it exists
      if (banks.value) {
        banks.value.unshift(newBank)
      }
      
      return newBank
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
      const response = await banksApi.updateBank(id, bankData)
      const updatedBank = response.data || response
      
      // Update in local banks array if it exists
      if (banks.value) {
        const index = banks.value.findIndex(b => b.id === id)
        if (index !== -1) {
          banks.value[index] = updatedBank
        }
      }
      
      // Update current bank if it's the same
      if (currentBank.value && currentBank.value.id === id) {
        currentBank.value = updatedBank
      }
      
      return updatedBank
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
      await banksApi.deleteBank(id)
      
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