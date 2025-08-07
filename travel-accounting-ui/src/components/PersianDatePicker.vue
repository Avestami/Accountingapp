<template>
  <div class="persian-date-picker">
    <label v-if="label" :for="inputId" class="block text-sm font-medium text-gray-700 mb-1">
      {{ label }}
      <span v-if="required" class="text-red-500">*</span>
    </label>
    
    <div class="relative">
      <input
        :id="inputId"
        ref="input"
        type="text"
        :value="displayValue"
        @input="handleInput"
        @focus="showCalendar"
        @blur="handleBlur"
        :placeholder="placeholder"
        :disabled="disabled"
        :readonly="readonly"
        :class="inputClasses"
        dir="rtl"
        autocomplete="off"
      />
      
      <div class="absolute inset-y-0 left-3 flex items-center pointer-events-none">
        <svg class="w-5 h-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"></path>
        </svg>
      </div>
      
      <div v-if="showClearButton && displayValue" class="absolute inset-y-0 right-3 flex items-center">
        <button
          type="button"
          @click="clearValue"
          class="text-gray-400 hover:text-gray-600 focus:outline-none"
        >
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
          </svg>
        </button>
      </div>
    </div>
    
    <!-- Calendar Dropdown -->
    <div v-if="isCalendarVisible" class="calendar-dropdown" @click.stop>
      <div class="calendar-header">
        <button @click="previousMonth" class="nav-button">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7"></path>
          </svg>
        </button>
        
        <div class="month-year">
          <span>{{ currentMonthName }} {{ currentYear }}</span>
        </div>
        
        <div class="calendar-toggle">
          <label class="flex items-center cursor-pointer">
            <span class="mr-2 text-sm">{{ calendarType === 'persian' ? 'شمسی' : 'میلادی' }}</span>
            <div class="relative">
              <input type="checkbox" class="sr-only" @change="toggleCalendarType">
              <div class="w-10 h-4 bg-gray-300 rounded-full shadow-inner"></div>
              <div class="dot absolute w-6 h-6 bg-white rounded-full shadow -left-1 -top-1 transition"></div>
            </div>
          </label>
        </div>
        
        <button @click="nextMonth" class="nav-button">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path>
          </svg>
        </button>
      </div>
      
      <div class="calendar-grid">
        <div class="weekdays">
          <div v-for="day in weekDays" :key="day" class="weekday">{{ day }}</div>
        </div>
        
        <div class="days">
          <button
            v-for="day in calendarDays"
            :key="day.key"
            @click="selectDate(day)"
            :class="getDayClasses(day)"
            :disabled="day.disabled"
          >
            {{ day.persianDay }}
          </button>
        </div>
      </div>
      
      <div class="calendar-footer">
        <button @click="goToToday" class="today-button">
          برو به امروز
        </button>
        <button @click="hideCalendar" class="close-button">
          بستن
        </button>
      </div>
    </div>
    
    <div v-if="error" class="mt-1 text-sm text-red-600">
      {{ error }}
    </div>
    
    <div v-if="hint" class="mt-1 text-sm text-gray-500">
      {{ hint }}
    </div>
  </div>
</template>

