<template>
  <div class="profile-view">
    <div class="mb-6">
      <h1 class="text-2xl font-bold text-gray-800 mb-2">پروفایل کاربری</h1>
      <p class="text-gray-600">مدیریت اطلاعات شخصی و تنظیمات حساب کاربری</p>
    </div>
    
    <!-- Tab Navigation -->
    <div class="bg-white rounded-lg shadow-md mb-6">
      <div class="border-b border-gray-200">
        <nav class="-mb-px flex space-x-8 space-x-reverse px-4">
          <button
            v-for="tab in tabs"
            :key="tab.key"
            @click="activeTab = tab.key"
            :class="[
              'py-4 px-1 border-b-2 font-medium text-sm',
              activeTab === tab.key
                ? 'border-primary-500 text-primary-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            ]"
          >
            {{ tab.label }}
          </button>
        </nav>
      </div>
    </div>

    <!-- Profile Information Tab -->
    <div v-if="activeTab === 'profile'" class="bg-white rounded-lg shadow-md">
      <div class="p-6">
        <div class="flex items-center space-x-6 space-x-reverse mb-8">
          <!-- Profile Picture -->
          <div class="relative">
            <div class="w-24 h-24 rounded-full overflow-hidden bg-gray-200 flex items-center justify-center">
              <img
                v-if="userProfile.profilePicture"
                :src="getProfilePictureUrl(userProfile.profilePicture)"
                :alt="userProfile.fullName"
                class="w-full h-full object-cover"
              />
              <div v-else class="text-2xl font-bold text-gray-500">
                {{ getUserInitials(userProfile.firstName, userProfile.lastName) }}
              </div>
            </div>
            <button
              @click="activeTab = 'picture'"
              class="absolute bottom-0 right-0 bg-primary-500 text-white rounded-full p-2 hover:bg-primary-600 transition-colors"
            >
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 9a2 2 0 012-2h.93a2 2 0 001.664-.89l.812-1.22A2 2 0 0110.07 4h3.86a2 2 0 011.664.89l.812 1.22A2 2 0 0018.07 7H19a2 2 0 012 2v9a2 2 0 01-2 2H5a2 2 0 01-2-2V9z" />
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 13a3 3 0 11-6 0 3 3 0 016 0z" />
              </svg>
            </button>
          </div>
          
          <!-- User Info -->
          <div>
            <h2 class="text-xl font-semibold text-gray-900">{{ userProfile.fullName }}</h2>
            <p class="text-gray-600">{{ userProfile.email }}</p>
            <p class="text-sm text-gray-500">{{ getRoleLabel(userProfile.role) }} • {{ userProfile.company }}</p>
          </div>
        </div>

        <!-- Edit Profile Form -->
        <form @submit.prevent="updateProfile" class="space-y-6">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">نام</label>
              <input
                v-model="profileForm.firstName"
                type="text"
                class="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent"
                :class="{ 'border-red-500': errors.firstName }"
              />
              <p v-if="errors.firstName" class="text-red-500 text-sm mt-1">{{ errors.firstName }}</p>
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">نام خانوادگی</label>
              <input
                v-model="profileForm.lastName"
                type="text"
                class="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent"
                :class="{ 'border-red-500': errors.lastName }"
              />
              <p v-if="errors.lastName" class="text-red-500 text-sm mt-1">{{ errors.lastName }}</p>
            </div>
            
            <div class="md:col-span-2">
              <label class="block text-sm font-medium text-gray-700 mb-1">ایمیل</label>
              <input
                v-model="profileForm.email"
                type="email"
                class="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent"
                :class="{ 'border-red-500': errors.email }"
              />
              <p v-if="errors.email" class="text-red-500 text-sm mt-1">{{ errors.email }}</p>
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">نام کاربری</label>
              <input
                :value="userProfile.username"
                type="text"
                disabled
                class="w-full border border-gray-300 rounded-md px-3 py-2 bg-gray-100 text-gray-500"
              />
              <p class="text-gray-500 text-sm mt-1">نام کاربری قابل تغییر نیست</p>
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">شرکت</label>
              <input
                :value="userProfile.company"
                type="text"
                disabled
                class="w-full border border-gray-300 rounded-md px-3 py-2 bg-gray-100 text-gray-500"
              />
            </div>
          </div>
          
          <div class="flex justify-end space-x-4 space-x-reverse">
            <AppButton
              type="secondary"
              @click="resetProfileForm"
              :disabled="isUpdating"
            >
              انصراف
            </AppButton>
            <AppButton
              type="primary"
              :loading="isUpdating"
              @click="updateProfile"
            >
              ذخیره تغییرات
            </AppButton>
          </div>
        </form>
      </div>
    </div>

    <!-- Change Password Tab -->
    <div v-else-if="activeTab === 'password'" class="bg-white rounded-lg shadow-md">
      <div class="p-6">
        <h2 class="text-lg font-semibold text-gray-900 mb-6">تغییر رمز عبور</h2>
        
        <form @submit.prevent="changePassword" class="space-y-6 max-w-md">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">رمز عبور فعلی</label>
            <input
              v-model="passwordForm.currentPassword"
              type="password"
              class="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent"
              :class="{ 'border-red-500': errors.currentPassword }"
            />
            <p v-if="errors.currentPassword" class="text-red-500 text-sm mt-1">{{ errors.currentPassword }}</p>
          </div>
          
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">رمز عبور جدید</label>
            <input
              v-model="passwordForm.newPassword"
              type="password"
              class="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent"
              :class="{ 'border-red-500': errors.newPassword }"
            />
            <p v-if="errors.newPassword" class="text-red-500 text-sm mt-1">{{ errors.newPassword }}</p>
          </div>
          
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">تأیید رمز عبور جدید</label>
            <input
              v-model="passwordForm.confirmPassword"
              type="password"
              class="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent"
              :class="{ 'border-red-500': errors.confirmPassword }"
            />
            <p v-if="errors.confirmPassword" class="text-red-500 text-sm mt-1">{{ errors.confirmPassword }}</p>
          </div>
          
          <div class="flex justify-end space-x-4 space-x-reverse">
            <AppButton
              type="secondary"
              @click="resetPasswordForm"
              :disabled="isChangingPassword"
            >
              انصراف
            </AppButton>
            <AppButton
              type="primary"
              :loading="isChangingPassword"
              @click="changePassword"
            >
              تغییر رمز عبور
            </AppButton>
          </div>
        </form>
      </div>
    </div>

    <!-- Profile Picture Tab -->
    <div v-else-if="activeTab === 'picture'" class="bg-white rounded-lg shadow-md">
      <div class="p-6">
        <h2 class="text-lg font-semibold text-gray-900 mb-6">تصویر پروفایل</h2>
        
        <div class="flex flex-col items-center space-y-6">
          <!-- Current Picture -->
          <div class="w-32 h-32 rounded-full overflow-hidden bg-gray-200 flex items-center justify-center">
            <img
              v-if="userProfile.profilePicture"
              :src="getProfilePictureUrl(userProfile.profilePicture)"
              :alt="userProfile.fullName"
              class="w-full h-full object-cover"
            />
            <div v-else class="text-4xl font-bold text-gray-500">
              {{ getUserInitials(userProfile.firstName, userProfile.lastName) }}
            </div>
          </div>
          
          <!-- Upload Area -->
          <div class="w-full max-w-md">
            <div
              @drop="handleDrop"
              @dragover.prevent
              @dragenter.prevent
              class="border-2 border-dashed border-gray-300 rounded-lg p-6 text-center hover:border-primary-500 transition-colors"
              :class="{ 'border-primary-500 bg-primary-50': isDragging }"
            >
              <svg class="mx-auto h-12 w-12 text-gray-400" stroke="currentColor" fill="none" viewBox="0 0 48 48">
                <path d="M28 8H12a4 4 0 00-4 4v20m32-12v8m0 0v8a4 4 0 01-4 4H12a4 4 0 01-4-4v-4m32-4l-3.172-3.172a4 4 0 00-5.656 0L28 28M8 32l9.172-9.172a4 4 0 015.656 0L28 28m0 0l4 4m4-24h8m-4-4v8m-12 4h.02" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
              </svg>
              <div class="mt-4">
                <label for="file-upload" class="cursor-pointer">
                  <span class="mt-2 block text-sm font-medium text-gray-900">
                    فایل را اینجا بکشید یا کلیک کنید
                  </span>
                  <input id="file-upload" name="file-upload" type="file" class="sr-only" @change="handleFileSelect" accept="image/*">
                </label>
                <p class="mt-1 text-xs text-gray-500">PNG، JPG، GIF تا ۵ مگابایت</p>
              </div>
            </div>
          </div>
          
          <!-- Upload Button -->
          <div v-if="selectedFile" class="w-full max-w-md">
            <div class="bg-gray-50 rounded-lg p-4 mb-4">
              <div class="flex items-center justify-between">
                <div class="flex items-center">
                  <svg class="h-8 w-8 text-gray-400" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M4 3a2 2 0 00-2 2v10a2 2 0 002 2h12a2 2 0 002-2V5a2 2 0 00-2-2H4zm12 12H4l4-8 3 6 2-4 3 6z" clip-rule="evenodd" />
                  </svg>
                  <div class="mr-3">
                    <p class="text-sm font-medium text-gray-900">{{ selectedFile.name }}</p>
                    <p class="text-sm text-gray-500">{{ formatFileSize(selectedFile.size) }}</p>
                  </div>
                </div>
                <button @click="selectedFile = null" class="text-gray-400 hover:text-gray-600">
                  <svg class="h-5 w-5" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd" />
                  </svg>
                </button>
              </div>
            </div>
            
            <div class="flex justify-end space-x-4 space-x-reverse">
              <AppButton
                type="secondary"
                @click="selectedFile = null"
                :disabled="isUploading"
              >
                انصراف
              </AppButton>
              <AppButton
                type="primary"
                :loading="isUploading"
                @click="uploadProfilePicture"
              >
                آپلود تصویر
              </AppButton>
            </div>
          </div>
          
          <!-- Delete Picture Button -->
          <div v-if="userProfile.profilePicture && !selectedFile" class="w-full max-w-md">
            <AppButton
              type="danger"
              @click="deleteProfilePicture"
              :loading="isDeleting"
              class="w-full"
            >
              حذف تصویر پروفایل
            </AppButton>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, reactive, onMounted, computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { userProfileApi } from '@/services/api'
