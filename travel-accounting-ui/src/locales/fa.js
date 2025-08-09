export default {
  // Common
  common: {
    save: 'ذخیره',
    cancel: 'لغو',
    delete: 'حذف',
    edit: 'ویرایش',
    add: 'افزودن',
    search: 'جستجو',
    filter: 'فیلتر',
    export: 'خروجی',
    import: 'ورودی',
    print: 'چاپ',
    close: 'بستن',
    confirm: 'تأیید',
    yes: 'بله',
    no: 'خیر',
    loading: 'در حال بارگذاری...',
    error: 'خطا',
    success: 'موفقیت',
    warning: 'هشدار',
    info: 'اطلاعات',
    required: 'الزامی',
    optional: 'اختیاری',
    all: 'همه',
    none: 'هیچکدام',
    select: 'انتخاب',
    clear: 'پاک کردن',
    reset: 'بازنشانی',
    refresh: 'بروزرسانی',
    back: 'بازگشت',
    next: 'بعدی',
    previous: 'قبلی',
    submit: 'ارسال',
    update: 'بروزرسانی',
    create: 'ایجاد',
    view: 'مشاهده',
    details: 'جزئیات',
    actions: 'عملیات',
    status: 'وضعیت',
    date: 'تاریخ',
    time: 'زمان',
    amount: 'مبلغ',
    total: 'مجموع',
    subtotal: 'جمع جزء',
    tax: 'مالیات',
    discount: 'تخفیف',
    description: 'توضیحات',
    name: 'نام',
    code: 'کد',
    type: 'نوع',
    category: 'دسته‌بندی',
    active: 'فعال',
    inactive: 'غیرفعال',
    enabled: 'فعال',
    saving: 'در حال ذخیره...',
    currency: 'واحد پول'
  },

  // Navigation
  nav: {
    dashboard: 'داشبورد',
    chartOfAccounts: 'دفتر حساب‌ها',
    sales: 'فروش',
    salesDocuments: 'اسناد فروش',
    vouchers: 'سندها',
    reports: 'گزارشات',
    settings: 'تنظیمات',
    users: 'کاربران',
    companies: 'شرکت‌ها',
    counterparties: 'طرف‌حساب‌ها',
    airlines: 'خطوط هوایی',
    locations: 'مکان‌ها',
    banks: 'بانک‌ها',
    logout: 'خروج'
  },

  // Authentication
  auth: {
    login: 'ورود',
    logout: 'خروج',
    username: 'نام کاربری',
    password: 'رمز عبور',
    company: 'شرکت',
    rememberMe: 'مرا به خاطر بسپار',
    forgotPassword: 'رمز عبور را فراموش کرده‌اید؟',
    loginFailed: 'ورود ناموفق',
    invalidCredentials: 'نام کاربری یا رمز عبور اشتباه است',
    accountLocked: 'حساب کاربری قفل شده است',
    sessionExpired: 'جلسه منقضی شده است',
    loginSuccess: 'ورود موفقیت‌آمیز',
    pleaseLogin: 'لطفاً وارد شوید',
    accessDenied: 'دسترسی مجاز نیست'
  },

  // Dashboard
  dashboard: {
    title: 'داشبورد',
    welcome: 'خوش آمدید',
    overview: 'نمای کلی',
    quickStats: 'آمار سریع',
    recentActivity: 'فعالیت‌های اخیر',
    totalSales: 'کل فروش',
    totalRevenue: 'کل درآمد',
    pendingDocuments: 'اسناد در انتظار',
    issuedDocuments: 'اسناد صادر شده',
    monthlyRevenue: 'درآمد ماهانه',
    salesTrend: 'روند فروش',
    topCounterparties: 'برترین طرف‌حساب‌ها',
    recentTransactions: 'تراکنش‌های اخیر'
  },

  // Sales Documents
  salesDocuments: {
    title: 'اسناد فروش',
    newDocument: 'سند جدید',
    documentNumber: 'شماره سند',
    documentDate: 'تاریخ سند',
    counterparty: 'طرف‌حساب',
    serviceType: 'نوع خدمات',
    passenger: 'مسافر',
    route: 'مسیر',
    flightDate: 'تاریخ پرواز',
    ticketNumber: 'شماره بلیط',
    pnr: 'PNR',
    airline: 'خط هوایی',
    saleAmount: 'مبلغ فروش',
    commission: 'کمیسیون',
    netAmount: 'مبلغ خالص',
    status: {
      draft: 'پیش‌نویس',
      pending: 'در انتظار',
      issued: 'صادر شده',
      cancelled: 'لغو شده',
      refunded: 'بازگشت داده شده'
    },
    serviceTypes: {
      air: 'هوایی',
      hotel: 'هتل',
      tour: 'تور',
      visa: 'ویزا',
      insurance: 'بیمه',
      other: 'سایر'
    }
  },

  // Vouchers
  vouchers: {
    title: 'سندها',
    newVoucher: 'سند جدید',
    voucherNumber: 'شماره سند',
    voucherDate: 'تاریخ سند',
    reference: 'مرجع',
    debit: 'بدهکار',
    credit: 'بستانکار',
    balance: 'مانده',
    account: 'حساب',
    accountCode: 'کد حساب',
    accountName: 'نام حساب',
    journalEntry: 'سند حسابداری',
    generalLedger: 'دفتر کل'
  },

  // Reports
  reports: {
    title: 'گزارشات',
    salesReport: 'گزارش فروش',
    financialReport: 'گزارش مالی',
    profitLoss: 'سود و زیان',
    balanceSheet: 'ترازنامه',
    cashFlow: 'جریان نقدی',
    agingReport: 'گزارش سنی',
    trialBalance: 'تراز آزمایشی',
    dateRange: 'بازه تاریخ',
    fromDate: 'از تاریخ',
    toDate: 'تا تاریخ',
    generateReport: 'تولید گزارش',
    exportToPdf: 'خروجی PDF',
    exportToExcel: 'خروجی Excel',
    printReport: 'چاپ گزارش'
  },

  // Settings
  settings: {
    title: 'تنظیمات',
    general: 'عمومی',
    accounting: 'حسابداری',
    users: 'کاربران',
    permissions: 'مجوزها',
    backup: 'پشتیبان‌گیری',
    restore: 'بازیابی',
    systemSettings: 'تنظیمات سیستم',
    companySettings: 'تنظیمات شرکت',
    fiscalYear: 'سال مالی',
    currency: 'واحد پول',
    language: 'زبان',
    timezone: 'منطقه زمانی',
    dateFormat: 'فرمت تاریخ',
    numberFormat: 'فرمت اعداد'
  },

  // Chart of Accounts
  chartOfAccounts: {
    title: 'دفتر حساب‌ها',
    newAccount: 'حساب جدید',
    createAccount: 'ایجاد حساب',
    editAccount: 'ویرایش حساب',
    accountCode: 'کد حساب',
    accountName: 'نام حساب',
    type: 'نوع',
    parentAccount: 'حساب والد',
    noParent: 'بدون والد',
    balance: 'مانده',
    asset: 'دارایی',
    liability: 'بدهی',
    equity: 'حقوق صاحبان سهام',
    revenue: 'درآمد',
    expense: 'هزینه',
    confirmDelete: 'آیا از حذف این حساب اطمینان دارید؟',
    accountCreated: 'حساب با موفقیت ایجاد شد',
    accountUpdated: 'حساب با موفقیت بروزرسانی شد',
    accountDeleted: 'حساب با موفقیت حذف شد'
  },

  // Users
  users: {
    title: 'کاربران',
    newUser: 'کاربر جدید',
    editUser: 'ویرایش کاربر',
    firstName: 'نام',
    lastName: 'نام خانوادگی',
    email: 'ایمیل',
    phone: 'تلفن',
    role: 'نقش',
    permissions: 'مجوزها',
    lastLogin: 'آخرین ورود',
    createdAt: 'تاریخ ایجاد',
    updatedAt: 'تاریخ بروزرسانی',
    roles: {
      admin: 'مدیر',
      manager: 'مدیر بخش',
      accountant: 'حسابدار',
      user: 'کاربر عادی'
    }
  },

  // Companies
  companies: {
    title: 'شرکت‌ها',
    newCompany: 'شرکت جدید',
    companyName: 'نام شرکت',
    registrationNumber: 'شماره ثبت',
    taxNumber: 'شماره مالیاتی',
    address: 'آدرس',
    city: 'شهر',
    country: 'کشور',
    postalCode: 'کد پستی',
    website: 'وب‌سایت',
    contactPerson: 'شخص تماس',
    establishedDate: 'تاریخ تأسیس'
  },

  // Counterparties
  counterparties: {
    title: 'طرف‌حساب‌ها',
    newCounterparty: 'طرف‌حساب جدید',
    customerType: 'نوع مشتری',
    supplierType: 'نوع تأمین‌کننده',
    individual: 'شخص حقیقی',
    company: 'شخص حقوقی',
    nationalId: 'کد ملی',
    economicCode: 'کد اقتصادی',
    creditLimit: 'سقف اعتبار',
    paymentTerms: 'شرایط پرداخت',
    accountBalance: 'مانده حساب'
  },

  // Airlines
  airlines: {
    title: 'خطوط هوایی',
    newAirline: 'خط هوایی جدید',
    airlineName: 'نام خط هوایی',
    airlineCode: 'کد خط هوایی',
    iataCode: 'کد IATA',
    icaoCode: 'کد ICAO',
    country: 'کشور',
    website: 'وب‌سایت',
    contactInfo: 'اطلاعات تماس',
    commissionRate: 'نرخ کمیسیون'
  },

  // Locations
  locations: {
    title: 'مکان‌ها',
    newLocation: 'مکان جدید',
    locationName: 'نام مکان',
    locationCode: 'کد مکان',
    locationType: 'نوع مکان',
    parentLocation: 'مکان والد',
    country: 'کشور',
    city: 'شهر',
    airport: 'فرودگاه',
    region: 'منطقه',
    timezone: 'منطقه زمانی'
  },

  // Banks
  banks: {
    title: 'بانک‌ها',
    newBank: 'بانک جدید',
    bankName: 'نام بانک',
    bankCode: 'کد بانک',
    swiftCode: 'کد SWIFT',
    branchName: 'نام شعبه',
    branchCode: 'کد شعبه',
    accountNumber: 'شماره حساب',
    accountHolder: 'صاحب حساب',
    iban: 'شماره شبا'
  },

  // Date and Time
  date: {
    today: 'امروز',
    yesterday: 'دیروز',
    tomorrow: 'فردا',
    thisWeek: 'این هفته',
    lastWeek: 'هفته گذشته',
    thisMonth: 'این ماه',
    lastMonth: 'ماه گذشته',
    thisYear: 'امسال',
    lastYear: 'سال گذشته',
    selectDate: 'انتخاب تاریخ',
    invalidDate: 'تاریخ نامعتبر',
    dateRequired: 'تاریخ الزامی است'
  },

  // Validation Messages
  validation: {
    required: 'این فیلد الزامی است',
    email: 'فرمت ایمیل صحیح نیست',
    phone: 'فرمت تلفن صحیح نیست',
    minLength: 'حداقل {min} کاراکتر وارد کنید',
    maxLength: 'حداکثر {max} کاراکتر مجاز است',
    numeric: 'فقط عدد وارد کنید',
    positive: 'عدد مثبت وارد کنید',
    integer: 'عدد صحیح وارد کنید',
    decimal: 'عدد اعشاری وارد کنید',
    unique: 'این مقدار قبلاً استفاده شده است',
    match: 'مقادیر مطابقت ندارند',
    invalidFormat: 'فرمت نامعتبر است'
  },

  // Error Messages
  errors: {
    networkError: 'خطای شبکه',
    serverError: 'خطای سرور',
    notFound: 'یافت نشد',
    unauthorized: 'عدم دسترسی',
    forbidden: 'ممنوع',
    badRequest: 'درخواست نامعتبر',
    timeout: 'زمان انتظار تمام شد',
    unknownError: 'خطای نامشخص',
    saveError: 'خطا در ذخیره',
    deleteError: 'خطا در حذف',
    loadError: 'خطا در بارگذاری',
    validationError: 'خطای اعتبارسنجی'
  },

  // Success Messages
  success: {
    saved: 'با موفقیت ذخیره شد',
    deleted: 'با موفقیت حذف شد',
    updated: 'با موفقیت بروزرسانی شد',
    created: 'با موفقیت ایجاد شد',
    imported: 'با موفقیت وارد شد',
    exported: 'با موفقیت خارج شد',
    sent: 'با موفقیت ارسال شد',
    completed: 'با موفقیت تکمیل شد'
  },

  // Confirmation Messages
  confirm: {
    delete: 'آیا از حذف این مورد اطمینان دارید؟',
    save: 'آیا تغییرات ذخیره شود؟',
    cancel: 'آیا از لغو این عملیات اطمینان دارید؟',
    exit: 'آیا از خروج اطمینان دارید؟',
    reset: 'آیا از بازنشانی اطمینان دارید؟',
    overwrite: 'آیا فایل موجود بازنویسی شود؟'
  }
};