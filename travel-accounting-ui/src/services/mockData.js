// Mock Data Service for Travel Accounting System

// Sample Airlines
export const airlines = [
  { id: 1, code: 'IR', name: 'Iran Air', nameEn: 'Iran Air' },
  { id: 2, code: 'W5', name: 'ماهان', nameEn: 'Mahan Air' },
  { id: 3, code: 'EP', name: 'آسمان', nameEn: 'Aseman Airlines' },
  { id: 4, code: 'QR', name: 'قطر ایرویز', nameEn: 'Qatar Airways' },
  { id: 5, code: 'TK', name: 'ترکیش ایرلاینز', nameEn: 'Turkish Airlines' }
];

// Sample Counterparties (Buyers/Suppliers)
export const counterparties = [
  {
    id: 1,
    type: 'buyer',
    name: 'شرکت گردشگری آریا',
    nameEn: 'Aria Tourism Company',
    phone: '021-88776655',
    email: 'info@aria-tourism.com',
    address: 'تهران، خیابان ولیعصر، پلاک 123',
    openingBalanceRial: 5000000,
    openingBalanceFx: 1200
  },
  {
    id: 2,
    type: 'supplier',
    name: 'آژانس مسافرتی پارس',
    nameEn: 'Pars Travel Agency',
    phone: '021-77889900',
    email: 'contact@pars-travel.ir',
    address: 'تهران، میدان انقلاب، ساختمان کوثر',
    openingBalanceRial: -2000000,
    openingBalanceFx: -800
  },
  {
    id: 3,
    type: 'buyer',
    name: 'هتل پارسیان آزادی',
    nameEn: 'Parsian Azadi Hotel',
    phone: '021-66941000',
    email: 'reservation@azadihotel.com',
    address: 'تهران، میدان آزادی',
    openingBalanceRial: 0,
    openingBalanceFx: 0
  }
];

// Sample Banks/Accounts
export const banks = [
  {
    id: 1,
    name: 'بانک ملی ایران',
    nameEn: 'Bank Melli Iran',
    accountNumber: '0109876543210',
    iban: 'IR120170000000109876543210',
    openingBalanceRial: 50000000,
    openingBalanceFx: 10000
  },
  {
    id: 2,
    name: 'بانک پاسارگاد',
    nameEn: 'Pasargad Bank',
    accountNumber: '5022334455667',
    iban: 'IR330570000005022334455667',
    openingBalanceRial: 25000000,
    openingBalanceFx: 5000
  },
  {
    id: 3,
    name: 'بانک صادرات ایران',
    nameEn: 'Export Development Bank',
    accountNumber: '4001122334455',
    iban: 'IR940200000004001122334455',
    openingBalanceRial: 15000000,
    openingBalanceFx: 3000
  }
];

// Sample Origins/Destinations
export const locations = [
  { id: 1, code: 'THR', name: 'تهران', nameEn: 'Tehran', type: 'city' },
  { id: 2, code: 'IFN', name: 'اصفهان', nameEn: 'Isfahan', type: 'city' },
  { id: 3, code: 'SYZ', name: 'شیراز', nameEn: 'Shiraz', type: 'city' },
  { id: 4, code: 'MHD', name: 'مشهد', nameEn: 'Mashhad', type: 'city' },
  { id: 5, code: 'DXB', name: 'دبی', nameEn: 'Dubai', type: 'city' },
  { id: 6, code: 'DOH', name: 'دوحه', nameEn: 'Doha', type: 'city' },
  { id: 7, code: 'IST', name: 'استانبول', nameEn: 'Istanbul', type: 'city' }
];

// Sample Users (matching backend API credentials)
export const users = [
  {
    id: 1,
    username: 'admin',
    password: 'admin123',
    company: 'demo',
    firstName: 'مدیر',
    lastName: 'سیستم',
    name: 'مدیر سیستم',
    nameEn: 'System Admin',
    email: 'admin@travel-accounting.com',
    role: 'admin',
    isActive: true,
    lastLogin: '2024-01-15T10:30:00Z',
    permissions: ['admin', 'sales', 'finance', 'reports', 'settings']
  },
  {
    id: 2,
    username: 'accountant',
    password: 'acc123',
    company: 'demo',
    firstName: 'حسابدار',
    lastName: 'اصلی',
    name: 'حسابدار اصلی',
    nameEn: 'Main Accountant',
    email: 'accountant@travel-accounting.com',
    role: 'finance',
    isActive: true,
    lastLogin: '2024-01-14T16:45:00Z',
    permissions: ['finance', 'reports']
  },
  {
    id: 3,
    username: 'sales',
    password: 'sales123',
    company: 'demo',
    firstName: 'کارشناس',
    lastName: 'فروش',
    name: 'کارشناس فروش',
    nameEn: 'Sales Representative',
    email: 'sales@travel-accounting.com',
    role: 'sales',
    isActive: true,
    lastLogin: '2024-01-14T14:20:00Z',
    permissions: ['sales']
  },
  {
    id: 4,
    username: 'user',
    password: 'user123',
    company: 'demo',
    firstName: 'کاربر',
    lastName: 'عادی',
    name: 'کاربر عادی',
    nameEn: 'Regular User',
    email: 'user@travel-accounting.com',
    role: 'user',
    isActive: true,
    lastLogin: '2024-01-14T14:20:00Z',
    permissions: []
  }
];

