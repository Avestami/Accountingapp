<template>
  <div class="min-h-screen bg-gray-50 rtl">
    <div class="container mx-auto px-4 py-8">
      <div class="bg-white rounded-lg shadow-sm p-6">
        <div class="flex justify-between items-center mb-6">
          <h1 class="text-2xl font-bold text-gray-900">انتقال وجه</h1>
          <AppButton @click="openCreateModal">
            انتقال جدید
          </AppButton>
        </div>

        <!-- Stats -->
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
          <div class="bg-blue-50 p-4 rounded-lg">
            <h3 class="text-sm font-medium text-blue-800">کل انتقالات ماه جاری</h3>
            <p class="text-2xl font-bold text-blue-900 mt-2">۸,۲۰۰,۰۰۰ ریال</p>
          </div>
          <div class="bg-green-50 p-4 rounded-lg">
            <h3 class="text-sm font-medium text-green-800">انتقالات امروز</h3>
            <p class="text-2xl font-bold text-green-900 mt-2">۹۵۰,۰۰۰ ریال</p>
          </div>
          <div class="bg-purple-50 p-4 rounded-lg">
            <h3 class="text-sm font-medium text-purple-800">تعداد تراکنش‌ها</h3>
            <p class="text-2xl font-bold text-purple-900 mt-2">۱۸</p>
          </div>
        </div>

        <!-- Filters -->
        <div class="mb-6 bg-gray-50 p-4 rounded-lg">
          <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">جستجو</label>
              <input
                type="text"
                v-model="filters.search"
                placeholder="جستجو..."
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">وضعیت</label>
              <select
                v-model="filters.status"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value="">همه</option>
                <option value="pending">در انتظار</option>
                <option value="completed">تکمیل شده</option>
                <option value="cancelled">لغو شده</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">از تاریخ</label>
              <input
                type="date"
                v-model="filters.startDate"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">تا تاریخ</label>
              <input
                type="date"
                v-model="filters.endDate"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </div>
          <div class="flex justify-end mt-4">
            <AppButton variant="secondary" @click="resetFilters" class="ml-2">
              پاک کردن فیلترها
            </AppButton>
            <AppButton @click="applyFilters">
              اعمال فیلترها
            </AppButton>
          </div>
        </div>

        <!-- Loading State -->
        <div v-if="loading" class="flex justify-center py-8">
          <AppSpinner />
        </div>

        <!-- Table -->
        <div v-else class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  شماره
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  تاریخ
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  از حساب
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  به حساب
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  مبلغ (ریال)
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
              <tr v-for="transfer in transfers" :key="transfer.id">
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ transfer.id }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ transfer.date }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ transfer.fromAccount }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ transfer.toAccount }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                  {{ formatCurrency(transfer.amount) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  <span :class="getStatusClass(transfer.status)" class="px-2 py-1 rounded-full text-xs font-medium">
                    {{ getStatusLabel(transfer.status) }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  <div class="flex space-x-2 space-x-reverse">
                    <button @click="viewTransfer(transfer.id)" class="text-blue-600 hover:text-blue-900">
                      <span class="sr-only">مشاهده</span>
                      <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                      </svg>
                    </button>
                    <button 
                      v-if="transfer.status === 'pending'"
                      @click="openEditModal(transfer)" 
                      class="text-indigo-600 hover:text-indigo-900"
                    >
                      <span class="sr-only">ویرایش</span>
                      <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                      </svg>
                    </button>
                    <button 
                      v-if="transfer.status === 'pending'"
                      @click="confirmTransfer(transfer.id)" 
                      class="text-green-600 hover:text-green-900"
                    >
                      <span class="sr-only">تأیید</span>
                      <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                      </svg>
                    </button>
                    <button 
                      v-if="transfer.status === 'pending'"
                      @click="cancelTransfer(transfer.id)" 
                      class="text-red-600 hover:text-red-900"
                    >
                      <span class="sr-only">لغو</span>
                      <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                      </svg>
                    </button>
                  </div>
                </td>
              </tr>
              <tr v-if="transfers.length === 0">
                <td colspan="7" class="px-6 py-4 text-center text-sm text-gray-500">
                  هیچ انتقالی یافت نشد.
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Pagination -->
        <div class="flex justify-between items-center mt-6">
          <div class="text-sm text-gray-700">
            نمایش <span class="font-medium">{{ pagination.from }}</span> تا <span class="font-medium">{{ pagination.to }}</span> از <span class="font-medium">{{ pagination.total }}</span> نتیجه
          </div>
          <div class="flex space-x-1 space-x-reverse">
            <button
              @click="changePage(pagination.currentPage - 1)"
              :disabled="pagination.currentPage === 1"
              :class="pagination.currentPage === 1 ? 'opacity-50 cursor-not-allowed' : ''"
              class="px-3 py-1 border border-gray-300 rounded-md text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
            >
              قبلی
            </button>
            <button
              v-for="page in pagination.totalPages"
              :key="page"
              @click="changePage(page)"
              :class="page === pagination.currentPage ? 'bg-blue-50 text-blue-600 border-blue-500' : 'bg-white text-gray-700 border-gray-300 hover:bg-gray-50'"
              class="px-3 py-1 border rounded-md text-sm font-medium"
            >
              {{ page }}
            </button>
            <button
              @click="changePage(pagination.currentPage + 1)"
              :disabled="pagination.currentPage === pagination.totalPages"
              :class="pagination.currentPage === pagination.totalPages ? 'opacity-50 cursor-not-allowed' : ''"
              class="px-3 py-1 border border-gray-300 rounded-md text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
            >
              بعدی
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Transfer Modal -->
    <TransferModal
      :isOpen="showModal"
      :transfer="selectedTransfer"
      @close="closeModal"
      @save="handleSaveTransfer"
    />
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useFinanceStore } from '../../stores/finance'
import { financeApi } from '../../services/api'
import AppButton from '@/components/AppButton.vue'
import AppSpinner from '@/components/AppSpinner.vue'
import TransferModal from '@/components/finance/TransferModal.vue'

export default {
  name: 'TransferView',
  components: {
    AppButton,
    AppSpinner,
    TransferModal
  },
  setup() {
    const router = useRouter()
    const financeStore = useFinanceStore()
    
    // Reactive data
    const loading = ref(false)
    const error = ref(null)
    const showModal = ref(false)
    const selectedTransfer = ref(null)
    const filters = ref({
      search: '',
      status: '',
      startDate: '',
      endDate: ''
    })
    
    const pagination = ref({
      page: 1,
      pageSize: 10,
      total: 0
    })
    
    // Computed properties
    const transfers = computed(() => financeStore.transfers)
    const totalTransfers = computed(() => financeStore.totalTransfers)
    
    // Methods
    const loadTransfers = async () => {
      loading.value = true
      error.value = null
      
      try {
        const params = {
          page: pagination.value.page,
          pageSize: pagination.value.pageSize,
          search: filters.value.search,
          fromDate: filters.value.startDate,
          toDate: filters.value.endDate
        }
        
        await financeStore.loadTransfers(params)
        pagination.value.total = totalTransfers.value
      } catch (err) {
        error.value = 'خطا در بارگذاری انتقالات'
        console.error('Error loading transfers:', err)
        
        // Fallback to mock data
        const mockTransfers = [
          {
            id: 1,
            date: '1403/05/15',
            fromAccount: 'حساب جاری بانک ملی',
            toAccount: 'حساب پس‌انداز بانک پاسارگاد',
            amount: 2500000,
            status: 'completed',
            description: 'انتقال وجه برای پرداخت'
          },
          {
            id: 2,
            date: '1403/05/14',
            fromAccount: 'حساب پس‌انداز بانک پاسارگاد',
            toAccount: 'حساب جاری بانک صادرات',
            amount: 1200000,
            status: 'pending',
            description: 'انتقال وجه برای خرید'
          },
          {
            id: 3,
            date: '1403/05/13',
            fromAccount: 'حساب جاری بانک صادرات',
            toAccount: 'حساب جاری بانک ملی',
            amount: 800000,
            status: 'completed',
            description: 'انتقال وجه داخلی'
          }
        ]
        financeStore.transfers = mockTransfers
        financeStore.totalTransfers = mockTransfers.length
      } finally {
        loading.value = false
      }
    }
    
    const createNewTransfer = () => {
      selectedTransfer.value = null
      showModal.value = true
    }
    
    const openCreateModal = () => {
      selectedTransfer.value = null
      showModal.value = true
    }
    
    const openEditModal = (transfer) => {
      selectedTransfer.value = { ...transfer }
      showModal.value = true
    }
    
    const closeModal = () => {
      showModal.value = false
      selectedTransfer.value = null
    }
    
    const handleSaveTransfer = async (transferData) => {
      try {
        if (selectedTransfer.value) {
          // Update existing transfer
          const index = financeStore.transfers.findIndex(transfer => transfer.id === selectedTransfer.value.id)
          if (index !== -1) {
            financeStore.transfers[index] = { ...transferData, id: selectedTransfer.value.id }
          }
        } else {
          // Create new transfer
          const newTransfer = {
            ...transferData,
            id: Date.now() // Temporary ID
          }
          financeStore.transfers.unshift(newTransfer)
          financeStore.totalTransfers++
        }
        closeModal()
      } catch (err) {
        console.error('Error saving transfer:', err)
      }
    }
    
    const viewTransfer = (id) => {
      router.push(`/finance/transfers/${id}`)
    }
    
    const editTransfer = (id) => {
      const transfer = financeStore.transfers.find(transfer => transfer.id === id)
      if (transfer) {
        openEditModal(transfer)
      }
    }
    
    const confirmTransfer = async (id) => {
      if (confirm('آیا از تأیید این انتقال اطمینان دارید؟')) {
        loading.value = true
        try {
          await financeApi.updateTransferStatus(id, { 
            status: 'Completed',
            notes: 'Transfer confirmed by user'
          })
          
          // Update local state
          const transfer = financeStore.transfers.find(t => t.id === id)
          if (transfer) {
            transfer.status = 'completed'
          }
        } catch (err) {
          error.value = 'خطا در تأیید انتقال'
          console.error('Error confirming transfer:', err)
        } finally {
          loading.value = false
        }
      }
    }
    
    const cancelTransfer = async (id) => {
      if (confirm('آیا از لغو این انتقال اطمینان دارید؟')) {
        loading.value = true
        try {
          await financeApi.updateTransferStatus(id, { 
            status: 'Cancelled',
            notes: 'Transfer cancelled by user'
          })
          
          // Update local state
          const transfer = financeStore.transfers.find(t => t.id === id)
          if (transfer) {
            transfer.status = 'cancelled'
          }
        } catch (err) {
          error.value = 'خطا در لغو انتقال'
          console.error('Error cancelling transfer:', err)
        } finally {
          loading.value = false
        }
      }
    }
    
    const applyFilters = () => {
      pagination.value.page = 1
      loadTransfers()
    }
    
    const resetFilters = () => {
      filters.value = {
        search: '',
        status: '',
        startDate: '',
        endDate: ''
      }
      pagination.value.page = 1
      loadTransfers()
    }
    
    const changePage = (page) => {
      if (page >= 1 && page <= Math.ceil(pagination.value.total / pagination.value.pageSize)) {
        pagination.value.page = page
        loadTransfers()
      }
    }
    
    const formatCurrency = (amount) => {
      return new Intl.NumberFormat('fa-IR').format(amount)
    }
    
    const getStatusClass = (status) => {
      const classes = {
        pending: 'bg-yellow-100 text-yellow-800',
        completed: 'bg-green-100 text-green-800',
        cancelled: 'bg-red-100 text-red-800'
      }
      return classes[status] || 'bg-gray-100 text-gray-800'
    }
    
    const getStatusLabel = (status) => {
      const labels = {
        pending: 'در انتظار',
        completed: 'تکمیل شده',
        cancelled: 'لغو شده'
      }
      return labels[status] || 'نامشخص'
    }
    
    // Lifecycle
    onMounted(() => {
      loadTransfers()
    })
    
    return {
      loading,
      error,
      showModal,
      selectedTransfer,
      filters,
      pagination,
      transfers,
      totalTransfers,
      loadTransfers,
      openCreateModal,
      openEditModal,
      closeModal,
      handleSaveTransfer,
      createNewTransfer,
      viewTransfer,
      editTransfer,
      confirmTransfer,
      cancelTransfer,
      applyFilters,
      resetFilters,
      changePage,
      formatCurrency,
      getStatusClass,
      getStatusLabel
    }
  }
}
</script>