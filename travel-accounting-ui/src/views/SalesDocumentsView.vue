<template>
  <div class="space-y-6" dir="rtl">
    <!-- Page Header -->
    <div class="bg-white rounded-lg shadow-sm p-6">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900">اسناد فروش</h1>
          <p class="text-gray-600 mt-1">مدیریت اسناد فروش و صدور بلیط</p>
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
          <router-link to="/sales/new">
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
            placeholder="شماره سند، طرف حساب..."
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          />
        </div>

        <!-- Status Filter -->
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">وضعیت</label>
          <select
            v-model="filters.status"
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          >
            <option value="">همه</option>
            <option value="unissued">صادر نشده</option>
            <option value="issued">صادر شده</option>
            <option value="canceled">لغو شده</option>
          </select>
        </div>

        <!-- Service Type Filter -->
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">نوع سرویس</label>
          <select
            v-model="filters.serviceType"
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent"
          >
            <option value="">همه</option>
            <option value="domestic">داخلی</option>
            <option value="international">بین‌المللی</option>
            <option value="charter">چارتر</option>
            <option value="hotel">هتل</option>
            <option value="tour">تور</option>
          </select>
        </div>

        <!-- Date Range -->
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">تاریخ پرواز</label>
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
          {{ filteredDocuments.length }} سند از {{ totalDocuments }} سند
        </div>
      </div>
    </div>

    <!-- Documents Table -->
    <div class="bg-white rounded-lg shadow-sm overflow-hidden">
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                شماره سند
              </th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                طرف حساب
              </th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                مسیر پرواز
              </th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                تاریخ پرواز
              </th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                نوع سرویس
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
            <tr v-else-if="filteredDocuments.length === 0">
              <td colspan="8" class="px-6 py-12 text-center text-gray-500">
                هیچ سندی یافت نشد
              </td>
            </tr>
            <tr v-else v-for="document in paginatedDocuments" :key="document.id" class="hover:bg-gray-50">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm font-medium text-gray-900">{{ document.documentNumber }}</div>
                <div class="text-sm text-gray-500">{{ formatDate(document.createdAt) }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900">{{ document.counterparty }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900">{{ document.route }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900">{{ formatDate(document.flightDate) }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span class="inline-flex px-2 py-1 text-xs font-semibold rounded-full" :class="getServiceTypeClass(document.serviceType)">
                  {{ getServiceTypeLabel(document.serviceType) }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm font-medium text-gray-900">{{ formatCurrency(document.totalAmount) }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span class="inline-flex px-2 py-1 text-xs font-semibold rounded-full" :class="getStatusClass(document.status)">
                  {{ getStatusLabel(document.status) }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                <div class="flex items-center space-x-2 space-x-reverse">
                  <button
                    @click="viewDocument(document)"
                    class="text-blue-600 hover:text-blue-900"
                    title="مشاهده"
                  >
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                    </svg>
                  </button>
                  <button
                    @click="editDocument(document)"
                    class="text-yellow-600 hover:text-yellow-900"
                    title="ویرایش"
                  >
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                    </svg>
                  </button>
                  <button
                    v-if="document.status === 'unissued'"
                    @click="issueDocument(document)"
                    class="text-green-600 hover:text-green-900"
                    title="صدور"
                  >
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                  </button>
                  <button
                    @click="deleteDocument(document)"
                    class="text-red-600 hover:text-red-900"
                    title="حذف"
                  >
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
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
              <span class="font-medium">{{ filteredDocuments.length }}</span>
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
  </div>
</template>

<script>
import { ref, computed, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useSalesStore } from '../stores/sales'
import { useExportService } from '../services/exportService'
import AppButton from '../components/AppButton.vue'
import AppSpinner from '../components/AppSpinner.vue'
import PersianDatePicker from '../components/PersianDatePicker.vue'

export default {
  name: 'SalesDocumentsView',
  components: {
    AppButton,
    AppSpinner,
    PersianDatePicker
  },
  setup() {
    const router = useRouter()
    const salesStore = useSalesStore()
    const exportService = useExportService()
    
    // Reactive data
    const isLoading = ref(false)
    const isExporting = ref(false)
    const currentPage = ref(1)
    const itemsPerPage = ref(10)
    
    const filters = ref({
      search: '',
      status: '',
      serviceType: '',
      dateFrom: null,
      dateTo: null
    })
    
    // Computed properties
    const documents = computed(() => salesStore.documents)
    const totalDocuments = computed(() => salesStore.totalDocuments)
    
    const filteredDocuments = computed(() => {
      let filtered = [...documents.value]
      
      // Search filter
      if (filters.value.search) {
        const searchTerm = filters.value.search.toLowerCase()
        filtered = filtered.filter(doc => 
          doc.documentNumber.toLowerCase().includes(searchTerm) ||
          doc.counterparty.toLowerCase().includes(searchTerm) ||
          doc.route.toLowerCase().includes(searchTerm)
        )
      }
      
      // Status filter
      if (filters.value.status) {
        filtered = filtered.filter(doc => doc.status === filters.value.status)
      }
      
      // Service type filter
      if (filters.value.serviceType) {
        filtered = filtered.filter(doc => doc.serviceType === filters.value.serviceType)
      }
      
      // Date range filter
      if (filters.value.dateFrom) {
        filtered = filtered.filter(doc => new Date(doc.flightDate) >= filters.value.dateFrom)
      }
      if (filters.value.dateTo) {
        filtered = filtered.filter(doc => new Date(doc.flightDate) <= filters.value.dateTo)
      }
      
      return filtered
    })
    
    const totalPages = computed(() => Math.ceil(filteredDocuments.value.length / itemsPerPage.value))
    
    const paginatedDocuments = computed(() => {
      const start = (currentPage.value - 1) * itemsPerPage.value
      const end = start + itemsPerPage.value
      return filteredDocuments.value.slice(start, end)
    })
    
    const startIndex = computed(() => {
      return filteredDocuments.value.length === 0 ? 0 : (currentPage.value - 1) * itemsPerPage.value + 1
    })
    
    const endIndex = computed(() => {
      return Math.min(currentPage.value * itemsPerPage.value, filteredDocuments.value.length)
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
    
    // Methods
    const formatDate = (dateStr) => {
      if (!dateStr) return '-'
      return new Date(dateStr).toLocaleDateString('fa-IR')
    }
    
    const formatCurrency = (amount) => {
      return new Intl.NumberFormat('fa-IR').format(amount) + ' ریال'
    }
    
    const getStatusLabel = (status) => {
      const labels = {
        unissued: 'صادر نشده',
        issued: 'صادر شده',
        canceled: 'لغو شده'
      }
      return labels[status] || status
    }
    
    const getStatusClass = (status) => {
      const classes = {
        unissued: 'bg-yellow-100 text-yellow-800',
        issued: 'bg-green-100 text-green-800',
        canceled: 'bg-red-100 text-red-800'
      }
      return classes[status] || 'bg-gray-100 text-gray-800'
    }
    
    const getServiceTypeLabel = (serviceType) => {
      const labels = {
        domestic: 'داخلی',
        international: 'بین‌المللی',
        charter: 'چارتر',
        hotel: 'هتل',
        tour: 'تور'
      }
      return labels[serviceType] || serviceType
    }
    
    const getServiceTypeClass = (serviceType) => {
      const classes = {
        domestic: 'bg-blue-100 text-blue-800',
        international: 'bg-purple-100 text-purple-800',
        charter: 'bg-orange-100 text-orange-800',
        hotel: 'bg-pink-100 text-pink-800',
        tour: 'bg-indigo-100 text-indigo-800'
      }
      return classes[serviceType] || 'bg-gray-100 text-gray-800'
    }
    
    const applyFilters = async () => {
      isLoading.value = true
      currentPage.value = 1
      
      try {
        await salesStore.loadDocuments(filters.value)
      } catch (error) {
        console.error('Error applying filters:', error)
      } finally {
        isLoading.value = false
      }
    }
    
    const clearFilters = () => {
      filters.value = {
        search: '',
        status: '',
        serviceType: '',
        dateFrom: null,
        dateTo: null
      }
      currentPage.value = 1
      applyFilters()
    }
    
    const exportToExcel = async () => {
      isExporting.value = true
      
      try {
        await exportService.exportSalesDocuments(filteredDocuments.value)
      } catch (error) {
        console.error('Error exporting to Excel:', error)
      } finally {
        isExporting.value = false
      }
    }
    
    const viewDocument = (document) => {
      router.push(`/sales/${document.id}`)
    }
    
    const editDocument = (document) => {
      router.push(`/sales/${document.id}/edit`)
    }
    
    const issueDocument = async (document) => {
      try {
        await salesStore.updateDocumentStatus(document.id, 'issued')
      } catch (error) {
        console.error('Error issuing document:', error)
      }
    }
    
    const deleteDocument = async (document) => {
      if (confirm(`آیا از حذف سند ${document.documentNumber} اطمینان دارید؟`)) {
        try {
          await salesStore.deleteDocument(document.id)
        } catch (error) {
          console.error('Error deleting document:', error)
        }
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
    watch(() => filteredDocuments.value.length, () => {
      if (currentPage.value > totalPages.value) {
        currentPage.value = Math.max(1, totalPages.value)
      }
    })
    
    // Lifecycle
    onMounted(async () => {
      isLoading.value = true
      try {
        await salesStore.loadDocuments()
      } catch (error) {
        console.error('Error loading documents:', error)
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
      documents,
      totalDocuments,
      filteredDocuments,
      paginatedDocuments,
      totalPages,
      startIndex,
      endIndex,
      visiblePages,
      
      // Methods
      formatDate,
      formatCurrency,
      getStatusLabel,
      getStatusClass,
      getServiceTypeLabel,
      getServiceTypeClass,
      applyFilters,
      clearFilters,
      exportToExcel,
      viewDocument,
      editDocument,
      issueDocument,
      deleteDocument,
      goToPage,
      previousPage,
      nextPage
    }
  }
}
</script>

<style scoped>
/* Custom styles for sales documents */
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