<template>
  <div class="reports-view">
    <div class="mb-6">
      <h1 class="text-2xl font-bold text-gray-800 mb-2">گزارش‌های مالی</h1>
      <p class="text-gray-600">گزارش‌های مالی سیستم حسابداری سفر را مشاهده کنید</p>
    </div>
    
    <!-- Report Type Selection -->
    <div class="bg-white rounded-lg shadow-md p-4 mb-6">
      <div class="flex flex-wrap items-center gap-4">
        <div class="flex-grow">
          <label class="block text-sm font-medium text-gray-700 mb-1">نوع گزارش</label>
          <select 
            v-model="selectedReportType" 
            class="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent"
          >
            <option value="balance-sheet">ترازنامه</option>
            <option value="income-statement">صورت سود و زیان</option>
            <option value="cash-flow">صورت جریان وجه نقد</option>
            <option value="trial-balance">تراز آزمایشی</option>
          </select>
        </div>
        
        <div class="w-full md:w-auto flex flex-wrap gap-4">
          <div v-if="selectedReportType === 'balance-sheet' || selectedReportType === 'trial-balance'" class="w-full md:w-auto">
            <label class="block text-sm font-medium text-gray-700 mb-1">تاریخ</label>
            <PersianDatePicker 
              v-model="reportDate" 
              class="w-full md:w-48"
              placeholder="انتخاب تاریخ"
            />
          </div>
          
          <div v-if="selectedReportType === 'income-statement' || selectedReportType === 'cash-flow'" class="w-full md:w-auto">
            <label class="block text-sm font-medium text-gray-700 mb-1">از تاریخ</label>
            <PersianDatePicker 
              v-model="dateFrom" 
              class="w-full md:w-48"
              placeholder="از تاریخ"
            />
          </div>
          
          <div v-if="selectedReportType === 'income-statement' || selectedReportType === 'cash-flow'" class="w-full md:w-auto">
            <label class="block text-sm font-medium text-gray-700 mb-1">تا تاریخ</label>
            <PersianDatePicker 
              v-model="dateTo" 
              class="w-full md:w-48"
              placeholder="تا تاریخ"
            />
          </div>
        </div>
        
        <div class="flex-shrink-0 flex items-end">
          <AppButton 
            type="primary" 
            icon="refresh" 
            :loading="isLoading"
            @click="generateReport"
          >
            تولید گزارش
          </AppButton>
        </div>
      </div>
    </div>
    
    <!-- Loading State -->
    <div v-if="isLoading" class="flex justify-center items-center py-12">
      <AppSpinner size="lg" color="primary" />
      <span class="mr-3 text-gray-600">در حال تولید گزارش...</span>
    </div>
    
    <!-- Error State -->
    <div v-else-if="error" class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-md mb-6">
      <div class="flex">
        <div class="flex-shrink-0">
          <svg class="h-5 w-5 text-red-500" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd" />
          </svg>
        </div>
        <div class="mr-3">
          <p class="text-sm">{{ error }}</p>
        </div>
      </div>
    </div>
    
    <!-- No Report Selected -->
    <div v-else-if="!currentReport" class="bg-gray-50 border border-gray-200 rounded-lg p-8 text-center">
      <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 17v-2m3 2v-4m3 4v-6m2 10H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
      </svg>
      <h3 class="mt-2 text-sm font-medium text-gray-900">گزارشی انتخاب نشده است</h3>
      <p class="mt-1 text-sm text-gray-500">برای مشاهده گزارش، نوع گزارش و بازه زمانی را انتخاب کرده و دکمه تولید گزارش را بزنید.</p>
    </div>
    
    <!-- Balance Sheet Report -->
    <div v-else-if="selectedReportType === 'balance-sheet' && currentReport" class="bg-white rounded-lg shadow-md overflow-hidden">
      <div class="p-4 border-b border-gray-200 flex justify-between items-center">
        <h2 class="text-lg font-semibold text-gray-800">ترازنامه</h2>
        <div class="text-sm text-gray-600">تاریخ: {{ formatDate(currentReport.date) }}</div>
      </div>
      
      <div class="p-4">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <!-- Assets -->
          <div class="border rounded-lg overflow-hidden">
            <div class="bg-gray-50 px-4 py-2 border-b font-medium">دارایی‌ها</div>
            <div class="p-4">
              <h4 class="font-medium mb-2">دارایی‌های جاری</h4>
              <div class="space-y-2 mb-4">
                <div class="flex justify-between">
                  <span>وجه نقد</span>
                  <span>{{ formatCurrency(currentReport.assets.current.cash) }}</span>
                </div>
                <div class="flex justify-between">
                  <span>حساب‌های دریافتنی</span>
                  <span>{{ formatCurrency(currentReport.assets.current.accountsReceivable) }}</span>
                </div>
                <div class="flex justify-between">
                  <span>موجودی کالا</span>
                  <span>{{ formatCurrency(currentReport.assets.current.inventory) }}</span>
                </div>
                <div class="flex justify-between font-medium border-t pt-2">
                  <span>جمع دارایی‌های جاری</span>
                  <span>{{ formatCurrency(currentReport.assets.current.total) }}</span>
                </div>
              </div>
              
              <h4 class="font-medium mb-2">دارایی‌های غیرجاری</h4>
              <div class="space-y-2 mb-4">
                <div class="flex justify-between">
                  <span>تجهیزات</span>
                  <span>{{ formatCurrency(currentReport.assets.nonCurrent.equipment) }}</span>
                </div>
                <div class="flex justify-between">
                  <span>ساختمان</span>
                  <span>{{ formatCurrency(currentReport.assets.nonCurrent.buildings) }}</span>
                </div>
                <div class="flex justify-between font-medium border-t pt-2">
                  <span>جمع دارایی‌های غیرجاری</span>
                  <span>{{ formatCurrency(currentReport.assets.nonCurrent.total) }}</span>
                </div>
              </div>
              
              <div class="flex justify-between font-bold border-t border-gray-400 pt-2">
                <span>جمع کل دارایی‌ها</span>
                <span>{{ formatCurrency(currentReport.assets.total) }}</span>
              </div>
            </div>
          </div>
          
          <!-- Liabilities and Equity -->
          <div>
            <!-- Liabilities -->
            <div class="border rounded-lg overflow-hidden mb-6">
              <div class="bg-gray-50 px-4 py-2 border-b font-medium">بدهی‌ها</div>
              <div class="p-4">
                <h4 class="font-medium mb-2">بدهی‌های جاری</h4>
                <div class="space-y-2 mb-4">
                  <div class="flex justify-between">
                    <span>حساب‌های پرداختنی</span>
                    <span>{{ formatCurrency(currentReport.liabilities.current.accountsPayable) }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span>بدهی کوتاه‌مدت</span>
                    <span>{{ formatCurrency(currentReport.liabilities.current.shortTermDebt) }}</span>
                  </div>
                  <div class="flex justify-between font-medium border-t pt-2">
                    <span>جمع بدهی‌های جاری</span>
                    <span>{{ formatCurrency(currentReport.liabilities.current.total) }}</span>
                  </div>
                </div>
                
                <h4 class="font-medium mb-2">بدهی‌های غیرجاری</h4>
                <div class="space-y-2 mb-4">
                  <div class="flex justify-between">
                    <span>بدهی بلندمدت</span>
                    <span>{{ formatCurrency(currentReport.liabilities.nonCurrent.longTermDebt) }}</span>
                  </div>
                  <div class="flex justify-between font-medium border-t pt-2">
                    <span>جمع بدهی‌های غیرجاری</span>
                    <span>{{ formatCurrency(currentReport.liabilities.nonCurrent.total) }}</span>
                  </div>
                </div>
                
                <div class="flex justify-between font-medium border-t border-gray-400 pt-2">
                  <span>جمع کل بدهی‌ها</span>
                  <span>{{ formatCurrency(currentReport.liabilities.total) }}</span>
                </div>
              </div>
            </div>
            
            <!-- Equity -->
            <div class="border rounded-lg overflow-hidden">
              <div class="bg-gray-50 px-4 py-2 border-b font-medium">حقوق صاحبان سهام</div>
              <div class="p-4">
                <div class="space-y-2 mb-4">
                  <div class="flex justify-between">
                    <span>سرمایه</span>
                    <span>{{ formatCurrency(currentReport.equity.capital) }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span>سود انباشته</span>
                    <span>{{ formatCurrency(currentReport.equity.retainedEarnings) }}</span>
                  </div>
                  <div class="flex justify-between font-medium border-t pt-2">
                    <span>جمع حقوق صاحبان سهام</span>
                    <span>{{ formatCurrency(currentReport.equity.total) }}</span>
                  </div>
                </div>
                
                <div class="flex justify-between font-bold border-t border-gray-400 pt-2">
                  <span>جمع کل بدهی‌ها و حقوق صاحبان سهام</span>
                  <span>{{ formatCurrency(currentReport.totalLiabilitiesAndEquity) }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Income Statement Report -->
    <div v-else-if="selectedReportType === 'income-statement' && currentReport" class="bg-white rounded-lg shadow-md overflow-hidden">
      <div class="p-4 border-b border-gray-200 flex justify-between items-center">
        <h2 class="text-lg font-semibold text-gray-800">صورت سود و زیان</h2>
        <div class="text-sm text-gray-600">دوره: {{ formatDate(currentReport.period.from) }} تا {{ formatDate(currentReport.period.to) }}</div>
      </div>
      
      <div class="p-4">
        <div class="border rounded-lg overflow-hidden">
          <!-- Revenue -->
          <div class="bg-gray-50 px-4 py-2 border-b font-medium">درآمدها</div>
          <div class="p-4 space-y-2">
            <div class="flex justify-between">
              <span>فروش</span>
              <span>{{ formatCurrency(currentReport.revenue.sales) }}</span>
            </div>
            <div class="flex justify-between">
              <span>خدمات</span>
              <span>{{ formatCurrency(currentReport.revenue.services) }}</span>
            </div>
            <div class="flex justify-between">
              <span>سایر درآمدها</span>
              <span>{{ formatCurrency(currentReport.revenue.other) }}</span>
            </div>
            <div class="flex justify-between font-medium border-t pt-2">
              <span>جمع درآمدها</span>
              <span>{{ formatCurrency(currentReport.revenue.total) }}</span>
            </div>
          </div>
          
          <!-- Expenses -->
          <div class="bg-gray-50 px-4 py-2 border-b border-t font-medium">هزینه‌ها</div>
          <div class="p-4 space-y-2">
            <div class="flex justify-between">
              <span>بهای تمام شده کالای فروش رفته</span>
              <span>{{ formatCurrency(currentReport.expenses.costOfGoodsSold) }}</span>
            </div>
            <div class="flex justify-between">
              <span>حقوق و دستمزد</span>
              <span>{{ formatCurrency(currentReport.expenses.salaries) }}</span>
            </div>
            <div class="flex justify-between">
              <span>اجاره</span>
              <span>{{ formatCurrency(currentReport.expenses.rent) }}</span>
            </div>
            <div class="flex justify-between">
              <span>آب و برق و تلفن</span>
              <span>{{ formatCurrency(currentReport.expenses.utilities) }}</span>
            </div>
            <div class="flex justify-between">
              <span>استهلاک</span>
              <span>{{ formatCurrency(currentReport.expenses.depreciation) }}</span>
            </div>
            <div class="flex justify-between">
              <span>سایر هزینه‌ها</span>
              <span>{{ formatCurrency(currentReport.expenses.other) }}</span>
            </div>
            <div class="flex justify-between font-medium border-t pt-2">
              <span>جمع هزینه‌ها</span>
              <span>{{ formatCurrency(currentReport.expenses.total) }}</span>
            </div>
          </div>
          
          <!-- Results -->
          <div class="bg-gray-50 px-4 py-2 border-b border-t font-medium">نتایج</div>
          <div class="p-4 space-y-2">
            <div class="flex justify-between">
              <span>سود ناخالص</span>
              <span>{{ formatCurrency(currentReport.grossProfit) }}</span>
            </div>
            <div class="flex justify-between">
              <span>سود عملیاتی</span>
              <span>{{ formatCurrency(currentReport.operatingIncome) }}</span>
            </div>
            <div class="flex justify-between font-bold border-t pt-2">
              <span>سود خالص</span>
              <span>{{ formatCurrency(currentReport.netIncome) }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Cash Flow Statement -->
    <div v-else-if="selectedReportType === 'cash-flow' && currentReport" class="bg-white rounded-lg shadow-md overflow-hidden">
      <div class="p-4 border-b border-gray-200 flex justify-between items-center">
        <h2 class="text-lg font-semibold text-gray-800">صورت جریان وجه نقد</h2>
        <div class="text-sm text-gray-600">دوره: {{ formatDate(currentReport.period.from) }} تا {{ formatDate(currentReport.period.to) }}</div>
      </div>
      
      <div class="p-4">
        <div class="border rounded-lg overflow-hidden">
          <!-- Operating Activities -->
          <div class="bg-gray-50 px-4 py-2 border-b font-medium">فعالیت‌های عملیاتی</div>
          <div class="p-4 space-y-2">
            <div class="flex justify-between">
              <span>سود خالص</span>
              <span>{{ formatCurrency(currentReport.operating.netIncome) }}</span>
            </div>
            <div class="flex justify-between">
              <span>استهلاک</span>
              <span>{{ formatCurrency(currentReport.operating.depreciation) }}</span>
            </div>
            <div class="flex justify-between">
              <span>تغییر در حساب‌های دریافتنی</span>
              <span>{{ formatCurrency(currentReport.operating.accountsReceivableChange) }}</span>
            </div>
            <div class="flex justify-between">
              <span>تغییر در حساب‌های پرداختنی</span>
              <span>{{ formatCurrency(currentReport.operating.accountsPayableChange) }}</span>
            </div>
            <div class="flex justify-between font-medium border-t pt-2">
              <span>جریان نقد خالص از فعالیت‌های عملیاتی</span>
              <span>{{ formatCurrency(currentReport.operating.total) }}</span>
            </div>
          </div>
          
          <!-- Investing Activities -->
          <div class="bg-gray-50 px-4 py-2 border-b border-t font-medium">فعالیت‌های سرمایه‌گذاری</div>
          <div class="p-4 space-y-2">
            <div class="flex justify-between">
              <span>خرید تجهیزات</span>
              <span>{{ formatCurrency(currentReport.investing.equipmentPurchase) }}</span>
            </div>
            <div class="flex justify-between font-medium border-t pt-2">
              <span>جریان نقد خالص از فعالیت‌های سرمایه‌گذاری</span>
              <span>{{ formatCurrency(currentReport.investing.total) }}</span>
            </div>
          </div>
          
          <!-- Financing Activities -->
          <div class="bg-gray-50 px-4 py-2 border-b border-t font-medium">فعالیت‌های تأمین مالی</div>
          <div class="p-4 space-y-2">
            <div class="flex justify-between">
              <span>دریافت وام</span>
              <span>{{ formatCurrency(currentReport.financing.loanProceeds) }}</span>
            </div>
            <div class="flex justify-between">
              <span>پرداخت سود سهام</span>
              <span>{{ formatCurrency(currentReport.financing.dividendsPaid) }}</span>
            </div>
            <div class="flex justify-between font-medium border-t pt-2">
              <span>جریان نقد خالص از فعالیت‌های تأمین مالی</span>
              <span>{{ formatCurrency(currentReport.financing.total) }}</span>
            </div>
          </div>
          
          <!-- Summary -->
          <div class="bg-gray-50 px-4 py-2 border-b border-t font-medium">خلاصه</div>
          <div class="p-4 space-y-2">
            <div class="flex justify-between font-medium">
              <span>جریان نقد خالص</span>
              <span>{{ formatCurrency(currentReport.netCashFlow) }}</span>
            </div>
            <div class="flex justify-between">
              <span>وجه نقد در ابتدای دوره</span>
              <span>{{ formatCurrency(currentReport.beginningCash) }}</span>
            </div>
            <div class="flex justify-between font-bold border-t pt-2">
              <span>وجه نقد در پایان دوره</span>
              <span>{{ formatCurrency(currentReport.endingCash) }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Trial Balance Report -->
    <div v-else-if="selectedReportType === 'trial-balance' && currentReport" class="bg-white rounded-lg shadow-md overflow-hidden">
      <div class="p-4 border-b border-gray-200 flex justify-between items-center">
        <h2 class="text-lg font-semibold text-gray-800">تراز آزمایشی</h2>
        <div class="text-sm text-gray-600">تاریخ: {{ formatDate(currentReport.date) }}</div>
      </div>
      
      <div class="p-4">
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200">
            <thead>
              <tr>
                <th class="px-6 py-3 bg-gray-50 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">کد حساب</th>
                <th class="px-6 py-3 bg-gray-50 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">نام حساب</th>
                <th class="px-6 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">بدهکار (ریال)</th>
                <th class="px-6 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">بستانکار (ریال)</th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr v-for="account in currentReport.accounts" :key="account.code">
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">{{ account.code }}</td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{{ account.name }}</td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 text-left">
                  {{ account.debit > 0 ? formatNumber(account.debit) : '-' }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 text-left">
                  {{ account.credit > 0 ? formatNumber(account.credit) : '-' }}
                </td>
              </tr>
            </tbody>
            <tfoot>
              <tr class="bg-gray-50">
                <td colspan="2" class="px-6 py-4 whitespace-nowrap text-sm font-bold text-gray-900">جمع</td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-bold text-gray-900 text-left">{{ formatNumber(currentReport.totalDebit) }}</td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-bold text-gray-900 text-left">{{ formatNumber(currentReport.totalCredit) }}</td>
              </tr>
            </tfoot>
          </table>
        </div>
        
        <div class="mt-4 flex justify-end">
          <div class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md" 
               :class="currentReport.isBalanced ? 'text-green-700 bg-green-50' : 'text-red-700 bg-red-50'">
            <svg v-if="currentReport.isBalanced" class="-ml-1 mr-2 h-5 w-5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
            </svg>
            <svg v-else class="-ml-1 mr-2 h-5 w-5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd" />
            </svg>
            <span>{{ currentReport.isBalanced ? 'تراز متوازن است' : 'تراز نامتوازن است' }}</span>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Report Actions -->
    <div v-if="currentReport" class="mt-6 flex justify-end space-x-4 space-x-reverse">
      <AppButton type="secondary" icon="printer" @click="printReport">
        چاپ گزارش
      </AppButton>
      <AppButton type="secondary" icon="download" @click="exportReport">
        خروجی اکسل
      </AppButton>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useFinanceStore } from '../stores/finance'
import PersianDatePicker from '../components/PersianDatePicker.vue'
import AppButton from '../components/AppButton.vue'
import AppSpinner from '../components/AppSpinner.vue'

const financeStore = useFinanceStore()

// State
const selectedReportType = ref('balance-sheet')
const reportDate = ref(new Date().toISOString().split('T')[0])
const dateFrom = ref(new Date(new Date().getFullYear(), 0, 1).toISOString().split('T')[0]) // Start of current year
const dateTo = ref(new Date().toISOString().split('T')[0])
const currentReport = ref(null)
const isLoading = ref(false)
const error = ref(null)

// Computed
const isDateRangeValid = computed(() => {
  if (selectedReportType.value === 'balance-sheet' || selectedReportType.value === 'trial-balance') {
    return !!reportDate.value
  } else {
    return !!dateFrom.value && !!dateTo.value && new Date(dateFrom.value) <= new Date(dateTo.value)
  }
})

// Methods
async function generateReport() {
  if (!isDateRangeValid.value) {
    error.value = 'لطفاً تاریخ‌های معتبر وارد کنید'
    return
  }
  
  error.value = null
  isLoading.value = true
  
  try {
    switch (selectedReportType.value) {
      case 'balance-sheet':
        currentReport.value = await financeStore.generateBalanceSheet(reportDate.value)
        break
      case 'income-statement':
        currentReport.value = await financeStore.generateIncomeStatement(dateFrom.value, dateTo.value)
        break
      case 'cash-flow':
        currentReport.value = await financeStore.generateCashFlowStatement(dateFrom.value, dateTo.value)
        break
      case 'trial-balance':
        currentReport.value = await financeStore.generateTrialBalance(reportDate.value)
        break
      default:
        throw new Error('نوع گزارش نامعتبر است')
    }
  } catch (err) {
    error.value = err.message || 'خطا در تولید گزارش'
    console.error('Error generating report:', err)
  } finally {
    isLoading.value = false
  }
}

function printReport() {
  window.print()
}

function exportReport() {
  // Mock export functionality
  const timestamp = new Date().toISOString().replace(/[:.]/g, '-')
  const filename = `${selectedReportType.value}-${timestamp}.xlsx`
  
  // Show success message (in a real app, this would actually generate and download an Excel file)
  alert(`گزارش با نام ${filename} با موفقیت ذخیره شد.`)
}

// Formatting helpers
function formatCurrency(value) {
  return new Intl.NumberFormat('fa-IR', {
    style: 'currency',
    currency: 'IRR',
    maximumFractionDigits: 0,
    minimumFractionDigits: 0
  }).format(value).replace('ریال', '') + ' ریال'
}

function formatNumber(value) {
  return new Intl.NumberFormat('fa-IR').format(value)
}

function formatDate(dateString) {
  if (!dateString) return ''
  
  // In a real app, this would convert to Persian date
  // For now, just format the ISO date
  const date = new Date(dateString)
  return date.toLocaleDateString('fa-IR')
}

// Initialize with balance sheet report on mount
onMounted(async () => {
  await generateReport()
})
</script>

<style scoped>
@media print {
  .reports-view > div:first-child,
  .bg-white > div:first-child,
  .mt-6 {
    display: none;
  }
  
  .bg-white {
    box-shadow: none;
    border: 1px solid #e5e7eb;
  }
}
</style>