import { defineStore } from 'pinia'
import { ref } from 'vue'
import api from '@/services/api'

export const useAirlineStore = defineStore('airline', () => {
  const airlines = ref([])
  const currentAirline = ref(null)
  const loading = ref(false)
  const error = ref(null)

  // Get all airlines with filtering and pagination
  const getAirlines = async (query = {}) => {
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

      const response = await api.get(`/airlines?${params.toString()}`)
      airlines.value = response.data.items
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to fetch airlines'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Get airline by ID
  const getAirlineById = async (id) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.get(`/airlines/${id}`)
      currentAirline.value = response.data
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to fetch airline'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Create new airline
  const createAirline = async (airlineData) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.post('/airlines', airlineData)
      
      // Add to local airlines array if it exists
      if (airlines.value) {
        airlines.value.unshift(response.data)
      }
      
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to create airline'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Update airline
  const updateAirline = async (id, airlineData) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.put(`/airlines/${id}`, airlineData)
      
      // Update in local airlines array if it exists
      if (airlines.value) {
        const index = airlines.value.findIndex(a => a.id === id)
        if (index !== -1) {
          airlines.value[index] = response.data
        }
      }
      
      // Update current airline if it's the same
      if (currentAirline.value && currentAirline.value.id === id) {
        currentAirline.value = response.data
      }
      
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to update airline'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Delete airline
  const deleteAirline = async (id) => {
    loading.value = true
    error.value = null
    
    try {
      await api.delete(`/airlines/${id}`)
      
      // Remove from local airlines array if it exists
      if (airlines.value) {
        const index = airlines.value.findIndex(a => a.id === id)
        if (index !== -1) {
          airlines.value.splice(index, 1)
        }
      }
      
      // Clear current airline if it's the same
      if (currentAirline.value && currentAirline.value.id === id) {
        currentAirline.value = null
      }
      
      return true
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to delete airline'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Get active airlines only
  const getActiveAirlines = async (query = {}) => {
    return await getAirlines({ ...query, isActive: true })
  }

  // Clear error
  const clearError = () => {
    error.value = null
  }

  // Clear current airline
  const clearCurrentAirline = () => {
    currentAirline.value = null
  }

  // Reset store
  const reset = () => {
    airlines.value = []
    currentAirline.value = null
    loading.value = false
    error.value = null
  }

  return {
    // State
    airlines,
    currentAirline,
    loading,
    error,
    
    // Actions
    getAirlines,
    getAirlineById,
    createAirline,
    updateAirline,
    deleteAirline,
    getActiveAirlines,
    clearError,
    clearCurrentAirline,
    reset
  }
})