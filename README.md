# 📚 BookAdventure - Library Management System API

A comprehensive library management system backend built with **.NET 9** and **Entity Framework Core**, implementing a layered architecture with Repository and Service patterns. Features JWT authentication with refresh tokens for secure user sessions. Ready for frontend integration with Angular, React, or any SPA framework.

## 🏗️ Project Architecture

### Layered Structure

```
BookAdventure/
├── src/
│   ├── BookAdventure.Api/          # 🌐 Presentation Layer (Controllers)
│   ├── BookAdventure.Services/     # 💼 Business Logic Layer
│   ├── BookAdventure.Repositories/ # 🗃️ Data Access Layer
│   ├── BookAdventure.Persistence/  # 🗄️ Database Context
│   ├── BookAdventure.Entities/     # 📋 Domain Models
│   └── BookAdventure.Dto/          # 📦 Data Transfer Objects
```

## ✨ Implemented Features

### 🔐 Authentication & Authorization

- ✅ JWT Token Authentication with Bearer scheme
- ✅ **Refresh Token Implementation** with automatic token renewal
- ✅ ASP.NET Identity Framework with role-based access (Admin/User roles)
- ✅ Comprehensive authorization policies on all endpoints
- ✅ User registration with automatic customer profile creation
- ✅ Login with secure password hashing and refresh token generation
- ✅ **Extended token lifetimes** optimized for hobby projects
- ✅ Swagger UI with JWT integration for testing
- ✅ Protected endpoints with ownership validation

#### 🔄 Refresh Token Features

- **Automatic Token Renewal**: Seamless user experience without frequent re-authentication
- **Long-lived Tokens**: JWT tokens valid for 7 days, refresh tokens for 90 days
- **Secure Storage**: Refresh tokens stored securely in database with expiration tracking
- **Hobby Project Optimized**: Extended lifetimes reduce server load and improve UX
- **Stateless Design**: JWT remains stateless while refresh provides persistence

### 📚 Book Management

- ✅ Complete CRUD operations for books
- ✅ Stock control and availability tracking
- ✅ **Advanced search and filtering system**
- ✅ **Multiple filtering options** (genre, author, availability)
- ✅ **Alphabetical sorting** (ascending/descending)
- ✅ **Search by genre name** (e.g., "Fantasy", "Science Fiction")
- ✅ **Combined filters** with pagination
- ✅ Genre categorization
- ✅ Image URL support for book covers

### 🔍 Advanced Search & Filtering Features

- **Genre filtering**: Filter by genre ID or genre name
- **Text search**: Search across title, author, and description
- **Alphabetical sorting**: Sort books A-Z or Z-A
- **Stock filtering**: Filter by availability (in stock/out of stock)
- **Author filtering**: Find books by specific authors
- **Combined filters**: Use multiple filters simultaneously
- **Pagination support**: Efficient handling of large result sets

### 🔄 Data Recovery & Restoration System

- ✅ **Soft deletion** for all major entities (Books, Customers, Genres, Rental Orders)
- ✅ **Individual entity restoration** with business rule validation
- ✅ **Bulk restoration operations** for multiple entities at once
- ✅ **Deleted entities overview** with comprehensive statistics
- ✅ **Conflict prevention** - duplicate name/DNI checking before restoration
- ✅ **Admin-only access** - all restoration operations require admin privileges
- ✅ **Detailed logging** for audit trails and troubleshooting
- ✅ **Query optimization** - existing queries automatically exclude deleted entities

### 🎛️ Admin Panel Management System

- ✅ **Unified admin endpoints** for comprehensive entity management
- ✅ **Admin-specific pagination** showing both active and deleted entities
- ✅ **Real-time status tracking** (Active/Deleted) across all modules
- ✅ **Bulk operations support** for efficient administration
- ✅ **Cross-entity restoration** with dependency validation
- ✅ **Administrative oversight** - complete visibility of all system data
- ✅ **Performance optimized** - efficient queries for large datasets

### 🏷️ Genre Management

- ✅ Complete CRUD operations for book genres
- ✅ Input validation and business rules
- ✅ Soft delete functionality

### 👥 Customer Management

- ✅ Complete CRUD operations for customers
- ✅ Automatic customer creation on user registration
- ✅ Rental metrics (total books, active rentals, overdue items)
- ✅ Customer profile with full details
- ✅ Integration with user authentication system

