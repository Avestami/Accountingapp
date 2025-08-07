# Travel Accounting System - Setup Guide

## Prerequisites
- .NET 6.0 SDK or later
- Node.js 16+ and npm
- SQL Server LocalDB (comes with Visual Studio)

## Database Setup

### 1. Create and Migrate Database
```bash
# Navigate to project root
cd "d:\work\Karlancer\Accountingproject1"

# Build the solution first
dotnet build

# Run database migrations
dotnet ef database update --project src/Accounting.Infrastructure --startup-project src/Accounting.API
```

### 2. Database Connection
- **Database Name**: `AccountingDb`
- **Connection String**: `Server=(localdb)\mssqllocaldb;Database=AccountingDb;Trusted_Connection=true;MultipleActiveResultSets=true`

## Running the Servers

### Backend API Server
```bash
# Navigate to project root
cd "d:\work\Karlancer\Accountingproject1"

# Run the backend API
dotnet run --project src/Accounting.API/Accounting.API.csproj
```
- **Backend URLs**: 
  - HTTPS: `https://localhost:5001`
  - HTTP: `http://localhost:5000`
- **API Base**: `/api`
- **Swagger UI**: `https://localhost:5001/swagger`

### Frontend Development Server
```bash
# Navigate to frontend directory
cd "d:\work\Karlancer\Accountingproject1\travel-accounting-ui"

# Install dependencies (first time only)
npm install

# Run the frontend development server
npm run dev
```
- **Frontend URL**: `http://localhost:5174` (or next available port)

## Default Users in Database

### Admin User (Seeded by Migration)
- **Username**: `admin`
- **Password**: `admin123`
- **Role**: Administrator
- **Full Name**: Admin User
- **Email**: admin@example.com
- **Status**: Active

## Frontend Routes

### Authentication
- `/login` - Login page

### Sales Management
- `/sales/unissued` - Unissued tickets view
- `/sales/issued` - Issued tickets view (displays passenger lists)
- `/sales/canceled` - Canceled tickets view
- `/sales/create` - Create new sales document (with passenger form for ticket sales)
- `/sales/edit/:id` - Edit existing sales document

### Finance Management
- `/finance/voucher` - Voucher management
- `/finance/costs` - Cost management
- `/finance/incomes` - Income management
- `/finance/transfer` - Transfer management

### Reports
- `/reports/sales` - Sales reports
- `/reports/finance` - Financial reports
- `/reports/tickets` - Ticket reports

### Settings
- `/settings/users` - User management
- `/settings/airlines` - Airline management
- `/settings/banks` - Bank management
- `/settings/counterparties` - Counterparty management
- `/settings/locations` - Location management
- `/settings/system` - System settings

### Error Pages
- `*` - 404 Not Found page (catch-all route)

## API Endpoints

### Authentication
- `POST /api/auth/login` - User login
  - Body: `{"username": "admin", "password": "admin123"}`
  - Returns: JWT token

### Vouchers
- `GET /api/vouchers` - Get all vouchers
- `POST /api/vouchers` - Create voucher
- `PUT /api/vouchers/{id}/submit` - Submit voucher
- `PUT /api/vouchers/{id}/approve` - Approve voucher
- `PUT /api/vouchers/{id}/reject` - Reject voucher
- `PUT /api/vouchers/{id}/post` - Post voucher

## Key Features Implemented

### Frontend Features
1. **Persian Date Picker**: Includes Gregorian toggle and "Go to Today" button
2. **Sales Document Creation**: Conditional passenger information form for ticket sales
3. **Passenger Management**: Add/remove passengers with detailed fields (name, passport, nationality, etc.)
4. **Issued Tickets View**: Displays multiple passengers per ticket instead of single passenger name
5. **Responsive Design**: Modern UI with Tailwind CSS
6. **Multi-language Support**: Persian/Farsi interface

### Backend Features
1. **Clean Architecture**: Domain, Application, Infrastructure layers
2. **Entity Framework Core**: Database ORM with migrations
3. **JWT Authentication**: Secure token-based authentication
4. **CQRS Pattern**: Command Query Responsibility Segregation with MediatR
5. **Repository Pattern**: Generic repository implementation
6. **AutoMapper**: Object-to-object mapping
7. **Swagger Documentation**: API documentation and testing interface

## Troubleshooting

### Common Issues
1. **404 Routes**: Make sure to use correct route paths as listed above
2. **Login Issues**: 
   - Ensure database is migrated and use default admin credentials
   - Make sure both backend and frontend servers are running
   - The frontend uses a proxy to handle SSL certificate issues with the backend
3. **Build Errors**: Stop all dotnet processes if DLL files are locked:
   ```powershell
   Get-Process dotnet -ErrorAction SilentlyContinue | Stop-Process -Force
   ```
4. **Database Connection**: Ensure SQL Server LocalDB is installed and running
5. **SSL Certificate Issues**: The Vite proxy configuration handles self-signed certificate issues automatically

### Development Tips
1. Both servers support hot reload during development
2. Check browser console and terminal for error messages
3. Use Swagger UI for API testing: `https://localhost:5001/swagger`
4. Frontend uses Vite for fast development builds

## Project Structure

### Backend (Clean Architecture)
```
src/
├── Accounting.API/          # Web API layer
├── Accounting.Application/   # Application logic layer
├── Accounting.Domain/        # Domain entities and business logic
└── Accounting.Infrastructure/ # Data access and external services
```

### Frontend (Vue.js)
```
travel-accounting-ui/
├── src/
│   ├── components/    # Reusable Vue components
│   ├── views/        # Page components
│   ├── router/       # Vue Router configuration
│   ├── stores/       # Pinia state management
│   ├── services/     # API service calls
│   └── utils/        # Utility functions
└── public/           # Static assets
```

This setup provides a complete travel accounting system with modern architecture, secure authentication, and comprehensive functionality for managing sales, finance, and reporting operations.