import { defineStore } from 'pinia'
import { ref } from 'vue'
import api from '@/services/api'

export const useReportsStore = defineStore('reports', () => {
  const reports = ref([])
  const currentReport = ref(null)
  const loading = ref(false)
  const error = ref(null)

  // Generate Sales Report
  const generateSalesReport = async (filters) => {
    loading.value = true
    error.value = null
    
    try {
      const params = new URLSearchParams()
      
      if (filters.startDate) params.append('startDate', filters.startDate)
      if (filters.endDate) params.append('endDate', filters.endDate)
      if (filters.searchTerm) params.append('searchTerm', filters.searchTerm)
      if (filters.airlines && filters.airlines.length > 0) {
        filters.airlines.forEach(airline => params.append('airlines', airline))
      }
      if (filters.pageNumber) params.append('pageNumber', filters.pageNumber)
      if (filters.pageSize) params.append('pageSize', filters.pageSize)
      
      const response = await api.get(`/api/reports/sales?${params.toString()}`)
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'خطا در تولید گزارش فروش'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Generate Financial Report
  const generateFinancialReport = async (filters) => {
    loading.value = true
    error.value = null
    
    try {
      const params = new URLSearchParams()
      
      if (filters.startDate) params.append('startDate', filters.startDate)
      if (filters.endDate) params.append('endDate', filters.endDate)
      if (filters.searchTerm) params.append('searchTerm', filters.searchTerm)
      if (filters.categories && filters.categories.length > 0) {
        filters.categories.forEach(category => params.append('categories', category))
      }
      if (filters.pageNumber) params.append('pageNumber', filters.pageNumber)
      if (filters.pageSize) params.append('pageSize', filters.pageSize)
      
      const response = await api.get(`/api/reports/financial?${params.toString()}`)
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'خطا در تولید گزارش مالی'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Generate Profit/Loss Report
  const generateProfitLossReport = async (filters) => {
    loading.value = true
    error.value = null
    
    try {
      const params = new URLSearchParams()
      
      if (filters.startDate) params.append('startDate', filters.startDate)
      if (filters.endDate) params.append('endDate', filters.endDate)
      if (filters.categories && filters.categories.length > 0) {
        filters.categories.forEach(category => params.append('categories', category))
      }
      if (filters.pageNumber) params.append('pageNumber', filters.pageNumber)
      if (filters.pageSize) params.append('pageSize', filters.pageSize)
      
      const response = await api.get(`/api/reports/profit-loss?${params.toString()}`)
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'خطا در تولید صورت سود و زیان'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Generate Balance Sheet Report
  const generateBalanceSheetReport = async (filters) => {
    loading.value = true
    error.value = null
    
    try {
      const params = new URLSearchParams()
      
      if (filters.asOfDate) params.append('asOfDate', filters.asOfDate)
      
      const response = await api.get(`/api/reports/balance-sheet?${params.toString()}`)
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'خطا در تولید ترازنامه'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Generate Cash Flow Report
  const generateCashFlowReport = async (filters) => {
    loading.value = true
    error.value = null
    
    try {
      const params = new URLSearchParams()
      
      if (filters.startDate) params.append('startDate', filters.startDate)
      if (filters.endDate) params.append('endDate', filters.endDate)
      
      const response = await api.get(`/api/reports/cash-flow?${params.toString()}`)
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'خطا در تولید صورت جریان وجه نقد'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Export Report
  const exportReport = async (reportType, exportData) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.post(`/api/reports/export/${reportType}`, exportData, {
        responseType: 'blob'
      })
      
      // Create download link
      const url = window.URL.createObjectURL(new Blob([response.data]))
      const link = document.createElement('a')
      link.href = url
      
      // Determine file extension based on format
      const format = exportData.format || 'pdf'
      const extensions = {
        pdf: 'pdf',
        excel: 'xlsx',
        csv: 'csv',
        json: 'json'
      }
      
      const extension = extensions[format] || 'pdf'
      const fileName = `${reportType}-report-${new Date().toISOString().split('T')[0]}.${extension}`
      
      link.setAttribute('download', fileName)
      document.body.appendChild(link)
      link.click()
      link.remove()
      window.URL.revokeObjectURL(url)
      
    } catch (err) {
      error.value = err.response?.data?.message || 'خطا در خروجی گزارش'
      throw err
    } finally {
      loading.value = false
    }
  }

  return {
    reports,
    currentReport,
    loading,
    error,
    generateSalesReport,
    generateFinancialReport,
    generateProfitLossReport,
    generateBalanceSheetReport,
    generateCashFlowReport,
    exportReport
  }
})