### 📋 Rental System

- ✅ **Smart rental order creation** with stock validation
- ✅ **Partial order support** - create orders with available books only
- ✅ **Individual book rental** for single-item transactions
- ✅ **Due date management** with automatic calculation
- ✅ **Return tracking** with dates and status
- ✅ **Overdue rental reports** for library management
- ✅ **Stock restoration** on book returns
- ✅ **Advanced status management** - Pending, Active, Returned, Overdue, Cancelled
- ✅ **Admin status control** - Change order status with business logic validation
- ✅ **Automatic book availability** - Restore stock when orders are returned/cancelled

### 🎯 Advanced Rental Features

- **Flexible ordering**: Choose strict mode (all-or-nothing) or partial orders
- **Real-time stock validation**: Prevents over-booking of books
- **Detailed responses**: Know exactly which books are available/unavailable
- **HTTP status codes**: 201 (success), 206 (partial), 400 (failed)
- **Status workflow**: Complete order lifecycle management from creation to completion
- **Admin controls**: Full administrative control over order statuses and transitions

## 🛠️ Technology Stack

- **.NET 9** - Latest framework with performance improvements
- **ASP.NET Core Web API** - RESTful API development
- **Entity Framework Core** - Modern ORM with migrations
- **SQL Server** - Robust relational database
- **AutoMapper** - Object-to-object mapping
- **JWT Bearer** - Stateless authentication
- **ASP.NET Identity** - User management framework
- **Serilog** - Structured logging
- **Swagger/OpenAPI** - API documentation

## 🗄️ Database Schema

### Core Entities

#### 📖 Book

```csharp
- Id, Title, Author, ISBN
- Description, Stock, IsAvailable
- GenreId (FK), ImageUrl
- CreatedAt, UpdatedAt, Status
```

#### 👤 Customer

```csharp
- Id, FullName, Email, DNI
- Phone, Address, UserId (FK)
- CreatedAt, UpdatedAt, Status
```

#### 📋 RentalOrder

```csharp
- Id, CustomerId (FK), OrderNumber
- OrderDate, DueDate, ReturnDate
- OrderStatus, Notes
- RentalOrderDetails (Collection)
```

#### 📝 RentalOrderDetail

```csharp
- Id, RentalOrderId (FK), BookId (FK)
- Quantity, RentalDays, DueDate
- ReturnDate, IsReturned, Notes
```

## 🚀 API Endpoints

### 📚 Books API

#### Basic Operations

```http
GET    /api/books              # List books with pagination
GET    /api/books/{id}         # Get book details
POST   /api/books              # Create new book [Admin]
PUT    /api/books/{id}         # Update book [Admin]
DELETE /api/books/{id}         # Soft delete book [Admin]
```

#### 🎛️ Admin Management Endpoints

```http
GET    /api/books/admin/all    # List ALL books (active + deleted) with pagination [Admin]
PUT    /api/books/{id}/restore # Restore deleted book [Admin]
GET    /api/books/deleted      # List only deleted books [Admin]
```

#### 🔍 Search & Filtering Endpoints

##### Basic Search

```http
GET    /api/books/search?title={title}                    # Search by book title
```

##### Genre Filtering

```http
GET    /api/books/genre/{genreId}                         # Filter by genre ID
GET    /api/books/genre/name/{genreName}                  # Filter by genre name (e.g., "Fantasy")
```

##### Alphabetical Sorting

```http
GET    /api/books/alphabetical?descending={true|false}    # Sort alphabetically A-Z or Z-A
GET    /api/books/genre/{genreId}/alphabetical            # Genre + alphabetical sorting
GET    /api/books/genre/name/{genreName}/alphabetical     # Genre name + alphabetical sorting
```

##### Advanced Search (Multiple Filters)

```http
GET    /api/books/advanced-search                         # Combined filters and sorting
```

#### 🔄 Restoration Endpoints

```http
GET    /api/books/deleted                             # List deleted books [Admin]
PUT    /api/books/{id}/restore                        # Restore deleted book [Admin]
```

#### 📝 Example Requests

```bash

```

**Advanced Search Parameters:**

- `genreId` (int): Filter by genre ID
- `author` (string): Filter by author name
- `search` (string): Search in title, author, description
- `inStock` (bool): Filter by availability
- `sortBy` (string): Sort field - "title", "author", "genre", "stock", "createdat"
- `sortDescending` (bool): Sort direction
- `page` (int): Page number for pagination
- `recordsPerPage` (int): Items per page

