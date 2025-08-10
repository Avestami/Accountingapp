import { defineStore } from 'pinia'
import { mockData } from '@/services/mockData'

export const useSettingsStore = defineStore('settings', {
  state: () => ({
    // Master data
    airlines: [],
    counterparties: [],
    banks: [],
    locations: [],
    users: [],
    roles: [],
    
    // System settings
    systemSettings: {
      companyName: 'سیستم حسابداری سفر',
      currency: 'IRR',
      sessionTimeout: 60,
      passwordPolicy: 'medium'
    },
    
    // UI state
    loading: false,
    error: null,
    
    // Current editing items
    currentAirline: null,
    currentCounterparty: null,
    currentBank: null,
    currentLocation: null,
    currentUser: null,
    currentRole: null,
    
    // Filters and pagination
    filters: {
      searchTerm: '',
      status: 'all',
      type: 'all'
    },
    
    pagination: {
      page: 1,
      limit: 20,
      total: 0
    }
  }),

  getters: {
    // Active airlines
    activeAirlines: (state) => {
      return state.airlines.filter(airline => airline.isActive)
    },
    
    // Active counterparties by type
    activeBuyers: (state) => {
      return state.counterparties.filter(cp => cp.type === 'buyer' && cp.isActive)
    },
    
    activeSuppliers: (state) => {
      return state.counterparties.filter(cp => cp.type === 'supplier' && cp.isActive)
    },
    
    // Active banks
    activeBanks: (state) => {
      return state.banks.filter(bank => bank.isActive)
    },
    
    // Active locations
    activeLocations: (state) => {
      return state.locations.filter(location => location.isActive)
    },
    
    // Active users
    activeUsers: (state) => {
      return state.users.filter(user => user.isActive)
    },
    
    // Get filtered airlines
    filteredAirlines: (state) => {
      let filtered = [...state.airlines]
      
      if (state.filters.searchTerm) {
        const searchTerm = state.filters.searchTerm.toLowerCase()
        filtered = filtered.filter(airline => 
          airline.name.toLowerCase().includes(searchTerm) ||
          airline.code.toLowerCase().includes(searchTerm) ||
          airline.iataCode?.toLowerCase().includes(searchTerm)
        )
      }
      
      if (state.filters.status !== 'all') {
        const isActive = state.filters.status === 'active'
        filtered = filtered.filter(airline => airline.isActive === isActive)
      }
      
      return filtered.sort((a, b) => a.name.localeCompare(b.name))
    },
    
    // Get filtered counterparties
    filteredCounterparties: (state) => {
      let filtered = [...state.counterparties]
      
      if (state.filters.searchTerm) {
        const searchTerm = state.filters.searchTerm.toLowerCase()
        filtered = filtered.filter(cp => 
          cp.name.toLowerCase().includes(searchTerm) ||
          cp.code?.toLowerCase().includes(searchTerm) ||
          cp.email?.toLowerCase().includes(searchTerm)
        )
      }
      
      if (state.filters.type !== 'all') {
        filtered = filtered.filter(cp => cp.type === state.filters.type)
      }
      
      if (state.filters.status !== 'all') {
        const isActive = state.filters.status === 'active'
        filtered = filtered.filter(cp => cp.isActive === isActive)
      }
      
      return filtered.sort((a, b) => a.name.localeCompare(b.name))
    },
    
    // Get filtered banks
    filteredBanks: (state) => {
      let filtered = [...state.banks]
      
      if (state.filters.searchTerm) {
        const searchTerm = state.filters.searchTerm.toLowerCase()
        filtered = filtered.filter(bank => 
          bank.name.toLowerCase().includes(searchTerm) ||
          bank.accountNumber?.toLowerCase().includes(searchTerm) ||
          bank.iban?.toLowerCase().includes(searchTerm)
        )
      }
      
      if (state.filters.status !== 'all') {
        const isActive = state.filters.status === 'active'
        filtered = filtered.filter(bank => bank.isActive === isActive)
      }
      
      return filtered.sort((a, b) => a.name.localeCompare(b.name))
    },
    
    // Get statistics
    statistics: (state) => {
      return {
        airlines: {
          total: state.airlines.length,
          active: state.airlines.filter(a => a.isActive).length
        },
        counterparties: {
          total: state.counterparties.length,
          buyers: state.counterparties.filter(cp => cp.type === 'buyer').length,
          suppliers: state.counterparties.filter(cp => cp.type === 'supplier').length,
          active: state.counterparties.filter(cp => cp.isActive).length
        },
        banks: {
          total: state.banks.length,
          active: state.banks.filter(b => b.isActive).length
        },
        users: {
          total: state.users.length,
          active: state.users.filter(u => u.isActive).length
        }
      }
    }
  },

  actions: {
    // Load all settings data
    async loadAllSettings() {
      this.loading = true
      this.error = null
      
      try {
        await new Promise(resolve => setTimeout(resolve, 800))
        
        this.airlines = [...mockData.airlines]
        this.counterparties = [...mockData.counterparties]
        this.banks = [...mockData.banks]
        this.locations = [...mockData.locations]
        this.users = [...mockData.users]
        
        // Mock roles data
        this.roles = [
          { id: '1', name: 'مدیر سیستم', code: 'admin', permissions: ['all'] },
          { id: '2', name: 'مدیر فروش', code: 'sales_manager', permissions: ['sales_read', 'sales_write', 'reports_read'] },
          { id: '3', name: 'کارشناس فروش', code: 'sales_user', permissions: ['sales_read', 'sales_write'] },
          { id: '4', name: 'حسابدار', code: 'accountant', permissions: ['finance_read', 'finance_write', 'reports_read'] },
          { id: '5', name: 'کاربر عادی', code: 'user', permissions: ['sales_read'] }
        ]
        
      } catch (error) {
        this.error = 'خطا در بارگذاری تنظیمات'
        console.error('Load settings error:', error)
      } finally {
        this.loading = false
      }
    },

    // Airlines CRUD operations
    async createAirline(airlineData) {
      this.loading = true
      this.error = null
      
      try {
        await new Promise(resolve => setTimeout(resolve, 500))
        
        const newAirline = {
          id: Date.now().toString(),
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
          isActive: true,
          ...airlineData
        }
        
        this.airlines.unshift(newAirline)
        this.currentAirline = newAirline
        
        return newAirline
        
      } catch (error) {
        this.error = 'خطا در ایجاد ایرلاین جدید'
        console.error('Create airline error:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    async updateAirline(id, updates) {
      this.loading = true
      this.error = null
      
      try {
        await new Promise(resolve => setTimeout(resolve, 500))
        
        const index = this.airlines.findIndex(airline => airline.id === id)
        if (index === -1) {
          throw new Error('ایرلاین یافت نشد')
        }
        
        const updatedAirline = {
          ...this.airlines[index],
          ...updates,
          updatedAt: new Date().toISOString()
        }
        
        this.airlines[index] = updatedAirline
        
        if (this.currentAirline?.id === id) {
          this.currentAirline = updatedAirline
        }
        
        return updatedAirline
        
      } catch (error) {
        this.error = 'خطا در به‌روزرسانی ایرلاین'
        console.error('Update airline error:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    async deleteAirline(id) {
      this.loading = true
      this.error = null
      
      try {
        await new Promise(resolve => setTimeout(resolve, 300))
        
        const index = this.airlines.findIndex(airline => airline.id === id)
        if (index === -1) {
          throw new Error('ایرلاین یافت نشد')
        }
        
        this.airlines.splice(index, 1)
        
        if (this.currentAirline?.id === id) {
          this.currentAirline = null
        }
        
        return true
        
      } catch (error) {
        this.error = 'خطا در حذف ایرلاین'
        console.error('Delete airline error:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    // Counterparties CRUD operations
    async createCounterparty(counterpartyData) {
      this.loading = true
      this.error = null
      
      try {
        await new Promise(resolve => setTimeout(resolve, 500))
        
        const newCounterparty = {
          id: Date.now().toString(),
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
          isActive: true,
          openingBalanceIRR: 0,
          openingBalanceUSD: 0,
          openingBalanceEUR: 0,
          ...counterpartyData
        }
        
        this.counterparties.unshift(newCounterparty)
        this.currentCounterparty = newCounterparty
        
        return newCounterparty
        
      } catch (error) {
        this.error = 'خطا در ایجاد طرف حساب جدید'
        console.error('Create counterparty error:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    async updateCounterparty(id, updates) {
      this.loading = true
      this.error = null
      
      try {
        await new Promise(resolve => setTimeout(resolve, 500))
        
        const index = this.counterparties.findIndex(cp => cp.id === id)
        if (index === -1) {
          throw new Error('طرف حساب یافت نشد')
        }
        
        const updatedCounterparty = {
          ...this.counterparties[index],
          ...updates,
          updatedAt: new Date().toISOString()
        }
        
        this.counterparties[index] = updatedCounterparty
        
        if (this.currentCounterparty?.id === id) {
          this.currentCounterparty = updatedCounterparty
        }
        
        return updatedCounterparty
        
      } catch (error) {
        this.error = 'خطا در به‌روزرسانی طرف حساب'
        console.error('Update counterparty error:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    async deleteCounterparty(id) {
      this.loading = true
      this.error = null
      
      try {
        await new Promise(resolve => setTimeout(resolve, 300))
        
        const index = this.counterparties.findIndex(cp => cp.id === id)
        if (index === -1) {
          throw new Error('طرف حساب یافت نشد')
        }
        
        this.counterparties.splice(index, 1)
        
        if (this.currentCounterparty?.id === id) {
          this.currentCounterparty = null
        }
        
        return true
        
      } catch (error) {
        this.error = 'خطا در حذف طرف حساب'
        console.error('Delete counterparty error:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    // Banks CRUD operations
    async createBank(bankData) {
      this.loading = true
      this.error = null
      
      try {
        await new Promise(resolve => setTimeout(resolve, 500))
        
        const newBank = {
          id: Date.now().toString(),
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
          isActive: true,
          openingBalanceIRR: 0,
          openingBalanceUSD: 0,
          openingBalanceEUR: 0,
          ...bankData
        }
        
        this.banks.unshift(newBank)
        this.currentBank = newBank
        
        return newBank
        
      } catch (error) {
        this.error = 'خطا در ایجاد بانک جدید'
        console.error('Create bank error:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    async updateBank(id, updates) {
      this.loading = true
      this.error = null
      
      try {
        await new Promise(resolve => setTimeout(resolve, 500))
        
        const index = this.banks.findIndex(bank => bank.id === id)
        if (index === -1) {
          throw new Error('بانک یافت نشد')
        }
        
        const updatedBank = {
          ...this.banks[index],
          ...updates,
          updatedAt: new Date().toISOString()
        }
        
        this.banks[index] = updatedBank
        
        if (this.currentBank?.id === id) {
          this.currentBank = updatedBank
        }
        
        return updatedBank
        
      } catch (error) {
        this.error = 'خطا در به‌روزرسانی بانک'
        console.error('Update bank error:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    async deleteBank(id) {
      this.loading = true
      this.error = null
      
      try {
        await new Promise(resolve => setTimeout(resolve, 300))
        
        const index = this.banks.findIndex(bank => bank.id === id)
        if (index === -1) {
          throw new Error('بانک یافت نشد')
        }
        
        this.banks.splice(index, 1)
        
        if (this.currentBank?.id === id) {
          this.currentBank = null
        }
        
        return true
        
      } catch (error) {
        this.error = 'خطا در حذف بانک'
        console.error('Delete bank error:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    // Locations CRUD operations
    async createLocation(locationData) {
      this.loading = true
      this.error = null
      
      try {
        await new Promise(resolve => setTimeout(resolve, 500))
        
        const newLocation = {
          id: Date.now().toString(),
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
          isActive: true,
          parentId: null,
          parentName: null,
          ...locationData
        }
        
        this.locations.unshift(newLocation)
        this.currentLocation = newLocation
        
        return newLocation
        
      } catch (error) {
        this.error = 'خطا در ایجاد مکان جدید'
        console.error('Create location error:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    async updateLocation(id, locationData) {
      this.loading = true
      this.error = null
      
      try {
        await new Promise(resolve => setTimeout(resolve, 400))
        
        const index = this.locations.findIndex(location => location.id === id)
        if (index === -1) {
          throw new Error('مکان یافت نشد')
        }
        
        const updatedLocation = {
          ...this.locations[index],
          ...locationData,
          updatedAt: new Date().toISOString()
        }
        
        this.locations[index] = updatedLocation
        this.currentLocation = updatedLocation
        
        return updatedLocation
        
      } catch (error) {
        this.error = 'خطا در ویرایش مکان'
        console.error('Update location error:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    async deleteLocation(id) {
      this.loading = true
      this.error = null
      
      try {
        await new Promise(resolve => setTimeout(resolve, 300))
        
        const index = this.locations.findIndex(location => location.id === id)
        if (index === -1) {
          throw new Error('مکان یافت نشد')
        }
        
        this.locations.splice(index, 1)
        
        if (this.currentLocation?.id === id) {
          this.currentLocation = null
        }
        
        return true
        
      } catch (error) {
        this.error = 'خطا در حذف مکان'
        console.error('Delete location error:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    // User management
    async loadUsers(params = {}) {
      this.loading = true
      this.error = null
      
      try {
        const { usersApi } = await import('@/services/api')
        const response = await usersApi.getUsers(params)
        
        if (response.success) {
          this.users = response.data.items || response.data
          return response.data
        } else {
          throw new Error(response.message || 'خطا در بارگذاری کاربران')
        }
        
      } catch (error) {
        this.error = 'خطا در بارگذاری کاربران'
        console.error('Load users error:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    async getUserById(id) {
      this.loading = true
      this.error = null
      
      try {
        const { usersApi } = await import('@/services/api')
        const response = await usersApi.getUser(id)
        
        if (response.success) {
          return response.data
        } else {
          throw new Error(response.message || 'کاربر یافت نشد')
        }
        
      } catch (error) {
        this.error = 'خطا در بارگذاری کاربر'
        console.error('Get user error:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    async createUser(userData) {
      this.loading = true
      this.error = null
      
      try {
        const { usersApi } = await import('@/services/api')
        const response = await usersApi.createUser(userData)
        
        if (response.success) {
          const newUser = response.data
          this.users.unshift(newUser)
          this.currentUser = newUser
          return newUser
        } else {
          throw new Error(response.message || 'خطا در ایجاد کاربر')
        }
        
      } catch (error) {
        this.error = error.message || 'خطا در ایجاد کاربر جدید'
        console.error('Create user error:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    async updateUser(id, updates) {
      this.loading = true
      this.error = null
      
      try {
        const { usersApi } = await import('@/services/api')
        const response = await usersApi.updateUser(id, updates)
        
        if (response.success) {
          const updatedUser = response.data
          const index = this.users.findIndex(user => user.id === id)
          
          if (index !== -1) {
            this.users[index] = updatedUser
          }
          
          if (this.currentUser?.id === id) {
            this.currentUser = updatedUser
          }
          
          return updatedUser
        } else {
          throw new Error(response.message || 'خطا در به‌روزرسانی کاربر')
        }
        
      } catch (error) {
        this.error = error.message || 'خطا در به‌روزرسانی کاربر'
        console.error('Update user error:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    async deleteUser(id) {
      this.loading = true
      this.error = null
      
      try {
        const { usersApi } = await import('@/services/api')
        const response = await usersApi.deleteUser(id)
        
        if (response.success) {
          const index = this.users.findIndex(user => user.id === id)
          if (index !== -1) {
            this.users.splice(index, 1)
          }
          
          if (this.currentUser?.id === id) {
            this.currentUser = null
          }
          
          return true
        } else {
          throw new Error(response.message || 'خطا در حذف کاربر')
        }
        
      } catch (error) {
        this.error = error.message || 'خطا در حذف کاربر'
        console.error('Delete user error:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    async toggleUserStatus(id) {
      this.loading = true
      this.error = null
      
      try {
        const user = this.users.find(u => u.id === id)
        if (!user) {
          throw new Error('کاربر یافت نشد')
        }
        
        const newStatus = !user.isActive
        const { usersApi } = await import('@/services/api')
        const response = await usersApi.updateUser(id, { isActive: newStatus })
        
        if (response.success) {
          const updatedUser = response.data
          const index = this.users.findIndex(user => user.id === id)
          
          if (index !== -1) {
            this.users[index] = updatedUser
          }
          
          return updatedUser
        } else {
          throw new Error(response.message || 'خطا در تغییر وضعیت کاربر')
        }
        
      } catch (error) {
        this.error = error.message || 'خطا در تغییر وضعیت کاربر'
        console.error('Toggle user status error:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    // Utility methods
    setFilters(filters) {
      this.filters = { ...this.filters, ...filters }
      this.pagination.page = 1
    },

    clearFilters() {
      this.filters = {
        searchTerm: '',
        status: 'all',
        type: 'all'
      }
      this.pagination.page = 1
    },

    setPagination(page, limit = 20) {
      this.pagination.page = page
      this.pagination.limit = limit
    },

    clearError() {
      this.error = null
    },

    // Get item by ID helpers
    getAirlineById(id) {
      return this.airlines.find(airline => airline.id === id)
    },

    getCounterpartyById(id) {
      return this.counterparties.find(cp => cp.id === id)
    },

    getBankById(id) {
      return this.banks.find(bank => bank.id === id)
    },

    getUserById(id) {
      return this.users.find(user => user.id === id)
    }
  }
})