<template>
  <div class="finance-report-view">
    <div class="page-header">
      <h1 class="page-title">گزارش مالی</h1>
      <div class="header-actions">
        <button class="btn btn-secondary" @click="exportReport">
          <svg class="w-4 h-4 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 10v6m0 0l-3-3m3 3l3-3m2 8H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
          </svg>
          خروجی گزارش
        </button>
      </div>
    </div>

    <!-- Date Range Filter -->
    <div class="filters-section">
      <div class="filters-grid">
        <div class="filter-group">
          <label>از تاریخ</label>
          <input v-model="dateFrom" type="date" class="form-input">
        </div>
        <div class="filter-group">
          <label>تا تاریخ</label>
          <input v-model="dateTo" type="date" class="form-input">
        </div>
        <div class="filter-group">
          <label>نوع گزارش</label>
          <select v-model="reportType" class="form-select">
            <option value="balance-sheet">ترازنامه</option>
            <option value="income-statement">صورت سود و زیان</option>
            <option value="cash-flow">صورت جریان نقدی</option>
            <option value="trial-balance">تراز آزمایشی</option>
          </select>
        </div>
        <div class="filter-group">
          <button @click="generateReport" class="btn btn-primary" :disabled="loading">
            {{ loading ? 'در حال تولید...' : 'تولید گزارش' }}
          </button>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>در حال تولید گزارش...</p>
    </div>

    <!-- Report Content -->
    <div v-else-if="reportData">
      <!-- Summary Cards -->
      <div class="stats-grid">
        <div class="stat-card" v-if="reportType === 'balance-sheet'">
          <div class="stat-icon bg-blue-100 text-blue-600">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1"></path>
            </svg>
          </div>
          <div>
            <div class="stat-label">کل دارایی‌ها</div>
            <div class="stat-value">{{ formatCurrency(reportData.totalAssets) }}</div>
          </div>
        </div>
        <div class="stat-card" v-if="reportType === 'income-statement'">
          <div class="stat-icon bg-green-100 text-green-600">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6"></path>
            </svg>
          </div>
          <div>
            <div class="stat-label">سود خالص</div>
            <div class="stat-value">{{ formatCurrency(reportData.netIncome) }}</div>
          </div>
        </div>
      </div>

      <!-- Chart Placeholder -->
      <div class="chart-container">
        <h3>نمودار تحلیلی</h3>
        <div class="chart-placeholder">
          <p>نمودار در اینجا نمایش داده خواهد شد</p>
        </div>
      </div>

      <!-- Financial Report Table -->
      <div class="table-container" v-if="reportData && reportData.details">
        <h3>{{ getReportTitle() }}</h3>
        <table class="data-table">
          <thead>
            <tr>
              <th v-for="header in tableHeaders" :key="header.key">
                {{ header.label }}
              </th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in reportData.details" :key="item.id" :class="getRowClass(item)">
              <td v-for="header in tableHeaders" :key="header.key">
                <span v-if="header.key === 'amount' || header.key === 'debit' || header.key === 'credit'">
                  {{ formatCurrency(item[header.key]) }}
                </span>
                <span v-else-if="header.key === 'date'">
                  {{ formatDate(item[header.key]) }}
                </span>
                <span v-else>
                  {{ item[header.key] }}
                </span>
              </td>
            </tr>
          </tbody>
          <tfoot v-if="showTotals">
            <tr class="total-row">
              <td colspan="2"><strong>جمع کل</strong></td>
              <td><strong>{{ formatCurrency(getTotalDebit()) }}</strong></td>
              <td><strong>{{ formatCurrency(getTotalCredit()) }}</strong></td>
            </tr>
          </tfoot>
        </table>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue'

