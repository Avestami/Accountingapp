<template>
  <div class="min-h-screen bg-gray-50 rtl">
    <div class="container mx-auto px-4 py-8">
      <div class="bg-white rounded-lg shadow-sm p-6">
        <div class="flex justify-between items-center mb-6">
          <h1 class="text-2xl font-bold text-gray-900">مشاهده سند</h1>
          <div class="flex space-x-2 space-x-reverse">
            <AppButton variant="secondary" @click="goBack">
              بازگشت
            </AppButton>
            <AppButton v-if="voucher.status === 'draft'" @click="editVoucher">
              ویرایش
            </AppButton>
            <AppButton v-if="voucher.status === 'draft'" variant="danger" @click="deleteVoucher">
              حذف
            </AppButton>
          </div>
        </div>

        <div v-if="loading" class="flex justify-center py-8">
          <AppSpinner />
        </div>

        <!-- Voucher Details -->
        <div v-else class="space-y-6">
          <!-- Header Information -->
          <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">شماره سند</label>
              <div class="px-3 py-2 bg-gray-50 border border-gray-300 rounded-md">
                {{ voucher.voucherNumber }}
              </div>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">تاریخ سند</label>
              <div class="px-3 py-2 bg-gray-50 border border-gray-300 rounded-md">
                {{ formatDate(voucher.date) }}
              </div>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">وضعیت</label>
              <div class="px-3 py-2">
                <span :class="getStatusBadgeClass(voucher.status)" class="px-2 py-1 rounded-full text-xs font-medium">
                  {{ getStatusLabel(voucher.status) }}
                </span>
              </div>
            </div>
          </div>

          <!-- Description -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">شرح سند</label>
            <div class="px-3 py-2 bg-gray-50 border border-gray-300 rounded-md">
              {{ voucher.description || 'بدون شرح' }}
            </div>
          </div>

          <!-- Voucher Entries -->
          <div>
            <h3 class="text-lg font-medium text-gray-900 mb-4">اقلام سند</h3>
            <div class="overflow-x-auto">
              <table class="min-w-full divide-y divide-gray-200">
                <thead class="bg-gray-50">
                  <tr>
                    <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                      حساب
                    </th>
                    <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                      شرح
                    </th>
                    <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                      بدهکار
                    </th>
                    <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                      بستانکار
                    </th>
                  </tr>
                </thead>
                <tbody class="bg-white divide-y divide-gray-200">
                  <tr v-for="entry in voucher.entries" :key="entry.id">
                    <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {{ entry.accountCode }} - {{ entry.accountName }}
                    </td>
                    <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {{ entry.description }}
                    </td>
                    <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {{ entry.debit ? formatCurrency(entry.debit) : '-' }}
                    </td>
                    <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {{ entry.credit ? formatCurrency(entry.credit) : '-' }}
                    </td>
                  </tr>
                </tbody>
                <tfoot class="bg-gray-50">
                  <tr>
                    <td colspan="2" class="px-6 py-4 text-sm font-medium text-gray-900">
                      جمع کل:
                    </td>
                    <td class="px-6 py-4 text-sm font-bold text-gray-900">
                      {{ formatCurrency(totalDebit) }}
                    </td>
                    <td class="px-6 py-4 text-sm font-bold text-gray-900">
                      {{ formatCurrency(totalCredit) }}
                    </td>
                  </tr>
                </tfoot>
              </table>
            </div>
          </div>

          <!-- Audit Information -->
          <div class="border-t pt-6">
            <h3 class="text-lg font-medium text-gray-900 mb-4">اطلاعات ثبت</h3>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">تاریخ ایجاد</label>
                <div class="px-3 py-2 bg-gray-50 border border-gray-300 rounded-md">
                  {{ formatDateTime(voucher.createdAt) }}
                </div>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">آخرین بروزرسانی</label>
                <div class="px-3 py-2 bg-gray-50 border border-gray-300 rounded-md">
                  {{ formatDateTime(voucher.updatedAt) }}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import AppButton from '@/components/AppButton.vue'
