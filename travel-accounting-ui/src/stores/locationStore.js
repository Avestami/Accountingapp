import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

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
      // Mock API call - replace with actual API endpoint
      const response = await fetch('/api/locations', {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          // Add authorization header if needed
        }
      })
      
      if (!response.ok) {
        throw new Error('Failed to fetch locations')
      }
      
      const data = await response.json()
      locations.value = data
    } catch (err) {
      error.value = err.message
      console.error('Error fetching locations:', err)
    } finally {
      loading.value = false
    }
  }

  const getLocationById = async (id) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await fetch(`/api/locations/${id}`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        }
      })
      
      if (!response.ok) {
        throw new Error('Failed to fetch location')
      }
      
      const data = await response.json()
      currentLocation.value = data
      return data
    } catch (err) {
      error.value = err.message
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
      const response = await fetch('/api/locations', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(locationData)
      })
      
      if (!response.ok) {
        throw new Error('Failed to create location')
      }
      
      const newLocation = await response.json()
      locations.value.push(newLocation)
      return newLocation
    } catch (err) {
      error.value = err.message
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
      const response = await fetch(`/api/locations/${id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(locationData)
      })
      
      if (!response.ok) {
        throw new Error('Failed to update location')
      }
      
      const updatedLocation = await response.json()
      const index = locations.value.findIndex(l => l.id === id)
      if (index !== -1) {
        locations.value[index] = updatedLocation
      }
      
      if (currentLocation.value && currentLocation.value.id === id) {
        currentLocation.value = updatedLocation
      }
      
      return updatedLocation
    } catch (err) {
      error.value = err.message
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
      const response = await fetch(`/api/locations/${id}`, {
        method: 'DELETE',
        headers: {
          'Content-Type': 'application/json',
        }
      })
      
      if (!response.ok) {
        throw new Error('Failed to delete location')
      }
      
      locations.value = locations.value.filter(l => l.id !== id)
      
      if (currentLocation.value && currentLocation.value.id === id) {
        currentLocation.value = null
      }
    } catch (err) {
      error.value = err.message
      console.error('Error deleting location:', err)
      throw err
    } finally {
      loading.value = false
    }
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
    
    // Utilities
    clearError,
    clearCurrentLocation,
    reset
  }
})