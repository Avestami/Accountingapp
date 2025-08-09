import { defineStore } from 'pinia'
import { mockData } from '@/services/mockData'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null,
    token: null,
    isAuthenticated: false,
    permissions: [],
    lastActivity: null,
    sessionTimeout: 8 * 60 * 60 * 1000, // 8 hours in milliseconds
    loginAttempts: 0,
    maxLoginAttempts: 5,
    lockoutTime: 15 * 60 * 1000, // 15 minutes in milliseconds
    lockoutUntil: null,
    isInitialized: false
  }),

  getters: {
    isLoggedIn: (state) => state.isAuthenticated && state.user !== null,
    
    userFullName: (state) => {
      if (!state.user) return ''
      return `${state.user.firstName} ${state.user.lastName}`
    },
    
    userRole: (state) => state.user?.role || 'user',
    
    hasPermission: (state) => {
      return (permission) => {
        if (!state.permissions) return false
        return state.permissions.includes(permission) || state.permissions.includes('admin')
      }
    },
    
    isSessionExpired: (state) => {
      if (!state.lastActivity) return false
      return Date.now() - state.lastActivity > state.sessionTimeout
    },
    
    isAccountLocked: (state) => {
      if (!state.lockoutUntil) return false
      return Date.now() < state.lockoutUntil
    },
    
    remainingLockoutTime: (state) => {
      if (!state.lockoutUntil) return 0
      const remaining = state.lockoutUntil - Date.now()
      return remaining > 0 ? Math.ceil(remaining / 1000) : 0
    }
  },

  actions: {
    async login(credentials) {
      try {
        // Check if account is locked
        if (this.isAccountLocked) {
          throw new Error(`حساب کاربری قفل شده است. ${Math.ceil(this.remainingLockoutTime / 60)} دقیقه صبر کنید.`)
        }

        const { company, username, password } = credentials
        
        // Validate required fields
        if (!company || !username || !password) {
          throw new Error('لطفاً تمام فیلدها را پر کنید')
        }
        
        const requestBody = { company, username, password }
        
        // Call backend API
        const response = await fetch('/api/auth/login', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify(requestBody)
        })
        
        if (!response.ok) {
          // Try mock authentication as fallback
          const mockUser = mockData.users.find(u => 
            u.username === username && 
            u.password === password && 
            u.company === company
          )
          
          if (mockUser) {
            // Reset login attempts on successful mock login
            this.loginAttempts = 0
            this.lockoutUntil = null
            localStorage.removeItem('auth_login_attempts')
            localStorage.removeItem('auth_lockout_until')
            
            // Set user data from mock
            this.user = {
              id: mockUser.id,
              username: mockUser.username,
              firstName: mockUser.firstName,
              lastName: mockUser.lastName,
              name: mockUser.name,
              nameEn: mockUser.nameEn,
              email: mockUser.email,
              role: mockUser.role,
              company: mockUser.company,
              lastLogin: new Date().toISOString(),
              preferences: {}
            }
            
            this.token = 'mock-token-' + Date.now()
            this.isAuthenticated = true
            this.permissions = mockUser.permissions || []
            this.lastActivity = Date.now()
            
            // Store in localStorage for persistence
            localStorage.setItem('auth_token', this.token)
            localStorage.setItem('auth_user', JSON.stringify(this.user))
            localStorage.setItem('auth_permissions', JSON.stringify(this.permissions))
            localStorage.setItem('last_activity', this.lastActivity.toString())
            
            return {
              success: true,
              user: this.user,
              token: this.token
            }
          }
          
          // If mock authentication also fails, handle login attempts
          this.loginAttempts++
          
          // Persist login attempts
          localStorage.setItem('auth_login_attempts', this.loginAttempts.toString())
          
          // Lock account after max attempts
          if (this.loginAttempts >= this.maxLoginAttempts) {
            this.lockoutUntil = Date.now() + this.lockoutTime
            localStorage.setItem('auth_lockout_until', this.lockoutUntil.toString())
            this.loginAttempts = 0
            localStorage.removeItem('auth_login_attempts')
            throw new Error('تعداد تلاش‌های ناموفق زیاد است. حساب کاربری قفل شد.')
          }
          
          throw new Error(`نام کاربری یا رمز عبور اشتباه است. ${this.maxLoginAttempts - this.loginAttempts} تلاش باقی مانده.`)
        }
        
        const loginResult = await response.json()
        
        // Reset login attempts on successful login
        this.loginAttempts = 0
        this.lockoutUntil = null
        localStorage.removeItem('auth_login_attempts')
        localStorage.removeItem('auth_lockout_until')
        
        const token = loginResult.token
        const user = loginResult.user
        
        // Set user data
        this.user = {
          id: user.id,
          username: user.username,
          firstName: user.firstName,
          lastName: user.lastName,
          email: user.email,
          role: user.role,
          company: user.company,
          avatar: user.avatar,
          lastLogin: new Date().toISOString(),
          preferences: user.preferences || {}
        }
        
        this.token = token
        this.isAuthenticated = true
        this.permissions = user.permissions || []
        this.lastActivity = Date.now()
        
        // Store in localStorage for persistence
        localStorage.setItem('auth_token', token)
        localStorage.setItem('auth_user', JSON.stringify(this.user))
        localStorage.setItem('auth_permissions', JSON.stringify(this.permissions))
        localStorage.setItem('last_activity', this.lastActivity.toString())
        
        return {
          success: true,
          user: this.user,
          token: this.token
        }
        
      } catch (error) {
        throw error
      }
    },

    async logout() {
      try {
        // Simulate API call
        await new Promise(resolve => setTimeout(resolve, 500))
        
        // Clear state
        this.user = null
        this.token = null
        this.isAuthenticated = false
        this.permissions = []
        this.lastActivity = null
        this.isInitialized = false
        
        // Clear localStorage
        localStorage.removeItem('auth_token')
        localStorage.removeItem('auth_user')
        localStorage.removeItem('auth_permissions')
        localStorage.removeItem('last_activity')
        
        return { success: true }
        
      } catch (error) {
        console.error('Logout error:', error)
        throw error
      }
    },

    async initializeAuth() {
      try {
        // Prevent multiple initializations
        if (this.isInitialized) {
          return this.isAuthenticated
        }
        
        const token = localStorage.getItem('auth_token')
        const userStr = localStorage.getItem('auth_user')
        const permissionsStr = localStorage.getItem('auth_permissions')
        const lastActivityStr = localStorage.getItem('last_activity')
        const loginAttemptsStr = localStorage.getItem('auth_login_attempts')
        const lockoutUntilStr = localStorage.getItem('auth_lockout_until')
        
        // Restore login attempts and lockout data
        this.loginAttempts = parseInt(loginAttemptsStr) || 0
        this.lockoutUntil = parseInt(lockoutUntilStr) || null
        
        this.isInitialized = true
        
        if (!token || !userStr) {
          return false
        }
        
        const lastActivity = parseInt(lastActivityStr) || 0
        
        // Check if session is expired (only if more than 1 hour has passed since last check)
        const timeSinceLastActivity = Date.now() - lastActivity
        if (timeSinceLastActivity > this.sessionTimeout) {
          // Only logout if session is truly expired and it's been more than the timeout period
          console.warn('Session expired, logging out user')
          await this.logout()
          return false
        }
        
        // Restore state
        this.token = token
        this.user = JSON.parse(userStr)
        this.permissions = JSON.parse(permissionsStr || '[]')
        this.isAuthenticated = true
        this.lastActivity = lastActivity
        
        // Update last activity
        this.updateActivity()
        
        return true
        
      } catch (error) {
        console.error('Auth initialization error:', error)
        this.isInitialized = true
        await this.logout()
        return false
      }
    },

    updateActivity() {
      this.lastActivity = Date.now()
      localStorage.setItem('last_activity', this.lastActivity.toString())
    },

    async changePassword(currentPassword, newPassword, confirmPassword) {
      try {
        if (!this.isAuthenticated) {
          throw new Error('کاربر وارد نشده است')
        }
        
        if (newPassword !== confirmPassword) {
          throw new Error('رمز عبور جدید و تکرار آن یکسان نیستند')
        }
        
        if (newPassword.length < 6) {
          throw new Error('رمز عبور باید حداقل 6 کاراکتر باشد')
        }
        
        // Simulate API call
        await new Promise(resolve => setTimeout(resolve, 1000))
        
        // In real app, verify current password and update
        // For mock, just simulate success
        
        return { success: true, message: 'رمز عبور با موفقیت تغییر یافت' }
        
      } catch (error) {
        console.error('Change password error:', error)
        throw error
      }
    },

    async updateProfile(profileData) {
      try {
        if (!this.isAuthenticated) {
          throw new Error('کاربر وارد نشده است')
        }
        
        // Simulate API call
        await new Promise(resolve => setTimeout(resolve, 1000))
        
        // Update user data
        this.user = {
          ...this.user,
          ...profileData,
          id: this.user.id, // Preserve ID
          username: this.user.username, // Preserve username
          role: this.user.role // Preserve role
        }
        
        // Update localStorage
        localStorage.setItem('auth_user', JSON.stringify(this.user))
        
        return { success: true, user: this.user }
        
      } catch (error) {
        console.error('Update profile error:', error)
        throw error
      }
    },

    async refreshToken() {
      try {
        if (!this.token) {
          throw new Error('No token to refresh')
        }
        
        // Simulate API call
        await new Promise(resolve => setTimeout(resolve, 500))
        
        // Generate new token
        const newToken = `mock_token_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`
        
        this.token = newToken
        this.updateActivity()
        
        localStorage.setItem('auth_token', newToken)
        
        return { success: true, token: newToken }
        
      } catch (error) {
        console.error('Token refresh error:', error)
        await this.logout()
        throw error
      }
    },

    // Helper method to check if user has specific role
    hasRole(role) {
      return this.user?.role === role
    },

    // Helper method to check multiple permissions
    hasAnyPermission(permissions) {
      if (!Array.isArray(permissions)) return false
      return permissions.some(permission => this.hasPermission(permission))
    },

    // Helper method to check all permissions
    hasAllPermissions(permissions) {
      if (!Array.isArray(permissions)) return false
      return permissions.every(permission => this.hasPermission(permission))
    },

    setDemoUser() {
      // Set a demo user for testing purposes
      const demoUser = {
        id: 1,
        username: 'demo',
        name: 'کاربر دمو',
        nameEn: 'Demo User',
        email: 'demo@travel-accounting.com',
        role: 'admin',
        company: 'demo',
        firstName: 'کاربر',
        lastName: 'دمو'
      };
      
      this.user = demoUser;
      this.token = 'demo-token-' + Date.now();
      this.isAuthenticated = true;
      this.permissions = ['admin', 'sales', 'finance', 'reports', 'settings'];
      this.lastActivity = Date.now();
      this.loginAttempts = 0;
      this.lockoutUntil = null;
      
      // Store in localStorage
      localStorage.setItem('auth_token', this.token);
      localStorage.setItem('auth_user', JSON.stringify(this.user));
      localStorage.setItem('auth_permissions', JSON.stringify(this.permissions));
      localStorage.setItem('last_activity', this.lastActivity.toString());
    },

    resetLoginAttempts() {
      this.loginAttempts = 0
      this.lockoutUntil = null
      localStorage.removeItem('auth_login_attempts')
      localStorage.removeItem('auth_lockout_until')
    },
    
    updateActivity() {
      this.lastActivity = Date.now()
      localStorage.setItem('last_activity', this.lastActivity.toString())
    },

    updateUserProfile(profileData) {
      if (this.user) {
        this.user.firstName = profileData.firstName
        this.user.lastName = profileData.lastName
        this.user.email = profileData.email
        this.user.profilePicture = profileData.profilePicture
        
        // Update localStorage
        localStorage.setItem('auth_user', JSON.stringify(this.user))
      }
    }
  }
})