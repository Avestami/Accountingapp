<template>
  <div class="captcha-input">
    <label v-if="label" :for="inputId" class="block text-sm font-medium text-gray-700 mb-1">
      {{ label }}
      <span v-if="required" class="text-red-500">*</span>
    </label>
    
    <div class="captcha-container">
      <!-- Captcha Display -->
      <div class="captcha-display" @click="refreshCaptcha">
        <canvas 
          ref="captchaCanvas" 
          :width="canvasWidth" 
          :height="canvasHeight"
          class="captcha-canvas"
        ></canvas>
        
        <button 
          type="button" 
          @click.stop="refreshCaptcha"
          class="captcha-refresh"
          :title="refreshTitle"
        >
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"></path>
          </svg>
        </button>
      </div>
      
      <!-- Input Field -->
      <div class="captcha-input-wrapper">
        <input
          :id="inputId"
          ref="input"
          type="text"
          :value="modelValue"
          @input="handleInput"
          @blur="handleBlur"
          :placeholder="placeholder"
          :disabled="disabled"
          :readonly="readonly"
          :maxlength="captchaLength"
          :class="inputClasses"
          :dir="inputDirection"
          autocomplete="off"
          spellcheck="false"
        />
        
        <div v-if="showValidation" class="validation-icon">
          <svg v-if="isValid" class="w-5 h-5 text-green-500" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
          </svg>
          <svg v-else-if="modelValue && !isValid" class="w-5 h-5 text-red-500" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd"></path>
          </svg>
        </div>
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
  name: 'CaptchaInput',
  props: {
    modelValue: {
      type: String,
      default: ''
    },
    label: {
      type: String,
      default: 'کد امنیتی'
    },
    placeholder: {
      type: String,
      default: 'کد امنیتی را وارد کنید'
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
    captchaLength: {
      type: Number,
      default: 5
    },
    canvasWidth: {
      type: Number,
      default: 120
    },
    canvasHeight: {
      type: Number,
      default: 40
    },
    showValidation: {
      type: Boolean,
      default: true
    },
    refreshTitle: {
      type: String,
      default: 'تولید کد جدید'
    },
    rtl: {
      type: Boolean,
      default: true
    },
    caseSensitive: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      inputId: `captcha-input-${Math.random().toString(36).substr(2, 9)}`,
      captchaCode: '',
      isValid: false
    };
  },
  computed: {
    inputClasses() {
      const baseClasses = 'block w-full px-3 py-2 pr-10 border rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent';
      const errorClasses = this.error ? 'border-red-300 text-red-900 placeholder-red-300' : 'border-gray-300';
      const disabledClasses = this.disabled ? 'bg-gray-50 text-gray-500 cursor-not-allowed' : 'bg-white';
      const validClasses = this.isValid ? 'border-green-300' : '';
      
      return [baseClasses, errorClasses, disabledClasses, validClasses].join(' ');
    },
    
    inputDirection() {
      return this.rtl ? 'rtl' : 'ltr';
    }
  },
  watch: {
    modelValue: {
      immediate: true,
      handler(newValue) {
        this.validateCaptcha(newValue);
      }
    }
  },
  mounted() {
    this.generateCaptcha();
  },
  methods: {
    generateCaptcha() {
      // For demo purposes, use a simple predictable captcha
      this.captchaCode = '1234';
      
      // Draw captcha on canvas
      this.drawCaptcha();
      
      // Revalidate current input
      this.validateCaptcha(this.modelValue);
    },
    
    drawCaptcha() {
      const canvas = this.$refs.captchaCanvas;
      if (!canvas) return;
      
      const ctx = canvas.getContext('2d');
      
      // Clear canvas
      ctx.clearRect(0, 0, canvas.width, canvas.height);
      
      // Background
      ctx.fillStyle = '#f8f9fa';
      ctx.fillRect(0, 0, canvas.width, canvas.height);
      
      // Add noise lines
      for (let i = 0; i < 5; i++) {
        ctx.strokeStyle = this.getRandomColor(0.3);
        ctx.lineWidth = 1;
        ctx.beginPath();
        ctx.moveTo(Math.random() * canvas.width, Math.random() * canvas.height);
        ctx.lineTo(Math.random() * canvas.width, Math.random() * canvas.height);
        ctx.stroke();
      }
      
      // Add noise dots
      for (let i = 0; i < 20; i++) {
        ctx.fillStyle = this.getRandomColor(0.4);
        ctx.beginPath();
        ctx.arc(
          Math.random() * canvas.width,
          Math.random() * canvas.height,
          Math.random() * 2,
          0,
          2 * Math.PI
        );
        ctx.fill();
      }
      
      // Draw captcha text
      const fontSize = 20;
      ctx.font = `bold ${fontSize}px Arial`;
      ctx.textAlign = 'center';
      ctx.textBaseline = 'middle';
      
      const charWidth = canvas.width / this.captchaLength;
      
      for (let i = 0; i < this.captchaCode.length; i++) {
        const char = this.captchaCode[i];
        const x = charWidth * i + charWidth / 2;
        const y = canvas.height / 2 + (Math.random() - 0.5) * 8;
        
        // Random rotation
        const angle = (Math.random() - 0.5) * 0.4;
        
        ctx.save();
        ctx.translate(x, y);
        ctx.rotate(angle);
        
        // Character color
        ctx.fillStyle = this.getRandomColor(0.8);
        ctx.fillText(char, 0, 0);
        
        ctx.restore();
      }
      
      // Add border
      ctx.strokeStyle = '#dee2e6';
      ctx.lineWidth = 1;
      ctx.strokeRect(0, 0, canvas.width, canvas.height);
    },
    
    getRandomColor(alpha = 1) {
      const r = Math.floor(Math.random() * 256);
      const g = Math.floor(Math.random() * 256);
      const b = Math.floor(Math.random() * 256);
      return `rgba(${r}, ${g}, ${b}, ${alpha})`;
    },
    
    refreshCaptcha() {
      this.generateCaptcha();
      this.$emit('refresh', this.captchaCode);
    },
    
    handleInput(event) {
      const value = event.target.value;
      this.$emit('update:modelValue', value);
      this.validateCaptcha(value);
    },
    
    handleBlur() {
      this.$emit('blur', this.modelValue);
    },
    
    validateCaptcha(value) {
      if (!value) {
        this.isValid = false;
        return;
      }
      
      const userInput = this.caseSensitive ? value : value.toUpperCase();
      const captchaCode = this.caseSensitive ? this.captchaCode : this.captchaCode.toUpperCase();
      
      this.isValid = userInput === captchaCode;
      
      this.$emit('validate', {
        isValid: this.isValid,
        value: value,
        captchaCode: this.captchaCode
      });
    },
    
    focus() {
      this.$refs.input.focus();
    },
    
    getCaptchaCode() {
      return this.captchaCode;
    },
    
    isValidCaptcha() {
      return this.isValid;
    }
  }
};
</script>

