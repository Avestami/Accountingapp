<template>
  <div class="bg-white rounded-lg shadow p-6">
    <!-- Chart Header -->
    <div class="flex items-center justify-between mb-4">
      <h3 class="text-lg font-medium text-gray-900">{{ title }}</h3>
      <div class="flex items-center space-x-2 space-x-reverse">
        <!-- Chart Type Selector -->
        <select
          v-model="selectedChartType"
          class="text-sm border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
          @change="updateChartType"
        >
          <option value="line">خطی</option>
          <option value="bar">ستونی</option>
          <option value="pie">دایره‌ای</option>
          <option value="area">ناحیه‌ای</option>
        </select>
        
        <!-- Time Period Selector -->
        <select
          v-model="selectedPeriod"
          class="text-sm border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
          @change="updatePeriod"
        >
          <option value="7d">7 روز گذشته</option>
          <option value="30d">30 روز گذشته</option>
          <option value="3m">3 ماه گذشته</option>
          <option value="6m">6 ماه گذشته</option>
          <option value="1y">سال گذشته</option>
        </select>
      </div>
    </div>

    <!-- Chart Container -->
    <div class="relative">
      <!-- Loading State -->
      <div v-if="isLoading" class="flex items-center justify-center h-64">
        <AppSpinner size="md" color="primary" :show-text="true" text="در حال بارگذاری نمودار..." />
      </div>

      <!-- Chart Placeholder -->
      <div v-else-if="!hasData" class="flex flex-col items-center justify-center h-64 text-gray-500">
        <svg class="w-8 h-8 mb-4 text-gray-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
        </svg>
        <p class="text-sm">داده‌ای برای نمایش وجود ندارد</p>
        <button
          @click="refreshData"
          class="mt-2 text-sm text-blue-600 hover:text-blue-500"
        >
          تلاش مجدد
        </button>
      </div>

      <!-- Mock Chart Display -->
      <div v-else class="h-64 relative">
        <!-- Line Chart Mock -->
        <div v-if="selectedChartType === 'line'" class="h-full flex items-end justify-between px-4 pb-4">
          <div
            v-for="(point, index) in mockLineData"
            :key="index"
            class="flex flex-col items-center"
          >
            <div
              class="w-2 bg-blue-500 rounded-full mb-2"
              :style="{ height: point.value + '%' }"
            ></div>
            <span class="text-xs text-gray-500">{{ point.label }}</span>
          </div>
        </div>

        <!-- Bar Chart Mock -->
        <div v-else-if="selectedChartType === 'bar'" class="h-full flex items-end justify-between px-4 pb-4">
          <div
            v-for="(bar, index) in mockBarData"
            :key="index"
            class="flex flex-col items-center flex-1 mx-1"
          >
            <div
              class="w-full rounded-t-md mb-2"
              :class="bar.color"
              :style="{ height: bar.value + '%' }"
            ></div>
            <span class="text-xs text-gray-500">{{ bar.label }}</span>
          </div>
        </div>

        <!-- Pie Chart Mock -->
        <div v-else-if="selectedChartType === 'pie'" class="h-full flex items-center justify-center">
          <div class="relative">
            <!-- Pie Chart SVG Mock -->
            <svg class="w-32 h-32" viewBox="0 0 100 100">
              <circle
                v-for="(segment, index) in mockPieData"
                :key="index"
                cx="50"
                cy="50"
                r="25"
                fill="none"
                :stroke="segment.color"
                stroke-width="20"
                :stroke-dasharray="segment.dashArray"
                :stroke-dashoffset="segment.dashOffset"
                transform="rotate(-90 50 50)"
              />
            </svg>
            <!-- Legend -->
            <div class="absolute -left-32 top-0 space-y-2">
              <div
                v-for="(segment, index) in mockPieData"
                :key="index"
                class="flex items-center text-sm"
              >
                <div
                  class="w-3 h-3 rounded-full ml-2"
                  :style="{ backgroundColor: segment.color }"
                ></div>
                <span>{{ segment.label }} ({{ segment.percentage }}%)</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Area Chart Mock -->
        <div v-else-if="selectedChartType === 'area'" class="h-full relative">
          <svg class="w-full h-full" viewBox="0 0 400 200">
            <defs>
              <linearGradient id="areaGradient" x1="0%" y1="0%" x2="0%" y2="100%">
                <stop offset="0%" style="stop-color:#3B82F6;stop-opacity:0.3" />
                <stop offset="100%" style="stop-color:#3B82F6;stop-opacity:0" />
              </linearGradient>
            </defs>
            <path
              d="M0,150 Q100,100 200,120 T400,80 L400,200 L0,200 Z"
              fill="url(#areaGradient)"
              stroke="#3B82F6"
              stroke-width="2"
            />
          </svg>
          <!-- X-axis labels -->
          <div class="absolute bottom-0 left-0 right-0 flex justify-between px-4 pb-2">
            <span v-for="label in mockAreaLabels" :key="label" class="text-xs text-gray-500">
              {{ label }}
            </span>
          </div>
        </div>
      </div>
    </div>

    <!-- Chart Statistics -->
    <div v-if="hasData && statistics" class="mt-4 pt-4 border-t border-gray-200">
      <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
        <div
          v-for="(stat, key) in statistics"
          :key="key"
          class="text-center"
        >
          <div class="text-2xl font-bold text-gray-900">{{ stat.value }}</div>
          <div class="text-sm text-gray-500">{{ stat.label }}</div>
          <div
            v-if="stat.change"
            class="text-xs"
            :class="stat.change > 0 ? 'text-green-600' : 'text-red-600'"
          >
            {{ stat.change > 0 ? '+' : '' }}{{ stat.change }}%
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted, watch } from 'vue'
import AppSpinner from './AppSpinner.vue'

