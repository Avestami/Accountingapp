<template>
  <div class="reports-view">
    <div class="page-header">
      <h1 class="page-title">گزارشات</h1>
      <p class="page-description">مشاهده و تولید گزارشات مختلف سیستم</p>
    </div>

    <!-- Report Categories -->
    <div class="reports-grid">
      <!-- Sales Reports -->
      <div class="report-category">
        <div class="category-header">
          <div class="category-icon bg-blue-100 text-blue-600">
            <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"></path>
            </svg>
          </div>
          <div>
            <h3 class="category-title">گزارشات فروش</h3>
            <p class="category-description">گزارشات مربوط به فروش و درآمد</p>
          </div>
        </div>
        <div class="report-items">
          <router-link to="/reports/sales" class="report-item">
            <span class="report-name">گزارش فروش</span>
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path>
            </svg>
          </router-link>
        </div>
      </div>

      <!-- Finance Reports -->
      <div class="report-category">
        <div class="category-header">
          <div class="category-icon bg-green-100 text-green-600">
            <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1"></path>
            </svg>
          </div>
          <div>
            <h3 class="category-title">گزارشات مالی</h3>
            <p class="category-description">گزارشات مربوط به امور مالی</p>
          </div>
        </div>
        <div class="report-items">
          <router-link to="/reports/finance" class="report-item">
            <span class="report-name">گزارش مالی</span>
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path>
            </svg>
          </router-link>
        </div>
      </div>

      <!-- Quick Reports -->
      <div class="report-category">
        <div class="category-header">
          <div class="category-icon bg-purple-100 text-purple-600">
            <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z"></path>
            </svg>
          </div>
          <div>
            <h3 class="category-title">گزارشات سریع</h3>
            <p class="category-description">گزارشات آماده و سریع</p>
          </div>
        </div>
        <div class="report-items">
          <div class="report-item" @click="generateQuickReport('daily')">
            <span class="report-name">گزارش روزانه</span>
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path>
            </svg>
          </div>
          <div class="report-item" @click="generateQuickReport('weekly')">
            <span class="report-name">گزارش هفتگی</span>
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path>
            </svg>
          </div>
          <div class="report-item" @click="generateQuickReport('monthly')">
            <span class="report-name">گزارش ماهانه</span>
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path>
            </svg>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'ReportsView',
  methods: {
    generateQuickReport(type) {
      // Handle quick report generation
      console.log(`Generating ${type} report`);
      // You can implement the logic to generate and download reports here
    }
  }
}
</script>

<style scoped>
.reports-view {
  padding: 24px;
}

.page-header {
  margin-bottom: 32px;
}

.page-title {
  font-size: 28px;
  font-weight: 700;
  color: #1f2937;
  margin-bottom: 8px;
}

.page-description {
  color: #6b7280;
  font-size: 16px;
}

.reports-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
  gap: 24px;
}

.report-category {
  background: white;
  border-radius: 12px;
  border: 1px solid #e5e7eb;
  overflow: hidden;
  transition: all 0.2s ease;
}

.report-category:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  border-color: #d1d5db;
}

.category-header {
  padding: 24px;
  display: flex;
  align-items: center;
  gap: 16px;
  border-bottom: 1px solid #f3f4f6;
}

.category-icon {
  width: 56px;
  height: 56px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.category-title {
  font-size: 18px;
  font-weight: 600;
  color: #1f2937;
  margin-bottom: 4px;
}

.category-description {
  color: #6b7280;
  font-size: 14px;
}

.report-items {
  padding: 16px;
}

.report-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 12px 16px;
  border-radius: 8px;
  transition: all 0.2s ease;
  cursor: pointer;
  text-decoration: none;
  color: #374151;
  margin-bottom: 8px;
}

.report-item:last-child {
  margin-bottom: 0;
}

.report-item:hover {
  background-color: #f9fafb;
  color: #1f2937;
}

.report-name {
  font-weight: 500;
}

/* RTL Support */
[dir="rtl"] .category-header {
  flex-direction: row-reverse;
}

[dir="rtl"] .report-item {
  flex-direction: row-reverse;
}
</style>