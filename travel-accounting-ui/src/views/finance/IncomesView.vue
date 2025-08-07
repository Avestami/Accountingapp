<template>
  <div class="min-h-screen bg-gray-50 rtl">
    <div class="container mx-auto px-4 py-8">
      <div class="bg-white rounded-lg shadow-sm p-6">
        <div class="flex justify-between items-center mb-6">
          <h1 class="text-2xl font-bold text-gray-900">درآمدها</h1>
          <AppButton @click="createNewIncome">
            ثبت درآمد جدید
          </AppButton>
        </div>

        <!-- Stats -->
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
          <div class="bg-blue-50 p-4 rounded-lg">
            <h3 class="text-sm font-medium text-blue-800">کل درآمد ماه جاری</h3>
            <p class="text-2xl font-bold text-blue-900 mt-2">۱۲,۵۰۰,۰۰۰ ریال</p>
          </div>
          <div class="bg-green-50 p-4 rounded-lg">
            <h3 class="text-sm font-medium text-green-800">درآمد امروز</h3>
            <p class="text-2xl font-bold text-green-900 mt-2">۱,۲۰۰,۰۰۰ ریال</p>
          </div>
          <div class="bg-purple-50 p-4 rounded-lg">
            <h3 class="text-sm font-medium text-purple-800">تعداد تراکنش‌های درآمد</h3>
            <p class="text-2xl font-bold text-purple-900 mt-2">۲۵</p>
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
              <label class="block text-sm font-medium text-gray-700 mb-1">نوع درآمد</label>
              <select
                v-model="filters.type"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value="">همه</option>
                <option value="sales">فروش</option>
                <option value="commission">کمیسیون</option>
                <option value="service">خدمات</option>
                <option value="other">سایر</option>
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
                  نوع درآمد
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  شرح
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  مبلغ (ریال)
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  عملیات
                </th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr v-for="income in incomes" :key="income.id">
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ income.id }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ income.date }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  <span :class="getTypeClass(income.type)" class="px-2 py-1 rounded-full text-xs font-medium">
                    {{ getTypeLabel(income.type) }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ income.description }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                  {{ formatCurrency(income.amount) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  <div class="flex space-x-2 space-x-reverse">
                    <button @click="viewIncome(income.id)" class="text-blue-600 hover:text-blue-900">
                      <span class="sr-only">مشاهده</span>
                      <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                      </svg>
                    </button>
                    <button @click="editIncome(income.id)" class="text-indigo-600 hover:text-indigo-900">
                      <span class="sr-only">ویرایش</span>
                      <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                      </svg>
                    </button>
                    <button @click="deleteIncome(income.id)" class="text-red-600 hover:text-red-900">
                      <span class="sr-only">حذف</span>
                      <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                      </svg>
                    </button>
                  </div>
                </td>
              </tr>
              <tr v-if="incomes.length === 0">
                <td colspan="6" class="px-6 py-4 text-center text-sm text-gray-500">
                  هیچ درآمدی یافت نشد.
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
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import AppButton from '@/components/AppButton.vue'
import AppSpinner from '@/components/AppSpinner.vue'

export default {
  name: 'IncomesView',
  components: {
    AppButton,
    AppSpinner
  },
  setup() {
    const router = useRouter()
    const loading = ref(false)
    const incomes = ref([])
    
    const filters = reactive({
      search: '',
      type: '',
      startDate: '',
      endDate: ''
    })
    
    const pagination = reactive({
      currentPage: 1,
      perPage: 10,
      total: 0,
      from: 0,
      to: 0,
      totalPages: 0
    })
    
    const loadIncomes = async () => {
      loading.value = true
      try {
        // Simulate API call
        await new Promise(resolve => setTimeout(resolve, 1000))
        
        // Mock data
        const mockIncomes = [
          {
            id: 1,
            date: '1403/05/15',
            type: 'sales',
            description: 'فروش بلیت هواپیما',
            amount: 3500000
          },
          {
            id: 2,
            date: '1403/05/14',
            type: 'commission',
            description: 'کمیسیون رزرو هتل',
            amount: 1200000
          },
          {
            id: 3,
            date: '1403/05/12',
            type: 'service',
            description: 'خدمات ویزا',
            amount: 2800000
          },
          {
            id: 4,
            date: '1403/05/10',
            type: 'other',
            description: 'مشاوره سفر',
            amount: 500000
          },
          {
            id: 5,
            date: '1403/05/08',
            type: 'sales',
            description: 'فروش تور داخلی',
            amount: 4500000
          }
        ]
        
        // Apply filters (in a real app, this would be done on the server)
        let filteredIncomes = [...mockIncomes]
        
        if (filters.search) {
          const searchTerm = filters.search.toLowerCase()
          filteredIncomes = filteredIncomes.filter(income => 
            income.description.toLowerCase().includes(searchTerm)
          )
        }
        
        if (filters.type) {
          filteredIncomes = filteredIncomes.filter(income => income.type === filters.type)
        }
        
        // Calculate pagination
        pagination.total = filteredIncomes.length
        pagination.totalPages = Math.ceil(pagination.total / pagination.perPage)
        pagination.from = (pagination.currentPage - 1) * pagination.perPage + 1
        pagination.to = Math.min(pagination.from + pagination.perPage - 1, pagination.total)
        
        // Slice for current page
        const start = (pagination.currentPage - 1) * pagination.perPage
        const end = start + pagination.perPage
        incomes.value = filteredIncomes.slice(start, end)
      } catch (error) {
        console.error('Error loading incomes:', error)
        alert('خطا در بارگذاری اطلاعات درآمدها')
      } finally {
        loading.value = false
      }
    }
    
    const resetFilters = () => {
      filters.search = ''
      filters.type = ''
      filters.startDate = ''
      filters.endDate = ''
      pagination.currentPage = 1
      loadIncomes()
    }
    
    const applyFilters = () => {
      pagination.currentPage = 1
      loadIncomes()
    }
    
    const changePage = (page) => {
      if (page < 1 || page > pagination.totalPages) return
      pagination.currentPage = page
      loadIncomes()
    }
    
    const createNewIncome = () => {
      router.push('/finance/incomes/create')
    }
    
    const viewIncome = (id) => {
      router.push(`/finance/incomes/${id}`)
    }
    
    const editIncome = (id) => {
      router.push(`/finance/incomes/${id}/edit`)
    }
    
    const deleteIncome = async (id) => {
      if (confirm('آیا از حذف این درآمد اطمینان دارید؟')) {
        try {
          // Simulate API call
          await new Promise(resolve => setTimeout(resolve, 500))
          
          // Remove from local state
          incomes.value = incomes.value.filter(income => income.id !== id)
          pagination.total--
          
          alert('درآمد با موفقیت حذف شد.')
          
          // Reload if page is now empty
          if (incomes.value.length === 0 && pagination.currentPage > 1) {
            pagination.currentPage--
            loadIncomes()
          }
        } catch (error) {
          console.error('Error deleting income:', error)
          alert('خطا در حذف درآمد')
        }
      }
    }
    
    const formatCurrency = (amount) => {
      return new Intl.NumberFormat('fa-IR').format(amount)
    }
    
    const getTypeLabel = (type) => {
      const labels = {
        sales: 'فروش',
        commission: 'کمیسیون',
        service: 'خدمات',
        other: 'سایر'
      }
      return labels[type] || type
    }
    
    const getTypeClass = (type) => {
      const classes = {
        sales: 'bg-green-100 text-green-800',
        commission: 'bg-blue-100 text-blue-800',
        service: 'bg-purple-100 text-purple-800',
        other: 'bg-gray-100 text-gray-800'
      }
      return classes[type] || 'bg-gray-100 text-gray-800'
    }
    
    onMounted(() => {
      loadIncomes()
    })
    
    return {
      loading,
      incomes,
      filters,
      pagination,
      loadIncomes,
      resetFilters,
      applyFilters,
      changePage,
      createNewIncome,
      viewIncome,
      editIncome,
      deleteIncome,
      formatCurrency,
      getTypeLabel,
      getTypeClass
    }
  }
}
</script>