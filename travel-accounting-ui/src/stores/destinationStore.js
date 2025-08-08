import { defineStore } from 'pinia'
import { ref } from 'vue'
import api from '@/services/api'

export const useDestinationStore = defineStore('destination', () => {
  const destinations = ref([])
  const currentDestination = ref(null)
  const loading = ref(false)
  const error = ref(null)

  // Get all destinations with filtering and pagination
  const getDestinations = async (query = {}) => {
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

      const response = await api.get(`/destinations?${params.toString()}`)
      destinations.value = response.data.items
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to fetch destinations'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Get destination by ID
  const getDestinationById = async (id) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.get(`/destinations/${id}`)
      currentDestination.value = response.data
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to fetch destination'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Create new destination
  const createDestination = async (destinationData) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.post('/destinations', destinationData)
      
      // Add to local destinations array if it exists
      if (destinations.value) {
        destinations.value.unshift(response.data)
      }
      
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to create destination'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Update destination
  const updateDestination = async (id, destinationData) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.put(`/destinations/${id}`, destinationData)
      
      // Update in local destinations array if it exists
      if (destinations.value) {
        const index = destinations.value.findIndex(d => d.id === id)
        if (index !== -1) {
          destinations.value[index] = response.data
        }
      }
      
      // Update current destination if it's the same
      if (currentDestination.value && currentDestination.value.id === id) {
        currentDestination.value = response.data
      }
      
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to update destination'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Delete destination
  const deleteDestination = async (id) => {
    loading.value = true
    error.value = null
    
    try {
      await api.delete(`/destinations/${id}`)
      
      // Remove from local destinations array if it exists
      if (destinations.value) {
        const index = destinations.value.findIndex(d => d.id === id)
        if (index !== -1) {
          destinations.value.splice(index, 1)
        }
      }
      
      // Clear current destination if it's the same
      if (currentDestination.value && currentDestination.value.id === id) {
        currentDestination.value = null
      }
      
      return true
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to delete destination'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Get active destinations only
  const getActiveDestinations = async (query = {}) => {
    return await getDestinations({ ...query, isActive: true })
  }

  // Clear error
  const clearError = () => {
    error.value = null
  }

  // Clear current destination
  const clearCurrentDestination = () => {
    currentDestination.value = null
  }

  // Reset store
  const reset = () => {
    destinations.value = []
    currentDestination.value = null
    loading.value = false
    error.value = null
  }

  return {
    // State
    destinations,
    currentDestination,
    loading,
    error,
    
    // Actions
    getDestinations,
    getDestinationById,
    createDestination,
    updateDestination,
    deleteDestination,
    getActiveDestinations,
    clearError,
    clearCurrentDestination,
    reset
  }
})