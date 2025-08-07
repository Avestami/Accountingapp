import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import router from './router'
import App from './App.vue'
import { useAuthStore } from './stores/auth'

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)
app.use(router)

// Initialize auth state on app startup
const authStore = useAuthStore()
authStore.initializeAuth().then(() => {
  app.mount('#app')
}).catch((error) => {
  console.error('Failed to initialize auth:', error)
  app.mount('#app')
})
