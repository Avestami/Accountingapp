<template>
  <div class="typeahead-select">
    <label v-if="label" :for="inputId" class="block text-sm font-medium text-gray-700 mb-1">
      {{ label }}
      <span v-if="required" class="text-red-500">*</span>
    </label>
    
    <div class="relative">
      <input
        :id="inputId"
        ref="input"
        type="text"
        :value="searchQuery"
        @input="handleInput"
        @focus="handleFocus"
        @blur="handleBlur"
        @keydown="handleKeydown"
        :placeholder="placeholder"
        :disabled="disabled"
        :readonly="readonly"
        :class="inputClasses"
        :dir="inputDirection"
        autocomplete="off"
      />
      
      <div class="absolute inset-y-0 left-3 flex items-center pointer-events-none">
        <svg v-if="!loading" class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
        </svg>
        <div v-else class="spinner w-5 h-5"></div>
      </div>
      
      <div v-if="showClearButton && (selectedItem || searchQuery)" class="absolute inset-y-0 right-3 flex items-center">
        <button
          type="button"
          @click="clearSelection"
          class="text-gray-400 hover:text-gray-600 focus:outline-none"
        >
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
          </svg>
        </button>
      </div>
      
      <button
        v-if="!disabled && !readonly"
        type="button"
        @click="toggleDropdown"
        class="absolute inset-y-0 right-8 flex items-center px-2 text-gray-400 hover:text-gray-600"
      >
        <svg class="w-4 h-4 transform transition-transform" :class="{ 'rotate-180': isDropdownVisible }" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"></path>
        </svg>
      </button>
    </div>
    
    <!-- Dropdown -->
    <div v-if="isDropdownVisible" class="dropdown" @click.stop>
      <div v-if="filteredOptions.length === 0 && !loading" class="no-results">
        {{ noResultsText }}
      </div>
      
      <div v-else class="options-list">
        <button
          v-for="(option, index) in filteredOptions"
          :key="getOptionKey(option)"
          @click="selectOption(option)"
          :class="getOptionClasses(option, index)"
          @mouseenter="highlightedIndex = index"
        >
          <div class="option-content">
            <div class="option-main">
              <span v-html="highlightMatch(getOptionLabel(option), searchQuery)"></span>
            </div>
            <div v-if="getOptionDescription(option)" class="option-description">
              {{ getOptionDescription(option) }}
            </div>
          </div>
          
          <div v-if="multiple && isSelected(option)" class="option-check">
            <svg class="w-4 h-4 text-blue-500" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
            </svg>
          </div>
        </button>
      </div>
      
      <div v-if="allowCreate && searchQuery && !exactMatch" class="create-option">
        <button @click="createOption" class="create-button">
          <svg class="w-4 h-4 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path>
          </svg>
          {{ createText }} "{{ searchQuery }}"
        </button>
      </div>
    </div>
    
    <!-- Selected items (for multiple selection) -->
    <div v-if="multiple && selectedItems.length > 0" class="selected-items">
      <div v-for="item in selectedItems" :key="getOptionKey(item)" class="selected-item">
        <span>{{ getOptionLabel(item) }}</span>
        <button @click="removeItem(item)" class="remove-item">
          <svg class="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
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
  name: 'TypeaheadSelect',
  props: {
    modelValue: {
      type: [Object, Array, String, Number],
      default: null
    },
    options: {
      type: Array,
      default: () => []
    },
    label: {
      type: String,
      default: ''
    },
    placeholder: {
      type: String,
      default: 'جستجو کنید...'
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
    multiple: {
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
    optionLabel: {
      type: String,
      default: 'label'
    },
    optionValue: {
      type: String,
      default: 'value'
    },
    optionDescription: {
      type: String,
      default: 'description'
    },
    searchFields: {
      type: Array,
      default: () => ['label']
    },
    minSearchLength: {
      type: Number,
      default: 0
    },
    maxResults: {
      type: Number,
      default: 50
    },
    allowCreate: {
      type: Boolean,
      default: false
    },
    createText: {
      type: String,
      default: 'ایجاد'
    },
    noResultsText: {
      type: String,
      default: 'نتیجه‌ای یافت نشد'
    },
    loading: {
      type: Boolean,
      default: false
    },
    rtl: {
      type: Boolean,
      default: true
    }
  },
  data() {
    return {
      inputId: `typeahead-select-${Math.random().toString(36).substr(2, 9)}`,
      searchQuery: '',
      isDropdownVisible: false,
      highlightedIndex: -1,
      selectedItem: null,
      selectedItems: []
    };
  },
  computed: {
    inputClasses() {
      const baseClasses = 'block w-full px-3 py-2 pl-10 pr-16 border rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent';
      const errorClasses = this.error ? 'border-red-300 text-red-900 placeholder-red-300' : 'border-gray-300';
      const disabledClasses = this.disabled ? 'bg-gray-50 text-gray-500 cursor-not-allowed' : 'bg-white';
      
      return [baseClasses, errorClasses, disabledClasses].join(' ');
    },
    
    inputDirection() {
      return this.rtl ? 'rtl' : 'ltr';
    },
    
    filteredOptions() {
      if (this.searchQuery.length < this.minSearchLength) {
        return this.options.slice(0, this.maxResults);
      }
      
      const query = this.searchQuery.toLowerCase();
      const filtered = this.options.filter(option => {
        return this.searchFields.some(field => {
          const value = this.getNestedValue(option, field);
          return value && value.toString().toLowerCase().includes(query);
        });
      });
      
      return filtered.slice(0, this.maxResults);
    },
    
    exactMatch() {
      return this.filteredOptions.some(option => 
        this.getOptionLabel(option).toLowerCase() === this.searchQuery.toLowerCase()
      );
    }
  },
  watch: {
    modelValue: {
      immediate: true,
      handler(newValue) {
        if (this.multiple) {
          this.selectedItems = Array.isArray(newValue) ? newValue : [];
        } else {
          this.selectedItem = newValue;
          if (newValue) {
            this.searchQuery = this.getOptionLabel(newValue);
          } else {
            this.searchQuery = '';
          }
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
    handleInput(event) {
      this.searchQuery = event.target.value;
      this.highlightedIndex = -1;
      
      if (!this.isDropdownVisible) {
        this.showDropdown();
      }
      
      // Clear selection if search query doesn't match
      if (!this.multiple && this.selectedItem) {
        if (this.getOptionLabel(this.selectedItem) !== this.searchQuery) {
          this.selectedItem = null;
          this.emitValue();
        }
      }
    },
    
    handleFocus() {
      if (!this.disabled && !this.readonly) {
        this.showDropdown();
      }
    },
    
    handleBlur() {
      // Delay hiding dropdown to allow for clicks
      setTimeout(() => {
        if (!this.$el.contains(document.activeElement)) {
          this.hideDropdown();
          
          // Reset search query if no valid selection
          if (!this.multiple && !this.selectedItem) {
            this.searchQuery = '';
          }
        }
      }, 200);
    },
    
    handleKeydown(event) {
      if (!this.isDropdownVisible) return;
      
      switch (event.key) {
        case 'ArrowDown':
          event.preventDefault();
          this.highlightNext();
          break;
        case 'ArrowUp':
          event.preventDefault();
          this.highlightPrevious();
          break;
        case 'Enter':
          event.preventDefault();
          if (this.highlightedIndex >= 0) {
            this.selectOption(this.filteredOptions[this.highlightedIndex]);
          } else if (this.allowCreate && this.searchQuery && !this.exactMatch) {
            this.createOption();
          }
          break;
        case 'Escape':
          this.hideDropdown();
          break;
      }
    },
    
    handleClickOutside(event) {
      if (!this.$el.contains(event.target)) {
        this.hideDropdown();
      }
    },
    
    showDropdown() {
      this.isDropdownVisible = true;
    },
    
    hideDropdown() {
      this.isDropdownVisible = false;
      this.highlightedIndex = -1;
    },
    
    toggleDropdown() {
      if (this.isDropdownVisible) {
        this.hideDropdown();
      } else {
        this.showDropdown();
        this.$refs.input.focus();
      }
    },
    
    selectOption(option) {
      if (this.multiple) {
        if (this.isSelected(option)) {
          this.removeItem(option);
        } else {
          this.selectedItems.push(option);
          this.emitValue();
        }
        this.searchQuery = '';
      } else {
        this.selectedItem = option;
        this.searchQuery = this.getOptionLabel(option);
        this.hideDropdown();
        this.emitValue();
      }
    },
    
    removeItem(item) {
      const index = this.selectedItems.findIndex(selected => 
        this.getOptionValue(selected) === this.getOptionValue(item)
      );
      if (index > -1) {
        this.selectedItems.splice(index, 1);
        this.emitValue();
      }
    },
    
    clearSelection() {
      if (this.multiple) {
        this.selectedItems = [];
      } else {
        this.selectedItem = null;
      }
      this.searchQuery = '';
      this.emitValue();
      this.$refs.input.focus();
    },
    
    createOption() {
      const newOption = {
        [this.optionLabel]: this.searchQuery,
        [this.optionValue]: this.searchQuery
      };
      
      this.$emit('create', newOption);
      this.selectOption(newOption);
    },
    
    highlightNext() {
      if (this.highlightedIndex < this.filteredOptions.length - 1) {
        this.highlightedIndex++;
      }
    },
    
    highlightPrevious() {
      if (this.highlightedIndex > 0) {
        this.highlightedIndex--;
      }
    },
    
    isSelected(option) {
      if (this.multiple) {
        return this.selectedItems.some(selected => 
          this.getOptionValue(selected) === this.getOptionValue(option)
        );
      } else {
        return this.selectedItem && 
               this.getOptionValue(this.selectedItem) === this.getOptionValue(option);
      }
    },
    
    getOptionClasses(option, index) {
      const baseClasses = 'option-item';
      const classes = [baseClasses];
      
      if (index === this.highlightedIndex) classes.push('highlighted');
      if (this.isSelected(option)) classes.push('selected');
      
      return classes.join(' ');
    },
    
    getOptionLabel(option) {
      if (typeof option === 'string') return option;
      return this.getNestedValue(option, this.optionLabel) || '';
    },
    
    getOptionValue(option) {
      if (typeof option === 'string') return option;
      return this.getNestedValue(option, this.optionValue) || option;
    },
    
    getOptionDescription(option) {
      if (typeof option === 'string') return '';
      return this.getNestedValue(option, this.optionDescription) || '';
    },
    
    getOptionKey(option) {
      return this.getOptionValue(option);
    },
    
    getNestedValue(obj, path) {
      return path.split('.').reduce((current, key) => current && current[key], obj);
    },
    
    highlightMatch(text, query) {
      if (!query) return text;
      
      const regex = new RegExp(`(${query})`, 'gi');
      return text.replace(regex, '<mark class="bg-yellow-200">$1</mark>');
    },
    
    emitValue() {
      let value;
      if (this.multiple) {
        value = this.selectedItems.map(item => this.getOptionValue(item));
      } else {
        value = this.selectedItem ? this.getOptionValue(this.selectedItem) : null;
      }
      this.$emit('update:modelValue', value);
      this.$emit('change', value);
    }
  }
};
</script>

<style scoped>
.typeahead-select {
  position: relative;
}

.dropdown {
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  margin-top: 0.25rem;
  background-color: white;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05);
  z-index: 50;
  max-height: 15rem;
  overflow-y: auto;
}

.no-results {
  padding-left: 1rem;
  padding-right: 1rem;
  padding-top: 0.75rem;
  padding-bottom: 0.75rem;
  color: #6b7280;
  text-align: center;
}

.options-list {
  padding-top: 0.25rem;
  padding-bottom: 0.25rem;
}

.option-item {
  width: 100%;
  padding-left: 1rem;
  padding-right: 1rem;
  padding-top: 0.5rem;
  padding-bottom: 0.5rem;
  text-align: right;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.option-item:hover {
  background-color: #f3f4f6;
}

.option-item:focus {
  outline: none;
  background-color: #f3f4f6;
}

.option-item.highlighted {
  background-color: #f3f4f6;
}

.option-item.selected {
  background-color: #eff6ff;
  color: #1d4ed8;
}

.option-content {
  flex: 1;
  text-align: right;
}

.option-main {
  font-weight: 500;
}

.option-description {
  font-size: 0.875rem;
  color: #6b7280;
  margin-top: 0.25rem;
}

.option-check {
  margin-right: 0.5rem;
}

.create-option {
  border-top: 1px solid #e5e7eb;
  padding: 0.5rem;
}

.create-button {
  width: 100%;
  padding: 0.5rem 0.75rem;
  text-align: right;
  background-color: #f9fafb;
  border-radius: 0.375rem;
  border: 1px dashed #d1d5db;
  color: #4b5563;
  display: flex;
  align-items: center;
  justify-content: center;
}

.create-button:hover {
  background-color: #f3f4f6;
}

.selected-items {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-top: 0.5rem;
}

.selected-item {
  display: inline-flex;
  align-items: center;
  padding: 0.25rem 0.75rem;
  border-radius: 9999px;
  font-size: 0.875rem;
  background-color: #dbeafe;
  color: #1e40af;
}

.remove-item {
  margin-right: 0.25rem;
  color: #2563eb;
  outline: none;
}

.remove-item:hover {
  color: #1e40af;
}

.remove-item:focus {
  outline: none;
}

.spinner {
  border: 2px solid #f3f3f3;
  border-top: 2px solid #3498db;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* RTL specific styles */
[dir="rtl"] .option-content {
  text-align: right;
}

[dir="ltr"] .option-content {
  text-align: left;
}
</style>