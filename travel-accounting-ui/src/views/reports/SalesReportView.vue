<template>
  <div class="sales-report-view">
    <div class="page-header">
      <h1 class="page-title">گزارش فروش</h1>
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
            <option value="summary">خلاصه</option>
            <option value="detailed">تفصیلی</option>
            <option value="by-customer">بر اساس مشتری</option>
            <option value="by-product">بر اساس محصول</option>
          </select>
        </div>
        <div class="filter-group">
          <button class="btn btn-primary" @click="generateReport">
            تولید گزارش
          </button>
        </div>
      </div>
    </div>

    <!-- Summary Cards -->
    <div class="stats-grid" v-if="reportData">
      <div class="stat-card">
        <div class="stat-icon bg-green-100 text-green-600">
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1"></path>
          </svg>
        </div>
        <div class="stat-content">
          <div class="stat-value">{{ formatCurrency(reportData.totalSales) }}</div>
          <div class="stat-label">کل فروش</div>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon bg-blue-100 text-blue-600">
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"></path>
          </svg>
        </div>
        <div class="stat-content">
          <div class="stat-value">{{ reportData.totalTransactions }}</div>
          <div class="stat-label">تعداد تراکنش</div>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon bg-purple-100 text-purple-600">
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"></path>
          </svg>
        </div>
        <div class="stat-content">
          <div class="stat-value">{{ reportData.uniqueCustomers }}</div>
          <div class="stat-label">مشتریان منحصر به فرد</div>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon bg-yellow-100 text-yellow-600">
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6"></path>
          </svg>
        </div>
        <div class="stat-content">
          <div class="stat-value">{{ formatCurrency(reportData.averageTransaction) }}</div>
          <div class="stat-label">میانگین تراکنش</div>
        </div>
      </div>
    </div>

    <!-- Chart Section -->
    <div class="chart-section" v-if="reportData">
      <div class="chart-container">
        <h3>نمودار فروش</h3>
        <div class="chart-placeholder">
          <p>نمودار فروش در اینجا نمایش داده می‌شود</p>
        </div>
      </div>
    </div>

    <!-- Report Table -->
    <div class="table-container" v-if="reportData && reportData.details">
      <h3>جزئیات گزارش</h3>
      <table class="data-table">
        <thead>
          <tr>
            <th v-for="header in tableHeaders" :key="header.key">
              {{ header.label }}
            </th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in reportData.details" :key="item.id">
            <td v-for="header in tableHeaders" :key="header.key">
              <span v-if="header.key === 'amount'">
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
      </table>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue'

export default {
  name: 'SalesReportView',
  setup() {
    // Reactive data
    const dateFrom = ref('')
    const dateTo = ref('')
    const reportType = ref('summary')
    const reportData = ref(null)
    const loading = ref(false)
    
    // Computed properties
    const tableHeaders = computed(() => {
      switch (reportType.value) {
        case 'detailed':
          return [
            { key: 'id', label: 'شماره' },
            { key: 'date', label: 'تاریخ' },
            { key: 'customer', label: 'مشتری' },
            { key: 'product', label: 'محصول' },
            { key: 'amount', label: 'مبلغ' }
          ]
        case 'by-customer':
          return [
            { key: 'customer', label: 'مشتری' },
            { key: 'transactions', label: 'تعداد تراکنش' },
            { key: 'amount', label: 'کل مبلغ' }
          ]
        case 'by-product':
          return [
            { key: 'product', label: 'محصول' },
            { key: 'quantity', label: 'تعداد' },
            { key: 'amount', label: 'کل مبلغ' }
          ]
        default:
          return [
            { key: 'date', label: 'تاریخ' },
            { key: 'transactions', label: 'تعداد تراکنش' },
            { key: 'amount', label: 'کل مبلغ' }
          ]
      }
    })
    
    // Methods
    const generateReport = async () => {
      loading.value = true
      
      // Simulate API call
      setTimeout(() => {
        reportData.value = {
          totalSales: 15000000,
          totalTransactions: 45,
          uniqueCustomers: 12,
          averageTransaction: 333333,
          details: [
            {
              id: 'S001',
              date: '2024-01-15',
              customer: 'شرکت الف',
              product: 'بلیط هواپیما',
              amount: 2500000,
              transactions: 5,
              quantity: 10
            },
            {
              id: 'S002',
              date: '2024-01-16',
              customer: 'شرکت ب',
              product: 'رزرو هتل',
              amount: 1800000,
              transactions: 3,
              quantity: 6
            },
            {
              id: 'S003',
              date: '2024-01-17',
              customer: 'شرکت ج',
              product: 'تور گردشگری',
              amount: 5200000,
              transactions: 8,
              quantity: 15
            }
          ]
        }
        loading.value = false
      }, 1000)
    }
    
    const exportReport = () => {
      // Simulate export functionality
      alert('گزارش با موفقیت خروجی گرفته شد')
    }
    
    const formatCurrency = (amount) => {
      return new Intl.NumberFormat('fa-IR').format(amount) + ' ریال'
    }
    
    const formatDate = (date) => {
      return new Date(date).toLocaleDateString('fa-IR')
    }
    
    // Lifecycle
    onMounted(() => {
      // Set default date range (last 30 days)
      const today = new Date()
      const thirtyDaysAgo = new Date(today.getTime() - 30 * 24 * 60 * 60 * 1000)
      
      dateTo.value = today.toISOString().split('T')[0]
      dateFrom.value = thirtyDaysAgo.toISOString().split('T')[0]
    })
    
    return {
      dateFrom,
      dateTo,
      reportType,
      reportData,
      loading,
      tableHeaders,
      generateReport,
      exportReport,
      formatCurrency,
      formatDate
    }
  }
}
</script>

<style scoped>
.sales-report-view {
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
  margin-bottom: 4px;
}

.stat-label {
  font-size: 14px;
  color: #6b7280;
}

.chart-section {
  background: white;
  border-radius: 8px;
  padding: 20px;
  margin-bottom: 24px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.chart-container h3 {
  margin: 0 0 16px 0;
  font-size: 18px;
  font-weight: 600;
  color: #1f2937;
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

.table-container h3 {
  margin: 0 0 16px 0;
  font-size: 18px;
  font-weight: 600;
  color: #1f2937;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
}

.data-table th {
  background: #f9fafb;
  padding: 12px;
  text-align: right;
  font-weight: 600;
  color: #374151;
  border-bottom: 1px solid #e5e7eb;
}

.data-table td {
  padding: 12px;
  border-bottom: 1px solid #e5e7eb;
  color: #1f2937;
}
</style>