<style scoped>
.captcha-input {
  width: 100%;
}

.captcha-container {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.captcha-display {
  position: relative;
  flex-shrink: 0;
  cursor: pointer;
}

.captcha-canvas {
  border: 1px solid #d1d5db;
  border-radius: 0.25rem;
  background-color: #f9fafb;
}

.captcha-refresh {
  position: absolute;
  top: 0.25rem;
  right: 0.25rem;
  padding: 0.25rem;
  background-color: white;
  background-color: rgba(255, 255, 255, 0.8);
  border-radius: 0.25rem;
  color: #4b5563;
  transition: colors 0.15s ease-in-out;
}

.captcha-refresh:hover {
  background-color: white;
  color: #1f2937;
}

.captcha-input-wrapper {
  position: relative;
  flex: 1;
}

.validation-icon {
  position: absolute;
  top: 0;
  bottom: 0;
  right: 0.75rem;
  display: flex;
  align-items: center;
  pointer-events: none;
}

/* RTL support */
[dir="rtl"] .captcha-container {
  flex-direction: row-reverse;
}

[dir="rtl"] .captcha-refresh {
  right: auto;
  left: 4px;
}

[dir="rtl"] .validation-icon {
  right: auto;
  left: 12px;
}

/* Responsive design */
@media (max-width: 640px) {
  .captcha-container {
    flex-direction: column;
    align-items: stretch;
    gap: 0.5rem;
  }
  
  .captcha-display {
    align-self: center;
  }
}
</style>