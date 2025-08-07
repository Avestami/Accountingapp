# Backend Implementation Plan

## 1. NuGet Packages Setup

### API Project
```bash
dotnet add src/Accounting.API package Microsoft.EntityFrameworkCore.Design
dotnet add src/Accounting.API package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add src/Accounting.API package Microsoft.AspNetCore.Authentication.Cookies
dotnet add src/Accounting.API package Swashbuckle.AspNetCore
```

### Application Project
```bash
dotnet add src/Accounting.Application package MediatR
dotnet add src/Accounting.Application package FluentValidation
dotnet add src/Accounting.Application package FluentValidation.DependencyInjectionExtensions
dotnet add src/Accounting.Application package AutoMapper
dotnet add src/Accounting.Application package AutoMapper.Extensions.Microsoft.DependencyInjection
```

### Infrastructure Project
```bash
dotnet add src/Accounting.Infrastructure package Microsoft.EntityFrameworkCore.SqlServer
dotnet add src/Accounting.Infrastructure package Microsoft.EntityFrameworkCore.Tools
dotnet add src/Accounting.Infrastructure package Microsoft.AspNetCore.Identity.EntityFrameworkCore
```

## 2. Domain Entities

### Core Entities
- **Company**: Multi-tenant support
- **User**: Authentication with company scope
- **Counterparty**: Buyers/suppliers with opening balances (Rial/FX)
- **Airline**: Flight service providers
- **Bank**: Banking institutions
- **BankAccount**: Company bank accounts with opening balances
- **Voucher**: Financial vouchers
- **Cost**: Expense transactions
- **Income**: Revenue transactions
- **Transfer**: Inter-account transfers
- **ServiceOrder**: Ticket/Hotel/Tour/Mixed orders
- **AuditEntry**: Change tracking

### Value Objects
- **Money**: Currency amount with precision
- **DocumentNumber**: Auto-generated unique numbers
- **ExchangeRate**: FX rates with timestamps

## 3. Authentication & Authorization

### Features
- Cookie/JWT hybrid authentication
- Company-scoped access
- Role-based permissions
- Captcha verification stub
- Account lockout after failed attempts

### Implementation
- ASP.NET Core Identity
- Custom UserManager with company filtering
- JWT tokens with company claims
- Permission-based authorization policies

## 4. Document Numbering System

### Requirements
- Generate on successful create only
- Unique per company
- Thread-safe generation
- Sequential numbering

### Implementation
- Database sequence per company/document type
- Optimistic concurrency control
- Retry mechanism for conflicts

## 5. Ticket Workflow APIs

### States
- **Unissued**: Initial state, editable
- **Issued**: Finalized, limited editing
- **Canceled**: Final state, read-only

### Business Rules
- Editing rules based on state
- Cancel rules with validation
- 5-day rule for color flagging
- Server-side sorting and computed fields

## 6. Finance APIs

### Voucher Management
- CRUD operations
- Multi-currency support
- Approval workflow

### Cost/Income Tracking
- Payment source integration
- Category classification
- Budget tracking

### Transfer Operations
- A→B transfers
- Double-entry posting
- Both account statements updated
- Reconciliation support

## 7. FX Smart Inventory (FIFO)

### Features
- FIFO per counterparty
- Currency lot tracking
- Running local-currency valuation
- Realized/unrealized gains

### Implementation
- FX lot entity with purchase details
- FIFO consumption algorithm
- Valuation service with historical rates
- Unit tests for complex scenarios

## 8. Reports System

### Report Types
- Financial statements (P&L, Balance Sheet)
- Counterparty statements
- FX position reports
- Audit trail reports

### Export Formats
- PDF generation
- Excel export
- Filtered datasets
- Real-time data

## 9. Audit Trail

### Implementation
- EF Core interceptors
- Change tracking middleware
- Created/modified timestamps
- User attribution
- Before/after values

### Storage
- Separate audit tables
- JSON serialization for complex changes
- Retention policies

## 10. Database Design

### Key Considerations
- Decimal precision for money (18,4)
- Composite keys where needed
- Proper indexing strategy
- Foreign key constraints
- Soft delete support

### Migrations Strategy
- Code-first approach
- Seed data in migrations
- Environment-specific data
- Rollback procedures

## 11. Docker Configuration

### Services
- SQL Server 2022
- .NET 8 API
- Vue.js UI (existing)

### Features
- Development environment
- Production-ready setup
- Health checks
- Volume persistence

## 12. Testing Strategy

### Unit Tests
- Domain logic
- FX inventory calculations
- Business rule validation
- Service layer methods

### Integration Tests
- API endpoints
- Database operations
- Authentication flows
- Report generation

## 13. Sample HTTP Calls

### Authentication
```http
POST /api/auth/login
POST /api/auth/refresh
POST /api/auth/logout
```

### Tickets
```http
GET /api/tickets?status=unissued
POST /api/tickets
PUT /api/tickets/{id}
PATCH /api/tickets/{id}/issue
PATCH /api/tickets/{id}/cancel
```

### Finance
```http
GET /api/vouchers
POST /api/vouchers
POST /api/transfers
GET /api/statements/{accountId}
```

### Reports
```http
GET /api/reports/profit-loss?from=2024-01-01&to=2024-12-31
GET /api/reports/counterparty-statement/{id}
GET /api/reports/fx-position
```

## Implementation Order

1. ✅ Solution scaffolding
2. 🔄 NuGet packages and basic setup
3. 🔄 Domain entities and EF configurations
4. 🔄 Authentication system
5. 🔄 Document numbering service
6. 🔄 Basic CRUD operations
7. 🔄 Ticket workflow
8. 🔄 Finance operations
9. 🔄 FX inventory system
10. 🔄 Reports and exports
11. 🔄 Audit trail
12. 🔄 Docker setup
13. 🔄 Testing and documentation