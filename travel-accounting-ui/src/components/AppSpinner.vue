<template>
  <div :class="containerClasses">
    <div :class="spinnerClasses" :style="spinnerStyle">
      <div class="spinner-inner"></div>
    </div>
    <div v-if="text" :class="textClasses">
      {{ text }}
    </div>
  </div>
</template>

<script>
export default {
  name: 'AppSpinner',
  props: {
    size: {
      type: String,
      default: 'md',
      validator: value => ['xs', 'sm', 'md', 'lg', 'xl'].includes(value)
    },
    color: {
      type: String,
      default: 'primary',
      validator: value => ['primary', 'secondary', 'white', 'gray'].includes(value)
    },
    text: {
      type: String,
      default: ''
    },
    overlay: {
      type: Boolean,
      default: false
    },
    centered: {
      type: Boolean,
      default: false
    },
    inline: {
      type: Boolean,
      default: false
    }
  },
  computed: {
    containerClasses() {
      const classes = [];
      
      if (this.overlay) {
        classes.push('fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50');
      } else if (this.centered) {
        classes.push('flex items-center justify-center');
      } else if (this.inline) {
        classes.push('inline-flex items-center');
      } else {
        classes.push('flex items-center');
      }
      
      return classes.join(' ');
    },
    
    spinnerClasses() {
      const classes = ['spinner'];
      
      // Size classes
      switch (this.size) {
        case 'xs':
          classes.push('w-3 h-3');
          break;
        case 'sm':
          classes.push('w-4 h-4');
          break;
        case 'md':
          classes.push('w-6 h-6');
          break;
        case 'lg':
          classes.push('w-8 h-8');
          break;
        case 'xl':
          classes.push('w-12 h-12');
          break;
      }
      
      return classes.join(' ');
    },
    
    spinnerStyle() {
      const colors = {
        primary: '#3B82F6',
        secondary: '#6B7280',
        white: '#FFFFFF',
        gray: '#9CA3AF'
      };
      
      return {
        borderTopColor: colors[this.color] || colors.primary
      };
    },
    
    textClasses() {
      const classes = ['spinner-text'];
      
      if (this.inline) {
        classes.push('mr-2');
      } else {
        classes.push('mt-2');
      }
      
      // Size-based text classes
      switch (this.size) {
        case 'xs':
        case 'sm':
          classes.push('text-xs');
          break;
        case 'md':
          classes.push('text-sm');
          break;
        case 'lg':
        case 'xl':
          classes.push('text-base');
          break;
      }
      
      // Color classes
      switch (this.color) {
        case 'white':
          classes.push('text-white');
          break;
        case 'gray':
          classes.push('text-gray-500');
          break;
        default:
          classes.push('text-gray-700');
      }
      
      return classes.join(' ');
    }
  }
};
</script>

<style scoped>
.spinner {
  border: 2px solid #f3f3f3;
  border-radius: 50%;
  border-top: 2px solid #3B82F6;
  animation: spin 1s linear infinite;
}

.spinner-inner {
  width: 100%;
  height: 100%;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* RTL support */
[dir="rtl"] .spinner-text.mr-2 {
  margin-right: 0;
  margin-left: 0.5rem;
}
</style>