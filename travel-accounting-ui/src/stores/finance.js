import { defineStore } from 'pinia'
import { mockData } from '../services/mockData'
import { financeApi } from '../services/api'

export const useFinanceStore = defineStore('finance', {
  state: () => ({
    // Costs
    costs: [],
    totalCosts: 0,
    currentCost: null,
    
    // Incomes
    incomes: [],
    totalIncomes: 0,
    currentIncome: null,
    
    // Transfers
    transfers: [],
    totalTransfers: 0,
    currentTransfer: null,
    
    // Vouchers (legacy)
    vouchers: [],
    totalVouchers: 0,
    currentVoucher: null,
    
    // Accounts
    accounts: [],
    totalAccounts: 0,
    currentAccount: null,
    
    // Reports
    reports: {
      balanceSheet: null,
      incomeStatement: null,
      cashFlow: null,
      trialBalance: null
    },
    
    // UI State
    isLoading: false,
    error: null,
    
    // Filters and pagination
    filters: {
      search: '',
      type: '',
      status: '',
      dateFrom: null,
      dateTo: null
    },
    pagination: {
      page: 1,
      limit: 10,
      total: 0
    }
  }),
  
  getters: {
    // Cost getters
    getCostById: (state) => (id) => {
      return state.costs.find(cost => cost.id === id)
    },
    
    totalCostAmount: (state) => {
      return state.costs.reduce((sum, cost) => sum + cost.amount, 0)
    },
    
    pendingCosts: (state) => {
      return state.costs.filter(c => c.status === 'pending').length
    },
    
    // Income getters
    getIncomeById: (state) => (id) => {
      return state.incomes.find(income => income.id === id)
    },
    
    totalIncomeAmount: (state) => {
      return state.incomes.reduce((sum, income) => sum + income.amount, 0)
    },
    
    pendingIncomes: (state) => {
      return state.incomes.filter(i => i.status === 'pending').length
    },
    
    // Transfer getters
    getTransferById: (state) => (id) => {
      return state.transfers.find(transfer => transfer.id === id)
    },
    
    totalTransferAmount: (state) => {
      return state.transfers.reduce((sum, transfer) => sum + transfer.amount, 0)
    },
    
    pendingTransfers: (state) => {
      return state.transfers.filter(t => t.status === 'pending').length
    },
    
    // Voucher getters (legacy)
    getVoucherById: (state) => (id) => {
      return state.vouchers.find(voucher => voucher.id === id)
    },
    
    getVouchersByType: (state) => (type) => {
      return state.vouchers.filter(voucher => voucher.type === type)
    },
    
    getVouchersByStatus: (state) => (status) => {
      return state.vouchers.filter(voucher => voucher.status === status)
    },
    
    // Financial calculations
    totalReceipts: (state) => {
      return state.vouchers
        .filter(v => v.type === 'receipt' && v.status === 'confirmed')
        .reduce((sum, v) => sum + v.amount, 0)
    },
    
    totalPayments: (state) => {
      return state.vouchers
        .filter(v => v.type === 'payment' && v.status === 'confirmed')
        .reduce((sum, v) => sum + v.amount, 0)
    },
    
    balance: (state, getters) => {
      return getters.totalReceipts - getters.totalPayments
    },
    
    pendingVouchers: (state) => {
      return state.vouchers.filter(v => v.status === 'draft').length
    },
    
    // Account getters
    getAccountById: (state) => (id) => {
      return state.accounts.find(account => account.id === id)
    },
    
    getAccountsByType: (state) => (type) => {
      return state.accounts.filter(account => account.type === type)
    },
    
    // Statistics
    voucherStats: (state) => {
      const stats = {
        total: state.vouchers.length,
        draft: 0,
        confirmed: 0,
        rejected: 0,
        receipt: 0,
        payment: 0,
        transfer: 0,
        adjustment: 0
      }
      
      state.vouchers.forEach(voucher => {
        stats[voucher.status]++
        stats[voucher.type]++
      })
      
      return stats
    },
    
    accountStats: (state) => {
      const stats = {
        total: state.accounts.length,
        active: 0,
        inactive: 0,
        asset: 0,
        liability: 0,
        equity: 0,
        revenue: 0,
        expense: 0
      }
      
      state.accounts.forEach(account => {
        if (account.isActive) {
          stats.active++
        } else {
          stats.inactive++
        }
        stats[account.type]++
      })
      
      return stats
    }
  },
  
  actions: {
    // Cost actions
    async loadCosts(filters = {}) {
      this.isLoading = true
      this.error = null
      
      try {
        const params = {
          page: filters.page || 1,
          pageSize: filters.pageSize || 10,
          search: filters.search,
          currency: filters.currency,
          counterparty: filters.counterparty,
          fromDate: filters.fromDate,
          toDate: filters.toDate
        }
        
        const response = await financeApi.getCosts(params)
        this.costs = response.data
        this.totalCosts = response.total
        
        return response
      } catch (error) {
        this.error = 'Error loading costs'
        console.error('Error loading costs:', error)
        throw error
      } finally {
        this.isLoading = false
      }
    },
    
    async loadCost(id) {
      this.isLoading = true
      this.error = null
      
      try {
        const cost = await financeApi.getCost(id)
        this.currentCost = cost
        return cost
      } catch (error) {
        this.error = 'Error loading cost'
        console.error('Error loading cost:', error)
        throw error
      } finally {
        this.isLoading = false
      }
    },
    
    async createCost(costData) {
      this.isLoading = true
      this.error = null
      
      try {
        const newCost = await financeApi.createCost(costData)
        this.costs.unshift(newCost)
        this.totalCosts++
        this.currentCost = newCost
        return newCost
      } catch (error) {
        this.error = 'Error creating cost'
        console.error('Error creating cost:', error)
        throw error
      } finally {
        this.isLoading = false
      }
    },
    
    // Income actions
    async loadIncomes(filters = {}) {
      this.isLoading = true
      this.error = null
      
      try {
        const params = {
          page: filters.page || 1,
          pageSize: filters.pageSize || 10,
          search: filters.search,
          currency: filters.currency,
          counterparty: filters.counterparty,
          fromDate: filters.fromDate,
          toDate: filters.toDate
        }
        
        const response = await financeApi.getIncomes(params)
        this.incomes = response.data
        this.totalIncomes = response.total
        
        return response
      } catch (error) {
        this.error = 'Error loading incomes'
        console.error('Error loading incomes:', error)
        throw error
      } finally {
        this.isLoading = false
      }
    },
    
    async loadIncome(id) {
      this.isLoading = true
      this.error = null
      
      try {
        const income = await financeApi.getIncome(id)
        this.currentIncome = income
        return income
      } catch (error) {
        this.error = 'Error loading income'
        console.error('Error loading income:', error)
        throw error
      } finally {
        this.isLoading = false
      }
    },
    
    async createIncome(incomeData) {
      this.isLoading = true
      this.error = null
      
      try {
        const newIncome = await financeApi.createIncome(incomeData)
        this.incomes.unshift(newIncome)
        this.totalIncomes++
        this.currentIncome = newIncome
        return newIncome
      } catch (error) {
        this.error = 'Error creating income'
        console.error('Error creating income:', error)
        throw error
      } finally {
        this.isLoading = false
      }
    },
    
    // Transfer actions
    async loadTransfers(filters = {}) {
      this.isLoading = true
      this.error = null
      
      try {
        const params = {
          page: filters.page || 1,
          pageSize: filters.pageSize || 10,
          search: filters.search,
          currency: filters.currency,
          fromDate: filters.fromDate,
          toDate: filters.toDate
        }
        
        const response = await financeApi.getTransfers(params)
        this.transfers = response.data
        this.totalTransfers = response.total
        
        return response
      } catch (error) {
        this.error = 'Error loading transfers'
        console.error('Error loading transfers:', error)
        throw error
      } finally {
        this.isLoading = false
      }
    },
    
    async loadTransfer(id) {
      this.isLoading = true
      this.error = null
      
      try {
        const transfer = await financeApi.getTransfer(id)
        this.currentTransfer = transfer
        return transfer
      } catch (error) {
        this.error = 'Error loading transfer'
        console.error('Error loading transfer:', error)
        throw error
      } finally {
        this.isLoading = false
      }
    },
    
    async createTransfer(transferData) {
      this.isLoading = true
      this.error = null
      
      try {
        const newTransfer = await financeApi.createTransfer(transferData)
        this.transfers.unshift(newTransfer)
        this.totalTransfers++
        this.currentTransfer = newTransfer
        return newTransfer
      } catch (error) {
        this.error = 'Error creating transfer'
        console.error('Error creating transfer:', error)
        throw error
      } finally {
        this.isLoading = false
      }
    },
    
    // Export action
    async exportFinanceData(exportData) {
      this.isLoading = true
      this.error = null
      
      try {
        const response = await financeApi.exportFinanceData(exportData)
        return response
      } catch (error) {
        this.error = 'Error exporting finance data'
        console.error('Error exporting finance data:', error)
        throw error
      } finally {
        this.isLoading = false
      }
    },
    
    // Voucher actions (legacy)
    async loadVouchers(filters = {}) {
      this.isLoading = true
      this.error = null
      
      try {
        // Simulate API delay
        await new Promise(resolve => setTimeout(resolve, 500))
        
        // Get mock data
        const mockVouchers = mockData.getVouchers()
        
        // Apply filters
        let filteredVouchers = [...mockVouchers]
        
        if (filters.search) {
          const searchTerm = filters.search.toLowerCase()
          filteredVouchers = filteredVouchers.filter(voucher => 
            voucher.voucherNumber.toLowerCase().includes(searchTerm) ||
            voucher.description.toLowerCase().includes(searchTerm) ||
            voucher.counterparty.toLowerCase().includes(searchTerm)
          )
        }
        
        if (filters.type) {
          filteredVouchers = filteredVouchers.filter(voucher => voucher.type === filters.type)
        }
        
        if (filters.status) {
          filteredVouchers = filteredVouchers.filter(voucher => voucher.status === filters.status)
        }
        
        if (filters.dateFrom) {
          filteredVouchers = filteredVouchers.filter(voucher => 
            new Date(voucher.date) >= new Date(filters.dateFrom)
          )
        }
        
        if (filters.dateTo) {
          filteredVouchers = filteredVouchers.filter(voucher => 
            new Date(voucher.date) <= new Date(filters.dateTo)
          )
        }
        
        this.vouchers = filteredVouchers
        this.totalVouchers = filteredVouchers.length
        this.filters = { ...filters }
        
      } catch (error) {
        this.error = 'خطا در بارگذاری اسناد حسابداری'
        console.error('Error loading vouchers:', error)
      } finally {
        this.isLoading = false
      }
    },
    
    async loadVoucher(id) {
      this.isLoading = true
      this.error = null
      
      try {
        // Simulate API delay
        await new Promise(resolve => setTimeout(resolve, 300))
        
        const voucher = mockData.getVoucherById(id)
        if (!voucher) {
          throw new Error('سند یافت نشد')
        }
        
        this.currentVoucher = voucher
        return voucher
        
      } catch (error) {
        this.error = error.message || 'خطا در بارگذاری سند'
        console.error('Error loading voucher:', error)
        throw error
      } finally {
        this.isLoading = false
      }
    },
    
    async createVoucher(voucherData) {
      this.isLoading = true
      this.error = null
      
      try {
        // Simulate API delay
        await new Promise(resolve => setTimeout(resolve, 800))
        
        // Generate voucher number
        const currentYear = new Date().getFullYear()
        const voucherCount = this.vouchers.filter(v => 
          v.voucherNumber.startsWith(`${currentYear}`)
        ).length + 1
        
        const newVoucher = {
          id: Date.now(),
          voucherNumber: `${currentYear}/${String(voucherCount).padStart(6, '0')}`,
          date: voucherData.date || new Date().toISOString().split('T')[0],
          type: voucherData.type,
          description: voucherData.description,
          counterparty: voucherData.counterparty,
          amount: voucherData.amount,
          currency: voucherData.currency || 'IRR',
          status: 'draft',
          entries: voucherData.entries || [],
          attachments: voucherData.attachments || [],
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
          createdBy: 'current-user' // Should come from auth store
        }
        
        this.vouchers.unshift(newVoucher)
        this.totalVouchers++
        this.currentVoucher = newVoucher
        
        return newVoucher
        
      } catch (error) {
        this.error = 'خطا در ایجاد سند جدید'
        console.error('Error creating voucher:', error)
        throw error
      } finally {
        this.isLoading = false
      }
    },
    
    async updateVoucher(id, voucherData) {
      this.isLoading = true
      this.error = null
      
      try {
        // Simulate API delay
        await new Promise(resolve => setTimeout(resolve, 600))
        
        const index = this.vouchers.findIndex(voucher => voucher.id === id)
        if (index === -1) {
          throw new Error('سند یافت نشد')
        }
        
        const updatedVoucher = {
          ...this.vouchers[index],
          ...voucherData,
          updatedAt: new Date().toISOString()
        }
        
        this.vouchers[index] = updatedVoucher
        this.currentVoucher = updatedVoucher
        
        return updatedVoucher
        
      } catch (error) {
        this.error = error.message || 'خطا در به‌روزرسانی سند'
        console.error('Error updating voucher:', error)
        throw error
      } finally {
        this.isLoading = false
      }
    },
    
    async updateVoucherStatus(id, status) {
      this.isLoading = true
      this.error = null
      
      try {
        // Simulate API delay
        await new Promise(resolve => setTimeout(resolve, 400))
        
        const index = this.vouchers.findIndex(voucher => voucher.id === id)
        if (index === -1) {
          throw new Error('سند یافت نشد')
        }
        
        this.vouchers[index].status = status
        this.vouchers[index].updatedAt = new Date().toISOString()
        
        if (status === 'confirmed') {
          this.vouchers[index].confirmedAt = new Date().toISOString()
          this.vouchers[index].confirmedBy = 'current-user' // Should come from auth store
        }
        
        return this.vouchers[index]
        
      } catch (error) {
        this.error = error.message || 'خطا در به‌روزرسانی وضعیت سند'
        console.error('Error updating voucher status:', error)
        throw error
      } finally {
        this.isLoading = false
      }
    },
    
    async deleteVoucher(id) {
      this.isLoading = true
      this.error = null
      
      try {
        // Simulate API delay
        await new Promise(resolve => setTimeout(resolve, 400))
        
        const index = this.vouchers.findIndex(voucher => voucher.id === id)
        if (index === -1) {
          throw new Error('سند یافت نشد')
        }
        
        // Check if voucher can be deleted
        if (this.vouchers[index].status === 'confirmed') {
          throw new Error('امکان حذف سند تایید شده وجود ندارد')
        }
        
        this.vouchers.splice(index, 1)
        this.totalVouchers--
        
        if (this.currentVoucher && this.currentVoucher.id === id) {
          this.currentVoucher = null
        }
        
      } catch (error) {
        this.error = error.message || 'خطا در حذف سند'
        console.error('Error deleting voucher:', error)
        throw error
      } finally {
        this.isLoading = false
      }
    },
    
    // Account actions
    async loadAccounts(filters = {}) {
      this.isLoading = true
      this.error = null
      
      try {
        // Simulate API delay
        await new Promise(resolve => setTimeout(resolve, 400))
        
        const mockAccounts = mockData.getAccounts()
        
        // Apply filters
        let filteredAccounts = [...mockAccounts]
        
        if (filters.search) {
          const searchTerm = filters.search.toLowerCase()
          filteredAccounts = filteredAccounts.filter(account => 
            account.code.toLowerCase().includes(searchTerm) ||
            account.name.toLowerCase().includes(searchTerm)
          )
        }
        
        if (filters.type) {
          filteredAccounts = filteredAccounts.filter(account => account.type === filters.type)
        }
        
        if (filters.isActive !== undefined) {
          filteredAccounts = filteredAccounts.filter(account => account.isActive === filters.isActive)
        }
        
        this.accounts = filteredAccounts
        this.totalAccounts = filteredAccounts.length
        
      } catch (error) {
        this.error = 'خطا در بارگذاری حساب‌ها'
        console.error('Error loading accounts:', error)
      } finally {
        this.isLoading = false
      }
    },
    
    async loadAccount(id) {
      this.isLoading = true
      this.error = null
      
      try {
        // Simulate API delay
        await new Promise(resolve => setTimeout(resolve, 300))
        
        const account = mockData.getAccountById(id)
        if (!account) {
          throw new Error('حساب یافت نشد')
        }
        
        this.currentAccount = account
        return account
        
      } catch (error) {
        this.error = error.message || 'خطا در بارگذاری حساب'
        console.error('Error loading account:', error)
        throw error
      } finally {
        this.isLoading = false
      }
    },
    
    async createAccount(accountData) {
      this.isLoading = true
      this.error = null
      
      try {
        // Simulate API delay
        await new Promise(resolve => setTimeout(resolve, 600))
        
        const newAccount = {
          id: Date.now(),
          code: accountData.code,
          name: accountData.name,
          type: accountData.type,
          parentId: accountData.parentId || null,
          level: accountData.level || 1,
          isActive: accountData.isActive !== false,
          balance: 0,
          debitBalance: 0,
          creditBalance: 0,
          description: accountData.description || '',
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString()
        }
        
        this.accounts.unshift(newAccount)
        this.totalAccounts++
        this.currentAccount = newAccount
        
        return newAccount
        
      } catch (error) {
        this.error = 'خطا در ایجاد حساب جدید'
        console.error('Error creating account:', error)
        throw error
      } finally {
        this.isLoading = false
      }
    },
    
    async updateAccount(id, accountData) {
      this.isLoading = true
      this.error = null
      
      try {
        // Simulate API delay
        await new Promise(resolve => setTimeout(resolve, 500))
        
        const index = this.accounts.findIndex(account => account.id === id)
        if (index === -1) {
          throw new Error('حساب یافت نشد')
        }
        
        const updatedAccount = {
          ...this.accounts[index],
          ...accountData,
          updatedAt: new Date().toISOString()
        }
        
        this.accounts[index] = updatedAccount
        this.currentAccount = updatedAccount
        
        return updatedAccount
        
      } catch (error) {
        this.error = error.message || 'خطا در به‌روزرسانی حساب'
        console.error('Error updating account:', error)
        throw error
      } finally {
        this.isLoading = false
      }
    },
    
    async deleteAccount(id) {
      this.isLoading = true
      this.error = null
      
      try {
        // Simulate API delay
        await new Promise(resolve => setTimeout(resolve, 400))
        
        const index = this.accounts.findIndex(account => account.id === id)
        if (index === -1) {
          throw new Error('حساب یافت نشد')
        }
        
        // Check if account has balance
        if (this.accounts[index].balance !== 0) {
          throw new Error('امکان حذف حساب دارای مانده وجود ندارد')
        }
        
        // Check if account has child accounts
        const hasChildren = this.accounts.some(account => account.parentId === id)
        if (hasChildren) {
          throw new Error('امکان حذف حساب دارای زیرحساب وجود ندارد')
        }
        
        this.accounts.splice(index, 1)
        this.totalAccounts--
        
        if (this.currentAccount && this.currentAccount.id === id) {
          this.currentAccount = null
        }
        
      } catch (error) {
        this.error = error.message || 'خطا در حذف حساب'
        console.error('Error deleting account:', error)
        throw error
      } finally {
        this.isLoading = false
      }
    },
    
    // Report actions
    async generateBalanceSheet(date = null) {
      this.isLoading = true
      this.error = null
      
      try {
        // Simulate API delay
        await new Promise(resolve => setTimeout(resolve, 1000))
        
        const reportDate = date || new Date().toISOString().split('T')[0]
        
        // Mock balance sheet data
        const balanceSheet = {
          date: reportDate,
          assets: {
            current: {
              cash: 50000000,
              accountsReceivable: 25000000,
              inventory: 15000000,
              total: 90000000
            },
            nonCurrent: {
              equipment: 80000000,
              buildings: 120000000,
              total: 200000000
            },
            total: 290000000
          },
          liabilities: {
            current: {
              accountsPayable: 20000000,
              shortTermDebt: 10000000,
              total: 30000000
            },
            nonCurrent: {
              longTermDebt: 60000000,
              total: 60000000
            },
            total: 90000000
          },
          equity: {
            capital: 150000000,
            retainedEarnings: 50000000,
            total: 200000000
          },
          totalLiabilitiesAndEquity: 290000000
        }
        
        this.reports.balanceSheet = balanceSheet
        return balanceSheet
        
      } catch (error) {
        this.error = 'خطا در تولید ترازنامه'
        console.error('Error generating balance sheet:', error)
        throw error
      } finally {
        this.isLoading = false
      }
    },
    
    async generateIncomeStatement(dateFrom, dateTo) {
      this.isLoading = true
      this.error = null
      
      try {
        // Simulate API delay
        await new Promise(resolve => setTimeout(resolve, 800))
        
        // Mock income statement data
        const incomeStatement = {
          period: { from: dateFrom, to: dateTo },
          revenue: {
            sales: 180000000,
            services: 45000000,
            other: 5000000,
            total: 230000000
          },
          expenses: {
            costOfGoodsSold: 120000000,
            salaries: 35000000,
            rent: 12000000,
            utilities: 8000000,
            depreciation: 15000000,
            other: 10000000,
            total: 200000000
          },
          grossProfit: 110000000,
          operatingIncome: 30000000,
          netIncome: 30000000
        }
        
        this.reports.incomeStatement = incomeStatement
        return incomeStatement
        
      } catch (error) {
        this.error = 'خطا در تولید صورت سود و زیان'
        console.error('Error generating income statement:', error)
        throw error
      } finally {
        this.isLoading = false
      }
    },
    
    async generateCashFlowStatement(dateFrom, dateTo) {
      this.isLoading = true
      this.error = null
      
      try {
        // Simulate API delay
        await new Promise(resolve => setTimeout(resolve, 900))
        
        // Mock cash flow statement data
        const cashFlow = {
          period: { from: dateFrom, to: dateTo },
          operating: {
            netIncome: 30000000,
            depreciation: 15000000,
            accountsReceivableChange: -5000000,
            accountsPayableChange: 3000000,
            total: 43000000
          },
          investing: {
            equipmentPurchase: -25000000,
            total: -25000000
          },
          financing: {
            loanProceeds: 20000000,
            dividendsPaid: -10000000,
            total: 10000000
          },
          netCashFlow: 28000000,
          beginningCash: 22000000,
          endingCash: 50000000
        }
        
        this.reports.cashFlow = cashFlow
        return cashFlow
        
      } catch (error) {
        this.error = 'خطا در تولید صورت جریان وجه نقد'
        console.error('Error generating cash flow statement:', error)
        throw error
      } finally {
        this.isLoading = false
      }
    },
    
    async generateTrialBalance(date = null) {
      this.isLoading = true
      this.error = null
      
      try {
        // Simulate API delay
        await new Promise(resolve => setTimeout(resolve, 600))
        
        const reportDate = date || new Date().toISOString().split('T')[0]
        
        // Mock trial balance data
        const trialBalance = {
          date: reportDate,
          accounts: [
            { code: '1001', name: 'صندوق', debit: 10000000, credit: 0 },
            { code: '1002', name: 'بانک ملی', debit: 40000000, credit: 0 },
            { code: '1101', name: 'حساب‌های دریافتنی', debit: 25000000, credit: 0 },
            { code: '1201', name: 'موجودی کالا', debit: 15000000, credit: 0 },
            { code: '1501', name: 'تجهیزات', debit: 80000000, credit: 0 },
            { code: '1502', name: 'ساختمان', debit: 120000000, credit: 0 },
            { code: '2001', name: 'حساب‌های پرداختنی', debit: 0, credit: 20000000 },
            { code: '2101', name: 'بدهی کوتاه‌مدت', debit: 0, credit: 10000000 },
            { code: '2201', name: 'بدهی بلندمدت', debit: 0, credit: 60000000 },
            { code: '3001', name: 'سرمایه', debit: 0, credit: 150000000 },
            { code: '3101', name: 'سود انباشته', debit: 0, credit: 50000000 }
          ],
          totalDebit: 290000000,
          totalCredit: 290000000,
          isBalanced: true
        }
        
        this.reports.trialBalance = trialBalance
        return trialBalance
        
      } catch (error) {
        this.error = 'خطا در تولید تراز آزمایشی'
        console.error('Error generating trial balance:', error)
        throw error
      } finally {
        this.isLoading = false
      }
    },
    
    // Utility actions
    clearError() {
      this.error = null
    },
    
    clearCurrentVoucher() {
      this.currentVoucher = null
    },
    
    clearCurrentAccount() {
      this.currentAccount = null
    },
    
    resetFilters() {
      this.filters = {
        search: '',
        type: '',
        status: '',
        dateFrom: null,
        dateTo: null
      }
    },
    
    resetPagination() {
      this.pagination = {
        page: 1,
        limit: 10,
        total: 0
      }
    }
  }
})