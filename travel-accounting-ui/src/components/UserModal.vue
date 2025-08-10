<template>
  <div v-if="isOpen" class="fixed inset-0 z-50 overflow-y-auto">
    <div class="flex items-center justify-center min-h-screen pt-4 px-4 pb-20 text-center sm:block sm:p-0">
      <!-- Background overlay -->
      <div class="fixed inset-0 transition-opacity" aria-hidden="true">
        <div class="absolute inset-0 bg-gray-500 opacity-75" @click="closeModal"></div>
      </div>

      <!-- Modal panel -->
      <div class="inline-block align-bottom bg-white rounded-lg text-right overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-lg sm:w-full">
        <div class="bg-white px-4 pt-5 pb-4 sm:p-6 sm:pb-4">
          <div class="sm:flex sm:items-start">
            <div class="mt-3 text-center sm:mt-0 sm:mr-4 sm:text-right w-full">
              <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">
                {{ isEditMode ? 'ویرایش کاربر' : 'کاربر جدید' }}
              </h3>
              
              <form @submit.prevent="handleSubmit" class="space-y-4">
                <!-- Username -->
                <div>
                  <label for="username" class="block text-sm font-medium text-gray-700 mb-1">نام کاربری</label>
                  <input
                    id="username"
                    v-model="form.username"
                    type="text"
                    required
                    :disabled="isEditMode"
                    class="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 text-right disabled:bg-gray-100"
                  />
                </div>

                <!-- Password (only for create) -->
                <div v-if="!isEditMode">
                  <label for="password" class="block text-sm font-medium text-gray-700 mb-1">رمز عبور</label>
                  <input
                    id="password"
                    v-model="form.password"
                    type="password"
                    required
                    class="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 text-right"
                  />
                </div>

                <!-- Email -->
                <div>
                  <label for="email" class="block text-sm font-medium text-gray-700 mb-1">ایمیل</label>
                  <input
                    id="email"
                    v-model="form.email"
                    type="email"
                    required
                    class="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 text-right"
                  />
                </div>

                <!-- First Name -->
                <div>
                  <label for="firstName" class="block text-sm font-medium text-gray-700 mb-1">نام</label>
                  <input
                    id="firstName"
                    v-model="form.firstName"
                    type="text"
                    required
                    class="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 text-right"
                  />
                </div>

                <!-- Last Name -->
                <div>
                  <label for="lastName" class="block text-sm font-medium text-gray-700 mb-1">نام خانوادگی</label>
                  <input
                    id="lastName"
                    v-model="form.lastName"
                    type="text"
                    required
                    class="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 text-right"
                  />
                </div>

                <!-- Role -->
                <div>
                  <label for="role" class="block text-sm font-medium text-gray-700 mb-1">نقش</label>
                  <select
                    id="role"
                    v-model="form.role"
                    required
                    class="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 text-right"
                  >
                    <option value="Admin">مدیر سیستم</option>
                    <option value="Manager">مدیر</option>
                    <option value="Accountant">حسابدار</option>
                    <option value="Sales">کارشناس فروش</option>
                    <option value="User">کاربر عادی</option>
                  </select>
                </div>

                <!-- Department -->
                <div>
                  <label for="department" class="block text-sm font-medium text-gray-700 mb-1">بخش</label>
                  <input
                    id="department"
                    v-model="form.department"
                    type="text"
                    class="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 text-right"
                  />
                </div>

                <!-- Position -->
                <div>
                  <label for="position" class="block text-sm font-medium text-gray-700 mb-1">سمت</label>
                  <input
                    id="position"
                    v-model="form.position"
                    type="text"
                    class="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 text-right"
                  />
                </div>
              </form>
            </div>
          </div>
        </div>
        
        <div class="bg-gray-50 px-4 py-3 sm:px-6 sm:flex sm:flex-row-reverse">
          <button
            @click="handleSubmit"
            :disabled="loading"
            type="button"
            class="w-full inline-flex justify-center rounded-md border border-transparent shadow-sm px-4 py-2 bg-blue-600 text-base font-medium text-white hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 sm:ml-3 sm:w-auto sm:text-sm disabled:opacity-50"
          >
            <svg v-if="loading" class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
            {{ isEditMode ? 'ویرایش' : 'ایجاد' }}
          </button>
          <button
            @click="closeModal"
            type="button"
            class="mt-3 w-full inline-flex justify-center rounded-md border border-gray-300 shadow-sm px-4 py-2 bg-white text-base font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 sm:mt-0 sm:ml-3 sm:w-auto sm:text-sm"
          >
            انصراف
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, watch, computed } from 'vue'

const props = defineProps({
  isOpen: {
    type: Boolean,
    default: false
  },
  user: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(['close', 'save'])

const loading = ref(false)

const form = ref({
  username: '',
  password: '',
  email: '',
  firstName: '',
  lastName: '',
  role: 'User',
  department: '',
  position: ''
})

const isEditMode = computed(() => !!props.user)

// Watch for user prop changes to populate form
watch(() => props.user, (newUser) => {
  if (newUser) {
    form.value = {
      username: newUser.username || '',
      password: '',
      email: newUser.email || '',
      firstName: newUser.firstName || '',
      lastName: newUser.lastName || '',
      role: newUser.role || 'User',
      department: newUser.department || '',
      position: newUser.position || ''
    }
  } else {
    resetForm()
  }
}, { immediate: true })

function resetForm() {
  form.value = {
    username: '',
    password: '',
    email: '',
    firstName: '',
    lastName: '',
    role: 'User',
    department: '',
    position: ''
  }
}

function closeModal() {
  resetForm()
  emit('close')
}

async function handleSubmit() {
  loading.value = true
  
  try {
    const userData = { ...form.value }
    
    // Remove password field for edit mode
    if (isEditMode.value) {
      delete userData.password
    }
    
    emit('save', userData)
  } finally {
    loading.value = false
  }
}
</script>