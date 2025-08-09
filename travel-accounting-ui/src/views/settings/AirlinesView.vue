<template>
  <div class="airlines-view">
    <div class="page-header">
      <h1 class="text-2xl font-bold text-gray-900">مدیریت شرکت‌های هواپیمایی</h1>
      <button @click="createAirline" class="btn-primary">
        <i class="fas fa-plus mr-2"></i>
        افزودن شرکت هواپیمایی
      </button>
    </div>

    <!-- Search and Filter -->
    <div class="bg-white rounded-lg shadow p-6 mb-6">
      <div class="flex flex-col md:flex-row gap-4">
        <div class="flex-1">
          <input
            v-model="searchQuery"
            type="text"
            placeholder="جستجوی شرکت‌های هواپیمایی..."
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
    <div v-if="airlineStore.loading" class="flex justify-center items-center py-12">
      <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600"></div>
      <span class="mr-3 text-gray-600">در حال بارگذاری...</span>
    </div>

    <!-- Airlines Table -->
    <div v-else class="bg-white rounded-lg shadow overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
              شرکت هواپیمایی
            </th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
              کد
            </th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
              کشور
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
          <tr v-for="airline in filteredAirlines" :key="airline.id">
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="flex items-center">
                <div class="ml-4">
                  <div class="text-sm font-medium text-gray-900">{{ airline.name }}</div>
                </div>
              </div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ airline.code }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ airline.country }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span :class="getStatusClass(airline.isActive)" class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full">
                {{ airline.isActive ? 'فعال' : 'غیرفعال' }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
              <button @click="editAirline(airline)" class="text-primary-600 hover:text-primary-900 ml-3">
                ویرایش
              </button>
              <button @click="deleteAirline(airline.id)" class="text-red-600 hover:text-red-900">
                حذف
              </button>
            </td>
          </tr>
        </tbody>
      </table>
      
      <!-- Empty State -->
      <div v-if="filteredAirlines.length === 0" class="text-center py-12">
        <i class="fas fa-plane text-4xl text-gray-400 mb-4"></i>
        <p class="text-gray-500">هیچ شرکت هواپیمایی‌ای یافت نشد</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useAirlineStore } from '@/stores/airlineStore'

const airlineStore = useAirlineStore()

// State
const searchQuery = ref('')
const statusFilter = ref('')

// Load data on component mount
onMounted(async () => {
  await airlineStore.getAirlines()
})

// Computed properties
const filteredAirlines = computed(() => {
  let filtered = airlineStore.airlines
  
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(airline => 
      airline.name.toLowerCase().includes(query) ||
      airline.code.toLowerCase().includes(query) ||
      (airline.country && airline.country.toLowerCase().includes(query))
    )
  }
  
  if (statusFilter.value !== '') {
    const isActive = statusFilter.value === 'true'
    filtered = filtered.filter(airline => airline.isActive === isActive)
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

async function createAirline() {
  const name = prompt('نام شرکت هواپیمایی:')
  if (!name) return
  
  const code = prompt('کد شرکت هواپیمایی (3 حرف):')
  if (!code || code.length !== 3) {
    alert('کد باید دقیقاً 3 حرف باشد')
    return
  }
  
  const country = prompt('کشور (اختیاری):')
  
  try {
    await airlineStore.createAirline({
      name,
      code: code.toUpperCase(),
      country: country || null,
      isActive: true
    })
    alert('شرکت هواپیمایی با موفقیت ایجاد شد')
  } catch (error) {
    alert('خطا در ایجاد شرکت هواپیمایی: ' + (error.response?.data?.message || error.message))
  }
}

async function editAirline(airline) {
  const name = prompt('نام شرکت هواپیمایی:', airline.name)
  if (name === null) return
  
  const code = prompt('کد شرکت هواپیمایی (3 حرف):', airline.code)
  if (code === null) return
  if (code.length !== 3) {
    alert('کد باید دقیقاً 3 حرف باشد')
    return
  }
  
  const country = prompt('کشور:', airline.country || '')
  if (country === null) return
  
  const isActive = confirm('آیا شرکت فعال باشد؟')
  
  try {
    await airlineStore.updateAirline(airline.id, {
      name,
      code: code.toUpperCase(),
      country: country || null,
      isActive
    })
    alert('شرکت هواپیمایی با موفقیت به‌روزرسانی شد')
  } catch (error) {
    alert('خطا در به‌روزرسانی شرکت هواپیمایی: ' + (error.response?.data?.message || error.message))
  }
}

async function deleteAirline(id) {
  if (!confirm('آیا از حذف این شرکت هواپیمایی اطمینان دارید؟')) {
    return
  }
  
  try {
    await airlineStore.deleteAirline(id)
    alert('شرکت هواپیمایی با موفقیت حذف شد')
  } catch (error) {
    alert('خطا در حذف شرکت هواپیمایی: ' + (error.response?.data?.message || error.message))
  }
}
</script>

<style scoped>
.airlines-view {
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