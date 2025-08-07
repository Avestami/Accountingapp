<template>
  <div id="app" class="min-h-screen bg-gray-50" dir="rtl">
    <!-- Loading Overlay -->
    <div
      v-if="isInitializing"
      class="fixed inset-0 bg-white z-50 flex items-center justify-center"
    >
      <div class="text-center">
        <AppSpinner size="lg" color="primary" :show-text="true" text="در حال بارگذاری..." />
        <p class="mt-4 text-gray-600">سیستم حسابداری سفر</p>
      </div>
    </div>

    <!-- Main Application -->
    <div v-else>
      <!-- Login Page -->
      <router-view v-if="!authStore.isAuthenticated" />
      
      <!-- Authenticated Layout -->
      <AppLayout v-else>
        <router-view />
      </AppLayout>
    </div>

    <!-- Global Notifications -->
    <div
      v-if="notifications.length > 0"
      class="fixed top-4 left-4 z-40 space-y-2"
    >
      <div
        v-for="notification in notifications"
        :key="notification.id"
        class="max-w-sm w-full bg-white shadow-lg rounded-lg pointer-events-auto ring-1 ring-black ring-opacity-5 overflow-hidden"
        :class="{
          'border-r-4 border-green-400': notification.type === 'success',
          'border-r-4 border-red-400': notification.type === 'error',
          'border-r-4 border-yellow-400': notification.type === 'warning',
          'border-r-4 border-blue-400': notification.type === 'info'
        }"
      >
        <div class="p-4">
          <div class="flex items-start">
            <div class="flex-shrink-0">
              <!-- Success Icon -->
              <svg
                v-if="notification.type === 'success'"
                class="h-6 w-6 text-green-400"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
              
              <!-- Error Icon -->
              <svg
                v-else-if="notification.type === 'error'"
                class="h-6 w-6 text-red-400"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
              
              <!-- Warning Icon -->
              <svg
                v-else-if="notification.type === 'warning'"
                class="h-6 w-6 text-yellow-400"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16c-.77.833.192 2.5 1.732 2.5z" />
              </svg>
              
              <!-- Info Icon -->
              <svg
                v-else
                class="h-6 w-6 text-blue-400"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
            
            <div class="mr-3 w-0 flex-1 pt-0.5">
              <p class="text-sm font-medium text-gray-900">
                {{ notification.title }}
              </p>
              <p v-if="notification.message" class="mt-1 text-sm text-gray-500">
                {{ notification.message }}
              </p>
            </div>
            
            <div class="mr-4 flex-shrink-0 flex">
              <button
                @click="removeNotification(notification.id)"
                class="bg-white rounded-md inline-flex text-gray-400 hover:text-gray-500 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500"
              >
                <span class="sr-only">بستن</span>
                <svg class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                  <path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd" />
                </svg>
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, onMounted } from 'vue'
import { useAuthStore } from './stores/auth'
import AppLayout from './components/AppLayout.vue'
import AppSpinner from './components/AppSpinner.vue'

export default {
  name: 'App',
  components: {
    AppLayout,
    AppSpinner
  },
  setup() {
    // Stores
    const authStore = useAuthStore()
    
    // Reactive data
    const isInitializing = ref(true)
    const notifications = ref([])
    
    // Notification management
    let notificationId = 0
    
    const addNotification = (notification) => {
      const id = ++notificationId
      const newNotification = {
        id,
        type: notification.type || 'info',
        title: notification.title,
        message: notification.message,
        duration: notification.duration || 5000
      }
      
      notifications.value.push(newNotification)
      
      // Auto remove after duration
      if (newNotification.duration > 0) {
        setTimeout(() => {
          removeNotification(id)
        }, newNotification.duration)
      }
      
      return id
    }
    
    const removeNotification = (id) => {
      const index = notifications.value.findIndex(n => n.id === id)
      if (index > -1) {
        notifications.value.splice(index, 1)
      }
    }
    
    // Initialize application
    const initializeApp = async () => {
      try {
        // Check for existing authentication
        await authStore.initializeAuth()
        
        // Add welcome notification for authenticated users
        if (authStore.isAuthenticated) {
          addNotification({
            type: 'success',
            title: 'خوش آمدید',
            message: `${authStore.userFullName} عزیز، به سیستم خوش آمدید`,
            duration: 3000
          })
        }
        
      } catch (error) {
        console.error('Initialization error:', error)
      } finally {
        // Simulate loading time
        setTimeout(() => {
          isInitializing.value = false
        }, 1500)
      }
    }
    
    // Lifecycle
    onMounted(() => {
      initializeApp()
    })
    
    return {
      // Stores
      authStore,
      
      // Data
      isInitializing,
      notifications,
      
      // Methods
      addNotification,
      removeNotification
    }
  }
}
</script>

<style>
/* Global styles */
#app {
  font-family: 'Vazir', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

/* RTL Support */
[dir="rtl"] {
  text-align: right;
}

[dir="rtl"] .space-x-reverse > :not([hidden]) ~ :not([hidden]) {
  --tw-space-x-reverse: 1;
}

/* Custom scrollbar */
::-webkit-scrollbar {
  width: 8px;
  height: 8px;
}

::-webkit-scrollbar-track {
  background: #f1f5f9;
}

::-webkit-scrollbar-thumb {
  background: #cbd5e1;
  border-radius: 4px;
}

::-webkit-scrollbar-thumb:hover {
  background: #94a3b8;
}

/* Animation classes */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