<script>
export default {
  name: 'PersianDatePicker',
  props: {
    modelValue: {
      type: [String, Date],
      default: null
    },
    label: {
      type: String,
      default: ''
    },
    placeholder: {
      type: String,
      default: 'تاریخ را انتخاب کنید'
    },
    disabled: {
      type: Boolean,
      default: false
    },
    readonly: {
      type: Boolean,
      default: false
    },
    required: {
      type: Boolean,
      default: false
    },
    error: {
      type: String,
      default: ''
    },
    hint: {
      type: String,
      default: ''
    },
    showClearButton: {
      type: Boolean,
      default: true
    },
    minDate: {
      type: [String, Date],
      default: null
    },
    maxDate: {
      type: [String, Date],
      default: null
    }
  },
  data() {
    return {
      inputId: `persian-date-picker-${Math.random().toString(36).substr(2, 9)}`,
      isCalendarVisible: false,
      currentDate: new Date(),
      selectedDate: null,
      calendarType: 'persian', // 'persian' or 'gregorian'
      weekDays: ['ش', 'ی', 'د', 'س', 'چ', 'پ', 'ج'],
      persianMonths: [
        'فروردین', 'اردیبهشت', 'خرداد', 'تیر', 'مرداد', 'شهریور',
        'مهر', 'آبان', 'آذر', 'دی', 'بهمن', 'اسفند'
      ]
    };
  },
  computed: {
    inputClasses() {
      const baseClasses = 'block w-full px-3 py-2 pl-10 pr-10 border rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent';
      const errorClasses = this.error ? 'border-red-300 text-red-900 placeholder-red-300' : 'border-gray-300';
      const disabledClasses = this.disabled ? 'bg-gray-50 text-gray-500 cursor-not-allowed' : 'bg-white';
      
      return [baseClasses, errorClasses, disabledClasses].join(' ');
    },
    
    displayValue() {
      if (!this.selectedDate) return '';
      if (this.calendarType === 'persian') {
        return this.formatPersianDate(this.selectedDate);
      }
      return this.selectedDate.toLocaleDateString('en-US');
    },
    
    currentYear() {
      if (this.calendarType === 'persian') {
        return this.toPersianYear(this.currentDate.getFullYear());
      }
      return this.currentDate.getFullYear();
    },
    
    currentMonthName() {
      if (this.calendarType === 'persian') {
        const persianMonth = this.toPersianMonth(this.currentDate.getMonth() + 1);
        return this.persianMonths[persianMonth - 1];
      }
      return this.currentDate.toLocaleString('en-US', { month: 'long' });
    },
    
    calendarDays() {
      const year = this.currentDate.getFullYear();
      const month = this.currentDate.getMonth();
      
      const firstDay = new Date(year, month, 1);
      const lastDay = new Date(year, month + 1, 0);
      const startDate = new Date(firstDay);
      
      // Adjust for Persian week (Saturday start) or Gregorian (Sunday start)
      const dayOfWeek = this.calendarType === 'persian' ? (firstDay.getDay() + 1) % 7 : firstDay.getDay();
      startDate.setDate(startDate.getDate() - dayOfWeek);
      
      const days = [];
      const current = new Date(startDate);
      
      for (let i = 0; i < 42; i++) {
        const isCurrentMonth = current.getMonth() === month;
        const isToday = this.isSameDay(current, new Date());
        const isSelected = this.selectedDate && this.isSameDay(current, this.selectedDate);
        const isDisabled = this.isDateDisabled(current);
        
        days.push({
          key: `${current.getFullYear()}-${current.getMonth()}-${current.getDate()}`,
          date: new Date(current),
          day: current.getDate(),
          persianDay: this.calendarType === 'persian' ? this.toPersianNumber(current.getDate()) : current.getDate(),
          isCurrentMonth,
          isToday,
          isSelected,
          disabled: isDisabled
        });
        
        current.setDate(current.getDate() + 1);
      }
      
      return days;
    }
  },
  watch: {
    modelValue: {
      immediate: true,
      handler(newValue) {
        if (newValue) {
          this.selectedDate = typeof newValue === 'string' ? new Date(newValue) : new Date(newValue);
        } else {
          this.selectedDate = null;
        }
      }
    }
  },
  mounted() {
    document.addEventListener('click', this.handleClickOutside);
  },
  beforeUnmount() {
    document.removeEventListener('click', this.handleClickOutside);
  },
  methods: {
      toggleCalendarType() {
        this.calendarType = this.calendarType === 'persian' ? 'gregorian' : 'persian';
      },
      goToToday() {
        this.currentDate = new Date();
        this.selectDate({ date: new Date(), isCurrentMonth: true });
      },
    handleInput(event) {
      const value = event.target.value;
      // Basic validation for Persian date format (YYYY/MM/DD)
      const persianDateRegex = /^[۰-۹]{4}\/[۰-۹]{1,2}\/[۰-۹]{1,2}$/;
      
      if (persianDateRegex.test(value)) {
        try {
          const date = this.parsePersianDate(value);
          if (date && !this.isDateDisabled(date)) {
            this.selectedDate = date;
            this.emitValue();
          }
        } catch (e) {
          // Invalid date format
        }
      }
    },
    
    handleBlur() {
      // Delay hiding calendar to allow for clicks
      setTimeout(() => {
        if (!this.$el.contains(document.activeElement)) {
          this.hideCalendar();
        }
      }, 200);
    },
    
    showCalendar() {
      if (!this.disabled && !this.readonly) {
        this.isCalendarVisible = true;
        if (this.selectedDate) {
          this.currentDate = new Date(this.selectedDate);
        }
      }
    },
    
    hideCalendar() {
      this.isCalendarVisible = false;
    },
    
    handleClickOutside(event) {
      if (!this.$el.contains(event.target)) {
        this.hideCalendar();
      }
    },
    
    selectDate(day) {
      if (!day.disabled) {
        this.selectedDate = new Date(day.date);
        this.emitValue();
        this.hideCalendar();
      }
    },
    
    selectToday() {
      const today = new Date();
      if (!this.isDateDisabled(today)) {
        this.selectedDate = today;
        this.currentDate = new Date(today);
        this.emitValue();
        this.hideCalendar();
      }
    },
    
    clearValue() {
      this.selectedDate = null;
      this.emitValue();
      this.$refs.input.focus();
    },
    
    previousMonth() {
      this.currentDate = new Date(this.currentDate.getFullYear(), this.currentDate.getMonth() - 1, 1);
    },
    
    nextMonth() {
      this.currentDate = new Date(this.currentDate.getFullYear(), this.currentDate.getMonth() + 1, 1);
    },
    
    getDayClasses(day) {
      const baseClasses = 'day-button';
      const classes = [baseClasses];
      
      if (!day.isCurrentMonth) classes.push('other-month');
      if (day.isToday) classes.push('today');
      if (day.isSelected) classes.push('selected');
      if (day.disabled) classes.push('disabled');
      
      return classes.join(' ');
    },
    
    isDateDisabled(date) {
      if (this.minDate) {
        const min = typeof this.minDate === 'string' ? new Date(this.minDate) : this.minDate;
        if (date < min) return true;
      }
      
      if (this.maxDate) {
        const max = typeof this.maxDate === 'string' ? new Date(this.maxDate) : this.maxDate;
        if (date > max) return true;
      }
      
      return false;
    },
    
    isSameDay(date1, date2) {
      return date1.getFullYear() === date2.getFullYear() &&
             date1.getMonth() === date2.getMonth() &&
             date1.getDate() === date2.getDate();
    },
    
    formatPersianDate(date) {
      const year = this.toPersianNumber(date.getFullYear());
      const month = this.toPersianNumber(date.getMonth() + 1).padStart(2, '۰');
      const day = this.toPersianNumber(date.getDate()).padStart(2, '۰');
      return `${year}/${month}/${day}`;
    },
    
    parsePersianDate(persianDateString) {
      const parts = persianDateString.split('/');
      if (parts.length !== 3) return null;
      
      const year = this.toEnglishNumber(parts[0]);
      const month = this.toEnglishNumber(parts[1]) - 1;
      const day = this.toEnglishNumber(parts[2]);
      
      return new Date(year, month, day);
    },
    
    toPersianNumber(num) {
      const persianDigits = '۰۱۲۳۴۵۶۷۸۹';
      return num.toString().replace(/\d/g, (digit) => persianDigits[digit]);
    },
    
    toEnglishNumber(persianNum) {
      const persianDigits = '۰۱۲۳۴۵۶۷۸۹';
      const englishDigits = '0123456789';
      return parseInt(persianNum.toString().replace(/[۰-۹]/g, (digit) => {
        return englishDigits[persianDigits.indexOf(digit)];
      }));
    },
    
    toPersianYear(gregorianYear) {
      // Simplified conversion - in real app, use proper Persian calendar library
      return gregorianYear - 621;
    },
    
    toPersianMonth(gregorianMonth) {
      // Simplified conversion - in real app, use proper Persian calendar library
      return gregorianMonth;
    },
    
    emitValue() {
      const value = this.selectedDate ? this.selectedDate.toISOString().split('T')[0] : null;
      this.$emit('update:modelValue', value);
      this.$emit('change', value);
    }
  }
};
</script>