export default {
  name: 'DashboardChart',
  components: {
    AppSpinner
  },
  props: {
    title: {
      type: String,
      required: true
    },
    chartType: {
      type: String,
      default: 'line',
      validator: (value) => ['line', 'bar', 'pie', 'area'].includes(value)
    },
    data: {
      type: Array,
      default: () => []
    },
    period: {
      type: String,
      default: '30d'
    },
    statistics: {
      type: Object,
      default: null
    }
  },
  emits: ['chart-type-changed', 'period-changed', 'refresh-requested'],
  setup(props, { emit }) {
    // Reactive data
    const selectedChartType = ref(props.chartType)
    const selectedPeriod = ref(props.period)
    const isLoading = ref(false)
    
    // Computed properties
    const hasData = computed(() => props.data && props.data.length > 0)
    
    // Mock data for different chart types
    const mockLineData = computed(() => [
      { label: 'شنبه', value: 65 },
      { label: 'یکشنبه', value: 45 },
      { label: 'دوشنبه', value: 80 },
      { label: 'سه‌شنبه', value: 55 },
      { label: 'چهارشنبه', value: 90 },
      { label: 'پنج‌شنبه', value: 70 },
      { label: 'جمعه', value: 85 }
    ])
    
    const mockBarData = computed(() => [
      { label: 'فروردین', value: 75, color: 'bg-blue-500' },
      { label: 'اردیبهشت', value: 60, color: 'bg-green-500' },
      { label: 'خرداد', value: 85, color: 'bg-yellow-500' },
      { label: 'تیر', value: 45, color: 'bg-red-500' },
      { label: 'مرداد', value: 90, color: 'bg-purple-500' },
      { label: 'شهریور', value: 70, color: 'bg-pink-500' }
    ])
    
    const mockPieData = computed(() => {
      const data = [
        { label: 'هوایی', percentage: 45, color: '#3B82F6' },
        { label: 'هتل', percentage: 30, color: '#10B981' },
        { label: 'قطار', percentage: 15, color: '#F59E0B' },
        { label: 'اتوبوس', percentage: 10, color: '#EF4444' }
      ]
      
      let cumulativePercentage = 0
      return data.map(item => {
        const dashArray = `${item.percentage} ${100 - item.percentage}`
        const dashOffset = 100 - cumulativePercentage
        cumulativePercentage += item.percentage
        
        return {
          ...item,
          dashArray,
          dashOffset
        }
      })
    })
    
    const mockAreaLabels = computed(() => ['فروردین', 'اردیبهشت', 'خرداد', 'تیر', 'مرداد', 'شهریور'])
    
    // Methods
    const updateChartType = () => {
      emit('chart-type-changed', selectedChartType.value)
    }
    
    const updatePeriod = () => {
      emit('period-changed', selectedPeriod.value)
    }
    
    const refreshData = () => {
      isLoading.value = true
      emit('refresh-requested')
      
      // Simulate loading
      setTimeout(() => {
        isLoading.value = false
      }, 1000)
    }
    
    // Watch for prop changes
    watch(() => props.chartType, (newType) => {
      selectedChartType.value = newType
    })
    
    watch(() => props.period, (newPeriod) => {
      selectedPeriod.value = newPeriod
    })
    
    // Lifecycle
    onMounted(() => {
      // Simulate initial loading
      if (!hasData.value) {
        refreshData()
      }
    })
    
    return {
      // Data
      selectedChartType,
      selectedPeriod,
      isLoading,
      
      // Computed
      hasData,
      mockLineData,
      mockBarData,
      mockPieData,
      mockAreaLabels,
      
      // Methods
      updateChartType,
      updatePeriod,
      refreshData
    }
  }
}
</script>

<style scoped>
/* Chart animations */
.chart-enter-active,
.chart-leave-active {
  transition: all 0.3s ease;
}

.chart-enter-from,
.chart-leave-to {
  opacity: 0;
  transform: translateY(10px);
}

/* Custom scrollbar for legend */
.legend-scroll {
  scrollbar-width: thin;
  scrollbar-color: #cbd5e0 #f7fafc;
}

.legend-scroll::-webkit-scrollbar {
  width: 4px;
}

.legend-scroll::-webkit-scrollbar-track {
  background: #f7fafc;
}

.legend-scroll::-webkit-scrollbar-thumb {
  background-color: #cbd5e0;
  border-radius: 2px;
}

/* Responsive adjustments */
@media (max-width: 640px) {
  .chart-container {
    height: 200px;
  }
  
  .legend {
    position: static;
    margin-top: 1rem;
    display: flex;
    flex-wrap: wrap;
    gap: 0.5rem;
  }
}
</style>