<template>
  <div class="costs-view">
    <div class="page-header">
      <h1 class="page-title">مدیریت هزینه‌ها</h1>
      <div class="header-actions">
        <button class="btn btn-primary" @click="showAddModal = true">
          <svg class="w-4 h-4 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"></path>
          </svg>
          افزودن هزینه جدید
        </button>
      </div>
    </div>

    <!-- Statistics Cards -->
    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-icon bg-red-100 text-red-600">
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1"></path>
          </svg>
        </div>
        <div class="stat-content">
          <div class="stat-value">{{ formatCurrency(totalCosts) }}</div>
          <div class="stat-label">کل هزینه‌ها</div>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon bg-orange-100 text-orange-600">
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"></path>
          </svg>
        </div>
        <div class="stat-content">
          <div class="stat-value">{{ costs.length }}</div>
          <div class="stat-label">تعداد هزینه‌ها</div>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon bg-yellow-100 text-yellow-600">
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"></path>
          </svg>
        </div>
        <div class="stat-content">
          <div class="stat-value">{{ pendingCosts }}</div>
          <div class="stat-label">هزینه‌های در انتظار</div>
        </div>
      </div>
    </div>

    <!-- Filters -->
    <div class="filters-section">
      <div class="filters-grid">
        <div class="filter-group">
          <label>جستجو</label>
          <input 
            v-model="searchQuery" 
            type="text" 
            placeholder="جستجو در شرح هزینه..."
            class="form-input"
          >
        </div>
        <div class="filter-group">
          <label>نوع هزینه</label>
          <select v-model="selectedType" class="form-select">
            <option value="">همه انواع</option>
            <option value="operational">عملیاتی</option>
            <option value="administrative">اداری</option>
            <option value="marketing">بازاریابی</option>
            <option value="travel">سفر</option>
          </select>
        </div>
        <div class="filter-group">
          <label>وضعیت</label>
          <select v-model="selectedStatus" class="form-select">
            <option value="">همه وضعیت‌ها</option>
            <option value="pending">در انتظار</option>
            <option value="approved">تایید شده</option>
            <option value="rejected">رد شده</option>
          </select>
        </div>
        <div class="filter-group">
          <label>از تاریخ</label>
          <input v-model="dateFrom" type="date" class="form-input">
        </div>
        <div class="filter-group">
          <label>تا تاریخ</label>
          <input v-model="dateTo" type="date" class="form-input">
        </div>
      </div>
    </div>

    <!-- Costs Table -->
    <div class="table-container">
      <table class="data-table">
        <thead>
          <tr>
            <th>شماره</th>
            <th>تاریخ</th>
            <th>شرح</th>
            <th>نوع هزینه</th>
            <th>مبلغ</th>
            <th>وضعیت</th>
            <th>عملیات</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="cost in filteredCosts" :key="cost.id">
            <td>{{ cost.id }}</td>
            <td>{{ formatDate(cost.date) }}</td>
            <td>{{ cost.description }}</td>
            <td>
              <span class="badge" :class="getTypeClass(cost.type)">
                {{ getTypeLabel(cost.type) }}
              </span>
            </td>
            <td>{{ formatCurrency(cost.amount) }}</td>
            <td>
              <span class="badge" :class="getStatusClass(cost.status)">
                {{ getStatusLabel(cost.status) }}
              </span>
            </td>
            <td>
              <div class="action-buttons">
                <button 
                  class="btn-icon btn-primary" 
                  @click="editCost(cost)"
                  title="ویرایش"
                >
                  <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"></path>
                  </svg>
                </button>
                <button 
                  class="btn-icon btn-danger" 
                  @click="deleteCost(cost.id)"
                  title="حذف"
                >
                  <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path>
                  </svg>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { financeApi } from '@/services/api'

