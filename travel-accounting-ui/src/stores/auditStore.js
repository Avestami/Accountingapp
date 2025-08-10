import { defineStore } from 'pinia'
import { auditLogsApi } from '@/services/api'

export const useAuditStore = defineStore('audit', {
  state: () => ({
    auditLogs: [],
    totalCount: 0,
    loading: false,
    error: null
  }),

  actions: {
    async getAuditLogs(query = {}) {
      this.loading = true
      this.error = null
      
      try {
        const response = await auditLogsApi.get(query)
        
        if (response.data.isSuccess) {
          this.auditLogs = response.data.data.items
          this.totalCount = response.data.data.totalCount
        } else {
          this.error = response.data.message || 'Failed to load audit logs'
        }
      } catch (error) {
        console.error('Error fetching audit logs:', error)
        this.error = error.response?.data?.message || 'Failed to load audit logs'
      } finally {
        this.loading = false
      }
    },

    clearError() {
      this.error = null
    }
  }
})