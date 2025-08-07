<template>
  <button
    :class="buttonClasses"
    :disabled="disabled || loading"
    @click="handleClick"
    :type="type"
  >
    <span v-if="loading" class="spinner ml-2"></span>
    <slot></slot>
  </button>
</template>

<script>
export default {
  name: 'AppButton',
  props: {
    variant: {
      type: String,
      default: 'primary',
      validator: (value) => ['primary', 'secondary', 'danger', 'success', 'outline'].includes(value)
    },
    size: {
      type: String,
      default: 'md',
      validator: (value) => ['sm', 'md', 'lg'].includes(value)
    },
    disabled: {
      type: Boolean,
      default: false
    },
    loading: {
      type: Boolean,
      default: false
    },
    type: {
      type: String,
      default: 'button',
      validator: (value) => ['button', 'submit', 'reset'].includes(value)
    },
    fullWidth: {
      type: Boolean,
      default: false
    }
  },
  computed: {
    buttonClasses() {
      const baseClasses = 'inline-flex items-center justify-center font-medium rounded transition-colors focus:outline-none focus:ring-2 focus:ring-offset-2';
      
      const variantClasses = {
        primary: 'bg-blue-500 hover:bg-blue-600 text-white focus:ring-blue-500',
        secondary: 'bg-gray-500 hover:bg-gray-600 text-white focus:ring-gray-500',
        danger: 'bg-danger-500 hover:bg-danger-600 text-white focus:ring-danger-500',
        success: 'bg-success-500 hover:bg-success-600 text-white focus:ring-success-500',
        outline: 'border border-gray-300 bg-white hover:bg-gray-50 text-gray-700 focus:ring-primary-500'
      };
      
      const sizeClasses = {
        sm: 'px-3 py-1.5 text-sm',
        md: 'px-4 py-2 text-base',
        lg: 'px-6 py-3 text-lg'
      };
      
      const disabledClasses = this.disabled || this.loading ? 'opacity-50 cursor-not-allowed' : '';
      const widthClasses = this.fullWidth ? 'w-full' : '';
      
      return [
        baseClasses,
        variantClasses[this.variant],
        sizeClasses[this.size],
        disabledClasses,
        widthClasses
      ].filter(Boolean).join(' ');
    }
  },
  methods: {
    handleClick(event) {
      if (!this.disabled && !this.loading) {
        this.$emit('click', event);
      }
    }
  }
};
</script>

<style scoped>
.spinner {
  width: 16px;
  height: 16px;
  border: 2px solid transparent;
  border-top: 2px solid currentColor;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}
</style>