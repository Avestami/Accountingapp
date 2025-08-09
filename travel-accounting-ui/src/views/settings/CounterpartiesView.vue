<template>
  <div class="counterparties-view">
    <div class="page-header">
      <h1 class="text-2xl font-bold text-gray-900">مدیریت طرف‌های حساب</h1>
      <button @click="createCounterparty" class="btn-primary">
        <i class="fas fa-plus mr-2"></i>
        افزودن طرف حساب
      </button>
    </div>

    <!-- Search and Filter -->
    <div class="bg-white rounded-lg shadow p-6 mb-6">
      <div class="flex flex-col md:flex-row gap-4">
        <div class="flex-1">
          <input
            v-model="searchQuery"
            type="text"
            placeholder="جستجوی طرف‌های حساب..."
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent"
          />
        </div>
        <div class="flex gap-2">
          <select v-model="typeFilter" class="px-4 py-2 border border-gray-300 rounded-lg">
            <option value="">همه انواع</option>
            <option value="buyer">خریدار</option>
            <option value="supplier">تأمین‌کننده</option>
          </select>
          <select v-model="statusFilter" class="px-4 py-2 border border-gray-300 rounded-lg">
            <option value="">همه وضعیت‌ها</option>
            <option value="true">فعال</option>
            <option value="false">غیرفعال</option>
          </select>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="counterpartyStore.loading" class="flex justify-center items-center py-12">
      <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600"></div>
      <span class="mr-3 text-gray-600">در حال بارگذاری...</span>
    </div>

    <!-- Counterparties Table -->
    <div v-else class="bg-white rounded-lg shadow overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
              نام
            </th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
              نوع
            </th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
              اطلاعات تماس
            </th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
              مانده
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
          <tr v-for="counterparty in filteredCounterparties" :key="counterparty.id">
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm font-medium text-gray-900">{{ counterparty.name }}</div>
              <div class="text-sm text-gray-500">{{ counterparty.code }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span :class="getTypeClass(counterparty.type)" class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full">
                {{ getTypeLabel(counterparty.type) }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm text-gray-900">{{ counterparty.email }}</div>
              <div class="text-sm text-gray-500">{{ counterparty.phone }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm">
              <span :class="getBalanceClass(counterparty.balance)">
                {{ formatCurrency(counterparty.balance) }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span :class="getStatusClass(counterparty.isActive)" class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full">
                {{ counterparty.isActive ? 'فعال' : 'غیرفعال' }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
              <button @click="editCounterparty(counterparty)" class="text-primary-600 hover:text-primary-900 ml-3">
                ویرایش
              </button>
              <button @click="deleteCounterparty(counterparty.id)" class="text-red-600 hover:text-red-900">
                حذف
              </button>
            </td>
          </tr>
        </tbody>
      </table>
      
      <!-- Empty State -->
      <div v-if="filteredCounterparties.length === 0" class="text-center py-12">
        <i class="fas fa-users text-4xl text-gray-400 mb-4"></i>
        <p class="text-gray-500">هیچ طرف حسابی یافت نشد</p>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue'
import { useCounterpartyStore } from '../../stores/counterpartyStore'

export default {
  name: 'CounterpartiesView',
  setup() {
    const counterpartyStore = useCounterpartyStore()
    
    // State
    const searchQuery = ref('')
    const typeFilter = ref('')
    const statusFilter = ref('')

    // Computed
    const filteredCounterparties = computed(() => {
      let filtered = counterpartyStore.counterparties
      
      if (searchQuery.value) {
        const query = searchQuery.value.toLowerCase()
        filtered = filtered.filter(counterparty => 
          counterparty.name.toLowerCase().includes(query) ||
          counterparty.code?.toLowerCase().includes(query) ||
          counterparty.email?.toLowerCase().includes(query)
        )
      }
      
      if (typeFilter.value) {
        filtered = filtered.filter(counterparty => counterparty.type === typeFilter.value)
      }
      
      if (statusFilter.value) {
        const isActive = statusFilter.value === 'active'
        filtered = filtered.filter(counterparty => counterparty.isActive === isActive)
      }
      
      return filtered
    })

    const getTypeClass = (type) => {
      return {
        'bg-blue-100 text-blue-800': type === 'buyer',
        'bg-green-100 text-green-800': type === 'supplier'
      }
    }

    const getTypeLabel = (type) => {
      const labels = {
        'buyer': 'خریدار',
        'supplier': 'تامین‌کننده'
      }
      return labels[type] || type
    }

    const getStatusClass = (isActive) => {
      return isActive 
        ? 'bg-green-100 text-green-800' 
        : 'bg-red-100 text-red-800'
    }

    const getBalanceClass = (balance) => {
      return {
        'text-green-600': balance > 0,
        'text-red-600': balance < 0,
        'text-gray-600': balance === 0
      }
    }

    const formatCurrency = (amount) => {
      return new Intl.NumberFormat('fa-IR', {
        style: 'currency',
        currency: 'IRR',
        minimumFractionDigits: 0
      }).format(amount)
    }

    // Methods
    const createCounterparty = async () => {
      const name = prompt('نام طرف حساب را وارد کنید:')
      if (name && name.trim()) {
        const code = prompt('کد طرف حساب را وارد کنید:')
        if (code && code.trim()) {
          const type = prompt('نوع طرف حساب را انتخاب کنید (buyer/supplier):', 'buyer')
          if (type && ['buyer', 'supplier'].includes(type.toLowerCase())) {
            const email = prompt('ایمیل را وارد کنید (اختیاری):')
            const phone = prompt('شماره تلفن را وارد کنید (اختیاری):')
            const address = prompt('آدرس را وارد کنید (اختیاری):')
            
            try {
              await counterpartyStore.createCounterparty({
                name: name.trim(),
                code: code.trim().toUpperCase(),
                type: type.toLowerCase(),
                email: email?.trim() || null,
                phone: phone?.trim() || null,
                address: address?.trim() || null,
                isActive: true
              })
              alert('طرف حساب با موفقیت ایجاد شد')
            } catch (error) {
              alert('خطا در ایجاد طرف حساب: ' + error.message)
            }
          } else {
            alert('نوع طرف حساب باید buyer یا supplier باشد')
          }
        }
      }
    }

    const editCounterparty = async (counterparty) => {
      const name = prompt('نام طرف حساب را ویرایش کنید:', counterparty.name)
      if (name && name.trim()) {
        const code = prompt('کد طرف حساب را ویرایش کنید:', counterparty.code)
        if (code && code.trim()) {
          const type = prompt('نوع طرف حساب را ویرایش کنید:', counterparty.type)
          if (type && ['buyer', 'supplier'].includes(type.toLowerCase())) {
            const email = prompt('ایمیل را ویرایش کنید:', counterparty.email || '')
            const phone = prompt('شماره تلفن را ویرایش کنید:', counterparty.phone || '')
            const address = prompt('آدرس را ویرایش کنید:', counterparty.address || '')
            const isActive = confirm('آیا این طرف حساب فعال باشد؟')
            
            try {
              await counterpartyStore.updateCounterparty(counterparty.id, {
                ...counterparty,
                name: name.trim(),
                code: code.trim().toUpperCase(),
                type: type.toLowerCase(),
                email: email?.trim() || null,
                phone: phone?.trim() || null,
                address: address?.trim() || null,
                isActive
              })
              alert('طرف حساب با موفقیت ویرایش شد')
            } catch (error) {
              alert('خطا در ویرایش طرف حساب: ' + error.message)
            }
          } else {
            alert('نوع طرف حساب باید buyer یا supplier باشد')
          }
        }
      }
    }

    const deleteCounterparty = async (counterpartyId) => {
      if (confirm('آیا از حذف این طرف حساب اطمینان دارید؟')) {
        try {
          await counterpartyStore.deleteCounterparty(counterpartyId)
          alert('طرف حساب با موفقیت حذف شد')
        } catch (error) {
          alert('خطا در حذف طرف حساب: ' + error.message)
        }
      }
    }

    // Lifecycle
    onMounted(() => {
      counterpartyStore.getCounterparties()
    })

    return {
      counterpartyStore,
      searchQuery,
      typeFilter,
      statusFilter,
      filteredCounterparties,
      getTypeClass,
      getTypeLabel,
      getStatusClass,
      getBalanceClass,
      formatCurrency,
      createCounterparty,
      editCounterparty,
      deleteCounterparty
    }
  }
}
</script>

<style scoped>
.counterparties-view {
  padding: 24px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.btn-primary {
  background-color: #2563eb;
  color: white;
  font-weight: 500;
  padding: 0.5rem 1rem;
  border-radius: 0.5rem;
  transition: background-color 0.2s ease-in-out;
}

.btn-primary:hover {
  background-color: #1d4ed8;
}
</style>