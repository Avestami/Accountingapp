<template>
  <div class="currency-input-wrapper">
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
        @blur="handleBlur"
        @focus="handleFocus"
        :placeholder="placeholder"
        :disabled="disabled"
        :readonly="readonly"
        :class="inputClasses"
        :dir="inputDirection"
      />
      
      <div v-if="currency" class="absolute inset-y-0 left-3 flex items-center pointer-events-none">
        <span class="text-gray-500 text-sm">{{ currencySymbol }}</span>
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
  name: 'CurrencyInput',
  props: {
    modelValue: {
      type: [Number, String],
      default: null
    },
    label: {
      type: String,
      default: ''
    },
    placeholder: {
      type: String,
      default: '0'
    },
    currency: {
      type: String,
      default: 'IRR',
      validator: (value) => ['IRR', 'USD', 'EUR'].includes(value)
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
    min: {
      type: Number,
      default: 0
    },
    max: {
      type: Number,
      default: null
    },
    decimalPlaces: {
      type: Number,
      default: 0
    }
  },
  data() {
    return {
      inputId: `currency-input-${Math.random().toString(36).substr(2, 9)}`,
      isFocused: false,
      internalValue: null
    };
  },
  computed: {
    inputClasses() {
      const baseClasses = 'block w-full px-3 py-2 border rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent';
      const errorClasses = this.error ? 'border-red-300 text-red-900 placeholder-red-300' : 'border-gray-300';
      const disabledClasses = this.disabled ? 'bg-gray-50 text-gray-500 cursor-not-allowed' : 'bg-white';
      const paddingClasses = this.currency ? 'pl-12' : '';
      
      return [baseClasses, errorClasses, disabledClasses, paddingClasses].join(' ');
    },
    
    currencySymbol() {
      const symbols = {
        'IRR': 'ریال',
        'USD': '$',
        'EUR': '€'
      };
      return symbols[this.currency] || this.currency;
    },
    
    inputDirection() {
      return this.currency === 'IRR' ? 'rtl' : 'ltr';
    },
    
    displayValue() {
      if (this.internalValue === null || this.internalValue === '') {
        return '';
      }
      
      if (this.isFocused) {
        return this.internalValue.toString();
      }
      
      return this.formatNumber(this.internalValue);
    },
    
    numericValue() {
      if (this.internalValue === null || this.internalValue === '') {
        return null;
      }
      return parseFloat(this.internalValue);
    }
  },
  watch: {
    modelValue: {
      immediate: true,
      handler(newValue) {
        if (newValue !== this.internalValue) {
          this.internalValue = newValue;
        }
      }
    }
  },
  methods: {
    handleInput(event) {
      let value = event.target.value;
      
      // Remove all non-numeric characters except decimal point
      value = value.replace(/[^0-9.]/g, '');
      
      // Handle decimal places
      if (this.decimalPlaces === 0) {
        value = value.replace(/\./g, '');
      } else {
        const parts = value.split('.');
        if (parts.length > 2) {
          value = parts[0] + '.' + parts.slice(1).join('');
        }
        if (parts[1] && parts[1].length > this.decimalPlaces) {
          value = parts[0] + '.' + parts[1].substring(0, this.decimalPlaces);
        }
      }
      
      // Convert to number for validation
      const numValue = parseFloat(value);
      
      // Validate min/max
      if (!isNaN(numValue)) {
        if (this.min !== null && numValue < this.min) {
          return;
        }
        if (this.max !== null && numValue > this.max) {
          return;
        }
      }
      
      this.internalValue = value;
      this.emitValue();
    },
    
    handleFocus() {
      this.isFocused = true;
    },
    
    handleBlur() {
      this.isFocused = false;
      
      // Clean up the value on blur
      if (this.internalValue !== null && this.internalValue !== '') {
        const numValue = parseFloat(this.internalValue);
        if (!isNaN(numValue)) {
          this.internalValue = numValue.toFixed(this.decimalPlaces);
          this.emitValue();
        }
      }
    },
    
    clearValue() {
      this.internalValue = null;
      this.emitValue();
      this.$refs.input.focus();
    },
    
    formatNumber(value) {
      if (value === null || value === '') {
        return '';
      }
      
      const numValue = parseFloat(value);
      if (isNaN(numValue)) {
        return value;
      }
      
      if (this.currency === 'IRR') {
        // Persian number formatting
        return new Intl.NumberFormat('fa-IR', {
          minimumFractionDigits: this.decimalPlaces,
          maximumFractionDigits: this.decimalPlaces
        }).format(numValue);
      } else {
        // English number formatting
        return new Intl.NumberFormat('en-US', {
          minimumFractionDigits: this.decimalPlaces,
          maximumFractionDigits: this.decimalPlaces
        }).format(numValue);
      }
    },
    
    emitValue() {
      const numValue = this.numericValue;
      this.$emit('update:modelValue', numValue);
      this.$emit('change', numValue);
    },
    
    focus() {
      this.$refs.input.focus();
    }
  }
};
</script>

<style scoped>
.currency-input-wrapper {
  position: relative;
}

/* RTL specific styles */
[dir="rtl"] input {
  text-align: right;
}

[dir="ltr"] input {
  text-align: left;
}
</style>