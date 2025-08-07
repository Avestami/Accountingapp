<template>
  <div class="p-6">
    <div class="mb-8">
      <h1 class="text-2xl font-bold text-gray-900 mb-2">مدیریت کاربران</h1>
      <p class="text-gray-600">در این بخش می‌توانید کاربران سیستم را مدیریت کنید.</p>
    </div>

    <!-- Search and Filter Section -->
    <div class="bg-white rounded-lg shadow p-4 mb-6">
      <div class="flex flex-col md:flex-row gap-4 mb-4">
        <div class="flex-1">
          <label for="search" class="block text-sm font-medium text-gray-700 mb-1">جستجو</label>
          <div class="relative">
            <input
              id="search"
              v-model="searchTerm"
              type="text"
              placeholder="جستجو بر اساس نام، نام کاربری یا ایمیل..."
              class="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 pr-10 text-right"
              @input="applyFilters"
            />
            <div class="absolute inset-y-0 right-0 flex items-center pr-3 pointer-events-none">
              <svg class="h-5 w-5 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
            </div>
          </div>
        </div>

        <div class="w-full md:w-48">
          <label for="role-filter" class="block text-sm font-medium text-gray-700 mb-1">نقش</label>
          <select
            id="role-filter"
            v-model="roleFilter"
            class="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 text-right"
            @change="applyFilters"
          >
            <option value="">همه نقش‌ها</option>
            <option value="admin">مدیر سیستم</option>
            <option value="manager">مدیر</option>
            <option value="accountant">حسابدار</option>
            <option value="sales">کارشناس فروش</option>
            <option value="user">کاربر عادی</option>
          </select>
        </div>

        <div class="w-full md:w-48">
          <label for="status-filter" class="block text-sm font-medium text-gray-700 mb-1">وضعیت</label>
          <select
            id="status-filter"
            v-model="statusFilter"
            class="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 text-right"
            @change="applyFilters"
          >
            <option value="">همه وضعیت‌ها</option>
            <option value="active">فعال</option>
            <option value="inactive">غیرفعال</option>
            <option value="locked">قفل شده</option>
          </select>
        </div>
      </div>

      <div class="flex justify-between items-center">
        <div class="text-sm text-gray-600">
          {{ filteredUsers.length }} کاربر یافت شد
        </div>
        <AppButton @click="createUser" variant="primary">
          <svg class="w-5 h-5 ml-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
          </svg>
          کاربر جدید
        </AppButton>
      </div>
    </div>

    <!-- Users Table -->
    <div class="bg-white rounded-lg shadow overflow-hidden">
      <div v-if="loading" class="flex justify-center items-center p-12">
        <AppSpinner size="lg" />
      </div>

      <div v-else-if="filteredUsers.length === 0" class="p-12 text-center">
        <svg class="w-8 h-8 text-gray-400 mx-auto mb-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
        </svg>
        <h3 class="text-lg font-medium text-gray-900 mb-1">کاربری یافت نشد</h3>
        <p class="text-gray-500">با معیارهای جستجوی فعلی هیچ کاربری یافت نشد.</p>
      </div>

      <table v-else class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th scope="col" class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
              نام کاربر
            </th>
            <th scope="col" class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
              نام کاربری
            </th>
            <th scope="col" class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
              نقش
            </th>
            <th scope="col" class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
              آخرین ورود
            </th>
            <th scope="col" class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
              وضعیت
            </th>
            <th scope="col" class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
              عملیات
            </th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="user in paginatedUsers" :key="user.id" class="hover:bg-gray-50">
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="flex items-center">
                <div class="flex-shrink-0 h-10 w-10 rounded-full bg-blue-100 flex items-center justify-center text-blue-600 font-bold">
                  {{ getInitials(user.name) }}
                </div>
                <div class="mr-4">
                  <div class="text-sm font-medium text-gray-900">{{ user.name }}</div>
                  <div class="text-sm text-gray-500">{{ user.email }}</div>
                </div>
              </div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ user.username }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full" :class="getRoleBadgeClass(user.role)">
                {{ getRoleLabel(user.role) }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ formatDate(user.lastLogin) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full" :class="getStatusBadgeClass(user.status)">
                {{ getStatusLabel(user.status) }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
              <div class="flex space-x-3 space-x-reverse">
                <button @click="editUser(user)" class="text-blue-600 hover:text-blue-900">
                  <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                  </svg>
                </button>
                <button @click="toggleUserStatus(user)" class="text-yellow-600 hover:text-yellow-900">
                  <svg v-if="user.status === 'active'" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
                  </svg>
                  <svg v-else class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 11V7a4 4 0 118 0m-4 8v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2z" />
                  </svg>
                </button>
                <button @click="deleteUser(user)" class="text-red-600 hover:text-red-900">
                  <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                  </svg>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>

      <!-- Pagination -->
      <div v-if="filteredUsers.length > 0" class="bg-white px-4 py-3 flex items-center justify-between border-t border-gray-200 sm:px-6">
        <div class="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
          <div>
            <p class="text-sm text-gray-700">
              نمایش
              <span class="font-medium">{{ paginationStart }}</span>
              تا
              <span class="font-medium">{{ paginationEnd }}</span>
              از
              <span class="font-medium">{{ filteredUsers.length }}</span>
              نتیجه
            </p>
          </div>
          <div>
            <nav class="relative z-0 inline-flex rounded-md shadow-sm -space-x-px" aria-label="Pagination">
              <button
                @click="currentPage--"
                :disabled="currentPage === 1"
                class="relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <span class="sr-only">قبلی</span>
                <svg class="h-5 w-5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                  <path fill-rule="evenodd" d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0z" clip-rule="evenodd" />
                </svg>
              </button>
              <span
                v-for="page in totalPages"
                :key="page"
                @click="currentPage = page"
                :class="[
                  'relative inline-flex items-center px-4 py-2 border border-gray-300 bg-white text-sm font-medium cursor-pointer',
                  currentPage === page ? 'text-blue-600 border-blue-500 z-10' : 'text-gray-500 hover:bg-gray-50'
                ]"
              >
                {{ page }}
              </span>
              <button
                @click="currentPage++"
                :disabled="currentPage === totalPages"
                class="relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <span class="sr-only">بعدی</span>
                <svg class="h-5 w-5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                  <path fill-rule="evenodd" d="M12.707 5.293a1 1 0 010 1.414L9.414 10l3.293 3.293a1 1 0 01-1.414 1.414l-4-4a1 1 0 010-1.414l4-4a1 1 0 011.414 0z" clip-rule="evenodd" />
                </svg>
              </button>
            </nav>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useSettingsStore } from '@/stores/settings'
