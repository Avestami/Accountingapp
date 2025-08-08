import { defineStore } from 'pinia'
import { ref } from 'vue'
import api from '@/services/api'

export const useOriginStore = defineStore('origin', () => {
  const origins = ref([])
  const currentOrigin = ref(null)
  const loading = ref(false)
  const error = ref(null)

  // Get all origins with filtering and pagination
  const getOrigins = async (query = {}) => {
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

      const response = await api.get(`/origins?${params.toString()}`)
      origins.value = response.data.items
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to fetch origins'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Get origin by ID
  const getOriginById = async (id) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.get(`/origins/${id}`)
      currentOrigin.value = response.data
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to fetch origin'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Create new origin
  const createOrigin = async (originData) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.post('/origins', originData)
      
      // Add to local origins array if it exists
      if (origins.value) {
        origins.value.unshift(response.data)
      }
      
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to create origin'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Update origin
  const updateOrigin = async (id, originData) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.put(`/origins/${id}`, originData)
      
      // Update in local origins array if it exists
      if (origins.value) {
        const index = origins.value.findIndex(o => o.id === id)
        if (index !== -1) {
          origins.value[index] = response.data
        }
      }
      
      // Update current origin if it's the same
      if (currentOrigin.value && currentOrigin.value.id === id) {
        currentOrigin.value = response.data
      }
      
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to update origin'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Delete origin
  const deleteOrigin = async (id) => {
    loading.value = true
    error.value = null
    
    try {
      await api.delete(`/origins/${id}`)
      
      // Remove from local origins array if it exists
      if (origins.value) {
        const index = origins.value.findIndex(o => o.id === id)
        if (index !== -1) {
          origins.value.splice(index, 1)
        }
      }
      
      // Clear current origin if it's the same
      if (currentOrigin.value && currentOrigin.value.id === id) {
        currentOrigin.value = null
      }
      
      return true
    } catch (err) {
      error.value = err.response?.data?.message || 'Failed to delete origin'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Get active origins only
  const getActiveOrigins = async (query = {}) => {
    return await getOrigins({ ...query, isActive: true })
  }

  // Clear error
  const clearError = () => {
    error.value = null
  }

  // Clear current origin
  const clearCurrentOrigin = () => {
    currentOrigin.value = null
  }

  // Reset store
  const reset = () => {
    origins.value = []
    currentOrigin.value = null
    loading.value = false
    error.value = null
  }

  return {
    // State
    origins,
    currentOrigin,
    loading,
    error,
    
    // Actions
    getOrigins,
    getOriginById,
    createOrigin,
    updateOrigin,
    deleteOrigin,
    getActiveOrigins,
    clearError,
    clearCurrentOrigin,
    reset
  }
})