##### Additional Filters

```http
GET    /api/books/author/{author}                         # Filter by author name
GET    /api/books/in-stock                                # Only books in stock
```

#### 📝 Example Requests

```bash
# Get all Fantasy books sorted alphabetically A-Z
GET /api/books/genre/name/Fantasy/alphabetical?descending=false

# Search for "harry" in all fields, sorted by title
GET /api/books/advanced-search?search=harry&sortBy=title&sortDescending=false

# Get books by Stephen King that are in stock
GET /api/books/advanced-search?author=Stephen King&inStock=true

# Science Fiction books sorted by author Z-A
GET /api/books/advanced-search?genreId=2&sortBy=author&sortDescending=true

# Combined search: Fantasy + "magic" + in stock + sorted by title
GET /api/books/advanced-search?genreId=1&search=magic&inStock=true&sortBy=title
```

### 🏷️ Genres API

```http
GET    /api/genres             # List all active genres
GET    /api/genres/{id}        # Get genre details
POST   /api/genres             # Create new genre [Admin]
PUT    /api/genres/{id}        # Update genre [Admin]
DELETE /api/genres/{id}        # Soft delete genre [Admin]

# Admin management endpoints
GET    /api/genres/admin/all   # List ALL genres (active + deleted) with pagination [Admin]
PUT    /api/genres/{id}/restore # Restore deleted genre [Admin]
GET    /api/genres/deleted     # List only deleted genres [Admin]
```

### 👥 Customers API

```http
GET    /api/customers          # List active customers with metrics [Admin]
GET    /api/customers/{id}     # Get customer details [User - own/Admin]
PUT    /api/customers/{id}     # Update customer [User - own/Admin]
DELETE /api/customers/{id}     # Soft delete customer [Admin]
GET    /api/customers/{dni}/rented-books  # Get rented books by DNI [Admin]

# Admin management endpoints
GET    /api/customers/admin/all # List ALL customers (active + deleted) with pagination [Admin]
PUT    /api/customers/{id}/restore # Restore deleted customer [Admin]
GET    /api/customers/deleted  # List only deleted customers [Admin]
```

### 📋 Rental Orders API

```http
GET    /api/rentalorders                     # List active rental orders [Admin]
GET    /api/rentalorders/{id}               # Get order details [Admin]
POST   /api/rentalorders                    # Create rental order [User - own orders]
POST   /api/rentalorders/create-for-me      # Create order for current user [User]
POST   /api/rentalorders/rent-single-book   # Rent single book [User - own orders]
PUT    /api/rentalorders/{id}               # Update order [Admin]
PUT    /api/rentalorders/{id}/status        # Update order status [Admin]
DELETE /api/rentalorders/{id}               # Soft delete order [Admin]
PUT    /api/rentalorders/{id}/cancel        # Cancel order (business logic) [Admin]
POST   /api/rentalorders/{id}/return        # Return books [User - own orders/Admin]
GET    /api/rentalorders/my-orders          # Get user's rental orders [User]
GET    /api/rentalorders/overdue            # Get overdue rentals [Admin]

# Admin management endpoints
GET    /api/rentalorders/admin/all          # List ALL rental orders (active + deleted) with pagination [Admin]
PUT    /api/rentalorders/{id}/restore       # Restore deleted rental order [Admin]
GET    /api/rentalorders/deleted            # List only deleted rental orders [Admin]
```

#### 🎛️ Status Management Endpoint

The new status management endpoint allows administrators to change rental order status with built-in business logic validation:

```http
PUT /api/rentalorders/{id}/status
Content-Type: application/json
Authorization: Bearer {admin-token}

{
  "status": 3  // OrderStatus enum value
}
```

**Available Status Values:**
- `1` - Pending: Order created but not yet processed
- `2` - Active: Order in progress, books are rented out  
- `3` - Returned: All books have been returned successfully
- `4` - Overdue: Order has passed due date with unreturned books
- `5` - Cancelled: Order was cancelled, stock restored

**Business Logic Features:**
- ✅ **Automatic Stock Restoration**: When status changes to Returned (3) or Cancelled (5), book stock is automatically restored
- ✅ **Admin Authorization**: Only admin users can change order status
- ✅ **Validation**: Prevents invalid status transitions
- ✅ **Audit Trail**: All status changes are logged for compliance
- ✅ **Error Handling**: Comprehensive error responses for invalid operations

