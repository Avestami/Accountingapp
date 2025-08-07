import { defineStore } from 'pinia'
import { mockData } from '@/services/mockData'

export const useSalesStore = defineStore('sales', {
  state: () => ({
    documents: [],
    currentDocument: null,
    loading: false,
    error: null,
    filters: {
      status: 'all',
      serviceType: 'all',
      dateFrom: null,
      dateTo: null,
      searchTerm: ''
    },
    sortBy: 'flightDate',
    sortOrder: 'asc',
    pagination: {
      page: 1,
      limit: 20,
      total: 0
    }
  }),

  getters: {
    // Get documents by status
    unissuedDocuments: (state) => {
      return state.documents.filter(doc => doc.status === 'unissued')
    },
    
    issuedDocuments: (state) => {
      return state.documents.filter(doc => doc.status === 'issued')
    },
    
    canceledDocuments: (state) => {
      return state.documents.filter(doc => doc.status === 'canceled')
    },

    // Get filtered and sorted documents
    filteredDocuments: (state) => {
      let filtered = [...state.documents]
      
      // Apply status filter
      if (state.filters.status !== 'all') {
        filtered = filtered.filter(doc => doc.status === state.filters.status)
      }
      
      // Apply service type filter
      if (state.filters.serviceType !== 'all') {
        filtered = filtered.filter(doc => doc.serviceType === state.filters.serviceType)
      }
      
      // Apply date range filter
      if (state.filters.dateFrom) {
        filtered = filtered.filter(doc => new Date(doc.flightDate) >= new Date(state.filters.dateFrom))
      }
      
      if (state.filters.dateTo) {
        filtered = filtered.filter(doc => new Date(doc.flightDate) <= new Date(state.filters.dateTo))
      }
      
      // Apply search filter
      if (state.filters.searchTerm) {
        const searchTerm = state.filters.searchTerm.toLowerCase()
        filtered = filtered.filter(doc => 
          doc.documentNo?.toLowerCase().includes(searchTerm) ||
          doc.referenceNo?.toLowerCase().includes(searchTerm) ||
          doc.passengerName?.toLowerCase().includes(searchTerm) ||
          doc.buyer?.name?.toLowerCase().includes(searchTerm)
        )
      }
      
      // Apply sorting
      filtered.sort((a, b) => {
        let aValue = a[state.sortBy]
        let bValue = b[state.sortBy]
        
        // Handle date sorting
        if (state.sortBy === 'flightDate' || state.sortBy === 'saleDate') {
          aValue = new Date(aValue)
          bValue = new Date(bValue)
        }
        
        // Handle string sorting
        if (typeof aValue === 'string') {
          aValue = aValue.toLowerCase()
          bValue = bValue.toLowerCase()
        }
        
        if (state.sortOrder === 'asc') {
          return aValue < bValue ? -1 : aValue > bValue ? 1 : 0
        } else {
          return aValue > bValue ? -1 : aValue < bValue ? 1 : 0
        }
      })
      
      return filtered
    },

    // Get documents with color coding based on flight date
    documentsWithColors: (state) => {
      const now = new Date()
      const fiveDaysFromNow = new Date(now.getTime() + (5 * 24 * 60 * 60 * 1000))
      
      return state.filteredDocuments.map(doc => {
        const flightDate = new Date(doc.flightDate)
        let colorClass = 'text-blue-600' // default blue
        
        if (flightDate <= fiveDaysFromNow && flightDate >= now) {
          colorClass = 'text-red-600' // red for first 5 days
        }
        
        return {
          ...doc,
          colorClass
        }
      })
    },

    // Get next document number
    nextDocumentNumber: (state) => {
      const currentYear = new Date().getFullYear()
      const yearDocs = state.documents.filter(doc => 
        doc.documentNo && doc.documentNo.includes(currentYear.toString())
      )
      
      if (yearDocs.length === 0) {
        return `${currentYear}-001`
      }
      
      const maxNumber = Math.max(...yearDocs.map(doc => {
        const parts = doc.documentNo.split('-')
        return parseInt(parts[parts.length - 1]) || 0
      }))
      
      return `${currentYear}-${String(maxNumber + 1).padStart(3, '0')}`
    },

    // Get statistics
    statistics: (state) => {
      const docs = state.documents
      return {
        total: docs.length,
        unissued: docs.filter(d => d.status === 'unissued').length,
        issued: docs.filter(d => d.status === 'issued').length,
        canceled: docs.filter(d => d.status === 'canceled').length,
        totalAmount: docs.reduce((sum, doc) => sum + (doc.totalAmount || 0), 0),
        totalProfit: docs.reduce((sum, doc) => sum + (doc.profit || 0), 0)
      }
    }
  },

  actions: {
    // Load documents from mock data
    async loadDocuments(filters = null) {
      this.loading = true
      this.error = null
      
      try {
        // Simulate API delay
        await new Promise(resolve => setTimeout(resolve, 500))
        
        this.documents = [...mockData.salesDocuments]
        this.pagination.total = this.documents.length
        
        // Apply filters if provided
        if (filters) {
          this.filters = { ...this.filters, ...filters }
        }
        
      } catch (error) {
        this.error = 'خطا در بارگذاری اسناد فروش'
        console.error('Load documents error:', error)
      } finally {
        this.loading = false
      }
    },

    // Get document by ID
    async getDocument(id) {
      this.loading = true
      this.error = null
      
      try {
        await new Promise(resolve => setTimeout(resolve, 300))
        
        const document = this.documents.find(doc => doc.id === id)
        if (!document) {
          throw new Error('سند یافت نشد')
        }
        
        this.currentDocument = { ...document }
        return document
        
      } catch (error) {
        this.error = error.message
        console.error('Get document error:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    // Create new document
    async createDocument(documentData) {
      this.loading = true
      this.error = null
      
      try {
        await new Promise(resolve => setTimeout(resolve, 1000))
        
        const newDocument = {
          id: Date.now().toString(),
          documentNo: null, // Will be assigned after save
          status: 'unissued',
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
          ...documentData
        }
        
        this.documents.unshift(newDocument)
        this.currentDocument = newDocument
        
        return newDocument
        
      } catch (error) {
        this.error = 'خطا در ایجاد سند جدید'
        console.error('Create document error:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    // Update document
    async updateDocument(id, updates) {
      this.loading = true
      this.error = null
      
      try {
        await new Promise(resolve => setTimeout(resolve, 800))
        
        const index = this.documents.findIndex(doc => doc.id === id)
        if (index === -1) {
          throw new Error('سند یافت نشد')
        }
        
        const updatedDocument = {
          ...this.documents[index],
          ...updates,
          updatedAt: new Date().toISOString()
        }
        
        this.documents[index] = updatedDocument
        
        if (this.currentDocument?.id === id) {
          this.currentDocument = updatedDocument
        }
        
        return updatedDocument
        
      } catch (error) {
        this.error = 'خطا در به‌روزرسانی سند'
        console.error('Update document error:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    // Save document (assign document number)
    async saveDocument(id) {
      try {
        const documentNo = this.nextDocumentNumber
        const updates = {
          documentNo,
          savedAt: new Date().toISOString()
        }
        
        return await this.updateDocument(id, updates)
        
      } catch (error) {
        console.error('Save document error:', error)
        throw error
      }
    },

    // Issue document
    async issueDocument(id) {
      try {
        const updates = {
          status: 'issued',
          issuedAt: new Date().toISOString()
        }
        
        return await this.updateDocument(id, updates)
        
      } catch (error) {
        console.error('Issue document error:', error)
        throw error
      }
    },

    // Cancel document
    async cancelDocument(id, reason = '') {
      try {
        const updates = {
          status: 'canceled',
          canceledAt: new Date().toISOString(),
          cancelReason: reason
        }
        
        return await this.updateDocument(id, updates)
        
      } catch (error) {
        console.error('Cancel document error:', error)
        throw error
      }
    },

    // Delete document
    async deleteDocument(id) {
      this.loading = true
      this.error = null
      
      try {
        await new Promise(resolve => setTimeout(resolve, 500))
        
        const index = this.documents.findIndex(doc => doc.id === id)
        if (index === -1) {
          throw new Error('سند یافت نشد')
        }
        
        this.documents.splice(index, 1)
        
        if (this.currentDocument?.id === id) {
          this.currentDocument = null
        }
        
        return true
        
      } catch (error) {
        this.error = 'خطا در حذف سند'
        console.error('Delete document error:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    // Set filters
    setFilters(filters) {
      this.filters = { ...this.filters, ...filters }
      this.pagination.page = 1 // Reset to first page
    },

    // Set sorting
    setSorting(sortBy, sortOrder = 'asc') {
      this.sortBy = sortBy
      this.sortOrder = sortOrder
    },

    // Set pagination
    setPagination(page, limit = 20) {
      this.pagination.page = page
      this.pagination.limit = limit
    },

    // Clear filters
    clearFilters() {
      this.filters = {
        status: 'all',
        serviceType: 'all',
        dateFrom: null,
        dateTo: null,
        searchTerm: ''
      }
      this.pagination.page = 1
    },

    // Search documents
    async searchDocuments(searchTerm) {
      this.setFilters({ searchTerm })
    },

    // Get documents by status with pagination
    getDocumentsByStatus(status, page = 1, limit = 20) {
      const filtered = status === 'all' ? 
        this.documents : 
        this.documents.filter(doc => doc.status === status)
      
      const start = (page - 1) * limit
      const end = start + limit
      
      return {
        documents: filtered.slice(start, end),
        total: filtered.length,
        page,
        limit,
        totalPages: Math.ceil(filtered.length / limit)
      }
    },

    // Clear current document
    clearCurrentDocument() {
      this.currentDocument = null
    },

    // Clear error
    clearError() {
      this.error = null
    }
  }
})