<style scoped>
.calendar-toggle {
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 1rem;
}

.calendar-toggle .relative {
  display: inline-block;
}

.calendar-toggle input:checked ~ .dot {
  transform: translateX(100%);
  background-color: #4a90e2;
}
.persian-date-picker {
  position: relative;
}

.calendar-dropdown {
  position: absolute;
  top: 100%;
  left: 0;
  margin-top: 0.25rem;
  background-color: white;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05);
  z-index: 50;
  padding: 1rem;
  min-width: 280px;
  direction: rtl;
}

.calendar-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1rem;
}

.nav-button {
  padding: 0.25rem;
  border-radius: 0.375rem;
  outline: none;
}

.nav-button:hover {
  background-color: #f3f4f6;
}

.nav-button:focus {
  outline: none;
  box-shadow: 0 0 0 2px #3b82f6;
}

.month-year {
  font-weight: 500;
  color: #111827;
}

.calendar-grid {
  margin-bottom: 1rem;
}

.weekdays {
  display: grid;
  grid-template-columns: repeat(7, minmax(0, 1fr));
  gap: 0.25rem;
  margin-bottom: 0.5rem;
}

.weekday {
  text-align: center;
  font-size: 0.875rem;
  font-weight: 500;
  color: #6b7280;
  padding: 0.5rem;
}

.days {
  display: grid;
  grid-template-columns: repeat(7, minmax(0, 1fr));
  gap: 0.25rem;
}

.day-button {
  width: 2rem;
  height: 2rem;
  font-size: 0.875rem;
  border-radius: 0.375rem;
  outline: none;
}

.day-button:hover {
  background-color: #f3f4f6;
}

.day-button:focus {
  outline: none;
  box-shadow: 0 0 0 2px #3b82f6;
}

.day-button.other-month {
  color: #9ca3af;
}

.day-button.today {
  background-color: #dbeafe;
  color: #2563eb;
  font-weight: 500;
}

.day-button.selected {
  background-color: #3b82f6;
  color: white;
}

.day-button.disabled {
  color: #d1d5db;
  cursor: not-allowed;
}

.calendar-footer {
  display: flex;
  justify-content: space-between;
}

.today-button,
.close-button {
  padding: 0.25rem 0.75rem;
  font-size: 0.875rem;
  border-radius: 0.375rem;
  border: 1px solid;
  outline: none;
}

.today-button:focus,
.close-button:focus {
  outline: none;
  box-shadow: 0 0 0 2px #3b82f6;
}

.today-button {
  background-color: #3b82f6;
  color: white;
  border-color: #3b82f6;
}

.today-button:hover {
  background-color: #2563eb;
}

.close-button {
  background-color: white;
  color: #374151;
  border-color: #d1d5db;
}

.close-button:hover {
  background-color: #f9fafb;
}
</style>