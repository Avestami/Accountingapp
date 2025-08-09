<template>
  <div class="space-y-6" dir="rtl">
    <!-- Page Header -->
    <div class="bg-white rounded-lg shadow-sm p-6">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900">داشبورد</h1>
          <p class="text-gray-600 mt-1">خلاصه‌ای از وضعیت سیستم حسابداری</p>
        </div>
        <div class="flex items-center space-x-4 space-x-reverse">
          <PersianDatePicker
            v-model="selectedDate"
            placeholder="انتخاب تاریخ"
            class="w-40"
          />
          <AppButton
            @click="refreshData"
            variant="outline"
            size="sm"
            :loading="isRefreshing"
          >
            <svg class="w-4 h-4 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
            </svg>
            بروزرسانی
          </AppButton>
        </div>
      </div>
    </div>

    <!-- Statistics Cards -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      <!-- Sales Documents -->
      <div class="bg-white rounded-lg shadow-sm p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-blue-100 rounded-lg flex items-center justify-center">
              <svg class="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
              </svg>
            </div>
          </div>
          <div class="mr-4 flex-1">
            <p class="text-sm font-medium text-gray-600">اسناد فروش</p>
            <p class="text-2xl font-bold text-gray-900">{{ formatNumber(stats.salesDocuments.total) }}</p>
            <p class="text-xs text-gray-500 mt-1">
              صادر شده: {{ formatNumber(stats.salesDocuments.issued) }} |
              صادر نشده: {{ formatNumber(stats.salesDocuments.unissued) }}
            </p>
          </div>
        </div>
      </div>

      <!-- Total Revenue -->
      <div class="bg-white rounded-lg shadow-sm p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-green-100 rounded-lg flex items-center justify-center">
              <svg class="w-5 h-5 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1" />
              </svg>
            </div>
          </div>
          <div class="mr-4 flex-1">
            <p class="text-sm font-medium text-gray-600">کل درآمد</p>
            <p class="text-2xl font-bold text-gray-900">{{ formatCurrency(stats.totalRevenue) }}</p>
            <p class="text-xs text-green-600 mt-1">
              +{{ stats.revenueGrowth }}% نسبت به ماه قبل
            </p>
          </div>
        </div>
      </div>

      <!-- Pending Vouchers -->
      <div class="bg-white rounded-lg shadow-sm p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-yellow-100 rounded-lg flex items-center justify-center">
              <svg class="w-5 h-5 text-yellow-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
          </div>
          <div class="mr-4 flex-1">
            <p class="text-sm font-medium text-gray-600">سندهای در انتظار</p>
            <p class="text-2xl font-bold text-gray-900">{{ formatNumber(stats.pendingVouchers) }}</p>
            <p class="text-xs text-yellow-600 mt-1">
              نیاز به بررسی
            </p>
          </div>
        </div>
      </div>

      <!-- Active Users -->
      <div class="bg-white rounded-lg shadow-sm p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-purple-100 rounded-lg flex items-center justify-center">
              <svg class="w-5 h-5 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.5 2.5 0 11-5 0 2.5 2.5 0 015 0z" />
              </svg>
            </div>
          </div>
          <div class="mr-4 flex-1">
            <p class="text-sm font-medium text-gray-600">کاربران فعال</p>
            <p class="text-2xl font-bold text-gray-900">{{ formatNumber(stats.activeUsers) }}</p>
            <p class="text-xs text-gray-500 mt-1">
              در ۲۴ ساعت گذشته
            </p>
          </div>
        </div>
      </div>
    </div>

    <!-- Charts Section -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Revenue Chart -->
      <div class="bg-white rounded-lg shadow-sm p-6">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-semibold text-gray-900">نمودار درآمد</h3>
          <select v-model="revenueChartPeriod" class="text-sm border-gray-300 rounded-md">
            <option value="7d">۷ روز گذشته</option>
            <option value="30d">۳۰ روز گذشته</option>
            <option value="90d">۹۰ روز گذشته</option>
          </select>
        </div>
        <DashboardChart
          :chart-type="'line'"
          :period="revenueChartPeriod"
          :title="'درآمد روزانه'"
          @period-change="revenueChartPeriod = $event"
        />
      </div>

      <!-- Sales Distribution -->
      <div class="bg-white rounded-lg shadow-sm p-6">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-semibold text-gray-900">توزیع فروش</h3>
          <select v-model="salesChartType" class="text-sm border-gray-300 rounded-md">
            <option value="pie">نمودار دایره‌ای</option>
            <option value="bar">نمودار ستونی</option>
          </select>
        </div>
        <DashboardChart
          :chart-type="salesChartType"
          :period="'30d'"
          :title="'فروش بر اساس نوع سرویس'"
          @chart-type-change="salesChartType = $event"
        />
      </div>
    </div>

    <!-- Recent Activities -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Recent Sales -->
      <div class="bg-white rounded-lg shadow-sm p-6">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-semibold text-gray-900">آخرین فروش‌ها</h3>
          <router-link to="/sales" class="text-sm text-blue-600 hover:text-blue-500">
            مشاهده همه
          </router-link>
        </div>
        <div class="space-y-3">
          <div v-for="sale in recentSales" :key="sale.id" class="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
            <div class="flex items-center">
              <div class="w-2 h-2 rounded-full ml-3" :class="getStatusColor(sale.status)"></div>
              <div>
                <p class="text-sm font-medium text-gray-900">{{ sale.documentNumber }}</p>
                <p class="text-xs text-gray-500">{{ sale.counterparty }}</p>
              </div>
            </div>
            <div class="text-left">
              <p class="text-sm font-medium text-gray-900">{{ formatCurrency(sale.amount) }}</p>
              <p class="text-xs text-gray-500">{{ formatDate(sale.date) }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Vouchers -->
      <div class="bg-white rounded-lg shadow-sm p-6">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-semibold text-gray-900">آخرین سندها</h3>
          <router-link to="/finance/vouchers" class="text-sm text-blue-600 hover:text-blue-500">
            مشاهده همه
          </router-link>
        </div>
        <div class="space-y-3">
          <div v-for="voucher in recentVouchers" :key="voucher.id" class="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
            <div class="flex items-center">
              <div class="w-8 h-8 bg-blue-100 rounded-lg flex items-center justify-center ml-3">
                <svg class="w-4 h-4 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                </svg>
              </div>
              <div>
                <p class="text-sm font-medium text-gray-900">{{ voucher.number }}</p>
                <p class="text-xs text-gray-500">{{ voucher.description }}</p>
              </div>
            </div>
            <div class="text-left">
              <p class="text-sm font-medium text-gray-900">{{ formatCurrency(voucher.amount) }}</p>
              <p class="text-xs text-gray-500">{{ formatDate(voucher.date) }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Quick Actions -->
    <div class="bg-white rounded-lg shadow-sm p-6">
      <h3 class="text-lg font-semibold text-gray-900 mb-4">عملیات سریع</h3>
      <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
        <router-link to="/sales/new" class="flex flex-col items-center p-4 border border-gray-200 rounded-lg hover:bg-gray-50 transition-colors">
          <div class="w-10 h-10 bg-blue-100 rounded-lg flex items-center justify-center mb-2">
            <svg class="w-6 h-6 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
            </svg>
          </div>
          <span class="text-sm font-medium text-gray-900">سند فروش جدید</span>
        </router-link>

        <router-link to="/finance/vouchers/new" class="flex flex-col items-center p-4 border border-gray-200 rounded-lg hover:bg-gray-50 transition-colors">
          <div class="w-10 h-10 bg-green-100 rounded-lg flex items-center justify-center mb-2">
            <svg class="w-6 h-6 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
            </svg>
          </div>
          <span class="text-sm font-medium text-gray-900">سند حسابداری جدید</span>
        </router-link>

        <router-link to="/reports" class="flex flex-col items-center p-4 border border-gray-200 rounded-lg hover:bg-gray-50 transition-colors">
          <div class="w-10 h-10 bg-purple-100 rounded-lg flex items-center justify-center mb-2">
            <svg class="w-6 h-6 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v4a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
            </svg>
          </div>
          <span class="text-sm font-medium text-gray-900">گزارش‌گیری</span>
        </router-link>

        <router-link to="/settings" class="flex flex-col items-center p-4 border border-gray-200 rounded-lg hover:bg-gray-50 transition-colors">
          <div class="w-10 h-10 bg-gray-100 rounded-lg flex items-center justify-center mb-2">
            <svg class="w-6 h-6 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
            </svg>
          </div>
          <span class="text-sm font-medium text-gray-900">تنظیمات</span>
        </router-link>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '../stores/auth'
import { useSalesStore } from '../stores/sales'
import { dashboardApi } from '../services/api'
import AppButton from '../components/AppButton.vue'
import PersianDatePicker from '../components/PersianDatePicker.vue'
import DashboardChart from '../components/DashboardChart.vue'

export default {
  name: 'DashboardView',
  components: {
    AppButton,
    PersianDatePicker,
    DashboardChart
  },
  setup() {
    // Stores
    const authStore = useAuthStore()
    const salesStore = useSalesStore()
    
    // Reactive data
    const selectedDate = ref(new Date())
    const isRefreshing = ref(false)
    const revenueChartPeriod = ref('30d')
    const salesChartType = ref('pie')
    
    // Dashboard statistics data
    const stats = ref({
      salesDocuments: {
        total: 0,
        issued: 0,
        unissued: 0
      },
      totalRevenue: 0,
      revenueGrowth: 0,
      pendingVouchers: 0,
      activeUsers: 0
    })
    
    const recentSales = ref([])
    const recentVouchers = ref([])
    
    // Load dashboard data from API
    const loadDashboardData = async () => {
      try {
        const params = {
          fromDate: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0], // 30 days ago
          toDate: new Date().toISOString().split('T')[0], // today
          period: 30
        }
        
        const response = await dashboardApi.getDashboardStats(params)
        
        if (response.success) {
          const data = response.data
          
          // Update statistics
          stats.value = {
            salesDocuments: {
              total: data.salesDocuments.total,
              issued: data.salesDocuments.issued,
              unissued: data.salesDocuments.unissued
            },
            totalRevenue: data.totalRevenue,
            revenueGrowth: data.revenueGrowth,
            pendingVouchers: data.pendingVouchers,
            activeUsers: data.activeUsers
          }
          
          // Update recent activities
          recentSales.value = data.recentSales.map(sale => ({
            id: sale.id,
            documentNumber: sale.documentNumber,
            counterparty: sale.counterparty,
            amount: sale.amount,
            date: sale.date,
            status: sale.status
          }))
          
          recentVouchers.value = data.recentVouchers.map(voucher => ({
            id: voucher.id,
            number: voucher.number,
            description: voucher.description,
            amount: voucher.amount,
            date: voucher.date
          }))
        }
      } catch (error) {
        console.error('Error loading dashboard data:', error)
        
        // Fallback to mock data if API fails
        const user = authStore.user
        const userSeed = user?.username ? user.username.length : 1
        const roleSeed = user?.role === 'admin' ? 3 : user?.role === 'finance' ? 2 : 1
        
        stats.value = {
          salesDocuments: {
            total: 800 + (userSeed * roleSeed * 50),
            issued: 650 + (userSeed * roleSeed * 40),
            unissued: 150 + (userSeed * roleSeed * 10)
          },
          totalRevenue: 1500000000 + (userSeed * roleSeed * 100000000),
          revenueGrowth: 5 + (userSeed * roleSeed * 2),
          pendingVouchers: 10 + (userSeed * roleSeed * 3),
          activeUsers: 3 + (userSeed * roleSeed)
        }
        
        // Mock recent activities
        recentSales.value = [
          {
            id: 1,
            documentNumber: 'S-1403-001234',
            counterparty: 'شرکت گردشگری آسمان',
            amount: 15750000,
            date: '1403/08/15',
            status: 'issued'
          },
          {
            id: 2,
            documentNumber: 'S-1403-001233',
            counterparty: 'آژانس مسافرتی پارس',
            amount: 8900000,
            date: '1403/08/14',
            status: 'unissued'
          },
          {
            id: 3,
            documentNumber: 'S-1403-001232',
            counterparty: 'دفتر خدمات مسافرتی ایران',
            amount: 12300000,
            date: '1403/08/13',
            status: 'issued'
          }
        ]
        
        recentVouchers.value = [
          {
            id: 1,
            number: 'V-1403-000456',
            description: 'دریافت از مشتری',
            amount: 15750000,
            date: '1403/08/15'
          },
          {
            id: 2,
            number: 'V-1403-000455',
            description: 'پرداخت کمیسیون',
            amount: -2100000,
            date: '1403/08/14'
          },
          {
            id: 3,
            number: 'V-1403-000454',
            description: 'دریافت از مشتری',
            amount: 8900000,
            date: '1403/08/13'
          }
        ]
      }
    }
    
    // Methods
    const formatNumber = (number) => {
      return new Intl.NumberFormat('fa-IR').format(number)
    }
    
    const formatCurrency = (amount) => {
      const absAmount = Math.abs(amount)
      const formatted = new Intl.NumberFormat('fa-IR').format(absAmount)
      const sign = amount < 0 ? '-' : ''
      return `${sign}${formatted} ریال`
    }
    
    const formatDate = (dateStr) => {
      return dateStr // Already in Persian format
    }
    
    const getStatusColor = (status) => {
      switch (status) {
        case 'issued':
          return 'bg-green-500'
        case 'unissued':
          return 'bg-yellow-500'
        case 'canceled':
          return 'bg-red-500'
        default:
          return 'bg-gray-500'
      }
    }
    
    const refreshData = async () => {
      isRefreshing.value = true
      
      try {
        await loadDashboardData()
      } catch (error) {
        console.error('Error refreshing data:', error)
      } finally {
        isRefreshing.value = false
      }
    }
    
    // Lifecycle
    onMounted(async () => {
      await loadDashboardData()
      await salesStore.loadDocuments()
    })
    
    return {
      // Data
      selectedDate,
      isRefreshing,
      revenueChartPeriod,
      salesChartType,
      stats,
      recentSales,
      recentVouchers,
      
      // Computed
      user: computed(() => authStore.user),
      
      // Methods
      formatNumber,
      formatCurrency,
      formatDate,
      getStatusColor,
      refreshData,
      loadDashboardData
    }
  }
}
</script>

<style scoped>
/* Custom styles for dashboard */
.grid {
  gap: 1.5rem;
}

@media (max-width: 768px) {
  .grid {
    gap: 1rem;
  }
}
</style>