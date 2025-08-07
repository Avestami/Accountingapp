<template>
  <div class="settings-view">
    <div class="mb-6">
      <h1 class="text-2xl font-bold text-gray-800 mb-2">تنظیمات سیستم</h1>
      <p class="text-gray-600">مدیریت اطلاعات پایه و تنظیمات سیستم حسابداری سفر</p>
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
    
    <!-- Airlines Tab -->
    <div v-if="activeTab === 'airlines'" class="bg-white rounded-lg shadow-md">
      <div class="p-4 border-b border-gray-200 flex justify-between items-center">
        <h2 class="text-lg font-semibold text-gray-800">مدیریت شرکت‌های هواپیمایی</h2>
        <AppButton type="primary" icon="plus" @click="openCreateModal('airline')">
          افزودن شرکت هواپیمایی
        </AppButton>
      </div>
      
      <!-- Search and Filter -->
      <div class="p-4 border-b border-gray-200">
        <div class="flex flex-wrap items-center gap-4">
          <div class="flex-grow">
            <input
              v-model="searchTerm"
              type="text"
              placeholder="جستجو در شرکت‌های هواپیمایی..."
              class="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>
          <div class="w-full md:w-auto">
            <select
              v-model="statusFilter"
              class="w-full md:w-48 border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            >
              <option value="">همه وضعیت‌ها</option>
              <option value="active">فعال</option>
              <option value="inactive">غیرفعال</option>
            </select>
          </div>
        </div>
      </div>
      
      <!-- Airlines Table -->
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead>
            <tr>
              <th class="px-6 py-3 bg-gray-50 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">کد</th>
              <th class="px-6 py-3 bg-gray-50 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">نام شرکت</th>
              <th class="px-6 py-3 bg-gray-50 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">کد IATA</th>
              <th class="px-6 py-3 bg-gray-50 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">کشور</th>
              <th class="px-6 py-3 bg-gray-50 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">وضعیت</th>
              <th class="px-6 py-3 bg-gray-50 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">عملیات</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-if="isLoading">
              <td colspan="6" class="px-6 py-4 text-center">
                <AppSpinner size="sm" color="primary" />
                <span class="mr-2 text-gray-600">در حال بارگذاری...</span>
              </td>
            </tr>
            <tr v-else-if="filteredAirlines.length === 0">
              <td colspan="6" class="px-6 py-4 text-center text-gray-500">
                هیچ شرکت هواپیمایی یافت نشد
              </td>
            </tr>
            <tr v-else v-for="airline in paginatedAirlines" :key="airline.id">
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">{{ airline.code }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{{ airline.name }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{{ airline.iataCode }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{{ airline.country }}</td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span :class="[
                  'inline-flex px-2 py-1 text-xs font-semibold rounded-full',
                  airline.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                ]">
                  {{ airline.isActive ? 'فعال' : 'غیرفعال' }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                <div class="flex space-x-2 space-x-reverse">
                  <button @click="editItem(airline)" class="text-indigo-600 hover:text-indigo-900">ویرایش</button>
                  <button @click="deleteItem('airline', airline.id)" class="text-red-600 hover:text-red-900">حذف</button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      
      <!-- Pagination -->
      <div class="px-6 py-3 border-t border-gray-200 flex items-center justify-between">
        <div class="text-sm text-gray-700">
          نمایش {{ ((currentPage - 1) * itemsPerPage) + 1 }} تا {{ Math.min(currentPage * itemsPerPage, filteredAirlines.length) }} از {{ filteredAirlines.length }} مورد
        </div>
        <div class="flex space-x-2 space-x-reverse">
          <button
            @click="currentPage--"
            :disabled="currentPage === 1"
            class="px-3 py-1 border border-gray-300 rounded-md text-sm disabled:opacity-50 disabled:cursor-not-allowed"
          >
            قبلی
          </button>
          <span class="px-3 py-1 text-sm">صفحه {{ currentPage }} از {{ totalPages }}</span>
          <button
            @click="currentPage++"
            :disabled="currentPage === totalPages"
            class="px-3 py-1 border border-gray-300 rounded-md text-sm disabled:opacity-50 disabled:cursor-not-allowed"
          >
            بعدی
          </button>
        </div>
      </div>
    </div>
    
    <!-- Counterparties Tab -->
    <div v-else-if="activeTab === 'counterparties'" class="bg-white rounded-lg shadow-md">
      <div class="p-4 border-b border-gray-200 flex justify-between items-center">
        <h2 class="text-lg font-semibold text-gray-800">مدیریت طرف‌های حساب</h2>
        <AppButton type="primary" icon="plus" @click="openCreateModal('counterparty')">
          افزودن طرف حساب
        </AppButton>
      </div>
      
      <!-- Search and Filter -->
      <div class="p-4 border-b border-gray-200">
        <div class="flex flex-wrap items-center gap-4">
          <div class="flex-grow">
            <input
              v-model="searchTerm"
              type="text"
              placeholder="جستجو در طرف‌های حساب..."
              class="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>
          <div class="w-full md:w-auto">
            <select
              v-model="typeFilter"
              class="w-full md:w-48 border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            >
              <option value="">همه انواع</option>
              <option value="customer">مشتری</option>
              <option value="supplier">تأمین‌کننده</option>
              <option value="agent">نماینده</option>
            </select>
          </div>
        </div>
      </div>
      
      <!-- Counterparties Table -->
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead>
            <tr>
              <th class="px-6 py-3 bg-gray-50 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">کد</th>
              <th class="px-6 py-3 bg-gray-50 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">نام</th>
              <th class="px-6 py-3 bg-gray-50 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">نوع</th>
              <th class="px-6 py-3 bg-gray-50 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">تلفن</th>
              <th class="px-6 py-3 bg-gray-50 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">ایمیل</th>
              <th class="px-6 py-3 bg-gray-50 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">وضعیت</th>
              <th class="px-6 py-3 bg-gray-50 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">عملیات</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-if="isLoading">
              <td colspan="7" class="px-6 py-4 text-center">
                <AppSpinner size="sm" color="primary" />
                <span class="mr-2 text-gray-600">در حال بارگذاری...</span>
              </td>
            </tr>
            <tr v-else-if="filteredCounterparties.length === 0">
              <td colspan="7" class="px-6 py-4 text-center text-gray-500">
                هیچ طرف حسابی یافت نشد
              </td>
            </tr>
            <tr v-else v-for="counterparty in paginatedCounterparties" :key="counterparty.id">
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">{{ counterparty.code }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{{ counterparty.name }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                <span :class="[
                  'inline-flex px-2 py-1 text-xs font-semibold rounded-full',
                  counterparty.type === 'customer' ? 'bg-blue-100 text-blue-800' :
                  counterparty.type === 'supplier' ? 'bg-green-100 text-green-800' :
                  'bg-purple-100 text-purple-800'
                ]">
                  {{ getCounterpartyTypeLabel(counterparty.type) }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{{ counterparty.phone }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{{ counterparty.email }}</td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span :class="[
                  'inline-flex px-2 py-1 text-xs font-semibold rounded-full',
                  counterparty.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                ]">
                  {{ counterparty.isActive ? 'فعال' : 'غیرفعال' }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                <div class="flex space-x-2 space-x-reverse">
                  <button @click="editItem(counterparty)" class="text-indigo-600 hover:text-indigo-900">ویرایش</button>
                  <button @click="deleteItem('counterparty', counterparty.id)" class="text-red-600 hover:text-red-900">حذف</button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      
      <!-- Pagination -->
      <div class="px-6 py-3 border-t border-gray-200 flex items-center justify-between">
        <div class="text-sm text-gray-700">
          نمایش {{ ((currentPage - 1) * itemsPerPage) + 1 }} تا {{ Math.min(currentPage * itemsPerPage, filteredCounterparties.length) }} از {{ filteredCounterparties.length }} مورد
        </div>
        <div class="flex space-x-2 space-x-reverse">
          <button
            @click="currentPage--"
            :disabled="currentPage === 1"
            class="px-3 py-1 border border-gray-300 rounded-md text-sm disabled:opacity-50 disabled:cursor-not-allowed"
          >
            قبلی
          </button>
          <span class="px-3 py-1 text-sm">صفحه {{ currentPage }} از {{ totalPages }}</span>
          <button
            @click="currentPage++"
            :disabled="currentPage === totalPages"
            class="px-3 py-1 border border-gray-300 rounded-md text-sm disabled:opacity-50 disabled:cursor-not-allowed"
          >
            بعدی
          </button>
        </div>
      </div>
    </div>
    
    <!-- Banks Tab -->
    <div v-else-if="activeTab === 'banks'" class="bg-white rounded-lg shadow-md">
      <div class="p-4 border-b border-gray-200 flex justify-between items-center">
        <h2 class="text-lg font-semibold text-gray-800">مدیریت بانک‌ها</h2>
        <AppButton type="primary" icon="plus" @click="openCreateModal('bank')">
          افزودن بانک
        </AppButton>
      </div>
      
      <!-- Search -->
      <div class="p-4 border-b border-gray-200">
        <input
          v-model="searchTerm"
          type="text"
          placeholder="جستجو در بانک‌ها..."
          class="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
        />
      </div>
      
      <!-- Banks Table -->
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead>
            <tr>
              <th class="px-6 py-3 bg-gray-50 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">کد</th>
              <th class="px-6 py-3 bg-gray-50 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">نام بانک</th>
              <th class="px-6 py-3 bg-gray-50 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">شماره حساب</th>
              <th class="px-6 py-3 bg-gray-50 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">شماره شبا</th>
              <th class="px-6 py-3 bg-gray-50 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">وضعیت</th>
              <th class="px-6 py-3 bg-gray-50 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">عملیات</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-if="isLoading">
              <td colspan="6" class="px-6 py-4 text-center">
                <AppSpinner size="sm" color="primary" />
                <span class="mr-2 text-gray-600">در حال بارگذاری...</span>
              </td>
            </tr>
            <tr v-else-if="filteredBanks.length === 0">
              <td colspan="6" class="px-6 py-4 text-center text-gray-500">
                هیچ بانکی یافت نشد
              </td>
            </tr>
            <tr v-else v-for="bank in paginatedBanks" :key="bank.id">
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">{{ bank.code }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{{ bank.name }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{{ bank.accountNumber }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{{ bank.iban }}</td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span :class="[
                  'inline-flex px-2 py-1 text-xs font-semibold rounded-full',
                  bank.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                ]">
                  {{ bank.isActive ? 'فعال' : 'غیرفعال' }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                <div class="flex space-x-2 space-x-reverse">
                  <button @click="editItem(bank)" class="text-indigo-600 hover:text-indigo-900">ویرایش</button>
                  <button @click="deleteItem('bank', bank.id)" class="text-red-600 hover:text-red-900">حذف</button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      
      <!-- Pagination -->
      <div class="px-6 py-3 border-t border-gray-200 flex items-center justify-between">
        <div class="text-sm text-gray-700">
          نمایش {{ ((currentPage - 1) * itemsPerPage) + 1 }} تا {{ Math.min(currentPage * itemsPerPage, filteredBanks.length) }} از {{ filteredBanks.length }} مورد
        </div>
        <div class="flex space-x-2 space-x-reverse">
          <button
            @click="currentPage--"
            :disabled="currentPage === 1"
            class="px-3 py-1 border border-gray-300 rounded-md text-sm disabled:opacity-50 disabled:cursor-not-allowed"
          >
            قبلی
          </button>
          <span class="px-3 py-1 text-sm">صفحه {{ currentPage }} از {{ totalPages }}</span>
          <button
            @click="currentPage++"
            :disabled="currentPage === totalPages"
            class="px-3 py-1 border border-gray-300 rounded-md text-sm disabled:opacity-50 disabled:cursor-not-allowed"
          >
            بعدی
          </button>
        </div>
      </div>
    </div>
    
    <!-- Users Tab -->
    <div v-else-if="activeTab === 'users'" class="bg-white rounded-lg shadow-md">
      <div class="p-4 border-b border-gray-200 flex justify-between items-center">
        <h2 class="text-lg font-semibold text-gray-800">مدیریت کاربران</h2>
        <AppButton type="primary" icon="plus" @click="openCreateModal('user')">
          افزودن کاربر
        </AppButton>
      </div>
      
      <!-- Search and Filter -->
      <div class="p-4 border-b border-gray-200">
        <div class="flex flex-wrap items-center gap-4">
          <div class="flex-grow">
            <input
              v-model="searchTerm"
              type="text"
              placeholder="جستجو در کاربران..."
              class="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>
          <div class="w-full md:w-auto">
            <select
              v-model="roleFilter"
              class="w-full md:w-48 border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            >
              <option value="">همه نقش‌ها</option>
              <option value="admin">مدیر سیستم</option>
              <option value="manager">مدیر</option>
              <option value="accountant">حسابدار</option>
              <option value="operator">اپراتور</option>
            </select>
          </div>
        </div>
      </div>
      
      <!-- Users Table -->
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead>
            <tr>
              <th class="px-6 py-3 bg-gray-50 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">نام کاربری</th>
              <th class="px-6 py-3 bg-gray-50 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">نام و نام خانوادگی</th>
              <th class="px-6 py-3 bg-gray-50 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">ایمیل</th>
              <th class="px-6 py-3 bg-gray-50 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">نقش</th>
              <th class="px-6 py-3 bg-gray-50 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">آخرین ورود</th>
              <th class="px-6 py-3 bg-gray-50 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">وضعیت</th>
              <th class="px-6 py-3 bg-gray-50 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">عملیات</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-if="isLoading">
              <td colspan="7" class="px-6 py-4 text-center">
                <AppSpinner size="sm" color="primary" />
                <span class="mr-2 text-gray-600">در حال بارگذاری...</span>
              </td>
            </tr>
            <tr v-else-if="filteredUsers.length === 0">
              <td colspan="7" class="px-6 py-4 text-center text-gray-500">
                هیچ کاربری یافت نشد
              </td>
            </tr>
            <tr v-else v-for="user in paginatedUsers" :key="user.id">
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">{{ user.username }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{{ user.fullName }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{{ user.email }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                <span :class="[
                  'inline-flex px-2 py-1 text-xs font-semibold rounded-full',
                  user.role === 'admin' ? 'bg-red-100 text-red-800' :
                  user.role === 'manager' ? 'bg-blue-100 text-blue-800' :
                  user.role === 'accountant' ? 'bg-green-100 text-green-800' :
                  'bg-gray-100 text-gray-800'
                ]">
                  {{ getRoleLabel(user.role) }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{{ formatDate(user.lastLogin) }}</td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span :class="[
                  'inline-flex px-2 py-1 text-xs font-semibold rounded-full',
                  user.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                ]">
                  {{ user.isActive ? 'فعال' : 'غیرفعال' }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                <div class="flex space-x-2 space-x-reverse">
                  <button @click="editItem(user)" class="text-indigo-600 hover:text-indigo-900">ویرایش</button>
                  <button @click="deleteItem('user', user.id)" class="text-red-600 hover:text-red-900">حذف</button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      
      <!-- Pagination -->
      <div class="px-6 py-3 border-t border-gray-200 flex items-center justify-between">
        <div class="text-sm text-gray-700">
          نمایش {{ ((currentPage - 1) * itemsPerPage) + 1 }} تا {{ Math.min(currentPage * itemsPerPage, filteredUsers.length) }} از {{ filteredUsers.length }} مورد
        </div>
        <div class="flex space-x-2 space-x-reverse">
          <button
            @click="currentPage--"
            :disabled="currentPage === 1"
            class="px-3 py-1 border border-gray-300 rounded-md text-sm disabled:opacity-50 disabled:cursor-not-allowed"
          >
            قبلی
          </button>
          <span class="px-3 py-1 text-sm">صفحه {{ currentPage }} از {{ totalPages }}</span>
          <button
            @click="currentPage++"
            :disabled="currentPage === totalPages"
            class="px-3 py-1 border border-gray-300 rounded-md text-sm disabled:opacity-50 disabled:cursor-not-allowed"
          >
            بعدی
          </button>
        </div>
      </div>
    </div>
    
    <!-- Statistics Summary -->
    <div class="mt-6 grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
      <div class="bg-white rounded-lg shadow-md p-4">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-blue-100 rounded-full flex items-center justify-center">
              <svg class="w-5 h-5 text-blue-600" fill="currentColor" viewBox="0 0 20 20">
                <path d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
          </div>
          <div class="mr-4">
            <div class="text-sm font-medium text-gray-500">شرکت‌های هواپیمایی فعال</div>
            <div class="text-2xl font-bold text-gray-900">{{ settingsStore.statistics.airlines.active }}</div>
          </div>
        </div>
      </div>
      
      <div class="bg-white rounded-lg shadow-md p-4">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-green-100 rounded-full flex items-center justify-center">
              <svg class="w-5 h-5 text-green-600" fill="currentColor" viewBox="0 0 20 20">
                <path d="M13 6a3 3 0 11-6 0 3 3 0 016 0zM18 8a2 2 0 11-4 0 2 2 0 014 0zM14 15a4 4 0 00-8 0v3h8v-3z" />
              </svg>
            </div>
          </div>
          <div class="mr-4">
            <div class="text-sm font-medium text-gray-500">طرف‌های حساب فعال</div>
            <div class="text-2xl font-bold text-gray-900">{{ settingsStore.statistics.counterparties.active }}</div>
          </div>
        </div>
      </div>
      
      <div class="bg-white rounded-lg shadow-md p-4">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-yellow-100 rounded-full flex items-center justify-center">
              <svg class="w-5 h-5 text-yellow-600" fill="currentColor" viewBox="0 0 20 20">
                <path d="M4 4a2 2 0 00-2 2v4a2 2 0 002 2V6h10a2 2 0 00-2-2H4zM14 6a2 2 0 012 2v4a2 2 0 01-2 2H6a2 2 0 01-2-2V8a2 2 0 012-2h8z" />
              </svg>
            </div>
          </div>
          <div class="mr-4">
            <div class="text-sm font-medium text-gray-500">بانک‌های فعال</div>
            <div class="text-2xl font-bold text-gray-900">{{ settingsStore.statistics.banks.active }}</div>
          </div>
        </div>
      </div>
      
      <div class="bg-white rounded-lg shadow-md p-4">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-purple-100 rounded-full flex items-center justify-center">
              <svg class="w-5 h-5 text-purple-600" fill="currentColor" viewBox="0 0 20 20">
                <path d="M9 6a3 3 0 11-6 0 3 3 0 016 0zM17 6a3 3 0 11-6 0 3 3 0 016 0zM12.93 17c.046-.327.07-.66.07-1a6.97 6.97 0 00-1.5-4.33A5 5 0 0119 16v1h-6.07zM6 11a5 5 0 015 5v1H1v-1a5 5 0 015-5z" />
              </svg>
            </div>
          </div>
          <div class="mr-4">
            <div class="text-sm font-medium text-gray-500">کاربران فعال</div>
            <div class="text-2xl font-bold text-gray-900">{{ settingsStore.statistics.users.active }}</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { useSettingsStore } from '../stores/settings'
import AppButton from '../components/AppButton.vue'
import AppSpinner from '../components/AppSpinner.vue'

const settingsStore = useSettingsStore()

// State
const activeTab = ref('airlines')
const searchTerm = ref('')
const statusFilter = ref('')
const typeFilter = ref('')
const roleFilter = ref('')
const currentPage = ref(1)
const itemsPerPage = 10
const isLoading = ref(false)

// Tab configuration
const tabs = [
  { key: 'airlines', label: 'شرکت‌های هواپیمایی' },
  { key: 'counterparties', label: 'طرف‌های حساب' },
  { key: 'banks', label: 'بانک‌ها' },
  { key: 'users', label: 'کاربران' }
]

// Computed properties for filtered data
const filteredAirlines = computed(() => {
  let filtered = settingsStore.airlines
  
  if (searchTerm.value) {
    filtered = filtered.filter(airline => 
      airline.name.toLowerCase().includes(searchTerm.value.toLowerCase()) ||
      airline.code.toLowerCase().includes(searchTerm.value.toLowerCase()) ||
      airline.iataCode.toLowerCase().includes(searchTerm.value.toLowerCase())
    )
  }
  
  if (statusFilter.value) {
    filtered = filtered.filter(airline => 
      statusFilter.value === 'active' ? airline.isActive : !airline.isActive
    )
  }
  
  return filtered
})

const filteredCounterparties = computed(() => {
  let filtered = settingsStore.counterparties
  
  if (searchTerm.value) {
    filtered = filtered.filter(counterparty => 
      counterparty.name.toLowerCase().includes(searchTerm.value.toLowerCase()) ||
      counterparty.code.toLowerCase().includes(searchTerm.value.toLowerCase()) ||
      counterparty.email.toLowerCase().includes(searchTerm.value.toLowerCase())
    )
  }
  
  if (typeFilter.value) {
    filtered = filtered.filter(counterparty => counterparty.type === typeFilter.value)
  }
  
  return filtered
})

const filteredBanks = computed(() => {
  let filtered = settingsStore.banks
  
  if (searchTerm.value) {
    filtered = filtered.filter(bank => 
      bank.name.toLowerCase().includes(searchTerm.value.toLowerCase()) ||
      bank.code.toLowerCase().includes(searchTerm.value.toLowerCase()) ||
      bank.accountNumber.includes(searchTerm.value)
    )
  }
  
  return filtered
})

const filteredUsers = computed(() => {
  let filtered = settingsStore.users
  
  if (searchTerm.value) {
    filtered = filtered.filter(user => 
      user.username.toLowerCase().includes(searchTerm.value.toLowerCase()) ||
      user.fullName.toLowerCase().includes(searchTerm.value.toLowerCase()) ||
      user.email.toLowerCase().includes(searchTerm.value.toLowerCase())
    )
  }
  
  if (roleFilter.value) {
    filtered = filtered.filter(user => user.role === roleFilter.value)
  }
  
  return filtered
})

// Computed properties for pagination
const totalPages = computed(() => {
  const currentData = getCurrentTabData()
  return Math.ceil(currentData.length / itemsPerPage)
})

const paginatedAirlines = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  const end = start + itemsPerPage
  return filteredAirlines.value.slice(start, end)
})

const paginatedCounterparties = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  const end = start + itemsPerPage
  return filteredCounterparties.value.slice(start, end)
})

const paginatedBanks = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  const end = start + itemsPerPage
  return filteredBanks.value.slice(start, end)
})

