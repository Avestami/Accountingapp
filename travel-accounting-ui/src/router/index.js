import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

// Import views
import LoginView from '@/views/LoginView.vue'
import DashboardView from '@/views/DashboardView.vue'
import ProfileView from '@/views/ProfileView.vue'
import ChartOfAccountsView from '@/views/ChartOfAccountsView.vue'
import UnissuedTicketsView from '@/views/sales/UnissuedTicketsView.vue'
import IssuedTicketsView from '@/views/sales/IssuedTicketsView.vue'
import CanceledTicketsView from '@/views/sales/CanceledTicketsView.vue'
import CreateDocumentView from '@/views/sales/CreateDocumentView.vue'
import EditDocumentView from '@/views/sales/EditDocumentView.vue'
import VoucherView from '@/views/finance/VoucherView.vue'
import CostsView from '@/views/finance/CostsView.vue'
import IncomesView from '@/views/finance/IncomesView.vue'
import TransferView from '@/views/finance/TransferView.vue'
import ReportsView from '@/views/reports/ReportsView.vue'
import SalesReportView from '@/views/reports/SalesReportView.vue'
import FinanceReportView from '@/views/reports/FinanceReportView.vue'
import UsersView from '@/views/settings/UsersView.vue'
import AirlinesView from '@/views/settings/AirlinesView.vue'
import BanksView from '@/views/settings/BanksView.vue'
import CounterpartiesView from '@/views/settings/CounterpartiesView.vue'
import LocationsView from '@/views/settings/LocationsView.vue'
import SystemSettingsView from '@/views/settings/SystemSettingsView.vue'

