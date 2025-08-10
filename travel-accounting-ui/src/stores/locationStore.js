import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { locationsApi } from '@/services/api'

export const useLocationStore = defineStore('location', () => {
  // State
  const locations = ref([])
  const currentLocation = ref(null)
  const loading = ref(false)
  const error = ref(null)

  // Getters
  const getActiveLocations = computed(() => 
    locations.value.filter(location => location.isActive)
  )

  // Actions
  const getLocations = async (filters = {}) => {
    loading.value = true
    error.value = null
    
    try {
      const params = new URLSearchParams()
      
      // Add query parameters
      if (filters.page) params.append('page', filters.page)
      if (filters.pageSize) params.append('pageSize', filters.pageSize)
      if (filters.searchTerm) params.append('searchTerm', filters.searchTerm)
      if (filters.isActive !== undefined) params.append('isActive', filters.isActive)
      if (filters.type) params.append('type', filters.type)
      if (filters.countryId) params.append('countryId', filters.countryId)
      if (filters.sortBy) params.append('sortBy', filters.sortBy)
      if (filters.sortDirection) params.append('sortDirection', filters.sortDirection)

      const response = await locationsApi.getLocations(Object.fromEntries(params))
      locations.value = response.data?.items || response.data || response
      return response.data || response
    } catch (err) {
      error.value = err.response?.data?.message || err.message || 'Failed to fetch locations'
      console.error('Error fetching locations:', err)
      throw err
    } finally {
      loading.value = false
    }
  }

  const getLocationById = async (id) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await locationsApi.getLocation(id)
      currentLocation.value = response.data || response
      return response.data || response
    } catch (err) {
      error.value = err.response?.data?.message || err.message || 'Failed to fetch location'
      console.error('Error fetching location:', err)
      throw err
    } finally {
      loading.value = false
    }
  }

  const createLocation = async (locationData) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await locationsApi.createLocation(locationData)
      const newLocation = response.data || response
      
      // Add to local locations array if it exists
      if (locations.value) {
        locations.value.unshift(newLocation)
      }
      
      return newLocation
    } catch (err) {
      error.value = err.response?.data?.message || err.message || 'Failed to create location'
      console.error('Error creating location:', err)
      throw err
    } finally {
      loading.value = false
    }
  }

  const updateLocation = async (id, locationData) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await locationsApi.updateLocation(id, locationData)
      const updatedLocation = response.data || response
      
      // Update in local locations array if it exists
      if (locations.value) {
        const index = locations.value.findIndex(l => l.id === id)
        if (index !== -1) {
          locations.value[index] = updatedLocation
        }
      }
      
      // Update current location if it's the same
      if (currentLocation.value && currentLocation.value.id === id) {
        currentLocation.value = updatedLocation
      }
      
      return updatedLocation
    } catch (err) {
      error.value = err.response?.data?.message || err.message || 'Failed to update location'
      console.error('Error updating location:', err)
      throw err
    } finally {
      loading.value = false
    }
  }

  const deleteLocation = async (id) => {
    loading.value = true
    error.value = null
    
    try {
      await locationsApi.deleteLocation(id)
      
      // Remove from local locations array if it exists
      if (locations.value) {
        const index = locations.value.findIndex(l => l.id === id)
        if (index !== -1) {
          locations.value.splice(index, 1)
        }
      }
      
      // Clear current location if it's the same
      if (currentLocation.value && currentLocation.value.id === id) {
        currentLocation.value = null
      }
      
      return true
    } catch (err) {
      error.value = err.response?.data?.message || err.message || 'Failed to delete location'
      console.error('Error deleting location:', err)
      throw err
    } finally {
      loading.value = false
    }
  }

  // Get countries
  const getCountries = async () => {
    try {
      const response = await locationsApi.getCountries()
      return response.data || response
    } catch (err) {
      error.value = err.response?.data?.message || err.message || 'Failed to fetch countries'
      throw err
    }
  }

  // Get cities by country
  const getCitiesByCountry = async (countryId) => {
    try {
      const response = await locationsApi.getCitiesByCountry(countryId)
      return response.data || response
    } catch (err) {
      error.value = err.response?.data?.message || err.message || 'Failed to fetch cities'
      throw err
    }
  }

  // Get active locations only
  const getActiveLocationsList = async (query = {}) => {
    return await getLocations({ ...query, isActive: true })
  }

  // Utility functions
  const clearError = () => {
    error.value = null
  }

  const clearCurrentLocation = () => {
    currentLocation.value = null
  }

  const reset = () => {
    locations.value = []
    currentLocation.value = null
    loading.value = false
    error.value = null
  }

  return {
    // State
    locations,
    currentLocation,
    loading,
    error,
    
    // Getters
    getActiveLocations,
    
    // Actions
    getLocations,
    getLocationById,
    createLocation,
    updateLocation,
    deleteLocation,
    getCountries,
    getCitiesByCountry,
    getActiveLocationsList,
    clearError,
    clearCurrentLocation,
    reset
  }
})