export default {
  name: 'FinanceReportView',
  setup() {
    // Reactive data
    const dateFrom = ref('')
    const dateTo = ref('')
    const reportType = ref('balance-sheet')
    const reportData = ref(null)
    const loading = ref(false)
    
    // Computed properties
    const tableHeaders = computed(() => {
      switch (reportType.value) {
        case 'balance-sheet':
          return [
            { key: 'account', label: 'حساب' },
            { key: 'category', label: 'دسته‌بندی' },
            { key: 'amount', label: 'مبلغ' }
          ]
        case 'income-statement':
          return [
            { key: 'account', label: 'حساب' },
            { key: 'category', label: 'نوع' },
            { key: 'amount', label: 'مبلغ' }
          ]
        case 'cash-flow':
          return [
            { key: 'date', label: 'تاریخ' },
            { key: 'description', label: 'شرح' },
            { key: 'inflow', label: 'ورودی' },
            { key: 'outflow', label: 'خروجی' }
          ]
        case 'trial-balance':
          return [
            { key: 'account', label: 'حساب' },
            { key: 'code', label: 'کد' },
            { key: 'debit', label: 'بدهکار' },
            { key: 'credit', label: 'بستانکار' }
          ]
        default:
          return []
      }
    })
    
    const showTotals = computed(() => {
      return reportType.value === 'trial-balance'
    })
    
    // Methods
    const generateReport = async () => {
      loading.value = true
      
      // Simulate API call
      setTimeout(() => {
        reportData.value = {
          totalAssets: 25000000,
          totalLiabilities: 8000000,
          equity: 17000000,
          netIncome: 3500000,
          details: getReportDetails()
        }
        loading.value = false
      }, 1000)
    }
    
    const getReportDetails = () => {
      switch (reportType.value) {
        case 'balance-sheet':
          return [
            { id: 1, account: 'نقد و بانک', category: 'دارایی جاری', amount: 5000000 },
            { id: 2, account: 'حساب‌های دریافتنی', category: 'دارایی جاری', amount: 3000000 },
            { id: 3, account: 'موجودی کالا', category: 'دارایی جاری', amount: 2000000 },
            { id: 4, account: 'تجهیزات', category: 'دارایی ثابت', amount: 15000000 },
            { id: 5, account: 'حساب‌های پرداختنی', category: 'بدهی جاری', amount: -3000000 },
            { id: 6, account: 'وام بانکی', category: 'بدهی بلندمدت', amount: -5000000 }
          ]
        case 'income-statement':
          return [
            { id: 1, account: 'درآمد فروش', category: 'درآمد', amount: 15000000 },
            { id: 2, account: 'بهای تمام شده کالای فروش رفته', category: 'هزینه', amount: -8000000 },
            { id: 3, account: 'هزینه‌های اداری', category: 'هزینه', amount: -2000000 },
            { id: 4, account: 'هزینه‌های فروش', category: 'هزینه', amount: -1500000 }
          ]
        case 'cash-flow':
          return [
            { id: 1, date: '2024-01-15', description: 'دریافت از مشتری', inflow: 2500000, outflow: 0 },
            { id: 2, date: '2024-01-16', description: 'پرداخت حقوق', inflow: 0, outflow: 1200000 },
            { id: 3, date: '2024-01-17', description: 'فروش کالا', inflow: 1800000, outflow: 0 },
            { id: 4, date: '2024-01-18', description: 'خرید تجهیزات', inflow: 0, outflow: 3000000 }
          ]
        case 'trial-balance':
          return [
            { id: 1, account: 'نقد', code: '1001', debit: 5000000, credit: 0 },
            { id: 2, account: 'بانک', code: '1002', debit: 3000000, credit: 0 },
            { id: 3, account: 'حساب‌های دریافتنی', code: '1101', debit: 2000000, credit: 0 },
            { id: 4, account: 'حساب‌های پرداختنی', code: '2001', debit: 0, credit: 1500000 },
            { id: 5, account: 'سرمایه', code: '3001', debit: 0, credit: 8500000 }
          ]
        default:
          return []
      }
    }
    
    const getReportTitle = () => {
      switch (reportType.value) {
        case 'balance-sheet': return 'ترازنامه'
        case 'income-statement': return 'صورت سود و زیان'
        case 'cash-flow': return 'صورت جریان نقدی'
        case 'trial-balance': return 'تراز آزمایشی'
        default: return 'گزارش مالی'
      }
    }
    
    const getRowClass = (item) => {
      if (reportType.value === 'balance-sheet' && item.amount < 0) {
        return 'liability-row'
      }
      if (reportType.value === 'income-statement' && item.amount < 0) {
        return 'expense-row'
      }
      return ''
    }
    
    const getTotalDebit = () => {
      if (reportType.value === 'trial-balance' && reportData.value) {
        return reportData.value.details.reduce((sum, item) => sum + (item.debit || 0), 0)
      }
      return 0
    }
    
    const getTotalCredit = () => {
      if (reportType.value === 'trial-balance' && reportData.value) {
        return reportData.value.details.reduce((sum, item) => sum + (item.credit || 0), 0)
      }
      return 0
    }
    
    const exportReport = () => {
      // Simulate export functionality
      alert('گزارش مالی با موفقیت خروجی گرفته شد')
    }
    
    const formatCurrency = (amount) => {
      return new Intl.NumberFormat('fa-IR').format(Math.abs(amount)) + ' ریال'
    }
    
    const formatDate = (date) => {
      return new Date(date).toLocaleDateString('fa-IR')
    }
    
    // Lifecycle
    onMounted(() => {
      // Set default date range (current fiscal year)
      const today = new Date()
      const startOfYear = new Date(today.getFullYear(), 0, 1)
      
      dateTo.value = today.toISOString().split('T')[0]
      dateFrom.value = startOfYear.toISOString().split('T')[0]
    })
    
    return {
      dateFrom,
      dateTo,
      reportType,
      reportData,
      loading,
      tableHeaders,
      showTotals,
      generateReport,
      getReportTitle,
      getRowClass,
      getTotalDebit,
      getTotalCredit,
      exportReport,
      formatCurrency,
      formatDate
    }
  }
}
</script>

