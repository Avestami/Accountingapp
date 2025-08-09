export default {
  // Common
  common: {
    save: 'Save',
    cancel: 'Cancel',
    delete: 'Delete',
    edit: 'Edit',
    add: 'Add',
    search: 'Search',
    filter: 'Filter',
    export: 'Export',
    import: 'Import',
    print: 'Print',
    close: 'Close',
    confirm: 'Confirm',
    yes: 'Yes',
    no: 'No',
    loading: 'Loading...',
    error: 'Error',
    success: 'Success',
    warning: 'Warning',
    info: 'Information',
    required: 'Required',
    optional: 'Optional',
    all: 'All',
    none: 'None',
    select: 'Select',
    clear: 'Clear',
    reset: 'Reset',
    refresh: 'Refresh',
    back: 'Back',
    next: 'Next',
    previous: 'Previous',
    submit: 'Submit',
    update: 'Update',
    create: 'Create',
    view: 'View',
    details: 'Details',
    actions: 'Actions',
    status: 'Status',
    date: 'Date',
    time: 'Time',
    amount: 'Amount',
    total: 'Total',
    subtotal: 'Subtotal',
    tax: 'Tax',
    discount: 'Discount',
    description: 'Description',
    name: 'Name',
    code: 'Code',
    type: 'Type',
    category: 'Category',
    active: 'Active',
    inactive: 'Inactive',
    enabled: 'Enabled',
    saving: 'Saving...',
    currency: 'Currency'
  },

  // Navigation
  nav: {
    dashboard: 'Dashboard',
    chartOfAccounts: 'Chart of Accounts',
    sales: 'Sales',
    salesDocuments: 'Sales Documents',
    vouchers: 'Vouchers',
    reports: 'Reports',
    settings: 'Settings',
    users: 'Users',
    companies: 'Companies',
    counterparties: 'Counterparties',
    airlines: 'Airlines',
    locations: 'Locations',
    banks: 'Banks',
    logout: 'Logout'
  },

  // Authentication
  auth: {
    login: 'Login',
    logout: 'Logout',
    username: 'Username',
    password: 'Password',
    company: 'Company',
    rememberMe: 'Remember Me',
    forgotPassword: 'Forgot Password?',
    loginFailed: 'Login Failed',
    invalidCredentials: 'Invalid username or password',
    accountLocked: 'Account is locked',
    sessionExpired: 'Session expired',
    loginSuccess: 'Login successful',
    pleaseLogin: 'Please login',
    accessDenied: 'Access denied'
  },

  // Dashboard
  dashboard: {
    title: 'Dashboard',
    welcome: 'Welcome',
    overview: 'Overview',
    quickStats: 'Quick Stats',
    recentActivity: 'Recent Activity',
    totalSales: 'Total Sales',
    totalRevenue: 'Total Revenue',
    pendingDocuments: 'Pending Documents',
    issuedDocuments: 'Issued Documents',
    monthlyRevenue: 'Monthly Revenue',
    salesTrend: 'Sales Trend',
    topCounterparties: 'Top Counterparties',
    recentTransactions: 'Recent Transactions'
  },

  // Sales Documents
  salesDocuments: {
    title: 'Sales Documents',
    newDocument: 'New Document',
    documentNumber: 'Document Number',
    documentDate: 'Document Date',
    counterparty: 'Counterparty',
    serviceType: 'Service Type',
    passenger: 'Passenger',
    route: 'Route',
    flightDate: 'Flight Date',
    ticketNumber: 'Ticket Number',
    pnr: 'PNR',
    airline: 'Airline',
    saleAmount: 'Sale Amount',
    commission: 'Commission',
    netAmount: 'Net Amount',
    status: {
      draft: 'Draft',
      pending: 'Pending',
      issued: 'Issued',
      cancelled: 'Cancelled',
      refunded: 'Refunded'
    },
    serviceTypes: {
      air: 'Air',
      hotel: 'Hotel',
      tour: 'Tour',
      visa: 'Visa',
      insurance: 'Insurance',
      other: 'Other'
    }
  },

  // Vouchers
  vouchers: {
    title: 'Vouchers',
    newVoucher: 'New Voucher',
    voucherNumber: 'Voucher Number',
    voucherDate: 'Voucher Date',
    reference: 'Reference',
    debit: 'Debit',
    credit: 'Credit',
    balance: 'Balance',
    account: 'Account',
    accountCode: 'Account Code',
    accountName: 'Account Name',
    journalEntry: 'Journal Entry',
    generalLedger: 'General Ledger'
  },

  // Reports
  reports: {
    title: 'Reports',
    salesReport: 'Sales Report',
    financialReport: 'Financial Report',
    profitLoss: 'Profit & Loss',
    balanceSheet: 'Balance Sheet',
    cashFlow: 'Cash Flow',
    agingReport: 'Aging Report',
    trialBalance: 'Trial Balance',
    dateRange: 'Date Range',
    fromDate: 'From Date',
    toDate: 'To Date',
    generateReport: 'Generate Report',
    exportToPdf: 'Export to PDF',
    exportToExcel: 'Export to Excel',
    printReport: 'Print Report'
  },

  // Settings
  settings: {
    title: 'Settings',
    general: 'General',
    accounting: 'Accounting',
    users: 'Users',
    permissions: 'Permissions',
    backup: 'Backup',
    restore: 'Restore',
    systemSettings: 'System Settings',
    companySettings: 'Company Settings',
    fiscalYear: 'Fiscal Year',
    currency: 'Currency',
    language: 'Language',
    timezone: 'Timezone',
    dateFormat: 'Date Format',
    numberFormat: 'Number Format'
  },

  // Chart of Accounts
  chartOfAccounts: {
    title: 'Chart of Accounts',
    newAccount: 'New Account',
    createAccount: 'Create Account',
    editAccount: 'Edit Account',
    accountCode: 'Account Code',
    accountName: 'Account Name',
    type: 'Type',
    parentAccount: 'Parent Account',
    noParent: 'No Parent',
    balance: 'Balance',
    asset: 'Asset',
    liability: 'Liability',
    equity: 'Equity',
    revenue: 'Revenue',
    expense: 'Expense',
    confirmDelete: 'Are you sure you want to delete this account?',
    accountCreated: 'Account created successfully',
    accountUpdated: 'Account updated successfully',
    accountDeleted: 'Account deleted successfully'
  },

  // Users
  users: {
    title: 'Users',
    newUser: 'New User',
    editUser: 'Edit User',
    firstName: 'First Name',
    lastName: 'Last Name',
    email: 'Email',
    phone: 'Phone',
    role: 'Role',
    permissions: 'Permissions',
    lastLogin: 'Last Login',
    createdAt: 'Created At',
    updatedAt: 'Updated At',
    roles: {
      admin: 'Admin',
      manager: 'Manager',
      accountant: 'Accountant',
      user: 'User'
    }
  },

  // Companies
  companies: {
    title: 'Companies',
    newCompany: 'New Company',
    companyName: 'Company Name',
    registrationNumber: 'Registration Number',
    taxNumber: 'Tax Number',
    address: 'Address',
    city: 'City',
    country: 'Country',
    postalCode: 'Postal Code',
    website: 'Website',
    contactPerson: 'Contact Person',
    establishedDate: 'Established Date'
  },

  // Counterparties
  counterparties: {
    title: 'Counterparties',
    newCounterparty: 'New Counterparty',
    customerType: 'Customer Type',
    supplierType: 'Supplier Type',
    individual: 'Individual',
    company: 'Company',
    nationalId: 'National ID',
    economicCode: 'Economic Code',
    creditLimit: 'Credit Limit',
    paymentTerms: 'Payment Terms',
    accountBalance: 'Account Balance'
  },

  // Airlines
  airlines: {
    title: 'Airlines',
    newAirline: 'New Airline',
    airlineName: 'Airline Name',
    airlineCode: 'Airline Code',
    iataCode: 'IATA Code',
    icaoCode: 'ICAO Code',
    country: 'Country',
    website: 'Website',
    contactInfo: 'Contact Info',
    commissionRate: 'Commission Rate'
  },

  // Locations
  locations: {
    title: 'Locations',
    newLocation: 'New Location',
    locationName: 'Location Name',
    locationCode: 'Location Code',
    locationType: 'Location Type',
    parentLocation: 'Parent Location',
    country: 'Country',
    city: 'City',
    airport: 'Airport',
    region: 'Region',
    timezone: 'Timezone'
  },

  // Banks
  banks: {
    title: 'Banks',
    newBank: 'New Bank',
    bankName: 'Bank Name',
    bankCode: 'Bank Code',
    swiftCode: 'SWIFT Code',
    branchName: 'Branch Name',
    branchCode: 'Branch Code',
    accountNumber: 'Account Number',
    accountHolder: 'Account Holder',
    iban: 'IBAN'
  },

  // Date and Time
  date: {
    today: 'Today',
    yesterday: 'Yesterday',
    tomorrow: 'Tomorrow',
    thisWeek: 'This Week',
    lastWeek: 'Last Week',
    thisMonth: 'This Month',
    lastMonth: 'Last Month',
    thisYear: 'This Year',
    lastYear: 'Last Year',
    selectDate: 'Select Date',
    invalidDate: 'Invalid Date',
    dateRequired: 'Date is required'
  },

  // Validation Messages
  validation: {
    required: 'This field is required',
    email: 'Invalid email format',
    phone: 'Invalid phone format',
    minLength: 'Minimum {min} characters required',
    maxLength: 'Maximum {max} characters allowed',
    numeric: 'Only numbers allowed',
    positive: 'Positive number required',
    integer: 'Integer required',
    decimal: 'Decimal number required',
    unique: 'This value is already used',
    match: 'Values do not match',
    invalidFormat: 'Invalid format'
  },

  // Error Messages
  errors: {
    networkError: 'Network Error',
    serverError: 'Server Error',
    notFound: 'Not Found',
    unauthorized: 'Unauthorized',
    forbidden: 'Forbidden',
    badRequest: 'Bad Request',
    timeout: 'Timeout',
    unknownError: 'Unknown Error',
    saveError: 'Save Error',
    deleteError: 'Delete Error',
    loadError: 'Load Error',
    validationError: 'Validation Error'
  },

  // Success Messages
  success: {
    saved: 'Successfully saved',
    deleted: 'Successfully deleted',
    updated: 'Successfully updated',
    created: 'Successfully created',
    imported: 'Successfully imported',
    exported: 'Successfully exported',
    sent: 'Successfully sent',
    completed: 'Successfully completed'
  },

  // Confirmation Messages
  confirm: {
    delete: 'Are you sure you want to delete this item?',
    save: 'Save changes?',
    cancel: 'Are you sure you want to cancel?',
    exit: 'Are you sure you want to exit?',
    reset: 'Are you sure you want to reset?',
    overwrite: 'Overwrite existing file?'
  }
};