import AppSpinner from '@/components/AppSpinner.vue'

export default {
  name: 'VoucherView',
  components: {
    AppButton,
    AppSpinner
  },
  setup() {
    const router = useRouter()
    const route = useRoute()
    const loading = ref(true)
    
    const voucher = ref({
      id: null,
      voucherNumber: '',
      date: '',
      description: '',
      status: 'draft',
      entries: [],
      createdAt: null,
      updatedAt: null
    })

    const totalDebit = computed(() => {
      return voucher.value.entries.reduce((sum, entry) => sum + (entry.debit || 0), 0)
    })

    const totalCredit = computed(() => {
      return voucher.value.entries.reduce((sum, entry) => sum + (entry.credit || 0), 0)
    })

    const loadVoucher = async () => {
      try {
        const voucherId = route.params.id
        
        // Simulate API call
        await new Promise(resolve => setTimeout(resolve, 1000))
        
        // Mock voucher data
        voucher.value = {
          id: voucherId,
          voucherNumber: `V${voucherId}`,
          date: '1403/05/15',
          description: 'سند فروش بلیت هواپیما',
          status: 'confirmed',
          entries: [
            {
              id: 1,
              accountCode: '1101',
              accountName: 'صندوق',
              description: 'دریافت وجه بابت فروش بلیت',
              debit: 3000000,
              credit: 0
            },
            {
              id: 2,
              accountCode: '4101',
              accountName: 'درآمد فروش',
              description: 'فروش بلیت هواپیما',
              debit: 0,
              credit: 3000000
            }
          ],
          createdAt: new Date('2024-01-15T10:30:00'),
          updatedAt: new Date('2024-01-15T14:20:00')
        }
        
        loading.value = false
      } catch (error) {
        console.error('Error loading voucher:', error)
        alert('خطا در بارگذاری سند')
        router.push('/vouchers')
      }
    }

    const editVoucher = () => {
      router.push(`/vouchers/${voucher.value.id}/edit`)
    }

    const deleteVoucher = async () => {
      if (confirm('آیا از حذف این سند اطمینان دارید؟')) {
        try {
          // Simulate API call
          console.log('Deleting voucher:', voucher.value.id)
          alert('سند با موفقیت حذف شد.')
          router.push('/vouchers')
        } catch (error) {
          console.error('Error deleting voucher:', error)
          alert('خطا در حذف سند')
        }
      }
    }

    const goBack = () => {
      router.go(-1)
    }

    const formatCurrency = (amount) => {
      return new Intl.NumberFormat('fa-IR').format(amount) + ' ریال'
    }

    const formatDate = (date) => {
      if (!date) return ''
      return new Intl.DateTimeFormat('fa-IR', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit'
      }).format(new Date(date))
    }

    const formatDateTime = (date) => {
      if (!date) return ''
      return new Intl.DateTimeFormat('fa-IR', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      }).format(new Date(date))
    }

    const getStatusLabel = (status) => {
      const labels = {
        draft: 'پیش‌نویس',
        confirmed: 'تأیید شده',
        cancelled: 'لغو شده'
      }
      return labels[status] || status
    }

    const getStatusBadgeClass = (status) => {
      const classes = {
        draft: 'bg-yellow-100 text-yellow-800',
        confirmed: 'bg-green-100 text-green-800',
        cancelled: 'bg-red-100 text-red-800'
      }
      return classes[status] || 'bg-gray-100 text-gray-800'
    }

    onMounted(() => {
      loadVoucher()
    })

    return {
      voucher,
      loading,
      totalDebit,
      totalCredit,
      editVoucher,
      deleteVoucher,
      goBack,
      formatCurrency,
      formatDate,
      formatDateTime,
      getStatusLabel,
      getStatusBadgeClass
    }
  }
}
</script>