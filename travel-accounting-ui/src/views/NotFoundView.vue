<template>
  <div class="min-h-screen bg-gray-50 flex flex-col justify-center py-12 sm:px-6 lg:px-8 rtl">
    <div class="sm:mx-auto sm:w-full sm:max-w-md">
      <div class="text-center">
        <!-- 404 Icon -->
        <div class="mx-auto h-16 w-16 text-gray-400 mb-8">
          <svg class="h-full w-full" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1" d="M9.172 16.172a4 4 0 015.656 0M9 12h6m-6-4h6m2 5.291A7.962 7.962 0 0112 15c-2.34 0-4.467-.881-6.08-2.33M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
          </svg>
        </div>
        
        <!-- 404 Text -->
        <h1 class="text-9xl font-bold text-gray-300 mb-4">404</h1>
        
        <!-- Error Message -->
        <h2 class="text-3xl font-bold text-gray-900 mb-4">
          صفحه یافت نشد
        </h2>
        
        <p class="text-lg text-gray-600 mb-8 max-w-md mx-auto">
          متأسفانه صفحه‌ای که به دنبال آن هستید وجود ندارد یا منتقل شده است.
        </p>
        
        <!-- Action Buttons -->
        <div class="flex flex-col sm:flex-row gap-4 justify-center items-center">
          <AppButton
            @click="goHome"
            variant="primary"
            size="lg"
            class="w-full sm:w-auto"
          >
            <svg class="w-5 h-5 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
            </svg>
            بازگشت به خانه
          </AppButton>
          
          <AppButton
            @click="goBack"
            variant="outline"
            size="lg"
            class="w-full sm:w-auto"
          >
            <svg class="w-5 h-5 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
            </svg>
            بازگشت
          </AppButton>
        </div>
        
        <!-- Additional Help -->
        <div class="mt-12 text-center">
          <p class="text-sm text-gray-500 mb-4">
            اگر فکر می‌کنید این یک خطا است، لطفاً با پشتیبانی تماس بگیرید.
          </p>
          
          <div class="flex justify-center space-x-6 space-x-reverse">
            <a href="mailto:support@travelaccounting.com" class="text-blue-600 hover:text-blue-500 text-sm">
              <svg class="w-4 h-4 inline ml-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 4.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
              </svg>
              پشتیبانی ایمیل
            </a>
            
            <a href="tel:+982188776655" class="text-blue-600 hover:text-blue-500 text-sm">
              <svg class="w-4 h-4 inline ml-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" />
              </svg>
              ۰۲۱-۸۸۷۷۶۶۵۵
            </a>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Background Pattern -->
    <div class="absolute inset-0 -z-10 overflow-hidden">
      <svg class="absolute left-[max(50%,25rem)] top-0 h-[64rem] w-[128rem] -translate-x-1/2 stroke-gray-200 [mask-image:radial-gradient(64rem_64rem_at_top,white,transparent)]" aria-hidden="true">
        <defs>
          <pattern id="e813992c-7d03-4cc4-a2bd-151760b470a0" width="200" height="200" x="50%" y="-1" patternUnits="userSpaceOnUse">
            <path d="M100 200V.5M.5 .5H200" fill="none" />
          </pattern>
        </defs>
        <rect width="100%" height="100%" stroke-width="0" fill="url(#e813992c-7d03-4cc4-a2bd-151760b470a0)" />
      </svg>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import AppButton from '@/components/AppButton.vue'

const router = useRouter()

function goHome() {
  router.push('/')
}

function goBack() {
  // Go back in history, or to home if no history
  if (window.history.length > 1) {
    router.go(-1)
  } else {
    router.push('/')
  }
}
</script>

<style scoped>
/* Additional RTL styles if needed */
.rtl {
  direction: rtl;
}

/* Animation for the 404 number */
@keyframes bounce {
  0%, 20%, 53%, 80%, 100% {
    transform: translate3d(0, 0, 0);
  }
  40%, 43% {
    transform: translate3d(0, -10px, 0);
  }
  70% {
    transform: translate3d(0, -5px, 0);
  }
  90% {
    transform: translate3d(0, -2px, 0);
  }
}

h1 {
  animation: bounce 2s infinite;
}

/* Hover effects */
.group:hover .group-hover\:scale-105 {
  transform: scale(1.05);
}

/* Focus styles for accessibility */
a:focus,
button:focus {
  outline: 2px solid #3b82f6;
  outline-offset: 2px;
}
</style>