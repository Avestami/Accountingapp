<template>
  <div class="space-y-6" dir="rtl">
    <!-- Page Header -->
    <div class="bg-white rounded-lg shadow-sm p-6">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900">سندهای حسابداری</h1>
          <p class="text-gray-600 mt-1">مدیریت سندهای حسابداری و تراکنش‌های مالی</p>
        </div>
        <div class="flex items-center space-x-4 space-x-reverse">
          <AppButton
            @click="exportToExcel"
            variant="outline"
            size="sm"
            :loading="isExporting"
          >
            <svg class="w-4 h-4 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 10v6m0 0l-3-3m3 3l3-3m2 8H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
            </svg>
            خروجی اکسل
          </AppButton>
          <router-link to="/finance/vouchers/new">
            <AppButton variant="primary" size="sm">
              <svg class="w-4 h-4 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
              </svg>
              سند جدید
            </AppButton>
          </router-link>
        </div>
      </div>
    </div>

    <!-- Filters -->
    <div class="bg-white rounded-lg shadow-sm p-6">
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <!-- Search -->
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">جستجو</label>
          <input
            v-model="filters.search"
            type="text"
            placeholder="شماره سند، شرح، طرف حساب..."
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          />
        </div>

        <!-- Type Filter -->
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">نوع سند</label>
          <select
            v-model="filters.type"
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          >
            <option value="">همه</option>
            <option value="receipt">دریافت</option>
            <option value="payment">پرداخت</option>
            <option value="transfer">انتقال</option>
            <option value="adjustment">اصلاحی</option>
          </select>
        </div>

        <!-- Status Filter -->
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">وضعیت</label>
          <select
            v-model="filters.status"
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          >
            <option value="">همه</option>
            <option value="draft">پیش‌نویس</option>
            <option value="confirmed">تایید شده</option>
            <option value="rejected">رد شده</option>
          </select>
        </div>

        <!-- Date Range -->
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">تاریخ سند</label>
          <div class="flex space-x-2 space-x-reverse">
            <PersianDatePicker
              v-model="filters.dateFrom"
              placeholder="از تاریخ"
              class="flex-1"
            />
            <PersianDatePicker
              v-model="filters.dateTo"
              placeholder="تا تاریخ"
              class="flex-1"
            />
          </div>
        </div>
      </div>

      <div class="flex items-center justify-between mt-4">
        <div class="flex items-center space-x-4 space-x-reverse">
          <AppButton
            @click="applyFilters"
            variant="primary"
            size="sm"
            :loading="isLoading"
          >
            اعمال فیلتر
          </AppButton>
          <AppButton
            @click="clearFilters"
            variant="outline"
            size="sm"
          >
            پاک کردن
          </AppButton>
        </div>
        <div class="text-sm text-gray-600">
          {{ filteredVouchers.length }} سند از {{ totalVouchers }} سند
        </div>
      </div>
    </div>

    <!-- Vouchers Table -->
    <div class="bg-white rounded-lg shadow-sm overflow-hidden">
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                شماره سند
              </th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                تاریخ
              </th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                نوع سند
              </th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                شرح
              </th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                طرف حساب
              </th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                مبلغ
              </th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                وضعیت
              </th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                عملیات
              </th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-if="isLoading">
              <td colspan="8" class="px-6 py-12 text-center">
                <AppSpinner size="md" text="در حال بارگذاری..." />
              </td>
            </tr>
            <tr v-else-if="filteredVouchers.length === 0">
              <td colspan="8" class="px-6 py-12 text-center text-gray-500">
                هیچ سندی یافت نشد
              </td>
            </tr>
            <tr v-else v-for="voucher in paginatedVouchers" :key="voucher.id" class="hover:bg-gray-50">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm font-medium text-gray-900">{{ voucher.voucherNumber }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900">{{ formatDate(voucher.date) }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span class="inline-flex px-2 py-1 text-xs font-semibold rounded-full" :class="getTypeClass(voucher.type)">
                  {{ getTypeLabel(voucher.type) }}
                </span>
              </td>
              <td class="px-6 py-4">
                <div class="text-sm text-gray-900 truncate max-w-xs">{{ voucher.description }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900">{{ voucher.counterparty }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm font-medium text-gray-900">{{ formatCurrency(voucher.amount) }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span class="inline-flex px-2 py-1 text-xs font-semibold rounded-full" :class="getStatusClass(voucher.status)">
                  {{ getStatusLabel(voucher.status) }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                <div class="flex items-center space-x-2 space-x-reverse">
                  <button
                    @click="viewVoucher(voucher)"
                    class="text-blue-600 hover:text-blue-900"
                    title="مشاهده"
                  >
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                    </svg>
                  </button>
                  <button
                    v-if="voucher.status === 'draft'"
                    @click="editVoucher(voucher)"
                    class="text-yellow-600 hover:text-yellow-900"
                    title="ویرایش"
                  >
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                    </svg>
                  </button>
                  <button
                    v-if="voucher.status === 'draft'"
                    @click="confirmVoucher(voucher)"
                    class="text-green-600 hover:text-green-900"
                    title="تایید"
                  >
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                  </button>
                  <button
                    v-if="voucher.status === 'draft'"
                    @click="rejectVoucher(voucher)"
                    class="text-red-600 hover:text-red-900"
                    title="رد"
                  >
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                  </button>
                  <button
                    @click="printVoucher(voucher)"
                    class="text-blue-600 hover:text-blue-900"
                    title="چاپ"
                  >
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 17h2a2 2 0 002-2v-4a2 2 0 00-2-2H5a2 2 0 00-2 2v4a2 2 0 002 2h2m2 4h6a2 2 0 002-2v-4a2 2 0 00-2-2H9a2 2 0 00-2 2v4a2 2 0 002 2zm8-12V5a2 2 0 00-2-2H9a2 2 0 00-2 2v4h10z" />
                    </svg>
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      <div class="bg-white px-4 py-3 flex items-center justify-between border-t border-gray-200 sm:px-6">
        <div class="flex-1 flex justify-between sm:hidden">
          <AppButton
            @click="previousPage"
            :disabled="currentPage === 1"
            variant="outline"
            size="sm"
          >
            قبلی
          </AppButton>
          <AppButton
            @click="nextPage"
            :disabled="currentPage === totalPages"
            variant="outline"
            size="sm"
          >
            بعدی
          </AppButton>
        </div>
        <div class="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
          <div>
            <p class="text-sm text-gray-700">
              نمایش
              <span class="font-medium">{{ startIndex }}</span>
              تا
              <span class="font-medium">{{ endIndex }}</span>
              از
              <span class="font-medium">{{ filteredVouchers.length }}</span>
              نتیجه
            </p>
          </div>
          <div>
            <nav class="relative z-0 inline-flex rounded-md shadow-sm -space-x-px" aria-label="Pagination">
              <button
                @click="previousPage"
                :disabled="currentPage === 1"
                class="relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <svg class="h-5 w-5" fill="currentColor" viewBox="0 0 20 20">
                  <path fill-rule="evenodd" d="M12.707 5.293a1 1 0 010 1.414L9.414 10l3.293 3.293a1 1 0 01-1.414 1.414l-4-4a1 1 0 010-1.414l4-4a1 1 0 011.414 0z" clip-rule="evenodd" />
                </svg>
              </button>
              
              <button
                v-for="page in visiblePages"
                :key="page"
                @click="goToPage(page)"
                :class="[
                  'relative inline-flex items-center px-4 py-2 border text-sm font-medium',
                  page === currentPage
                    ? 'z-10 bg-blue-50 border-blue-500 text-blue-600'
                    : 'bg-white border-gray-300 text-gray-500 hover:bg-gray-50'
                ]"
              >
                {{ page }}
              </button>
              
              <button
                @click="nextPage"
                :disabled="currentPage === totalPages"
                class="relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <svg class="h-5 w-5" fill="currentColor" viewBox="0 0 20 20">
                  <path fill-rule="evenodd" d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0z" clip-rule="evenodd" />
                </svg>
              </button>
            </nav>
          </div>
        </div>
      </div>
    </div>

    <!-- Summary Cards -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
      <!-- Total Receipts -->
      <div class="bg-white rounded-lg shadow-sm p-6">
        <div class="flex items-center">
          <div class="p-3 rounded-full bg-green-100 text-green-600">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </div>
          <div class="mr-4">
            <p class="text-sm font-medium text-gray-500">مجموع دریافت‌ها</p>
            <p class="text-lg font-semibold text-gray-900">{{ formatCurrency(totalReceipts) }}</p>
          </div>
        </div>
      </div>

      <!-- Total Payments -->
      <div class="bg-white rounded-lg shadow-sm p-6">
        <div class="flex items-center">
          <div class="p-3 rounded-full bg-red-100 text-red-600">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </div>
          <div class="mr-4">
            <p class="text-sm font-medium text-gray-500">مجموع پرداخت‌ها</p>
            <p class="text-lg font-semibold text-gray-900">{{ formatCurrency(totalPayments) }}</p>
          </div>
        </div>
      </div>

      <!-- Balance -->
      <div class="bg-white rounded-lg shadow-sm p-6">
        <div class="flex items-center">
          <div class="p-3 rounded-full bg-blue-100 text-blue-600">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 6l3 1m0 0l-3 9a5.002 5.002 0 006.001 0M6 7l3 9M6 7l6-2m6 2l3-1m-3 1l-3 9a5.002 5.002 0 006.001 0M18 7l3 9m-3-9l-6-2m0-2v2m0 16V5m0 16H9m3 0h3" />
            </svg>
          </div>
          <div class="mr-4">
            <p class="text-sm font-medium text-gray-500">مانده</p>
            <p class="text-lg font-semibold" :class="balance >= 0 ? 'text-green-600' : 'text-red-600'">
              {{ formatCurrency(balance) }}
            </p>
          </div>
        </div>
      </div>

      <!-- Pending Vouchers -->
      <div class="bg-white rounded-lg shadow-sm p-6">
        <div class="flex items-center">
          <div class="p-3 rounded-full bg-yellow-100 text-yellow-600">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </div>
          <div class="mr-4">
            <p class="text-sm font-medium text-gray-500">اسناد در انتظار تایید</p>
            <p class="text-lg font-semibold text-gray-900">{{ pendingVouchers }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useFinanceStore } from '../stores/finance'
import { useExportService } from '../services/exportService'
import AppButton from '../components/AppButton.vue'
import AppSpinner from '../components/AppSpinner.vue'
import PersianDatePicker from '../components/PersianDatePicker.vue'

export default {
  name: 'VouchersView',
  components: {
    AppButton,
    AppSpinner,
    PersianDatePicker
  },
  setup() {
    const router = useRouter()
    const financeStore = useFinanceStore()
    const exportService = useExportService()
    
    // Reactive data
    const isLoading = ref(false)
    const isExporting = ref(false)
    const currentPage = ref(1)
    const itemsPerPage = ref(10)
    
    const filters = ref({
      search: '',
      type: '',
      status: '',
      dateFrom: null,
      dateTo: null
    })
    
    // Computed properties
    const vouchers = computed(() => financeStore.vouchers)
    const totalVouchers = computed(() => financeStore.totalVouchers)
    
    const filteredVouchers = computed(() => {
      let filtered = [...vouchers.value]
      
      // Search filter
      if (filters.value.search) {
        const searchTerm = filters.value.search.toLowerCase()
        filtered = filtered.filter(voucher => 
          voucher.voucherNumber.toLowerCase().includes(searchTerm) ||
          voucher.description.toLowerCase().includes(searchTerm) ||
          voucher.counterparty.toLowerCase().includes(searchTerm)
        )
      }
      
      // Type filter
      if (filters.value.type) {
        filtered = filtered.filter(voucher => voucher.type === filters.value.type)
      }
      
      // Status filter
      if (filters.value.status) {
        filtered = filtered.filter(voucher => voucher.status === filters.value.status)
      }
      
      // Date range filter
      if (filters.value.dateFrom) {
        filtered = filtered.filter(voucher => new Date(voucher.date) >= filters.value.dateFrom)
      }
      if (filters.value.dateTo) {
        filtered = filtered.filter(voucher => new Date(voucher.date) <= filters.value.dateTo)
      }
      
      return filtered
    })
    
    const totalPages = computed(() => Math.ceil(filteredVouchers.value.length / itemsPerPage.value))
    
    const paginatedVouchers = computed(() => {
      const start = (currentPage.value - 1) * itemsPerPage.value
      const end = start + itemsPerPage.value
      return filteredVouchers.value.slice(start, end)
    })
    
    const startIndex = computed(() => {
      return filteredVouchers.value.length === 0 ? 0 : (currentPage.value - 1) * itemsPerPage.value + 1
    })
    
    const endIndex = computed(() => {
      return Math.min(currentPage.value * itemsPerPage.value, filteredVouchers.value.length)
    })
    
    const visiblePages = computed(() => {
      const pages = []
      const maxVisible = 5
      let start = Math.max(1, currentPage.value - Math.floor(maxVisible / 2))
      let end = Math.min(totalPages.value, start + maxVisible - 1)
      
      if (end - start + 1 < maxVisible) {
        start = Math.max(1, end - maxVisible + 1)
      }
      
      for (let i = start; i <= end; i++) {
        pages.push(i)
      }
      
      return pages
    })
    
    // Financial summary computed properties
    const totalReceipts = computed(() => {
      return vouchers.value
        .filter(v => v.type === 'receipt' && v.status === 'confirmed')
        .reduce((sum, v) => sum + v.amount, 0)
    })
    
    const totalPayments = computed(() => {
      return vouchers.value
        .filter(v => v.type === 'payment' && v.status === 'confirmed')
        .reduce((sum, v) => sum + v.amount, 0)
    })
    
    const balance = computed(() => {
      return totalReceipts.value - totalPayments.value
    })
    
    const pendingVouchers = computed(() => {
      return vouchers.value.filter(v => v.status === 'draft').length
    })
    
    // Methods
    const formatDate = (dateStr) => {
      if (!dateStr) return '-'
      return new Date(dateStr).toLocaleDateString('fa-IR')
    }
    
    const formatCurrency = (amount) => {
      return new Intl.NumberFormat('fa-IR').format(amount) + ' ریال'
    }
    
    const getTypeLabel = (type) => {
      const labels = {
        receipt: 'دریافت',
        payment: 'پرداخت',
        transfer: 'انتقال',
        adjustment: 'اصلاحی'
      }
      return labels[type] || type
    }
    
    const getTypeClass = (type) => {
      const classes = {
        receipt: 'bg-green-100 text-green-800',
        payment: 'bg-red-100 text-red-800',
        transfer: 'bg-blue-100 text-blue-800',
        adjustment: 'bg-purple-100 text-purple-800'
      }
      return classes[type] || 'bg-gray-100 text-gray-800'
    }
    
    const getStatusLabel = (status) => {
      const labels = {
        draft: 'پیش‌نویس',
        confirmed: 'تایید شده',
        rejected: 'رد شده'
      }
      return labels[status] || status
    }
    
    const getStatusClass = (status) => {
      const classes = {
        draft: 'bg-yellow-100 text-yellow-800',
        confirmed: 'bg-green-100 text-green-800',
        rejected: 'bg-red-100 text-red-800'
      }
      return classes[status] || 'bg-gray-100 text-gray-800'
    }
    
    const applyFilters = async () => {
      isLoading.value = true
      currentPage.value = 1
      
      try {
        await financeStore.loadVouchers(filters.value)
      } catch (error) {
        console.error('Error applying filters:', error)
      } finally {
        isLoading.value = false
      }
    }
    
    const clearFilters = () => {
      filters.value = {
        search: '',
        type: '',
        status: '',
        dateFrom: null,
        dateTo: null
      }
      currentPage.value = 1
      applyFilters()
    }
    
    const exportToExcel = async () => {
      isExporting.value = true
      
      try {
        await exportService.exportVouchers(filteredVouchers.value)
      } catch (error) {
        console.error('Error exporting to Excel:', error)
      } finally {
        isExporting.value = false
      }
    }
    
    const viewVoucher = (voucher) => {
      router.push(`/finance/vouchers/${voucher.id}`)
    }
    
    const editVoucher = (voucher) => {
      router.push(`/finance/vouchers/${voucher.id}/edit`)
    }
    
    const confirmVoucher = async (voucher) => {
      try {
        await financeStore.updateVoucherStatus(voucher.id, 'confirmed')
      } catch (error) {
        console.error('Error confirming voucher:', error)
      }
    }
    
    const rejectVoucher = async (voucher) => {
      if (confirm(`آیا از رد سند ${voucher.voucherNumber} اطمینان دارید؟`)) {
        try {
          await financeStore.updateVoucherStatus(voucher.id, 'rejected')
        } catch (error) {
          console.error('Error rejecting voucher:', error)
        }
      }
    }
    
    const printVoucher = async (voucher) => {
      try {
        await exportService.printVoucher(voucher)
      } catch (error) {
        console.error('Error printing voucher:', error)
      }
    }
    
    // Pagination methods
    const goToPage = (page) => {
      if (page >= 1 && page <= totalPages.value) {
        currentPage.value = page
      }
    }
    
    const previousPage = () => {
      if (currentPage.value > 1) {
        currentPage.value--
      }
    }
    
    const nextPage = () => {
      if (currentPage.value < totalPages.value) {
        currentPage.value++
      }
    }
    
    // Watchers
    watch(() => filteredVouchers.value.length, () => {
      if (currentPage.value > totalPages.value) {
        currentPage.value = Math.max(1, totalPages.value)
      }
    })
    
    // Lifecycle
    onMounted(async () => {
      isLoading.value = true
      try {
        await financeStore.loadVouchers()
      } catch (error) {
        console.error('Error loading vouchers:', error)
      } finally {
        isLoading.value = false
      }
    })
    
    return {
      // Data
      isLoading,
      isExporting,
      currentPage,
      itemsPerPage,
      filters,
      
      // Computed
      vouchers,
      totalVouchers,
      filteredVouchers,
      paginatedVouchers,
      totalPages,
      startIndex,
      endIndex,
      visiblePages,
      totalReceipts,
      totalPayments,
      balance,
      pendingVouchers,
      
      // Methods
      formatDate,
      formatCurrency,
      getTypeLabel,
      getTypeClass,
      getStatusLabel,
      getStatusClass,
      applyFilters,
      clearFilters,
      exportToExcel,
      viewVoucher,
      editVoucher,
      confirmVoucher,
      rejectVoucher,
      printVoucher,
      goToPage,
      previousPage,
      nextPage
    }
  }
}
</script>

<style scoped>
/* Custom styles for vouchers */
.table-container {
  max-height: 600px;
  overflow-y: auto;
}

@media (max-width: 768px) {
  .grid {
    grid-template-columns: 1fr;
  }
}
</style>