const paginatedUsers = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  const end = start + itemsPerPage
  return filteredUsers.value.slice(start, end)
})

// Methods
function getCurrentTabData() {
  switch (activeTab.value) {
    case 'airlines': return filteredAirlines.value
    case 'counterparties': return filteredCounterparties.value
    case 'banks': return filteredBanks.value
    case 'users': return filteredUsers.value
    default: return []
  }
}

function openCreateModal(type) {
  // Mock create modal functionality
  alert(`باز کردن فرم ایجاد ${getTypeLabel(type)} جدید`)
}

function editItem(item) {
  // Mock edit functionality
  alert(`ویرایش ${item.name || item.username || item.code}`)
}

async function deleteItem(type, id) {
  if (confirm('آیا از حذف این مورد اطمینان دارید؟')) {
    try {
      await settingsStore.deleteItem(type, id)
      alert('مورد با موفقیت حذف شد')
    } catch (error) {
      alert('خطا در حذف مورد: ' + error.message)
    }
  }
}

function getTypeLabel(type) {
  const labels = {
    airline: 'شرکت هواپیمایی',
    counterparty: 'طرف حساب',
    bank: 'بانک',
    user: 'کاربر'
  }
  return labels[type] || type
}

function getCounterpartyTypeLabel(type) {
  const labels = {
    customer: 'مشتری',
    supplier: 'تأمین‌کننده',
    agent: 'نماینده'
  }
  return labels[type] || type
}