#### 🔄 Rental Order Lifecycle

| Action             | Endpoint              | OrderStatus Change | EntityStatus | Description                     |
| ------------------ | --------------------- | ------------------ | ------------ | ------------------------------- |
| **Create**         | `POST /create-for-me` | → `Active`         | `Active`     | New rental order                |
| **Return Partial** | `POST /{id}/return`   | `Active`           | `Active`     | Some books returned             |
| **Return All**     | `POST /{id}/return`   | → `Returned`       | `Active`     | All books returned              |
| **Cancel**         | `PUT /{id}/cancel`    | → `Cancelled`      | `Active`     | Order cancelled, stock restored |
| **Soft Delete**    | `DELETE /{id}`        | (unchanged)        | → `Deleted`  | Removed from normal queries     |
| **Restore**        | `PUT /{id}/restore`   | (unchanged)        | → `Active`   | Restored to normal queries      |

#### 📦 Return Books Request Format

```json
[1, 2, 3] // Array of book IDs to return
```

### 🛡️ Admin Management API

```http
GET    /api/admin/deleted-summary           # Get summary of deleted entities [Admin]
GET    /api/admin/deleted-entities          # Get detailed deleted entities [Admin]
POST   /api/admin/bulk-restore              # Bulk restore multiple entities [Admin]
```

### 🎛️ Admin Panel Unified Endpoints

All admin panel endpoints include both active and deleted entities for comprehensive management:

```http
# Books Management
GET    /api/books/admin/all?page=1&recordsPerPage=10      # All books (active + deleted)

# Genres Management
GET    /api/genres/admin/all?page=1&recordsPerPage=10     # All genres (active + deleted)

# Customers Management
GET    /api/customers/admin/all?page=1&recordsPerPage=10  # All customers (active + deleted)

# Rental Orders Management
GET    /api/rentalorders/admin/all?page=1&recordsPerPage=10 # All rental orders (active + deleted)
```

**Key Features of Admin Endpoints:**

- ✅ **Unified Data View**: See both active and deleted entities in one response
- ✅ **Real-time Status**: Each entity includes accurate `status` field (true=active, false=deleted)
- ✅ **Backend Pagination**: Efficient server-side pagination for large datasets
- ✅ **Restore Capability**: Direct access to restoration endpoints from admin interface
- ✅ **Performance Optimized**: Minimal queries with proper includes and ordering
- ✅ **Admin Authorization**: All endpoints require admin role for security

#### 🔄 Bulk Restore Request Format

```json
{
  "bookIds": [1, 2, 3],
  "customerIds": [4, 5],
  "genreIds": [6],
  "rentalOrderIds": [7, 8, 9]
}
```

### 🔐 Users API

```http
POST   /api/users/register           # Register new user (creates customer) [Public]
POST   /api/users/login              # User authentication with refresh token [Public]
POST   /api/users/refresh-token      # Refresh expired JWT token [Public]
GET    /api/users/profile            # Get user profile [User]
PUT    /api/users/profile            # Update user profile [User]
GET    /api/users/my-rental-orders   # Get user's rental orders [User]
```

#### 🔄 Authentication Flow

##### Login Response Format

```json
{
  "success": true,
  "data": {
    "id": "user-id",
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expirationDate": "2025-09-02T12:00:00Z",
    "refreshToken": "base64-encoded-refresh-token",
    "refreshTokenExpirationDate": "2025-11-25T12:00:00Z",
    "roles": ["User"]
  }
}
```

##### Refresh Token Request

```json
{
  "refreshToken": "your-refresh-token-here"
}
```

##### Refresh Token Response

```json
{
  "success": true,
  "data": {
    "id": "user-id",
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "token": "new-jwt-token",
    "expirationDate": "2025-09-09T12:00:00Z",
    "refreshToken": "new-refresh-token",
    "refreshTokenExpirationDate": "2025-12-02T12:00:00Z",
    "roles": ["User"]
  }
}
```

### ⚕️ Health Checks

```http
GET    /healthcheck           # API health status
```

## 🏃‍♂️ Getting Started

### 1. **Clone the Repository**

```bash
git clone https://github.com/marco-soria/bookadventure-backend.git
cd BookAdventure
```

### 2. **Configure Database**

