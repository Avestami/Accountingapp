# Travel Accounting System - Server Setup Guide

## Running the Servers

### Backend (.NET API Server)

```bash
# Navigate to the API project directory
cd src/Accounting.API

# Run the .NET backend server
dotnet run
```

**Backend Server URLs:**
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

### Frontend (Vue.js Development Server)

```bash
# Navigate to the frontend directory
cd travel-accounting-ui

# Run the Vue.js development server
npm run dev
```

**Frontend Server URL:**
- Local: `http://localhost:5173/`

## Available User Accounts

### Frontend Users (mockData.js)

#### 1. Admin User
- **Username:** `admin`
- **Password:** `admin123`
- **Company:** `demo`
- **Role:** `admin` (System Administrator)
- **Name:** مدیر سیستم (System Admin)
- **Email:** admin@travel-accounting.com
- **Permissions:** admin, sales, finance, reports, settings

#### 2. Sales User
- **Username:** `sales1`
- **Password:** `sales123`
- **Company:** `demo`
- **Role:** `sales` (Sales Representative)
- **Name:** احمد محمدی (Ahmad Mohammadi)
- **Email:** ahmad@travel-accounting.com
- **Permissions:** sales

#### 3. Finance User
- **Username:** `finance1`
- **Password:** `finance123`
- **Company:** `demo`
- **Role:** `finance` (Finance Manager)
- **Name:** فاطمه احمدی (Fatemeh Ahmadi)
- **Email:** fatemeh@travel-accounting.com
- **Permissions:** finance

#### 4. Test User
- **Username:** `test`
- **Password:** `test123`
- **Company:** `demo`
- **Role:** `user` (Regular User)
- **Name:** کاربر تست (Test User)
- **Email:** test@travel-accounting.com
- **Permissions:** sales, reports

### Backend API Users (AuthController.cs)

#### 1. Admin
- **Username:** `admin`
- **Password:** `admin123`
- **Company:** `demo`
- **Role:** `admin`
- **Name:** مدیر سیستم

#### 2. Accountant
- **Username:** `accountant`
- **Password:** `acc123`
- **Company:** `demo`
- **Role:** `accountant`
- **Name:** حسابدار اصلی

#### 3. Regular User
- **Username:** `user`
- **Password:** `user123`
- **Company:** `demo`
- **Role:** `user`
- **Name:** کاربر عادی

## Role Descriptions

- **admin**: Full system access with all permissions
- **sales**: Sales operations and ticket management
- **finance**: Financial operations and accounting
- **accountant**: Accounting and financial reporting
- **user**: Basic user with limited permissions

## Login Instructions

1. Access the application at `http://localhost:5173/`
2. Use any of the above username/password combinations
3. Set the company field to `demo` for all users
4. The system uses Persian (Farsi) interface

## Notes

- All users are currently active and ready for testing
- The system includes comprehensive settings management
- Features include sales document handling and financial reporting
- Mock data is used for development and testing purposes