export default {
  name: 'CostsView',
  setup() {
    const router = useRouter()
    
    // Reactive data
    const costs = ref([])
    const searchQuery = ref('')
    const selectedType = ref('')
    const selectedStatus = ref('')
    const dateFrom = ref('')
    const dateTo = ref('')
    const showAddModal = ref(false)
    const loading = ref(false)
    const error = ref(null)
    const pagination = ref({
      page: 1,
      pageSize: 10,
      total: 0
    })
    
    // Computed properties
    const totalCosts = computed(() => {
      return costs.value.reduce((sum, cost) => sum + cost.amount, 0)
    })
    
    const pendingCosts = computed(() => {
      return costs.value.filter(cost => cost.status === 'pending').length
    })
    
    const filteredCosts = computed(() => {
      let filtered = costs.value
      
      if (searchQuery.value) {
        filtered = filtered.filter(cost => 
          cost.description.toLowerCase().includes(searchQuery.value.toLowerCase())
        )
      }
      
      if (selectedType.value) {
        filtered = filtered.filter(cost => cost.type === selectedType.value)
      }
      
      if (selectedStatus.value) {
        filtered = filtered.filter(cost => cost.status === selectedStatus.value)
      }
      
      if (dateFrom.value) {
        filtered = filtered.filter(cost => cost.date >= dateFrom.value)
      }
      
      if (dateTo.value) {
        filtered = filtered.filter(cost => cost.date <= dateTo.value)
      }
      
      return filtered
    })
    
    // Methods
    const loadCosts = async () => {
      try {
        loading.value = true
        error.value = null
        
        const params = {
          page: pagination.value.page,
          pageSize: pagination.value.pageSize,
          fromDate: dateFrom.value || undefined,
          toDate: dateTo.value || undefined,
          searchTerm: searchQuery.value || undefined
        }
        
        const response = await financeApi.getCosts(params)
        
        if (response.success) {
          costs.value = response.data.items || []
          pagination.value.total = response.data.totalCount || 0
        } else {
          throw new Error(response.message || 'خطا در بارگذاری هزینه‌ها')
        }
      } catch (err) {
        error.value = err.message || 'خطا در بارگذاری هزینه‌ها'
        console.error('Error loading costs:', err)
        
        // Fallback to mock data for development
        costs.value = [
          {
            id: 'C001',
            date: '2024-01-15',
            description: 'هزینه سوخت خودرو',
            type: 'operational',
            amount: 500000,
            status: 'approved'
          },
          {
            id: 'C002',
            date: '2024-01-16',
            description: 'هزینه اجاره دفتر',
            type: 'administrative',
            amount: 2000000,
            status: 'pending'
          },
          {
            id: 'C003',
            date: '2024-01-17',
            description: 'هزینه تبلیغات',
            type: 'marketing',
            amount: 1500000,
            status: 'approved'
          }
        ]
      } finally {
        loading.value = false
      }
    }

    const createCost = async (costData) => {
      try {
        loading.value = true
        error.value = null
        
        const response = await financeApi.createCost(costData)
        
        if (response.success) {
          await loadCosts() // Reload the list
          showAddModal.value = false
          return response.data
        } else {
          throw new Error(response.message || 'خطا در ایجاد هزینه')
        }
      } catch (err) {
        error.value = err.message || 'خطا در ایجاد هزینه'
        console.error('Error creating cost:', err)
        throw err
      } finally {
        loading.value = false
      }
    }

    const deleteCost = async (costId) => {
      if (confirm('آیا از حذف این هزینه اطمینان دارید؟')) {
        try {
          loading.value = true
          error.value = null
          
          // Note: Delete endpoint not implemented yet, using local removal
          costs.value = costs.value.filter(cost => cost.id !== costId)
          
        } catch (err) {
          error.value = err.message || 'خطا در حذف هزینه'
          console.error('Error deleting cost:', err)
        } finally {
          loading.value = false
        }
      }
    }
    
    const formatCurrency = (amount) => {
      return new Intl.NumberFormat('fa-IR').format(amount) + ' ریال'
    }
    
    const formatDate = (date) => {
      return new Date(date).toLocaleDateString('fa-IR')
    }
    
    const getTypeClass = (type) => {
      const classes = {
        operational: 'badge-blue',
        administrative: 'badge-gray',
        marketing: 'badge-purple',
        travel: 'badge-green'
      }
      return classes[type] || 'badge-gray'
    }
    
    const getTypeLabel = (type) => {
      const labels = {
        operational: 'عملیاتی',
        administrative: 'اداری',
        marketing: 'بازاریابی',
        travel: 'سفر'
      }
      return labels[type] || type
    }
    
    const getStatusClass = (status) => {
      const classes = {
        pending: 'badge-yellow',
        approved: 'badge-green',
        rejected: 'badge-red'
      }
      return classes[status] || 'badge-gray'
    }
    
    const getStatusLabel = (status) => {
      const labels = {
        pending: 'در انتظار',
        approved: 'تایید شده',
        rejected: 'رد شده'
      }
      return labels[status] || status
    }
    
    const editCost = (cost) => {
      // Navigate to edit page or open modal
      console.log('Edit cost:', cost)
    }
    
    // Lifecycle
    onMounted(() => {
      loadCosts()
    })
    
    return {
      costs,
      searchQuery,
      selectedType,
      selectedStatus,
      dateFrom,
      dateTo,
      showAddModal,
      totalCosts,
      pendingCosts,
      filteredCosts,
      formatCurrency,
      formatDate,
      getTypeClass,
      getTypeLabel,
      getStatusClass,
      getStatusLabel,
      editCost,
      deleteCost
    }
  }
}
</script>

<style scoped>
.costs-view {
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
}

.filter-group label {
  display: block;
  font-size: 14px;
  font-weight: 500;
  color: #374151;
  margin-bottom: 4px;
}

.table-container {
  background: white;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
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

.action-buttons {
  display: flex;
  gap: 8px;
}

.btn-icon {
  width: 32px;
  height: 32px;
  border-radius: 4px;
  border: none;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-icon.btn-primary {
  background: #3b82f6;
  color: white;
}

.btn-icon.btn-primary:hover {
  background: #2563eb;
}

.btn-icon.btn-danger {
  background: #ef4444;
  color: white;
}

.btn-icon.btn-danger:hover {
  background: #dc2626;
}

.badge {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
}

.badge-blue {
  background: #dbeafe;
  color: #1d4ed8;
}

.badge-gray {
  background: #f3f4f6;
  color: #374151;
}

.badge-purple {
  background: #e9d5ff;
  color: #7c3aed;
}

.badge-green {
  background: #d1fae5;
  color: #065f46;
}

.badge-yellow {
  background: #fef3c7;
  color: #92400e;
}

.badge-red {
  background: #fee2e2;
  color: #991b1b;
}
</style>