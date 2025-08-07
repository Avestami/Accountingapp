<template>
  <div class="banks-view">
    <div class="page-header">
      <h1 class="text-2xl font-bold text-gray-900">مدیریت بانک‌ها</h1>
      <button @click="createBank" class="btn-primary">
        <i class="fas fa-plus mr-2"></i>
        افزودن بانک
      </button>
    </div>

    <!-- Search and Filter -->
    <div class="bg-white rounded-lg shadow p-6 mb-6">
      <div class="flex flex-col md:flex-row gap-4">
        <div class="flex-1">
          <input
            v-model="searchQuery"
            type="text"
            placeholder="جستجوی بانک‌ها..."
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent"
          />
        </div>
        <div class="flex gap-2">
          <select v-model="statusFilter" class="px-4 py-2 border border-gray-300 rounded-lg">
            <option value="">همه وضعیت‌ها</option>
            <option value="true">فعال</option>
            <option value="false">غیرفعال</option>
          </select>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="settingsStore.loading" class="flex justify-center items-center py-12">
      <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600"></div>
      <span class="mr-3 text-gray-600">در حال بارگذاری...</span>
    </div>

    <!-- Banks Table -->
    <div v-else class="bg-white rounded-lg shadow overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
              نام بانک
            </th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
              شماره حساب
            </th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
              شبا
            </th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
              ارز
            </th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
              موجودی
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
          <tr v-for="bank in filteredBanks" :key="bank.id">
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm font-medium text-gray-900">{{ bank.name }}</div>
              <div class="text-sm text-gray-500">{{ bank.branch }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ bank.accountNumber }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ bank.iban }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ bank.currency }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ formatCurrency(bank.balance, bank.currency) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span :class="getStatusClass(bank.isActive)" class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full">
                {{ bank.isActive ? 'فعال' : 'غیرفعال' }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
              <button @click="editBank(bank)" class="text-primary-600 hover:text-primary-900 ml-3">
                ویرایش
              </button>
              <button @click="deleteBank(bank.id)" class="text-red-600 hover:text-red-900">
                حذف
              </button>
            </td>
          </tr>
        </tbody>
      </table>
      
      <!-- Empty State -->
      <div v-if="filteredBanks.length === 0" class="text-center py-12">
        <i class="fas fa-university text-4xl text-gray-400 mb-4"></i>
        <p class="text-gray-500">هیچ بانکی یافت نشد</p>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue'
import { useSettingsStore } from '@/stores/settings'

export default {
  name: 'BanksView',
  setup() {
    const settingsStore = useSettingsStore()
    const searchQuery = ref('')
    const statusFilter = ref('')
    
    onMounted(() => {
      settingsStore.loadAllSettings()
    })

    const filteredBanks = computed(() => {
      let filtered = settingsStore.banks
      
      if (searchQuery.value) {
        filtered = filtered.filter(bank => 
          bank.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
          bank.accountNumber.includes(searchQuery.value) ||
          bank.iban.includes(searchQuery.value)
        )
      }
      
      if (statusFilter.value !== '') {
        const isActive = statusFilter.value === 'true'
        filtered = filtered.filter(bank => bank.isActive === isActive)
      }
      
      return filtered
    })

    const formatCurrency = (amount, currency) => {
      return new Intl.NumberFormat('fa-IR', {
        style: 'currency',
        currency: currency
      }).format(amount)
    }

    const getStatusClass = (isActive) => {
      return isActive 
        ? 'bg-green-100 text-green-800'
        : 'bg-red-100 text-red-800'
    }

    const createBank = async () => {
      const name = prompt('نام بانک را وارد کنید:')
      if (name) {
        const accountNumber = prompt('شماره حساب را وارد کنید:')
        if (accountNumber) {
          const iban = prompt('شماره شبا را وارد کنید:')
          if (iban) {
            const currency = prompt('نوع ارز را وارد کنید (IRR, USD, EUR):', 'IRR')
            if (currency) {
              const balance = parseFloat(prompt('موجودی اولیه را وارد کنید:', '0'))
              if (!isNaN(balance)) {
                await settingsStore.createBank({
                  name,
                  accountNumber,
                  iban,
                  currency,
                  balance,
                  branch: 'شعبه اصلی',
                  isActive: true
                })
                alert('بانک با موفقیت ایجاد شد')
              }
            }
          }
        }
      }
    }

    const editBank = async (bank) => {
      const name = prompt('نام بانک را ویرایش کنید:', bank.name)
      if (name !== null) {
        const accountNumber = prompt('شماره حساب را ویرایش کنید:', bank.accountNumber)
        if (accountNumber !== null) {
          const iban = prompt('شماره شبا را ویرایش کنید:', bank.iban)
          if (iban !== null) {
            const currency = prompt('نوع ارز را ویرایش کنید:', bank.currency)
            if (currency !== null) {
              const balance = parseFloat(prompt('موجودی را ویرایش کنید:', bank.balance.toString()))
              if (!isNaN(balance)) {
                await settingsStore.updateBank(bank.id, {
                  ...bank,
                  name,
                  accountNumber,
                  iban,
                  currency,
                  balance
                })
                alert('بانک با موفقیت ویرایش شد')
              }
            }
          }
        }
      }
    }

    const deleteBank = async (bankId) => {
      if (confirm('آیا از حذف این بانک اطمینان دارید؟')) {
        await settingsStore.deleteBank(bankId)
        alert('بانک با موفقیت حذف شد')
      }
    }

    return {
      settingsStore,
      searchQuery,
      statusFilter,
      filteredBanks,
      formatCurrency,
      getStatusClass,
      createBank,
      editBank,
      deleteBank
    }
  }
}
</script>

<style scoped>
.banks-view {
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