import AppButton from '@/components/AppButton.vue'
import AppSpinner from '@/components/AppSpinner.vue'

const settingsStore = useSettingsStore()

// State
const searchTerm = ref('')
const roleFilter = ref('')
const statusFilter = ref('')
const currentPage = ref(1)
const itemsPerPage = ref(10)
const loading = ref(true)

// Load users on component mount
onMounted(async () => {
  try {
    await settingsStore.loadAllSettings()
  } finally {
    loading.value = false
  }
})

// Computed properties
const filteredUsers = computed(() => {
  let result = settingsStore.users

  if (searchTerm.value) {
    const term = searchTerm.value.toLowerCase()
    result = result.filter(user => 
      user.name.toLowerCase().includes(term) || 
      user.username.toLowerCase().includes(term) || 
      (user.email && user.email.toLowerCase().includes(term))
    )
  }

  if (roleFilter.value) {
    result = result.filter(user => user.role === roleFilter.value)
  }

  if (statusFilter.value) {
    result = result.filter(user => user.status === statusFilter.value)
  }

  return result
})

const totalPages = computed(() => {
  return Math.ceil(filteredUsers.value.length / itemsPerPage.value)
})

const paginatedUsers = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  const end = start + itemsPerPage.value
  return filteredUsers.value.slice(start, end)
})

const paginationStart = computed(() => {
  return (currentPage.value - 1) * itemsPerPage.value + 1
})

const paginationEnd = computed(() => {
  return Math.min(currentPage.value * itemsPerPage.value, filteredUsers.value.length)
})

// Methods
function applyFilters() {
  currentPage.value = 1
}

function getInitials(name) {
  if (!name) return ''
  return name.split(' ')
    .map(part => part.charAt(0))
    .join('')
    .substring(0, 2)
    .toUpperCase()
}

function getRoleLabel(role) {
  const labels = {
    admin: 'مدیر سیستم',
    manager: 'مدیر',
    accountant: 'حسابدار',
    sales: 'کارشناس فروش',
    user: 'کاربر عادی'
  }
  return labels[role] || role
}

function getStatusLabel(status) {
  const labels = {
    active: 'فعال',
    inactive: 'غیرفعال',
    locked: 'قفل شده'
  }
  return labels[status] || status
}

function getRoleBadgeClass(role) {
  const classes = {
    admin: 'bg-purple-100 text-purple-800',
    manager: 'bg-blue-100 text-blue-800',
    accountant: 'bg-green-100 text-green-800',
    sales: 'bg-yellow-100 text-yellow-800',
    user: 'bg-gray-100 text-gray-800'
  }
  return classes[role] || 'bg-gray-100 text-gray-800'
}

function getStatusBadgeClass(status) {
  const classes = {
    active: 'bg-green-100 text-green-800',
    inactive: 'bg-gray-100 text-gray-800',
    locked: 'bg-red-100 text-red-800'
  }
  return classes[status] || 'bg-gray-100 text-gray-800'
}

function formatDate(dateString) {
  if (!dateString) return ''
  // Convert to Persian date format
  return dateString
}

// CRUD operations
async function createUser() {
  // This would open a modal or navigate to a create user form
  const userData = {
    name: 'کاربر جدید',
    username: 'newuser',
    email: 'newuser@example.com',
    role: 'user',
    status: 'active'
  }
  
  try {
    await settingsStore.createUser(userData)
    alert('کاربر جدید با موفقیت ایجاد شد')
  } catch (error) {
    alert('خطا در ایجاد کاربر: ' + error.message)
  }
}

async function editUser(user) {
  // This would open a modal or navigate to an edit user form
  const updates = {
    name: user.name + ' (ویرایش شده)',
    email: user.email,
    role: user.role,
    status: user.status
  }
  
  try {
    await settingsStore.updateUser(user.id, updates)
    alert(`کاربر ${user.name} با موفقیت ویرایش شد`)
  } catch (error) {
    alert('خطا در ویرایش کاربر: ' + error.message)
  }
}

async function toggleUserStatus(user) {
  const newStatus = user.status === 'active' ? 'inactive' : 'active'
  
  try {
    await settingsStore.updateUser(user.id, { status: newStatus })
    alert(`وضعیت کاربر ${user.name} با موفقیت تغییر کرد`)
  } catch (error) {
    alert('خطا در تغییر وضعیت کاربر: ' + error.message)
  }
}

async function deleteUser(user) {
  const confirmed = confirm(`آیا از حذف کاربر ${user.name} اطمینان دارید؟`)
  if (confirmed) {
    try {
      await settingsStore.deleteUser(user.id)
      alert(`کاربر ${user.name} با موفقیت حذف شد`)
    } catch (error) {
      alert('خطا در حذف کاربر: ' + error.message)
    }
  }
}
</script>