function getRoleLabel(role) {
  const labels = {
    admin: 'مدیر سیستم',
    manager: 'مدیر',
    accountant: 'حسابدار',
    operator: 'اپراتور'
  }
  return labels[role] || role
}

function formatDate(dateString) {
  if (!dateString) return '-'
  
  // In a real app, this would convert to Persian date
  const date = new Date(dateString)
  return date.toLocaleDateString('fa-IR')
}

// Watch for tab changes to reset pagination and filters
watch(activeTab, () => {
  currentPage.value = 1
  searchTerm.value = ''
  statusFilter.value = ''
  typeFilter.value = ''
  roleFilter.value = ''
})

// Watch for search/filter changes to reset pagination
watch([searchTerm, statusFilter, typeFilter, roleFilter], () => {
  currentPage.value = 1
})

// Initialize data on mount
onMounted(async () => {
  isLoading.value = true
  try {
    await settingsStore.loadAllData()
  } catch (error) {
    console.error('Error loading settings data:', error)
  } finally {
    isLoading.value = false
  }
})
</script>

<style scoped>
/* Custom styles for better RTL support */
.settings-view {
  direction: rtl;
}

/* Ensure proper spacing for buttons */
.space-x-reverse > * + * {
  margin-right: 0.5rem;
  margin-left: 0;
}

.space-x-2.space-x-reverse > * + * {
  margin-right: 0.5rem;
  margin-left: 0;
}
</style>