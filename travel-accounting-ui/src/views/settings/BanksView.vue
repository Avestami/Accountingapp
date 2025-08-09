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
              کد SWIFT
            </th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
              آدرس
            </th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
              تلفن
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
              <div class="text-sm text-gray-500">{{ bank.website || '-' }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ bank.swiftCode || '-' }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ bank.address || '-' }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ bank.phone || '-' }}
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

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useBankStore } from '@/stores/bankStore'

const bankStore = useBankStore()

// State
const searchQuery = ref('')
const statusFilter = ref('')

// Load data on component mount
onMounted(async () => {
  await bankStore.getBanks()
})

// Computed properties
const filteredBanks = computed(() => {
  let filtered = bankStore.banks
  
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(bank => 
      bank.name.toLowerCase().includes(query) ||
      (bank.swiftCode && bank.swiftCode.toLowerCase().includes(query)) ||
      (bank.address && bank.address.toLowerCase().includes(query))
    )
  }
  
  if (statusFilter.value !== '') {
    const isActive = statusFilter.value === 'true'
    filtered = filtered.filter(bank => bank.isActive === isActive)
  }
  
  return filtered
})

// Methods
function getStatusClass(isActive) {
  return {
    'bg-green-100 text-green-800': isActive,
    'bg-red-100 text-red-800': !isActive
  }
}

async function createBank() {
  const name = prompt('نام بانک:')
  if (!name) return
  
  const swiftCode = prompt('کد SWIFT (اختیاری):')
  const address = prompt('آدرس (اختیاری):')
  const phone = prompt('تلفن (اختیاری):')
  const website = prompt('وب‌سایت (اختیاری):')
  
  try {
    await bankStore.createBank({
      name,
      swiftCode: swiftCode || null,
      address: address || null,
      phone: phone || null,
      website: website || null,
      isActive: true
    })
    alert('بانک با موفقیت ایجاد شد')
  } catch (error) {
    alert('خطا در ایجاد بانک: ' + (error.response?.data?.message || error.message))
  }
}

async function editBank(bank) {
  const name = prompt('نام بانک:', bank.name)
  if (name === null) return
  
  const swiftCode = prompt('کد SWIFT:', bank.swiftCode || '')
  if (swiftCode === null) return
  
  const address = prompt('آدرس:', bank.address || '')
  if (address === null) return
  
  const phone = prompt('تلفن:', bank.phone || '')
  if (phone === null) return
  
  const website = prompt('وب‌سایت:', bank.website || '')
  if (website === null) return
  
  const isActive = confirm('آیا بانک فعال باشد؟')
  
  try {
    await bankStore.updateBank(bank.id, {
      name,
      swiftCode: swiftCode || null,
      address: address || null,
      phone: phone || null,
      website: website || null,
      isActive
    })
    alert('بانک با موفقیت به‌روزرسانی شد')
  } catch (error) {
    alert('خطا در به‌روزرسانی بانک: ' + (error.response?.data?.message || error.message))
  }
}

async function deleteBank(id) {
  if (!confirm('آیا از حذف این بانک اطمینان دارید؟')) {
    return
  }
  
  try {
    await bankStore.deleteBank(id)
    alert('بانک با موفقیت حذف شد')
  } catch (error) {
    alert('خطا در حذف بانک: ' + (error.response?.data?.message || error.message))
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