Update `appsettings.json` with your SQL Server connection:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BookAdventureDb;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### 3. **Apply Migrations**

```bash
dotnet ef database update --project src/BookAdventure.Persistence
```

### 4. **Run the Application**

```bash
dotnet run --project src/BookAdventure.Api
```

### 5. **Access the API**

- **API Base URL**: `https://localhost:7260`
- **Swagger Documentation**: `https://localhost:7260/swagger`
- **Health Check**: `https://localhost:7260/healthcheck`

## 📊 Technical Features

### 🔧 Design Patterns

- **Repository Pattern** - Data access abstraction
- **Service Layer Pattern** - Business logic separation
- **DTO Pattern** - Optimized data transfer
- **Dependency Injection** - Loose coupling
- **Factory Pattern** - Object creation abstraction
- **Soft Delete Pattern** - Data preservation with logical deletion

### 🔄 Data Recovery Architecture

#### Soft Deletion System

- **BaseEntity Pattern**: All entities inherit from `BaseEntity` with `EntityStatus` enum
- **Status Values**: `Active`, `Deleted`
- **Query Filtering**: Automatic exclusion of deleted entities from standard queries
- **Include Deleted**: Special queries to access deleted entities for restoration

#### Restoration Validation

```csharp
// Business Rule Examples
- Books: Check for duplicate ISBN before restoration
- Customers: Validate DNI uniqueness across active customers
- Genres: Prevent duplicate genre names in active state
- Rental Orders: Validate customer and book availability
```

#### Admin Safety Features

- **Role-based Access**: Only admin users can access restoration endpoints
- **Conflict Detection**: Automatic validation prevents business rule violations
- **Audit Logging**: All restoration operations are logged for compliance
- **Batch Operations**: Bulk restore with individual result tracking

### 📈 Advanced Features

- **Automatic pagination** on all list endpoints
- **Soft delete** on all entities
- **AutoMapper profiles** for clean object mapping
- **Structured logging** with Serilog
- **Global exception handling** with custom filters
- **Automatic data seeding** on first run
- **CORS configuration** for frontend integration
- **Request/Response DTOs** for API contracts

## 🧪 Sample Data

On first run, the application automatically seeds:

- ✅ User roles (Admin, User)
- ✅ Admin user account
- ✅ Book genres (Fiction, Non-Fiction, etc.)
- ✅ Sample books with stock
- ✅ Test customers with user accounts
- ✅ Sample rental orders with different statuses

### Default Admin Account

```text
Email: admin@gmail.com
Password: Admin123!
Role: Admin
```

### Sample Customer Account

```text
Email: john.doe@example.com
Password: Customer123!
Role: User
```

## 🔧 Configuration

### 🔑 JWT & Refresh Token Settings

**Default Token Lifetimes (Hobby Project Optimized):**

- **JWT Token**: 7 days (604,800 seconds)
- **Refresh Token**: 90 days

```json
{
  "JWT": {
    "JWTKey": "your-super-secret-key-here-at-least-256-bits",
    "LifetimeInSeconds": 604800,
    "RefreshTokenExpirationDays": 90
  }
}
```

> **Note**: These extended lifetimes are optimized for hobby projects and development. For production environments, consider shorter durations:
>
> - **JWT**: 15-60 minutes (900-3600 seconds)
> - **Refresh Token**: 1-7 days

### 🔄 Token Refresh Implementation

- **Automatic Token Refresh**: Frontend automatically refreshes expired JWT tokens
- **Secure Storage**: Refresh tokens stored securely in the database
- **Base64 Encoding**: Refresh tokens are base64-encoded for safe transmission
- **Extended Sessions**: 90-day refresh token lifetime for hobby project convenience
- **One-Time Use**: Each refresh generates a new token pair for security

### CORS Configuration

The API is configured to accept requests from any origin during development. Update CORS policy for production use.

## 🎯 Project Status

✅ **PRODUCTION READY** - Complete API backend featuring:

- ✅ Layered architecture implemented
- ✅ All controllers, services, and repositories
- ✅ Database migrations and seeding
- ✅ Complete RESTful API
- ✅ JWT authentication with Swagger integration
- ✅ Comprehensive error handling
- ✅ Stock management and validation
- ✅ Partial order support
- ✅ Health monitoring
- ✅ Structured logging
- ✅ **Advanced search and filtering system**
- ✅ **Multiple filtering options** (genre, author, stock, text search)
- ✅ **Alphabetical sorting** (A-Z and Z-A)
- ✅ **Genre filtering by name** (user-friendly URLs)
- ✅ **Combined filters** with pagination support
- ✅ **Frontend-ready endpoints** with comprehensive examples

