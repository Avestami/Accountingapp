<template>
  <div class="min-h-screen bg-gray-50 rtl" dir="rtl">
    <!-- Header -->
    <header class="bg-white shadow-sm border-b border-gray-200">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center h-16">
          <!-- Logo and Title -->
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <h1 class="text-xl font-bold text-gray-900">سیستم حسابداری سفر</h1>
            </div>
          </div>

          <!-- Global Search -->
          <div class="flex-1 max-w-lg mx-8">
            <div class="relative">
              <div style="position: absolute; top: 0; bottom: 0; right: 0; padding-right: 0.75rem; display: flex; align-items: center; pointer-events: none;">
                <svg class="h-5 w-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                </svg>
              </div>
              <input
                v-model="globalSearch"
                type="text"
                class="block w-full pr-10 pl-3 py-2 border border-gray-300 rounded-md leading-5 bg-white placeholder-gray-500 focus:outline-none focus:placeholder-gray-400 focus:ring-1 focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                placeholder="جستجو بر اساس شماره سند، سریال، مرجع..."
                @keyup.enter="performGlobalSearch"
              />
            </div>
          </div>

          <!-- User Menu -->
          <div class="flex items-center space-x-4 space-x-reverse">
            <!-- Notifications -->
            <button class="p-2 text-gray-400 hover:text-gray-500 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:ring-offset-2 rounded-full">
              <svg class="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 17h5l-5 5v-5zM10.07 2.82l3.12 3.12M7.05 5.84L10.17 8.96" />
              </svg>
            </button>

            <!-- User Dropdown -->
            <div class="relative">
              <button
                @click="showUserMenu = !showUserMenu"
                class="flex items-center text-sm rounded-full focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500"
              >
                <div class="h-8 w-8 rounded-full bg-blue-500 flex items-center justify-center">
                  <span class="text-white text-sm font-medium">{{ userInitials }}</span>
                </div>
                <span class="mr-2 text-gray-700">{{ currentUser?.name }}</span>
                <svg class="mr-1 h-4 w-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                </svg>
              </button>

              <!-- User Dropdown Menu -->
              <div
                v-show="showUserMenu"
                style="transform-origin: top right; position: absolute; left: 0; margin-top: 0.5rem; width: 12rem; border-radius: 0.375rem; box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05); background-color: white; border: 1px solid rgba(0, 0, 0, 0.05); z-index: 50;"
                class="focus:outline-none"
                @click.away="showUserMenu = false"
              >
                <div class="py-1">
                  <a href="#" class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100">پروفایل کاربری</a>
                  <a href="#" class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100">تنظیمات</a>
                  <div class="border-t border-gray-100"></div>
                  <button
                    @click="logout"
                    class="block w-full text-right px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                  >
                    خروج از سیستم
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </header>

    <!-- Horizontal Navigation -->
    <nav class="bg-gray-50 border-b border-gray-200">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex space-x-8 space-x-reverse">
          <router-link
            v-for="item in navigation"
            :key="item.name"
            :to="item.to"
            class="px-3 py-4 text-sm font-medium text-gray-700 hover:text-gray-900 border-b-2 border-transparent hover:border-gray-300 transition-colors"
            :class="isActiveRoute(item.to) ? 'text-blue-600 border-blue-600' : ''"
          >
            {{ item.name }}
          </router-link>
        </div>
      </div>
    </nav>

    <div class="flex">
      <!-- Sidebar -->
      <nav class="bg-white w-64 min-h-screen shadow-sm border-l border-gray-200">
        <div class="p-4">
          <!-- Main Navigation -->
          <div class="space-y-2">
            <!-- Dashboard -->
            <router-link
              to="/"
              class="flex items-center px-3 py-2 text-sm font-medium rounded-md transition-colors"
              :class="isActiveRoute('/') ? 'bg-primary-100 text-primary-700' : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'"
            >
              <svg class="ml-3 h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 7v10a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2H5a2 2 0 00-2-2z" />
              </svg>
              داشبورد
            </router-link>

            <!-- Sales Section -->
            <div v-if="authStore.hasPermission('sales')">
              <div class="flex items-center justify-between px-3 py-2 text-sm font-medium text-gray-900">
                <span class="flex items-center">
                  <svg class="ml-3 h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                  </svg>
                  فروش
                </span>
                <button
                  @click="toggleSection('sales')"
                  class="p-1 rounded hover:bg-gray-100"
                >
                  <svg
                    class="h-4 w-4 transform transition-transform"
                    :class="openSections.sales ? 'rotate-180' : ''"
                    fill="none"
                    stroke="currentColor"
                    viewBox="0 0 24 24"
                  >
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                  </svg>
                </button>
              </div>
              <div v-show="openSections.sales" class="mr-6 space-y-1">
                <router-link
                  to="/sales/create"
                  class="block px-3 py-2 text-sm rounded-md transition-colors"
                  :class="isActiveRoute('/sales/create') ? 'bg-primary-100 text-primary-700' : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'"
                >
                  ایجاد سند جدید
                </router-link>
                <router-link
                  to="/sales/unissued"
                  class="block px-3 py-2 text-sm rounded-md transition-colors"
                  :class="isActiveRoute('/sales/unissued') ? 'bg-primary-100 text-primary-700' : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'"
                >
                  اسناد صادر نشده
                </router-link>
                <router-link
                  to="/sales/issued"
                  class="block px-3 py-2 text-sm rounded-md transition-colors"
                  :class="isActiveRoute('/sales/issued') ? 'bg-primary-100 text-primary-700' : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'"
                >
                  اسناد صادر شده
                </router-link>
                <router-link
                  to="/sales/canceled"
                  class="block px-3 py-2 text-sm rounded-md transition-colors"
                  :class="isActiveRoute('/sales/canceled') ? 'bg-primary-100 text-primary-700' : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'"
                >
                  اسناد لغو شده
                </router-link>
              </div>
            </div>

            <!-- Finance Section -->
            <div v-if="authStore.hasPermission('finance')">
              <div class="flex items-center justify-between px-3 py-2 text-sm font-medium text-gray-900">
                <span class="flex items-center">
                  <svg class="ml-3 h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1" />
                  </svg>
                  مالی
                </span>
                <button
                  @click="toggleSection('finance')"
                  class="p-1 rounded hover:bg-gray-100"
                >
                  <svg
                    class="h-4 w-4 transform transition-transform"
                    :class="openSections.finance ? 'rotate-180' : ''"
                    fill="none"
                    stroke="currentColor"
                    viewBox="0 0 24 24"
                  >
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                  </svg>
                </button>
              </div>
              <div v-show="openSections.finance" class="mr-6 space-y-1">
                <router-link
                  to="/finance/voucher"
                  class="block px-3 py-2 text-sm rounded-md transition-colors"
                  :class="isActiveRoute('/finance/voucher') ? 'bg-primary-100 text-primary-700' : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'"
                >
                  سند حسابداری
                </router-link>
                <router-link
                  to="/finance/costs"
                  class="block px-3 py-2 text-sm rounded-md transition-colors"
                  :class="isActiveRoute('/finance/costs') ? 'bg-primary-100 text-primary-700' : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'"
                >
                  هزینه‌ها
                </router-link>
                <router-link
                  to="/finance/incomes"
                  class="block px-3 py-2 text-sm rounded-md transition-colors"
                  :class="isActiveRoute('/finance/incomes') ? 'bg-primary-100 text-primary-700' : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'"
                >
                  درآمدها
                </router-link>
                <router-link
                  to="/finance/transfer"
                  class="block px-3 py-2 text-sm rounded-md transition-colors"
                  :class="isActiveRoute('/finance/transfer') ? 'bg-primary-100 text-primary-700' : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'"
                >
                  انتقال وجه
                </router-link>
              </div>
            </div>

            <!-- Reports -->
            <router-link
              v-if="authStore.hasPermission('reports')"
              to="/reports"
              class="flex items-center px-3 py-2 text-sm font-medium rounded-md transition-colors"
              :class="isActiveRoute('/reports') ? 'bg-primary-100 text-primary-700' : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'"
            >
              <svg class="ml-3 h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
              </svg>
              گزارشات
            </router-link>

            <!-- Settings Section -->
            <div v-if="authStore.hasPermission('settings')">
              <div class="flex items-center justify-between px-3 py-2 text-sm font-medium text-gray-900">
                <span class="flex items-center">
                  <svg class="ml-3 h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                  </svg>
                  تنظیمات
                </span>
                <button
                  @click="toggleSection('settings')"
                  class="p-1 rounded hover:bg-gray-100"
                >
                  <svg
                    class="h-4 w-4 transform transition-transform"
                    :class="openSections.settings ? 'rotate-180' : ''"
                    fill="none"
                    stroke="currentColor"
                    viewBox="0 0 24 24"
                  >
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                  </svg>
                </button>
              </div>
              <div v-show="openSections.settings" class="mr-6 space-y-1">
                <router-link
                  to="/settings/users"
                  class="block px-3 py-2 text-sm rounded-md transition-colors"
                  :class="isActiveRoute('/settings/users') ? 'bg-primary-100 text-primary-700' : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'"
                >
                  کاربران و نقش‌ها
                </router-link>
                <router-link
                  to="/settings/airlines"
                  class="block px-3 py-2 text-sm rounded-md transition-colors"
                  :class="isActiveRoute('/settings/airlines') ? 'bg-primary-100 text-primary-700' : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'"
                >
                  ایرلاین‌ها
                </router-link>
                <router-link
                  to="/settings/banks"
                  class="block px-3 py-2 text-sm rounded-md transition-colors"
                  :class="isActiveRoute('/settings/banks') ? 'bg-primary-100 text-primary-700' : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'"
                >
                  بانک‌ها و حساب‌ها
                </router-link>
                <router-link
                  to="/settings/counterparties"
                  class="block px-3 py-2 text-sm rounded-md transition-colors"
                  :class="isActiveRoute('/settings/counterparties') ? 'bg-primary-100 text-primary-700' : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'"
                >
                  اشخاص و طرف‌حساب‌ها
                </router-link>
                <router-link
                  to="/settings/locations"
                  class="block px-3 py-2 text-sm rounded-md transition-colors"
                  :class="isActiveRoute('/settings/locations') ? 'bg-primary-100 text-primary-700' : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'"
                >
                  مبادی و مقاصد
                </router-link>
              </div>
            </div>
          </div>
        </div>
      </nav>

      <!-- Main Content -->
      <main class="flex-1 overflow-x-hidden">
        <!-- Breadcrumb -->
        <div v-if="breadcrumbs.length > 0" class="bg-white border-b border-gray-200 px-6 py-3">
          <nav class="flex" aria-label="Breadcrumb">
            <ol class="flex items-center space-x-2 space-x-reverse">
              <li v-for="(breadcrumb, index) in breadcrumbs" :key="index">
                <div class="flex items-center">
                  <router-link
                    v-if="breadcrumb.to && index < breadcrumbs.length - 1"
                    :to="breadcrumb.to"
                    class="text-sm font-medium text-gray-500 hover:text-gray-700"
                  >
                    {{ breadcrumb.title }}
                  </router-link>
                  <span
                    v-else
                    class="text-sm font-medium"
                    :class="index === breadcrumbs.length - 1 ? 'text-gray-900' : 'text-gray-500'"
                  >
                    {{ breadcrumb.title }}
                  </span>
                  <svg
                    v-if="index < breadcrumbs.length - 1"
                    class="flex-shrink-0 h-5 w-5 text-gray-400 mx-2"
                    fill="currentColor"
                    viewBox="0 0 20 20"
                  >
                    <path fill-rule="evenodd" d="M12.707 5.293a1 1 0 010 1.414L9.414 10l3.293 3.293a1 1 0 01-1.414 1.414l-4-4a1 1 0 010-1.414l4-4a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                </div>
              </li>
            </ol>
          </nav>
        </div>

        <!-- Page Content -->
        <div class="p-6">
          <router-view />
        </div>
      </main>
    </div>

    <!-- Loading Overlay -->
    <div v-if="isLoading" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <AppSpinner size="lg" color="white" :show-text="true" text="در حال بارگذاری..." />
    </div>
  </div>