const routes = [
  {
    path: '/login',
    name: 'Login',
    component: LoginView,
    meta: {
      requiresAuth: false,
      title: 'ورود به سیستم'
    }
  },
  {
    path: '/',
    name: 'Dashboard',
    component: DashboardView,
    meta: {
      requiresAuth: true,
      title: 'داشبورد',
      breadcrumb: [{ text: 'داشبورد', to: '/' }]
    }
  },
  
  // Profile Route
  {
    path: '/profile',
    name: 'Profile',
    component: ProfileView,
    meta: {
      requiresAuth: true,
      title: 'پروفایل کاربری',
      breadcrumb: [
        { text: 'داشبورد', to: '/' },
        { text: 'پروفایل کاربری', to: '/profile' }
      ]
    }
  },
  
  // Chart of Accounts Route
  {
    path: '/chart-of-accounts',
    name: 'ChartOfAccounts',
    component: ChartOfAccountsView,
    meta: {
      requiresAuth: true,
      title: 'دفتر حساب‌ها',
      breadcrumb: [
        { text: 'داشبورد', to: '/' },
        { text: 'دفتر حساب‌ها', to: '/chart-of-accounts' }
      ]
    }
  },
  
  // Sales Routes
  {
    path: '/sales',
    meta: {
      requiresAuth: true,
      title: 'فروش'
    },
    children: [
      {
        path: 'unissued',
        name: 'UnissuedTickets',
        component: UnissuedTicketsView,
        meta: {
          title: 'بلیط‌های صادر نشده',
          breadcrumb: [
            { text: 'داشبورد', to: '/' },
            { text: 'فروش', to: '/sales' },
            { text: 'صادر نشده', to: '/sales/unissued' }
          ]
        }
      },
      {
        path: 'issued',
        name: 'IssuedTickets',
        component: IssuedTicketsView,
        meta: {
          title: 'بلیط‌های صادر شده',
          breadcrumb: [
            { text: 'داشبورد', to: '/' },
            { text: 'فروش', to: '/sales' },
            { text: 'صادر شده', to: '/sales/issued' }
          ]
        }
      },
      {
        path: 'canceled',
        name: 'CanceledTickets',
        component: CanceledTicketsView,
        meta: {
          title: 'بلیط‌های کنسل شده',
          breadcrumb: [
            { text: 'داشبورد', to: '/' },
            { text: 'فروش', to: '/sales' },
            { text: 'کنسل شده', to: '/sales/canceled' }
          ]
        }
      },
      {
        path: 'create',
        name: 'CreateDocument',
        component: CreateDocumentView,
        meta: {
          title: 'ایجاد سند جدید',
          breadcrumb: [
            { text: 'داشبورد', to: '/' },
            { text: 'فروش', to: '/sales' },
            { text: 'ایجاد سند', to: '/sales/create' }
          ]
        }
      },
      {
        path: 'edit/:id',
        name: 'EditDocument',
        component: EditDocumentView,
        meta: {
          title: 'ویرایش سند',
          breadcrumb: [
            { text: 'داشبورد', to: '/' },
            { text: 'فروش', to: '/sales' },
            { text: 'ویرایش سند', to: null }
          ]
        }
      }
    ]
  },
  
  // Finance Routes
  {
    path: '/finance',
    meta: {
      requiresAuth: true,
      title: 'مالی'
    },
    children: [
      {
        path: 'voucher',
        name: 'Voucher',
        component: VoucherView,
        meta: {
          title: 'سند حسابداری',
          breadcrumb: [
            { text: 'داشبورد', to: '/' },
            { text: 'مالی', to: '/finance' },
            { text: 'سند حسابداری', to: '/finance/voucher' }
          ]
        }
      },
      {
        path: 'costs',
        name: 'Costs',
        component: CostsView,
        meta: {
          title: 'هزینه‌ها',
          breadcrumb: [
            { text: 'داشبورد', to: '/' },
            { text: 'مالی', to: '/finance' },
            { text: 'هزینه‌ها', to: '/finance/costs' }
          ]
        }
      },
      {
        path: 'incomes',
        name: 'Incomes',
        component: IncomesView,
        meta: {
          title: 'درآمدها',
          breadcrumb: [
            { text: 'داشبورد', to: '/' },
            { text: 'مالی', to: '/finance' },
            { text: 'درآمدها', to: '/finance/incomes' }
          ]
        }
      },
      {
        path: 'transfer',
        name: 'Transfer',
        component: TransferView,
        meta: {
          title: 'انتقال وجه',
          breadcrumb: [
            { text: 'داشبورد', to: '/' },
            { text: 'مالی', to: '/finance' },
            { text: 'انتقال وجه', to: '/finance/transfer' }
          ]
        }
      }
    ]
  },
  
  // Reports Routes
  {
    path: '/reports',
    name: 'Reports',
    component: ReportsView,
    meta: {
      requiresAuth: true,
      title: 'گزارشات',
      breadcrumb: [
        { text: 'داشبورد', to: '/' },
        { text: 'گزارشات', to: '/reports' }
      ]
    }
  },
  {
    path: '/reports/sales',
    name: 'SalesReport',
    component: SalesReportView,
    meta: {
      requiresAuth: true,
      title: 'گزارش فروش',
      breadcrumb: [
        { text: 'داشبورد', to: '/' },
        { text: 'گزارشات', to: '/reports' },
        { text: 'گزارش فروش', to: '/reports/sales' }
      ]
    }
  },
  {
    path: '/reports/finance',
    name: 'FinanceReport',
    component: FinanceReportView,
    meta: {
      requiresAuth: true,
      title: 'گزارش مالی',
      breadcrumb: [
        { text: 'داشبورد', to: '/' },
        { text: 'گزارشات', to: '/reports' },
        { text: 'گزارش مالی', to: '/reports/finance' }
      ]
    }
  },
  
  // Settings Routes
  {
    path: '/settings',
    meta: {
      requiresAuth: true,
      title: 'تنظیمات'
    },
    children: [
      {
        path: 'users',
        name: 'Users',
        component: UsersView,
        meta: {
          title: 'کاربران',
          breadcrumb: [
            { text: 'داشبورد', to: '/' },
            { text: 'تنظیمات', to: '/settings' },
            { text: 'کاربران', to: '/settings/users' }
          ]
        }
      },
      {
        path: 'airlines',
        name: 'Airlines',
        component: AirlinesView,
        meta: {
          title: 'ایرلاین‌ها',
          breadcrumb: [
            { text: 'داشبورد', to: '/' },
            { text: 'تنظیمات', to: '/settings' },
            { text: 'ایرلاین‌ها', to: '/settings/airlines' }
          ]
        }
      },
      {
        path: 'banks',
        name: 'Banks',
        component: BanksView,
        meta: {
          title: 'بانک‌ها',
          breadcrumb: [
            { text: 'داشبورد', to: '/' },
            { text: 'تنظیمات', to: '/settings' },
            { text: 'بانک‌ها', to: '/settings/banks' }
          ]
        }
      },
      {
        path: 'counterparties',
        name: 'Counterparties',
        component: CounterpartiesView,
        meta: {
          title: 'طرف‌حساب‌ها',
          breadcrumb: [
            { text: 'داشبورد', to: '/' },
            { text: 'تنظیمات', to: '/settings' },
            { text: 'طرف‌حساب‌ها', to: '/settings/counterparties' }
          ]
        }
      },
      {
        path: 'locations',
        name: 'Locations',
        component: LocationsView,
        meta: {
          title: 'مقاصد',
          breadcrumb: [
            { text: 'داشبورد', to: '/' },
            { text: 'تنظیمات', to: '/settings' },
            { text: 'مقاصد', to: '/settings/locations' }
          ]
        }
      },
      {
        path: 'system',
        name: 'SystemSettings',
        component: SystemSettingsView,
        meta: {
          title: 'تنظیمات سیستم',
          breadcrumb: [
            { text: 'داشبورد', to: '/' },
            { text: 'تنظیمات', to: '/settings' },
            { text: 'تنظیمات سیستم', to: '/settings/system' }
          ]
        }
      }
    ]
  },
  
  // Catch all 404
  {
    path: '/:pathMatch(.*)*',
    name: 'NotFound',
    component: () => import('@/views/NotFoundView.vue'),
    meta: {
      title: 'صفحه یافت نشد'
    }
  }
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
  scrollBehavior(to, from, savedPosition) {
    if (savedPosition) {
      return savedPosition
    } else {
      return { top: 0 }
    }
  }
})