### 🆕 Latest Features (August 2025)

#### 🔍 Advanced Search & Filtering

- **Genre Name Filtering**: Search by genre names like "Fantasy", "Science Fiction"
- **Advanced Search Endpoint**: Combine multiple filters in a single request
- **Alphabetical Sorting**: Sort books A-Z or Z-A by title, author, or genre
- **Stock Filtering**: Filter by book availability (in stock/out of stock)
- **Author Filtering**: Find books by specific authors
- **Text Search**: Search across title, author, and description fields
- **Flexible Pagination**: Efficient handling of large datasets
- **Frontend Examples**: Complete integration examples for React, Angular, Vue

#### 🔄 Data Recovery System

- **Comprehensive Restoration**: Restore deleted Books, Customers, Genres, and Rental Orders
- **Admin Dashboard Endpoints**: View deleted entities summary and detailed lists
- **Bulk Restoration**: Restore multiple entities simultaneously with individual success tracking
- **Business Rule Validation**: Prevent conflicts when restoring (duplicate names, DNIs, etc.)
- **Audit Trail**: Complete logging of all restoration operations
- **Query Optimization**: Existing endpoints automatically exclude deleted entities
- **Safety Features**: Admin-only access with comprehensive error handling
- **State Management**: Clear separation between business logic states and entity lifecycle states

#### 🎛️ Advanced Status Management System

- **Admin Status Control**: New PUT /api/rentalorders/{id}/status endpoint for complete order lifecycle management
- **Intelligent Business Logic**: Automatic stock restoration when orders are returned or cancelled
- **Status Workflow**: Complete order status transitions (Pending → Active → Returned/Overdue/Cancelled)
- **Authorization Security**: Admin-only access with role-based validation
- **Audit Compliance**: Full logging of all status changes for business intelligence
- **Error Prevention**: Comprehensive validation prevents invalid status transitions
- **Stock Management**: Automatic book availability restoration for completed orders
- **Frontend Ready**: DTO-based interface ready for admin panel integration

#### 🎯 Entity State Management

The system implements a sophisticated dual-state management approach:

**Entity Status** (BaseEntity.Status):

- `Active`: Entity exists and is available for normal operations
- `Deleted`: Soft-deleted entity, excluded from normal queries, available for restoration

**Order Status** (RentalOrder.OrderStatus):

- `Pending`: Order created but not yet processed
- `Active`: Order in progress, books are rented out
- `Returned`: All books have been returned successfully
- `Overdue`: Order has passed due date with unreturned books
- `Cancelled`: Order was cancelled, stock restored, but remains in system for audit

**Key Principle**: Cancelled orders remain `EntityStatus.Active` for business intelligence, auditing, and customer service purposes while clearly marked as `OrderStatus.Cancelled` for operational logic.

## 🌐 Frontend Integration

This backend is designed to work with modern frontend frameworks:

### Recommended Frontend Stack

- **Angular** - Full-featured SPA framework
- **React** - Component-based library
- **Vue.js** - Progressive framework

### API Integration Points

- Use `/api/users/login` to get JWT token
- Include `Authorization: Bearer {token}` header for protected endpoints
- Handle 201, 206, and 400 status codes for rental operations
- Implement pagination using query parameters

### 🔍 Frontend Filter Implementation Examples

#### JavaScript/TypeScript Examples

```javascript
// Basic book search with filters
class BookService {
  // Search books with multiple filters
  async searchBooks(filters = {}) {
    const params = new URLSearchParams();

    if (filters.genreName) params.append("genreId", filters.genreId);
    if (filters.author) params.append("author", filters.author);
    if (filters.search) params.append("search", filters.search);
    if (filters.inStock !== undefined)
      params.append("inStock", filters.inStock);
    if (filters.sortBy) params.append("sortBy", filters.sortBy);
    if (filters.sortDescending)
      params.append("sortDescending", filters.sortDescending);

    params.append("page", filters.page || 1);
    params.append("recordsPerPage", filters.recordsPerPage || 10);

    const response = await fetch(`/api/books/advanced-search?${params}`);
    return response.json();
  }

  // Get books by genre name (user-friendly)
  async getBooksByGenre(
    genreName,
    sortAlphabetically = false,
    descending = false
  ) {
    const endpoint = sortAlphabetically
      ? `/api/books/genre/name/${encodeURIComponent(
          genreName
        )}/alphabetical?descending=${descending}`
      : `/api/books/genre/name/${encodeURIComponent(genreName)}`;

    const response = await fetch(endpoint);
    return response.json();
  }

  // Get books sorted alphabetically
  async getBooksAlphabetically(descending = false, page = 1) {
    const response = await fetch(
      `/api/books/alphabetical?descending=${descending}&page=${page}`
    );
    return response.json();
  }
}
```

