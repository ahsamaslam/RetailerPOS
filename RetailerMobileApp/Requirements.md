# 📱 MAUI POS Application – Copilot Execution Plan

This document describes a **step-by-step, Copilot-driven plan** to build a **cross-platform .NET MAUI POS application** that integrates with the existing **Auth Module** and **Retailer.API**.

---

## 🎯 Objective

Build a **.NET MAUI POS Client Application** that:

* Authenticates users using **Auth Module**
* Consumes **Retailer.API** for business operations
* Supports **multi-company & multi-branch** access
* Runs on **Android, iOS, iPad, Windows, macOS**
* Provides **Sales, Purchases, and Reports** modules
* Follows **Clean Architecture + MVVM** best practices

---

## 🧱 Existing System Overview

### Current Modules

1. **Auth Module**

   * Companies
   * Users
   * Roles & Permissions
   * Authentication & Authorization APIs

2. **Retailer.API**

   * Branches
   * Sales
   * Purchases
   * Items
   * Business rules & validations

3. **Web Application (Razor Pages)**

   * UI for Users, Companies, Sales, Purchases, Items
   * Calls Auth & Retailer APIs via HTTP

---

## 1️⃣ New Project Setup

### 1.1 Create MAUI Project

* **Project Name:** `Retailer.POS.Mobile`
* **Template:** `.NET MAUI App`
* **Target Platforms:**

  * Android
  * iOS
  * Mac Catalyst
  * Windows

---

## 2️⃣ Application Architecture

### 2.1 Architectural Pattern

* **MVVM (Model–View–ViewModel)**
* **Clean Architecture** separation
* API-first approach (no business logic in UI)

### 2.2 Project Structure

```text
Retailer.POS.Mobile
│
├── Core
│   ├── Models            # DTOs (shared with APIs)
│   ├── Interfaces        # Service contracts
│   └── Constants
│
├── Infrastructure
│   ├── Api
│   │   ├── AuthApiClient
│   │   ├── SalesApiClient
│   │   ├── PurchaseApiClient
│   │   └── ReportsApiClient
│   ├── Http
│   │   ├── AuthenticatedHttpClient
│   │   └── ApiExceptionHandler
│   └── Storage
│       ├── SecureStorageService
│       └── PreferencesService
│
├── Features
│   ├── Authentication
│   ├── Sales
│   ├── Purchases
│   └── Reports
│
├── ViewModels
├── Views
└── AppShell.xaml
```

---

## 3️⃣ Authentication & Authorization

### 3.1 Authentication Flow

1. User enters:

   * Username
   * Password
   * Company Code / CompanyId
2. Auth Module validates credentials
3. API returns:

   * JWT Access Token
   * Refresh Token
   * UserId
   * CompanyId
   * BranchId
   * Permissions

### 3.2 Token Storage

* Use **SecureStorage** for sensitive data:

  * AccessToken
  * RefreshToken
  * UserId
  * CompanyId
  * BranchId

### 3.3 Authorization

* JWT claims drive:

  * Feature visibility
  * UI access control
  * API authorization

---

## 4️⃣ API Communication Layer

### 4.1 API Endpoints

* **Auth API:** `https://auth.yourdomain.com`
* **Retailer API:** `https://api.yourdomain.com`

### 4.2 HTTP Client Strategy

* Typed `HttpClient`
* DelegatingHandler for JWT injection
* Automatic token refresh on 401
* Centralized error handling

---

## 5️⃣ Navigation & Shell Design

### 5.1 AppShell Navigation

```text
Login
      └── Dashboard
           ├── Sales
           │    ├── Create Sale
           │    └── Sales History
           ├── Purchases
           │    ├── Create Purchase
           │    └── Purchase History
           └── Reports
                ├── Daily Sales
                ├── Monthly Sales
                └── Stock Summary
```

* Use **Shell navigation**
* Flyout or Bottom Tabs depending on platform

---

## 6️⃣ Feature Modules

### 🔐 Authentication Module

**Screens**

* Login Page
* Company Selection
* Branch Selection

**ViewModels**

* LoginViewModel
* CompanySelectionViewModel
* BranchSelectionViewModel

---

### 🧾 Sales Module

**Screens**

* Sales List
* Create Sale
* Sale Details

**Features**

* Item lookup
* Quantity & price input
* Discount & GST calculation
* Auto total calculation
* Submit sale to API

---

### 📦 Purchases Module

**Screens**

* Purchase List
* Create Purchase

**Features**

* Supplier selection
* Item selection
* Quantity & cost
* Inventory update via API

---

### 📊 Reports Module

**Screens**

* Daily Sales Report
* Monthly Sales Report
* Item-wise Sales

**Features**

* Date filters
* Branch-based filtering
* Chart & list views

---

## 7️⃣ Offline Support (Optional)

### 7.1 Offline Strategy

* Local SQLite database
* Cache master data (Items, Branches)
* Queue Sales & Purchases when offline
* Background sync when online

---

## 8️⃣ Shared DTO Strategy

### Recommended Approach

Create a shared project:

```text
Retailer.Shared.Contracts
```

Contains:

* Auth DTOs
* Sales DTOs
* Purchase DTOs
* Report DTOs

Referenced by:

* Auth Module
* Retailer.API
* MAUI Mobile App

---

## 9️⃣ Configuration & Environments

### 9.1 Environments

* Development
* Staging
* Production

### 9.2 Configuration Files

* `appsettings.json`
* `appsettings.Development.json`

---

## 🔟 Copilot Usage Rules

Use these rules when prompting Copilot:

* Follow MVVM strictly
* No business logic in Views
* Always use ApiClient for HTTP calls
* Do not duplicate DTOs
* Use async/await everywhere
* Use Shell navigation
* Handle token expiration
* Follow Clean Architecture boundaries

---

## 🧠 Recommended Tech Stack

| Area    | Technology               |
| ------- | ------------------------ |
| UI      | .NET MAUI (XAML)         |
| MVVM    | CommunityToolkit.Mvvm    |
| Storage | SecureStorage + SQLite   |
| API     | HttpClient               |
| Auth    | JWT                      |
| Charts  | Syncfusion / Microcharts |
| DI      | Built-in MAUI DI         |

---

## ✅ Final Outcome

By following this plan, the MAUI application will:

* Be fully integrated with existing Auth & Retailer APIs
* Support mobile, tablet, and desktop platforms
* Be secure, scalable, and maintainable
* Be easy to extend (Inventory, Payments, Printers, etc.)

---

📌 **Next Steps (Optional)**

* Generate MAUI solution skeleton
* Define Auth & Sales API contracts
* Build Login UI
* Implement Sales Create screen
* Add offline sync

---