// Sample Sales Documents
export const salesDocuments = [
  {
    id: 1,
    documentNo: 'TKT-2024-001',
    status: 'unissued',
    serviceType: 'air',
    saleDate: '2024-01-10',
    flightDate: '2024-01-20',
    buyer: counterparties[0],
    passenger: {
      name: 'علی احمدی',
      nameEn: 'Ali Ahmadi',
      passportNo: 'A12345678',
      nationality: 'Iranian',
      birthDate: '1985-05-15',
      type: 'adult'
    },
    supplier: counterparties[1],
    route: {
      origin: locations[0],
      destination: locations[4],
      airline: airlines[1],
      flightNo: 'W5-101'
    },
    finance: {
      salePrice: 15000000,
      costPrice: 12000000,
      currency: 'IRR',
      profit: 3000000
    },
    createdBy: users[1],
    createdAt: '2024-01-10T09:15:00Z'
  },
  {
    id: 2,
    documentNo: 'TKT-2024-002',
    status: 'issued',
    serviceType: 'air',
    saleDate: '2024-01-08',
    flightDate: '2024-01-18',
    buyer: counterparties[2],
    passenger: {
      name: 'مریم رضایی',
      nameEn: 'Maryam Rezaei',
      passportNo: 'B87654321',
      nationality: 'Iranian',
      birthDate: '1990-03-22',
      type: 'adult'
    },
    supplier: counterparties[1],
    route: {
      origin: locations[0],
      destination: locations[6],
      airline: airlines[4],
      flightNo: 'TK-876'
    },
    finance: {
      salePrice: 25000000,
      costPrice: 20000000,
      currency: 'IRR',
      profit: 5000000
    },
    createdBy: users[1],
    createdAt: '2024-01-08T11:30:00Z'
  },
  {
    id: 3,
    documentNo: 'TKT-2024-003',
    status: 'canceled',
    serviceType: 'air',
    saleDate: '2024-01-05',
    flightDate: '2024-01-25',
    buyer: counterparties[0],
    passenger: {
      name: 'حسن کریمی',
      nameEn: 'Hassan Karimi',
      passportNo: 'C11223344',
      nationality: 'Iranian',
      birthDate: '1978-11-10',
      type: 'adult'
    },
    supplier: counterparties[1],
    route: {
      origin: locations[1],
      destination: locations[5],
      airline: airlines[3],
      flightNo: 'QR-445'
    },
    finance: {
      salePrice: 18000000,
      costPrice: 15000000,
      currency: 'IRR',
      profit: 3000000
    },
    createdBy: users[1],
    createdAt: '2024-01-05T14:45:00Z',
    canceledAt: '2024-01-12T10:20:00Z',
    cancelReason: 'درخواست مسافر'
  }
];

// Sample Vouchers
export const vouchers = [
  {
    id: 1,
    voucherNo: 'VCH-2024-001',
    date: '2024-01-10',
    type: 'receipt',
    description: 'دریافت وجه بابت بلیط هواپیما',
    amount: 15000000,
    currency: 'IRR',
    counterparty: counterparties[0],
    bank: banks[0],
    createdBy: users[2],
    createdAt: '2024-01-10T15:30:00Z'
  },
  {
    id: 2,
    voucherNo: 'VCH-2024-002',
    date: '2024-01-11',
    type: 'payment',
    description: 'پرداخت هزینه بلیط به تامین کننده',
    amount: 12000000,
    currency: 'IRR',
    counterparty: counterparties[1],
    bank: banks[0],
    createdBy: users[2],
    createdAt: '2024-01-11T09:45:00Z'
  }
];

// Sample Dashboard Data
export const dashboardData = {
  totalIncome: 58000000,
  totalExpense: 47000000,
  netProfit: 11000000,
  accountsReceivable: 5000000,
  accountsPayable: 2000000,
  monthlyIncome: [
    { month: 'فروردین', amount: 45000000 },
    { month: 'اردیبهشت', amount: 52000000 },
    { month: 'خرداد', amount: 48000000 },
    { month: 'تیر', amount: 58000000 }
  ],
  monthlyExpense: [
    { month: 'فروردین', amount: 38000000 },
    { month: 'اردیبهشت', amount: 42000000 },
    { month: 'خرداد', amount: 39000000 },
    { month: 'تیر', amount: 47000000 }
  ]
};

// Helper Functions
export const getDocumentsByStatus = (status) => {
  return salesDocuments.filter(doc => doc.status === status);
};

export const getDocumentById = (id) => {
  return salesDocuments.find(doc => doc.id === parseInt(id));
};

export const getNextDocumentNumber = () => {
  const lastDoc = salesDocuments[salesDocuments.length - 1];
  const lastNumber = parseInt(lastDoc.documentNo.split('-')[2]);
  return `TKT-2024-${String(lastNumber + 1).padStart(3, '0')}`;
};

export const getLastSaleDate = () => {
  if (salesDocuments.length === 0) return new Date().toISOString().split('T')[0];
  return salesDocuments[salesDocuments.length - 1].saleDate;
};

export const sortDocumentsByFlightDate = (documents) => {
  return documents.sort((a, b) => new Date(a.flightDate) - new Date(b.flightDate));
};

export const getFlightDateStatus = (flightDate) => {
  const today = new Date();
  const flight = new Date(flightDate);
  const diffTime = flight - today;
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  
  if (diffDays <= 5) {
    return 'red'; // First 5 days - red
  } else {
    return 'blue'; // Rest - blue
  }
};

// Combined export for backward compatibility
export const mockData = {
  airlines,
  counterparties,
  banks,
  locations,
  salesDocuments,
  users,
  vouchers,
  dashboardData
};