#### React Component Example

```jsx
import React, { useState, useEffect } from "react";

const BookFilter = () => {
  const [books, setBooks] = useState([]);
  const [filters, setFilters] = useState({
    search: "",
    genreName: "",
    author: "",
    inStock: null,
    sortBy: "title",
    sortDescending: false,
  });

  const searchBooks = async () => {
    try {
      const bookService = new BookService();
      const result = await bookService.searchBooks(filters);
      setBooks(result.data || []);
    } catch (error) {
      console.error("Error searching books:", error);
    }
  };

  const handleGenreFilter = async (genreName) => {
    setFilters({ ...filters, genreName });
    const bookService = new BookService();
    const result = await bookService.getBooksByGenre(genreName, true, false);
    setBooks(result.data || []);
  };

  return (
    <div>
      {/* Search input */}
      <input
        type="text"
        placeholder="Search books..."
        value={filters.search}
        onChange={(e) => setFilters({ ...filters, search: e.target.value })}
      />

      {/* Genre filter */}
      <select onChange={(e) => handleGenreFilter(e.target.value)}>
        <option value="">All Genres</option>
        <option value="Fantasy">Fantasy</option>
        <option value="Science Fiction">Science Fiction</option>
        <option value="Mystery">Mystery</option>
      </select>

      {/* Sort controls */}
      <button
        onClick={() =>
          setFilters({ ...filters, sortBy: "title", sortDescending: false })
        }
      >
        Sort A-Z
      </button>
      <button
        onClick={() =>
          setFilters({ ...filters, sortBy: "title", sortDescending: true })
        }
      >
        Sort Z-A
      </button>

      {/* In stock filter */}
      <label>
        <input
          type="checkbox"
          checked={filters.inStock === true}
          onChange={(e) =>
            setFilters({ ...filters, inStock: e.target.checked ? true : null })
          }
        />
        In Stock Only
      </label>

      <button onClick={searchBooks}>Search</button>

      {/* Results */}
      <div>
        {books.map((book) => (
          <div key={book.id}>
            <h3>{book.title}</h3>
            <p>Author: {book.author}</p>
            <p>Genre: {book.genreName}</p>
            <p>Stock: {book.stock}</p>
          </div>
        ))}
      </div>
    </div>
  );
};
```

### 📱 Mobile-Friendly Filtering

```javascript
// Mobile app integration example
class MobileBookService {
  // Quick genre-based filtering for mobile
  async getPopularByGenre(genreName) {
    return await fetch(
      `/api/books/genre/name/${encodeURIComponent(
        genreName
      )}/alphabetical?recordsPerPage=20`
    ).then((r) => r.json());
  }

  // Search with autocomplete
  async searchWithAutocomplete(query) {
    return await fetch(
      `/api/books/advanced-search?search=${encodeURIComponent(
        query
      )}&recordsPerPage=5`
    ).then((r) => r.json());
  }
}
```

### 🎯 Common Integration Patterns

1. **Genre Dropdown**: Use `/api/genres` to populate dropdown, then filter with genre names
2. **Search Bar**: Use `/api/books/advanced-search` with `search` parameter
3. **Sort Buttons**: Toggle `sortDescending` parameter for A-Z/Z-A sorting
4. **Stock Filter**: Use `inStock` parameter for availability filtering
5. **Pagination**: Handle `TotalRecordsQuantity` header for page controls

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Follow the existing code patterns
4. Add tests for new features
5. Submit a pull request

## 📄 License

This project is for educational and portfolio purposes.

---

**Author**: Marco Soria  
**Built with**: .NET 9, Entity Framework Core, SQL Server  
**Date**: August 2025