// Navigation guard for authentication
router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore();
  
  // Initialize auth state from localStorage only if not already authenticated
  if (!authStore.isAuthenticated) {
    await authStore.initializeAuth();
  }
  
  // Set page title
  if (to.meta.title) {
    document.title = `${to.meta.title} - سیستم حسابداری سفر`
  }
  
  // Check if route requires authentication
  if (to.meta.requiresAuth !== false && !authStore.isAuthenticated) {
    // Redirect to login if not authenticated
    next('/login');
  } else if (to.path === '/login' && authStore.isAuthenticated) {
    // Redirect to dashboard if already authenticated and trying to access login
    next('/');
  } else if (authStore.isAuthenticated) {
    // Check permissions for specific routes
    const routePermissions = {
      '/sales': 'sales',
      '/finance': 'finance', 
      '/reports': 'reports',
      '/settings': 'settings'
    };
    
    // Check if route requires specific permission
    const requiredPermission = Object.keys(routePermissions).find(path => to.path.startsWith(path));
    
    if (requiredPermission && !authStore.hasPermission(routePermissions[requiredPermission])) {
      // Prevent infinite redirect loop - only redirect if not already going to dashboard
      if (to.path !== '/') {
        next('/');
      } else {
        next();
      }
    } else {
      // Allow navigation
      next();
    }
  } else {
    // Allow navigation
    next();
  }
})

// After each route change
router.afterEach((to, from) => {
  // You can add analytics tracking here
  // gtag('config', 'GA_MEASUREMENT_ID', {
  //   page_title: to.meta.title,
  //   page_location: window.location.href
  // })
})

export default router