</template>

<script>
import { ref, computed, watch, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import AppSpinner from './AppSpinner.vue'

export default {
  name: 'AppLayout',
  components: {
    AppSpinner
  },
  setup() {
    const route = useRoute()
    const router = useRouter()
    const authStore = useAuthStore()
    
    // Reactive data
    const globalSearch = ref('')
    const showUserMenu = ref(false)
    const openSections = ref({
      sales: true,
      finance: false,
      settings: false
    })
    const isLoading = ref(false)
    
    // Computed properties
    const currentUser = computed(() => authStore.user)
    
    const userInitials = computed(() => {
      if (!currentUser.value?.name) return 'U'
      const names = currentUser.value.name.split(' ')
      return names.map(name => name.charAt(0)).join('').substring(0, 2).toUpperCase()
    })
    
    const breadcrumbs = computed(() => {
      const matched = route.matched.filter(record => record.meta && record.meta.breadcrumb)
      return matched.map(record => ({
        title: record.meta.breadcrumb,
        to: record.path === route.path ? null : record.path
      }))
    })
    
    // Navigation items for horizontal menu with role-based filtering
    const navigation = computed(() => {
      const allNavigation = [
        { name: 'داشبورد', to: '/', permission: null }, // Dashboard is always visible
        { name: 'فروش', to: '/sales', permission: 'sales' },
        { name: 'مالی', to: '/finance', permission: 'finance' },
        { name: 'گزارشات', to: '/reports', permission: 'reports' },
        { name: 'تنظیمات', to: '/settings', permission: 'settings' }
      ]
      
      return allNavigation.filter(item => 
        !item.permission || authStore.hasPermission(item.permission)
      )
    })
    
    // Methods
    const isActiveRoute = (path) => {
      return route.path.startsWith(path)
    }
    
    const toggleSection = (section) => {
      openSections.value[section] = !openSections.value[section]
    }
    
    const performGlobalSearch = () => {
      if (globalSearch.value.trim()) {
        router.push({
          name: 'GlobalSearch',
          query: { q: globalSearch.value.trim() }
        })
      }
    }
    
    const logout = async () => {
      try {
        isLoading.value = true
        await authStore.logout()
        router.push('/login')
      } catch (error) {
        console.error('Logout error:', error)
      } finally {
        isLoading.value = false
      }
    }
    
    // Activity tracking with throttling
    let activityTimeout = null
    const trackActivity = () => {
      if (activityTimeout) return
      
      activityTimeout = setTimeout(() => {
        authStore.updateActivity()
        activityTimeout = null
      }, 30000) // Update activity at most once every 30 seconds
    }
    
    // Watch route changes to close user menu
    watch(route, () => {
      showUserMenu.value = false
      trackActivity() // Track activity on route changes
    })
    
    // Add activity tracking event listeners
    onMounted(() => {
      const events = ['click', 'keypress', 'scroll', 'mousemove']
      events.forEach(event => {
        document.addEventListener(event, trackActivity, { passive: true })
      })
    })
    
    onUnmounted(() => {
      const events = ['click', 'keypress', 'scroll', 'mousemove']
      events.forEach(event => {
        document.removeEventListener(event, trackActivity)
      })
    })
    
    return {
      // Data
      globalSearch,
      showUserMenu,
      openSections,
      isLoading,
      navigation,
      
      // Stores
      authStore,
      
      // Computed
      currentUser,
      userInitials,
      breadcrumbs,
      
      // Methods
      isActiveRoute,
      toggleSection,
      performGlobalSearch,
      logout
    }
  }
}
</script>

<style scoped>
/* Custom scrollbar for sidebar */
nav {
  scrollbar-width: thin;
  scrollbar-color: #cbd5e0 #f7fafc;
}

nav::-webkit-scrollbar {
  width: 6px;
}

nav::-webkit-scrollbar-track {
  background: #f7fafc;
}

nav::-webkit-scrollbar-thumb {
  background-color: #cbd5e0;
  border-radius: 3px;
}

nav::-webkit-scrollbar-thumb:hover {
  background-color: #a0aec0;
}

/* Smooth transitions */
.transition-colors {
  transition-property: color, background-color, border-color;
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
  transition-duration: 150ms;
}

/* RTL specific adjustments */
.rtl .space-x-reverse > :not([hidden]) ~ :not([hidden]) {
  --tw-space-x-reverse: 1;
  margin-right: calc(1rem * var(--tw-space-x-reverse));
  margin-left: calc(1rem * calc(1 - var(--tw-space-x-reverse)));
}
</style>