<style scoped>
.finance-report-view {
  padding: 24px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.page-title {
  font-size: 24px;
  font-weight: 700;
  color: #1f2937;
  margin: 0;
}

.header-actions {
  display: flex;
  gap: 12px;
}

.filters-section {
  background: white;
  border-radius: 8px;
  padding: 20px;
  margin-bottom: 24px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.filters-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 16px;
  align-items: end;
}

.filter-group label {
  display: block;
  font-size: 14px;
  font-weight: 500;
  color: #374151;
  margin-bottom: 4px;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 20px;
  margin-bottom: 24px;
}

.stat-card {
  background: white;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  display: flex;
  align-items: center;
  gap: 16px;
}

.stat-icon {
  width: 48px;
  height: 48px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.stat-value {
  font-size: 24px;
  font-weight: 700;
  color: #1f2937;
}

.stat-label {
  font-size: 14px;
  color: #6b7280;
  margin-bottom: 4px;
}

.chart-container {
  background: white;
  border-radius: 8px;
  padding: 20px;
  margin-bottom: 24px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.chart-placeholder {
  height: 300px;
  background: #f9fafb;
  border: 2px dashed #d1d5db;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #6b7280;
}

.table-container {
  background: white;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.data-table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 16px;
}

.data-table th,
.data-table td {
  padding: 12px;
  text-align: right;
  border-bottom: 1px solid #e5e7eb;
}

.data-table th {
  background-color: #f9fafb;
  font-weight: 600;
  color: #374151;
}

.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 60px;
  color: #6b7280;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #e5e7eb;
  border-top: 4px solid #3b82f6;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 16px;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.liability-row {
  background-color: #fef2f2;
}

.expense-row {
  background-color: #fef2f2;
}

.total-row {
  background-color: #f3f4f6;
  font-weight: 600;
}

.total-row td {
  border-top: 2px solid #374151;
}
</style>