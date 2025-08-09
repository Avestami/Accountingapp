// API Service for making HTTP requests
const API_BASE_URL = process.env.VUE_APP_API_URL || 'http://localhost:5000/api'

// Generic API client
class ApiClient {
  constructor(baseURL = API_BASE_URL) {
    this.baseURL = baseURL
  }

  async request(endpoint, options = {}) {
    const url = `${this.baseURL}${endpoint}`
    const token = localStorage.getItem('auth_token')
    
    const config = {
      headers: {
        'Content-Type': 'application/json',
        ...(token && { Authorization: `Bearer ${token}` }),
        ...options.headers
      },
      ...options
    }

    try {
      const response = await fetch(url, config)
      
      if (!response.ok) {
        const errorData = await response.text()
        throw new Error(errorData || `HTTP error! status: ${response.status}`)
      }

      const contentType = response.headers.get('content-type')
      if (contentType && contentType.includes('application/json')) {
        return await response.json()
      }
      
      return await response.text()
    } catch (error) {
      console.error('API request failed:', error)
      throw error
    }
  }

  async get(endpoint, params = {}) {
    const queryString = new URLSearchParams(params).toString()
    const url = queryString ? `${endpoint}?${queryString}` : endpoint
    return this.request(url, { method: 'GET' })
  }

  async post(endpoint, data, options = {}) {
    const requestOptions = {
      method: 'POST',
      ...options
    }
    
    // Handle FormData differently - don't stringify it and don't set Content-Type
    if (data instanceof FormData) {
      requestOptions.body = data
      // Remove Content-Type header to let browser set it with boundary
      if (requestOptions.headers && requestOptions.headers['Content-Type'] === 'multipart/form-data') {
        delete requestOptions.headers['Content-Type']
      }
    } else {
      requestOptions.body = JSON.stringify(data)
    }
    
    return this.request(endpoint, requestOptions)
  }

  async put(endpoint, data) {
    return this.request(endpoint, {
      method: 'PUT',
      body: JSON.stringify(data)
    })
  }

  async delete(endpoint) {
    return this.request(endpoint, { method: 'DELETE' })
  }
}

const apiClient = new ApiClient()

// Accounts API
export const accountsApi = {
  async getAccounts(params = {}) {
    return apiClient.get('/accounts', params)
  },

  async getAccount(id) {
    return apiClient.get(`/accounts/${id}`)
  },

  async createAccount(accountData) {
    return apiClient.post('/accounts', accountData)
  },

  async updateAccount(id, accountData) {
    return apiClient.put(`/accounts/${id}`, accountData)
  },

  async deleteAccount(id) {
    return apiClient.delete(`/accounts/${id}`)
  }
}

// Auth API
export const authApi = {
  async login(credentials) {
    return apiClient.post('/auth/login', credentials)
  },

  async logout() {
    return apiClient.post('/auth/logout')
  },

  async refreshToken() {
    return apiClient.post('/auth/refresh')
  },

  async changePassword(passwordData) {
    return apiClient.post('/auth/change-password', passwordData)
  }
}

// Finance API
export const financeApi = {
  // Costs
  async getCosts(params = {}) {
    return apiClient.get('/finance/costs', params)
  },

  async getCost(id) {
    return apiClient.get(`/finance/costs/${id}`)
  },

  async createCost(costData) {
    return apiClient.post('/finance/costs', costData)
  },

  // Incomes
  async getIncomes(params = {}) {
    return apiClient.get('/finance/incomes', params)
  },

  async getIncome(id) {
    return apiClient.get(`/finance/incomes/${id}`)
  },

  async createIncome(incomeData) {
    return apiClient.post('/finance/incomes', incomeData)
  },

  // Transfers
  async getTransfers(params = {}) {
    return apiClient.get('/finance/transfers', params)
  },

  async getTransfer(id) {
    return apiClient.get(`/finance/transfers/${id}`)
  },

  async createTransfer(transferData) {
    return apiClient.post('/finance/transfers', transferData)
  },

  // Export
  async exportFinanceData(exportData) {
    const response = await apiClient.post('/finance/export', exportData)
    return response
  },

  // Legacy methods for backward compatibility
  async getTransactions(params = {}) {
    return apiClient.get('/finance/transactions', params)
  },

  async createTransaction(transactionData) {
    return apiClient.post('/finance/transactions', transactionData)
  },

  async updateTransaction(id, transactionData) {
    return apiClient.put(`/finance/transactions/${id}`, transactionData)
  },

  async deleteTransaction(id) {
    return apiClient.delete(`/finance/transactions/${id}`)
  }
}

// Vouchers API
export const vouchersApi = {
  async getVouchers(params = {}) {
    return apiClient.get('/vouchers', params)
  },

  async getVoucher(id) {
    return apiClient.get(`/vouchers/${id}`)
  },

  async createVoucher(voucherData) {
    return apiClient.post('/vouchers', voucherData)
  },

  async updateVoucher(id, voucherData) {
    return apiClient.put(`/vouchers/${id}`, voucherData)
  },

  async deleteVoucher(id) {
    return apiClient.delete(`/vouchers/${id}`)
  }
}

// Sales API
export const salesApi = {
  async getSalesDocuments(params = {}) {
    return apiClient.get('/sales/documents', params)
  },

  async getSalesDocument(id) {
    return apiClient.get(`/sales/documents/${id}`)
  },

  async createSalesDocument(documentData) {
    return apiClient.post('/sales/documents', documentData)
  },

  async updateSalesDocument(id, documentData) {
    return apiClient.put(`/sales/documents/${id}`, documentData)
  },

  async deleteSalesDocument(id) {
    return apiClient.delete(`/sales/documents/${id}`)
  }
}

// Reports API
export const reportsApi = {
  async getFinancialReport(params = {}) {
    return apiClient.get('/reports/financial', params)
  },

  async getSalesReport(params = {}) {
    return apiClient.get('/reports/sales', params)
  },

  async getAccountsReport(params = {}) {
    return apiClient.get('/reports/accounts', params)
  },

  async exportReport(reportType, format, params = {}) {
    return apiClient.get(`/reports/${reportType}/export/${format}`, params)
  }
}

// Users API
export const usersApi = {
  async getUsers(params = {}) {
    return apiClient.get('/users', params)
  },

  async getUser(id) {
    return apiClient.get(`/users/${id}`)
  },

  async createUser(userData) {
    return apiClient.post('/users', userData)
  },

  async updateUser(id, userData) {
    return apiClient.put(`/users/${id}`, userData)
  },

  async deleteUser(id) {
    return apiClient.delete(`/users/${id}`)
  }
}

// Settings API
export const settingsApi = {
  async getSettings() {
    return apiClient.get('/settings')
  },

  async updateSettings(settingsData) {
    return apiClient.put('/settings', settingsData)
  },

  async getCompanySettings() {
    return apiClient.get('/settings/company')
  },

  async updateCompanySettings(companyData) {
    return apiClient.put('/settings/company', companyData)
  }
}

// Dashboard API
export const dashboardApi = {
  async getDashboardStats(params = {}) {
    return apiClient.get('/dashboard/stats', params)
  }
}

// User Profile API
export const userProfileApi = {
  getProfile: () => apiClient.get('/userprofile'),
  updateProfile: (data) => apiClient.put('/userprofile', data),
  changePassword: (data) => apiClient.post('/userprofile/change-password', data),
  uploadProfilePicture: (formData) => apiClient.post('/userprofile/upload-picture', formData, {
    headers: {}
  }),
  deleteProfilePicture: () => apiClient.delete('/userprofile/profile-picture')
}

export default apiClient