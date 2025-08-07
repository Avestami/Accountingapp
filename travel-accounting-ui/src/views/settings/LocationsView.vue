<template>
  <div class="locations-view">
    <div class="page-header">
      <h1 class="text-2xl font-bold text-gray-900">مدیریت مکان‌ها</h1>
      <button @click="createLocation" class="btn-primary">
        <i class="fas fa-plus mr-2"></i>
        افزودن مکان
      </button>
    </div>

    <!-- Search and Filter -->
    <div class="bg-white rounded-lg shadow p-6 mb-6">
      <div class="flex flex-col md:flex-row gap-4">
        <div class="flex-1">
          <input
            v-model="searchQuery"
            type="text"
            placeholder="جستجوی مکان‌ها..."
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent"
          />
        </div>
        <div class="flex gap-2">
          <select v-model="typeFilter" class="px-4 py-2 border border-gray-300 rounded-lg">
            <option value="">همه انواع</option>
            <option value="country">کشور</option>
            <option value="city">شهر</option>
            <option value="airport">فرودگاه</option>
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
    <div v-if="settingsStore.loading" class="bg-white rounded-lg shadow p-8 text-center">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600 mx-auto mb-4"></div>
      <p class="text-gray-600">در حال بارگذاری...</p>
    </div>

    <!-- Empty State -->
    <div v-else-if="filteredLocations.length === 0" class="bg-white rounded-lg shadow p-8 text-center">
      <i class="fas fa-map-marker-alt text-4xl text-gray-400 mb-4"></i>
      <h3 class="text-lg font-medium text-gray-900 mb-2">مکانی یافت نشد</h3>
      <p class="text-gray-600">هیچ مکانی با فیلترهای انتخابی یافت نشد.</p>
    </div>

    <!-- Locations Table -->
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
              کد
            </th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
              والد
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
          <tr v-for="location in filteredLocations" :key="location.id">
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm font-medium text-gray-900">{{ location.name }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span :class="getTypeClass(location.type)" class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full">
                {{ getTypeLabel(location.type) }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ location.code }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ location.parentName || '-' }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span :class="getStatusClass(location.isActive)" class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full">
                {{ location.isActive ? 'فعال' : 'غیرفعال' }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
              <button @click="editLocation(location)" class="text-primary-600 hover:text-primary-900 mr-3">
                ویرایش
              </button>
              <button @click="deleteLocation(location.id)" class="text-red-600 hover:text-red-900">
                حذف
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue'
import { useSettingsStore } from '@/stores/settings'

export default {
  name: 'LocationsView',
  setup() {
    const settingsStore = useSettingsStore()
    const searchQuery = ref('')
    const typeFilter = ref('')
    const statusFilter = ref('')
    
    onMounted(() => {
      settingsStore.loadAllSettings()
    })

    const filteredLocations = computed(() => {
      let filtered = settingsStore.locations
      
      if (searchQuery.value) {
        filtered = filtered.filter(location => 
          location.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
          location.code.toLowerCase().includes(searchQuery.value.toLowerCase())
        )
      }
      
      if (typeFilter.value) {
        filtered = filtered.filter(location => location.type === typeFilter.value)
      }
      
      if (statusFilter.value !== '') {
        const isActive = statusFilter.value === 'true'
        filtered = filtered.filter(location => location.isActive === isActive)
      }
      
      return filtered
    })

    const getTypeClass = (type) => {
      return {
        'bg-blue-100 text-blue-800': type === 'country',
        'bg-green-100 text-green-800': type === 'city',
        'bg-purple-100 text-purple-800': type === 'airport'
      }
    }

    const getTypeLabel = (type) => {
      const labels = {
        'country': 'کشور',
        'city': 'شهر',
        'airport': 'فرودگاه'
      }
      return labels[type] || type
    }

    const getStatusClass = (isActive) => {
      return isActive 
        ? 'bg-green-100 text-green-800'
        : 'bg-red-100 text-red-800'
    }

    const createLocation = async () => {
      const name = prompt('نام مکان را وارد کنید:')
      if (name) {
        const code = prompt('کد مکان را وارد کنید:')
        if (code) {
          const type = prompt('نوع مکان را انتخاب کنید (country/city/airport):', 'city')
          if (type === 'country' || type === 'city' || type === 'airport') {
            try {
              const newLocation = {
                name,
                code,
                type,
                parentId: null,
                parentName: null,
                isActive: true
              }
              
              await settingsStore.createLocation(newLocation)
              alert('مکان با موفقیت ایجاد شد')
            } catch (error) {
              alert('خطا در ایجاد مکان: ' + error.message)
            }
          } else {
            alert('نوع مکان باید country، city یا airport باشد')
          }
        }
      }
    }

    const editLocation = async (location) => {
      const name = prompt('نام مکان را ویرایش کنید:', location.name)
      if (name !== null) {
        const code = prompt('کد مکان را ویرایش کنید:', location.code)
        if (code !== null) {
          const type = prompt('نوع مکان را ویرایش کنید:', location.type)
          if (type !== null && (type === 'country' || type === 'city' || type === 'airport')) {
            try {
              const updatedData = {
                name,
                code,
                type
              }
              
              await settingsStore.updateLocation(location.id, updatedData)
              alert('مکان با موفقیت ویرایش شد')
            } catch (error) {
              alert('خطا در ویرایش مکان: ' + error.message)
            }
          } else if (type !== null) {
            alert('نوع مکان باید country، city یا airport باشد')
          }
        }
      }
    }

    const deleteLocation = async (location) => {
      if (confirm(`آیا از حذف مکان "${location.name}" اطمینان دارید؟`)) {
        try {
          await settingsStore.deleteLocation(location.id)
          alert('مکان با موفقیت حذف شد')
        } catch (error) {
          alert('خطا در حذف مکان: ' + error.message)
        }
      }
    }

    return {
      settingsStore,
      searchQuery,
      typeFilter,
      statusFilter,
      filteredLocations,
      getTypeClass,
      getTypeLabel,
      getStatusClass,
      createLocation,
      editLocation,
      deleteLocation
    }
  }
}
</script>

<style scoped>
.locations-view {
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