import AppButton from '@/components/AppButton.vue'

export default {
  name: 'ProfileView',
  components: {
    AppButton
  },
  setup() {
    const authStore = useAuthStore()
    
    // Reactive data
    const activeTab = ref('profile')
    const userProfile = ref({})
    const selectedFile = ref(null)
    const isDragging = ref(false)
    
    // Loading states
    const isLoading = ref(false)
    const isUpdating = ref(false)
    const isChangingPassword = ref(false)
    const isUploading = ref(false)
    const isDeleting = ref(false)
    
    // Forms
    const profileForm = reactive({
      firstName: '',
      lastName: '',
      email: ''
    })
    
    const passwordForm = reactive({
      currentPassword: '',
      newPassword: '',
      confirmPassword: ''
    })
    
    // Errors
    const errors = reactive({})
    
    // Tabs configuration
    const tabs = [
      { key: 'profile', label: 'اطلاعات پروفایل' },
      { key: 'password', label: 'تغییر رمز عبور' },
      { key: 'picture', label: 'تصویر پروفایل' }
    ]
    
    // Methods
    const loadUserProfile = async () => {
      try {
        isLoading.value = true
        const response = await userProfileApi.getProfile()
        
        if (response.success) {
          userProfile.value = response.data
          
          // Populate form
          profileForm.firstName = response.data.firstName
          profileForm.lastName = response.data.lastName
          profileForm.email = response.data.email
        }
      } catch (error) {
        console.error('Error loading profile:', error)
      } finally {
        isLoading.value = false
      }
    }
    
    const updateProfile = async () => {
      try {
        // Clear previous errors
        Object.keys(errors).forEach(key => delete errors[key])
        
        // Validate
        if (!profileForm.firstName.trim()) {
          errors.firstName = 'نام الزامی است'
        }
        if (!profileForm.lastName.trim()) {
          errors.lastName = 'نام خانوادگی الزامی است'
        }
        if (!profileForm.email.trim()) {
          errors.email = 'ایمیل الزامی است'
        } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(profileForm.email)) {
          errors.email = 'فرمت ایمیل صحیح نیست'
        }
        
        if (Object.keys(errors).length > 0) {
          return
        }
        
        isUpdating.value = true
        const response = await userProfileApi.updateProfile({
          firstName: profileForm.firstName,
          lastName: profileForm.lastName,
          email: profileForm.email
        })
        
        if (response.success) {
          userProfile.value = response.data
          // Update auth store
          authStore.updateUserProfile(response.data)
          alert('پروفایل با موفقیت به‌روزرسانی شد')
        } else {
          alert(response.error || 'خطا در به‌روزرسانی پروفایل')
        }
      } catch (error) {
        console.error('Error updating profile:', error)
        alert('خطا در به‌روزرسانی پروفایل')
      } finally {
        isUpdating.value = false
      }
    }
    
    const changePassword = async () => {
      try {
        // Clear previous errors
        Object.keys(errors).forEach(key => delete errors[key])
        
        // Validate
        if (!passwordForm.currentPassword) {
          errors.currentPassword = 'رمز عبور فعلی الزامی است'
        }
        if (!passwordForm.newPassword) {
          errors.newPassword = 'رمز عبور جدید الزامی است'
        } else if (passwordForm.newPassword.length < 6) {
          errors.newPassword = 'رمز عبور باید حداقل ۶ کاراکتر باشد'
        }
        if (!passwordForm.confirmPassword) {
          errors.confirmPassword = 'تأیید رمز عبور الزامی است'
        } else if (passwordForm.newPassword !== passwordForm.confirmPassword) {
          errors.confirmPassword = 'رمز عبور جدید و تأیید آن مطابقت ندارند'
        }
        
        if (Object.keys(errors).length > 0) {
          return
        }
        
        isChangingPassword.value = true
        const response = await userProfileApi.changePassword({
          currentPassword: passwordForm.currentPassword,
          newPassword: passwordForm.newPassword,
          confirmPassword: passwordForm.confirmPassword
        })
        
        if (response.success) {
          resetPasswordForm()
          alert('رمز عبور با موفقیت تغییر یافت')
        } else {
          alert(response.error || 'خطا در تغییر رمز عبور')
        }
      } catch (error) {
        console.error('Error changing password:', error)
        alert('خطا در تغییر رمز عبور')
      } finally {
        isChangingPassword.value = false
      }
    }
    
    const handleFileSelect = (event) => {
      const file = event.target.files[0]
      if (file) {
        selectedFile.value = file
      }
    }
    
    const handleDrop = (event) => {
      event.preventDefault()
      isDragging.value = false
      
      const files = event.dataTransfer.files
      if (files.length > 0) {
        selectedFile.value = files[0]
      }
    }
    
    const uploadProfilePicture = async () => {
      if (!selectedFile.value) return
      
      try {
        isUploading.value = true
        const formData = new FormData()
        formData.append('file', selectedFile.value)
        
        const response = await userProfileApi.uploadProfilePicture(formData)
        
        if (response.success) {
          userProfile.value.profilePicture = response.data.profilePictureUrl
          selectedFile.value = null
          alert('تصویر پروفایل با موفقیت آپلود شد')
        } else {
          alert(response.error || 'خطا در آپلود تصویر')
        }
      } catch (error) {
        console.error('Error uploading picture:', error)
        alert('خطا در آپلود تصویر')
      } finally {
        isUploading.value = false
      }
    }
    
    const deleteProfilePicture = async () => {
      if (!confirm('آیا از حذف تصویر پروفایل اطمینان دارید؟')) {
        return
      }
      
      try {
        isDeleting.value = true
        const response = await userProfileApi.deleteProfilePicture()
        
        if (response.success) {
          userProfile.value.profilePicture = null
          alert('تصویر پروفایل حذف شد')
        } else {
          alert(response.error || 'خطا در حذف تصویر')
        }
      } catch (error) {
        console.error('Error deleting picture:', error)
        alert('خطا در حذف تصویر')
      } finally {
        isDeleting.value = false
      }
    }
    
    const resetProfileForm = () => {
      profileForm.firstName = userProfile.value.firstName
      profileForm.lastName = userProfile.value.lastName
      profileForm.email = userProfile.value.email
      Object.keys(errors).forEach(key => delete errors[key])
    }
    
    const resetPasswordForm = () => {
      passwordForm.currentPassword = ''
      passwordForm.newPassword = ''
      passwordForm.confirmPassword = ''
      Object.keys(errors).forEach(key => delete errors[key])
    }
    
    const getUserInitials = (firstName, lastName) => {
      const first = firstName ? firstName.charAt(0) : ''
      const last = lastName ? lastName.charAt(0) : ''
      return (first + last).toUpperCase()
    }
    
    const getProfilePictureUrl = (profilePicture) => {
      if (profilePicture.startsWith('http')) {
        return profilePicture
      }
      return `${process.env.VUE_APP_API_URL || 'http://localhost:5000'}${profilePicture}`
    }
    
    const getRoleLabel = (role) => {
      const roleLabels = {
        admin: 'مدیر سیستم',
        manager: 'مدیر',
        accountant: 'حسابدار',
        operator: 'اپراتور',
        user: 'کاربر'
      }
      return roleLabels[role] || role
    }
    
    const formatFileSize = (bytes) => {
      if (bytes === 0) return '0 Bytes'
      const k = 1024
      const sizes = ['Bytes', 'KB', 'MB', 'GB']
      const i = Math.floor(Math.log(bytes) / Math.log(k))
      return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
    }
    
    // Lifecycle
    onMounted(() => {
      loadUserProfile()
    })
    
    return {
      // Data
      activeTab,
      userProfile,
      selectedFile,
      isDragging,
      tabs,
      
      // Forms
      profileForm,
      passwordForm,
      errors,
      
      // Loading states
      isLoading,
      isUpdating,
      isChangingPassword,
      isUploading,
      isDeleting,
      
      // Methods
      updateProfile,
      changePassword,
      handleFileSelect,
      handleDrop,
      uploadProfilePicture,
      deleteProfilePicture,
      resetProfileForm,
      resetPasswordForm,
      getUserInitials,
      getProfilePictureUrl,
      getRoleLabel,
      formatFileSize
    }
  }
}
</script>

<style scoped>
.profile-view {
  max-width: 4xl;
}

.sr-only {
  position: absolute;
  width: 1px;
  height: 1px;
  padding: 0;
  margin: -1px;
  overflow: hidden;
  clip: rect(0, 0, 0, 0);
  white-space: nowrap;
  border: 0;
}
</style>