<template>
  <div class="chart-of-accounts">
    <div class="page-header">
      <h1 class="text-2xl font-bold text-gray-900">{{ $t('chartOfAccounts.title') }}</h1>
      <button
        @click="showCreateModal = true"
        class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md flex items-center gap-2"
      >
        <PlusIcon class="w-5 h-5" />
        {{ $t('chartOfAccounts.newAccount') }}
      </button>
    </div>

    <!-- Filters -->
    <div class="bg-white p-4 rounded-lg shadow mb-6">
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">
            {{ $t('chartOfAccounts.accountType') }}
          </label>
          <select
            v-model="filters.accountType"
            @change="loadAccounts"
            class="w-full border border-gray-300 rounded-md px-3 py-2"
          >
            <option value="">{{ $t('common.all') }}</option>
            <option value="Asset">{{ $t('chartOfAccounts.asset') }}</option>
            <option value="Liability">{{ $t('chartOfAccounts.liability') }}</option>
            <option value="Equity">{{ $t('chartOfAccounts.equity') }}</option>
            <option value="Revenue">{{ $t('chartOfAccounts.revenue') }}</option>
            <option value="Expense">{{ $t('chartOfAccounts.expense') }}</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">
            {{ $t('common.status') }}
          </label>
          <select
            v-model="filters.isActive"
            @change="loadAccounts"
            class="w-full border border-gray-300 rounded-md px-3 py-2"
          >
            <option value="">{{ $t('common.all') }}</option>
            <option :value="true">{{ $t('common.active') }}</option>
            <option :value="false">{{ $t('common.inactive') }}</option>
          </select>
        </div>
        <div class="flex items-end">
          <button
            @click="resetFilters"
            class="bg-gray-500 hover:bg-gray-600 text-white px-4 py-2 rounded-md"
          >
            {{ $t('common.reset') }}
          </button>
        </div>
      </div>
    </div>

    <!-- Accounts Tree -->
    <div class="bg-white rounded-lg shadow overflow-hidden">
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                {{ $t('chartOfAccounts.accountCode') }}
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                {{ $t('chartOfAccounts.accountName') }}
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                {{ $t('chartOfAccounts.type') }}
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                {{ $t('chartOfAccounts.balance') }}
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                {{ $t('common.status') }}
              </th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                {{ $t('common.actions') }}
              </th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr
              v-for="account in accounts"
              :key="account.id"
              :class="{ 'bg-gray-50': !account.isActive }"
            >
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                {{ account.fullAccountCode }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                <span :style="{ paddingLeft: getIndentLevel(account) + 'px' }">
                  {{ account.accountName }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                <span
                  class="inline-flex px-2 py-1 text-xs font-semibold rounded-full"
                  :class="getTypeClass(account.type)"
                >
                  {{ $t(`chartOfAccounts.${account.type.toLowerCase()}`) }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ formatCurrency(account.balance, account.currency) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span
                  class="inline-flex px-2 py-1 text-xs font-semibold rounded-full"
                  :class="account.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'"
                >
                  {{ account.isActive ? $t('common.active') : $t('common.inactive') }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <button
                  @click="editAccount(account)"
                  class="text-indigo-600 hover:text-indigo-900 mr-3"
                >
                  {{ $t('common.edit') }}
                </button>
                <button
                  @click="deleteAccount(account)"
                  class="text-red-600 hover:text-red-900"
                  :disabled="!account.isLeafAccount"
                >
                  {{ $t('common.delete') }}
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Create/Edit Modal -->
    <div
      v-if="showCreateModal || showEditModal"
      class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50"
    >
      <div class="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white">
        <div class="mt-3">
          <h3 class="text-lg font-medium text-gray-900 mb-4">
            {{ showCreateModal ? $t('chartOfAccounts.createAccount') : $t('chartOfAccounts.editAccount') }}
          </h3>
          
          <form @submit.prevent="saveAccount">
            <div class="mb-4">
              <label class="block text-sm font-medium text-gray-700 mb-1">
                {{ $t('chartOfAccounts.accountCode') }}
              </label>
              <input
                v-model="accountForm.accountCode"
                type="text"
                required
                class="w-full border border-gray-300 rounded-md px-3 py-2"
              />
            </div>

            <div class="mb-4">
              <label class="block text-sm font-medium text-gray-700 mb-1">
                {{ $t('chartOfAccounts.accountName') }}
              </label>
              <input
                v-model="accountForm.accountName"
                type="text"
                required
                class="w-full border border-gray-300 rounded-md px-3 py-2"
              />
            </div>

            <div class="mb-4">
              <label class="block text-sm font-medium text-gray-700 mb-1">
                {{ $t('chartOfAccounts.description') }}
              </label>
              <textarea
                v-model="accountForm.description"
                rows="3"
                class="w-full border border-gray-300 rounded-md px-3 py-2"
              ></textarea>
            </div>

            <div class="mb-4">
              <label class="block text-sm font-medium text-gray-700 mb-1">
                {{ $t('chartOfAccounts.type') }}
              </label>
              <select
                v-model="accountForm.type"
                required
                class="w-full border border-gray-300 rounded-md px-3 py-2"
              >
                <option value="Asset">{{ $t('chartOfAccounts.asset') }}</option>
                <option value="Liability">{{ $t('chartOfAccounts.liability') }}</option>
                <option value="Equity">{{ $t('chartOfAccounts.equity') }}</option>
                <option value="Revenue">{{ $t('chartOfAccounts.revenue') }}</option>
                <option value="Expense">{{ $t('chartOfAccounts.expense') }}</option>
              </select>
            </div>

            <div class="mb-4">
              <label class="block text-sm font-medium text-gray-700 mb-1">
                {{ $t('chartOfAccounts.parentAccount') }}
              </label>
              <select
                v-model="accountForm.parentAccountId"
                class="w-full border border-gray-300 rounded-md px-3 py-2"
              >
                <option value="">{{ $t('chartOfAccounts.noParent') }}</option>
                <option
                  v-for="account in parentAccounts"
                  :key="account.id"
                  :value="account.id"
                >
                  {{ account.fullAccountCode }} - {{ account.accountName }}
                </option>
              </select>
            </div>

            <div class="mb-4">
              <label class="block text-sm font-medium text-gray-700 mb-1">
                {{ $t('common.currency') }}
              </label>
              <select
                v-model="accountForm.currency"
                class="w-full border border-gray-300 rounded-md px-3 py-2"
              >
                <option value="USD">USD</option>
                <option value="EUR">EUR</option>
                <option value="IRR">IRR</option>
                <option value="AED">AED</option>
              </select>
            </div>

            <div class="mb-6">
              <label class="flex items-center">
                <input
                  v-model="accountForm.isActive"
                  type="checkbox"
                  class="rounded border-gray-300 text-blue-600 shadow-sm focus:border-blue-300 focus:ring focus:ring-blue-200 focus:ring-opacity-50"
                />
                <span class="ml-2 text-sm text-gray-700">{{ $t('common.active') }}</span>
              </label>
            </div>

            <div class="flex justify-end space-x-3">
              <button
                type="button"
                @click="closeModal"
                class="bg-gray-300 hover:bg-gray-400 text-gray-800 px-4 py-2 rounded-md"
              >
                {{ $t('common.cancel') }}
              </button>
              <button
                type="submit"
                :disabled="loading"
                class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md disabled:opacity-50"
              >
                {{ loading ? $t('common.saving') : $t('common.save') }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, reactive, onMounted, computed } from 'vue'
import { PlusIcon } from '@heroicons/vue/24/outline'
import { useI18n } from 'vue-i18n'
import { useToast } from '@/composables/useToast'
import { accountsApi } from '@/services/api'

export default {
  name: 'ChartOfAccountsView',
  components: {
    PlusIcon
  },
  setup() {
    const { t } = useI18n()
    const { showToast } = useToast()
    
    const accounts = ref([])
    const loading = ref(false)
    const showCreateModal = ref(false)
    const showEditModal = ref(false)
    
    const filters = reactive({
      accountType: '',
      isActive: ''
    })
    
    const accountForm = reactive({
      id: null,
      accountCode: '',
      accountName: '',
      description: '',
      type: 'Asset',
      parentAccountId: null,
      currency: 'USD',
      isActive: true
    })

    const parentAccounts = computed(() => {
      return accounts.value.filter(account => 
        account.id !== accountForm.id && account.isActive
      )
    })

    const loadAccounts = async () => {
      try {
        loading.value = true
        const params = {}
        if (filters.accountType) params.accountType = filters.accountType
        if (filters.isActive !== '') params.isActive = filters.isActive
        
        const response = await accountsApi.getAccounts(params)
        accounts.value = response.data
      } catch (error) {
        showToast(t('common.error'), 'error')
        console.error('Error loading accounts:', error)
      } finally {
        loading.value = false
      }
    }

    const resetFilters = () => {
      filters.accountType = ''
      filters.isActive = ''
      loadAccounts()
    }

    const resetForm = () => {
      accountForm.id = null
      accountForm.accountCode = ''
      accountForm.accountName = ''
      accountForm.description = ''
      accountForm.type = 'Asset'
      accountForm.parentAccountId = null
      accountForm.currency = 'USD'
      accountForm.isActive = true
    }

    const editAccount = (account) => {
      accountForm.id = account.id
      accountForm.accountCode = account.accountCode
      accountForm.accountName = account.accountName
      accountForm.description = account.description
      accountForm.type = account.type
      accountForm.parentAccountId = account.parentAccountId
      accountForm.currency = account.currency
      accountForm.isActive = account.isActive
      showEditModal.value = true
    }

    const saveAccount = async () => {
      try {
        loading.value = true
        
        if (showCreateModal.value) {
          await accountsApi.createAccount(accountForm)
          showToast(t('chartOfAccounts.accountCreated'), 'success')
        } else {
          await accountsApi.updateAccount(accountForm.id, accountForm)
          showToast(t('chartOfAccounts.accountUpdated'), 'success')
        }
        
        closeModal()
        await loadAccounts()
      } catch (error) {
        showToast(error.response?.data || t('common.error'), 'error')
      } finally {
        loading.value = false
      }
    }

    const deleteAccount = async (account) => {
      if (!confirm(t('chartOfAccounts.confirmDelete'))) return
      
      try {
        await accountsApi.deleteAccount(account.id)
        showToast(t('chartOfAccounts.accountDeleted'), 'success')
        await loadAccounts()
      } catch (error) {
        showToast(error.response?.data || t('common.error'), 'error')
      }
    }

    const closeModal = () => {
      showCreateModal.value = false
      showEditModal.value = false
      resetForm()
    }

    const getIndentLevel = (account) => {
      const level = (account.fullAccountCode.match(/\./g) || []).length
      return level * 20
    }

    const getTypeClass = (type) => {
      const classes = {
        Asset: 'bg-blue-100 text-blue-800',
        Liability: 'bg-red-100 text-red-800',
        Equity: 'bg-green-100 text-green-800',
        Revenue: 'bg-yellow-100 text-yellow-800',
        Expense: 'bg-purple-100 text-purple-800'
      }
      return classes[type] || 'bg-gray-100 text-gray-800'
    }

    const formatCurrency = (amount, currency) => {
      return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: currency || 'USD'
      }).format(amount)
    }

    onMounted(() => {
      loadAccounts()
    })

    return {
      accounts,
      loading,
      showCreateModal,
      showEditModal,
      filters,
      accountForm,
      parentAccounts,
      loadAccounts,
      resetFilters,
      editAccount,
      saveAccount,
      deleteAccount,
      closeModal,
      getIndentLevel,
      getTypeClass,
      formatCurrency
    }
  }
}
</script>

<style scoped>
.page-header {
  @apply flex justify-between items-center mb-6;
}

.chart-of-accounts {
  @apply p-6;
}
</style>