# Travel Accounting System: UI/Frontend Plan

This document outlines the complete UI-first architectural plan for the travel-accounting system frontend.

---

### 1. Technology Stack Selection (Frontend)

*   **Framework:** **Vue 3** with Vite. This provides a modern, performant, and highly organized development experience for building a Single Page Application (SPA).
*   **UI Library:** A component library like PrimeVue or Quasar will be used to accelerate development, ensuring accessibility and providing a solid foundation for RTL support and theming.
*   **State Management:** Pinia for centralized, type-safe state management.
*   **Date & Calendar:** A dedicated Persian date-picker library (e.g., `vue-persian-datetime-picker`) will be integrated.

### 2. Sitemap & Navigation

The application will be structured as a Single Page Application (SPA) with client-side routing. The navigation will consist of a primary sidebar menu.

*   **/login** (Public Page)
*   **Dashboard** (`/`)
*   **Sales Documents**
    *   New Sale (`/sales/new`)
        *   Air Ticket
        *   Train/Bus Ticket
        *   Hotel
        *   Tour
        *   Mixed Booking
    *   Unissued Tickets (`/sales/tickets/unissued`)
    *   Issued Tickets (`/sales/tickets/issued`)
    *   Canceled Tickets (`/sales/tickets/canceled`)
*   **Finance**
    *   New Voucher (`/finance/vouchers/new`)
    *   Voucher List (`/finance/vouchers`)
    *   New Cost/Income (`/finance/transactions/new`)
    *   Cost/Income List (`/finance/transactions`)
    *   New Transfer (`/finance/transfers/new`)
    *   Transfer List (`/finance/transfers`)
*   **Reports**
    *   Profit & Loss (`/reports/pl`)
    *   Accounts Receivable (A/R) (`/reports/ar`)
    *   Accounts Payable (A/P) (`/reports/ap`)
    *   Received Amounts (`/reports/received`)
    *   Ticket Status (`/reports/tickets`)
    *   General Ledger (`/reports/ledger`)
*   **Settings**
    *   Users (`/settings/users`)
    *   Roles (`/settings/roles`)
    *   Airlines (`/settings/airlines`)
    *   Banks & Accounts (`/settings/banks`)
    *   Payment Gateways (`/settings/gateways`)
    *   Counterparties (`/settings/counterparties`)
    *   System (`/settings/system`) - For origins, destinations, etc.

### 3. Component Inventory

*   **Base Components:**
    *   `AppButton`: Standard button with icon support.
    *   `AppCard`: A container with header, body, footer slots.
    *   `AppIcon`: SVG icon component.
    *   `AppModal`: For dialogs and wizards.
    *   `AppSpinner`: Loading indicator.
*   **Form Input Components:**
    *   `TextInput`: Standard text input with validation.
    *   `CurrencyInput`: Input formatted for thousands separators (Rial & FX).
    *   `PersianDatePicker`: A wrapper for the date picker library, supporting Persian calendar.
    *   `TypeaheadSelect`: Autocomplete/select for airlines, counterparties, etc.
    *   `CaptchaInput`: Displays and validates the captcha.
    *   `FileUpload`: For uploading logos, etc.
    *   `ToggleSwitch`: For boolean flags like `buyer=traveler`.
*   **Data Display Components:**
    *   `DataTable`: A virtualized, sortable, and filterable table with export-to-Excel/PDF functionality.
    *   `AuditTrail`: Displays the history of a document.
    *   `StatCard`: Displays a single metric (e.g., 'Total Income') on the dashboard.
    *   `BarChart`, `LineChart`: Wrappers for a charting library (e.g., Chart.js) for dashboard visuals.

### 4. Wireframe Notes per Page

*   **Login Page:** Centered card with fields for `Company/Panel`, `Username`, `Password`, and the `CaptchaInput` component. A single 'Login' button.
*   **Main Layout:** A persistent header containing the global quick search `TextInput` and user menu. A collapsible sidebar for the main navigation.
*   **Dashboard:** A grid of `StatCard` components for key metrics (Income, Expense, A/R, A/P). Below, two `BarChart` or `LineChart` components showing income/expense trends.
*   **Ticket Lists (Unissued/Issued/Canceled):**
    *   A `DataTable` component is the main feature.
    *   **Columns:** Doc No, Sale Date, Service, Passenger Name, Itinerary, Supplier, Amount, Flight Date.
    *   **Filters:** Date Range, Counterparty, Passenger, etc.
    *   **Color Rule (Unissued/Issued):** Rows will have a colored left border. **Red** if flight date is within 5 days, **Blue** otherwise. Sorting defaults to nearest flight date.
*   **Sales Document Entry Page:**
    *   A multi-step wizard inside an `AppCard`.
    *   **Header:** Buyer `TypeaheadSelect`, `ToggleSwitch` for 'Buyer is also the main traveler'.
    *   **Passenger Details:** A repeatable section for passenger name, age group (select).
    *   **Service Details:** Tabs for Air/Train/Hotel etc. Each tab has relevant fields (e.g., Airline `TypeaheadSelect`, Origin/Destination `TypeaheadSelect`, flight date `PersianDatePicker`, price `CurrencyInput`).
    *   **Footer:** 'Save' and 'Cancel' buttons. Document number is shown as 'Auto' until saved.
*   **Voucher Form:**
    *   `AppCard` with fields: Counterparty `TypeaheadSelect`, Payer Account, Our Bank Account `TypeaheadSelect`, Amount `CurrencyInput`, Tracking No., Date `PersianDatePicker`, Currency `TypeaheadSelect`.
    *   'Save', 'Export PDF', 'Export Excel' buttons.
*   **Reports:** Each report page will have a filter bar at the top (date ranges, `TypeaheadSelect` for counterparties, etc.) and a 'Generate Report' button. The results are displayed in a `DataTable` with 'Export to PDF/Excel' buttons.

### 5. Static JSON View-Models (Mock Data Shape)

This section defines the initial shape of data models that will drive the UI components before the backend API is available.

```json
// ViewModel for /sales/new (Ticket Form)
{
  "buyerId": null,
  "isBuyerTraveler": true,
  "passengers": [
    { "name": "", "ageGroup": "Adult" }
  ],
  "services": [
    {
      "type": "AirTicket",
      "airlineId": null,
      "originId": null,
      "destinationId": null,
      "flightDate": "1403/05/10",
      "price": 0,
      "currency": "IRR"
    }
  ]
}

// ViewModel for a row in /sales/tickets/issued
{
  "docId": 1052,
  "docNumber": "SL-2024-1052",
  "saleDate": "1403/05/01",
  "passengerName": "John Doe",
  "itinerary": "Tehran -> Dubai",
  "flightDate": "1403/05/08", // Used for color rule
  "status": "Issued"
}
```

### 6. Frontend Folder Structure (Vue 3)

```
dotnet_project/
└── ClientApp/
    ├── src/
    │   ├── assets/         # CSS, fonts, images
    │   ├── components/     # Reusable components (Button, Inputs, etc.)
    │   │   ├── base/
    │   │   ├── forms/
    │   │   └── layout/
    │   ├── views/          # Page components (Dashboard.vue, Login.vue)
    │   ├── router/         # Vue Router configuration (index.js)
    │   ├── store/          # Pinia state management stores
    │   ├── services/       # API layer, mocks
    │   │   └── mocks.js    # Static JSON mock data
    │   ├── styles/         # Global styles, variables
    │   ├── utils/          # Helpers, formatters (e.g., currency)
    │   └── main.js         # App entry point
    ├── public/             # Static assets
    └── package.json
```