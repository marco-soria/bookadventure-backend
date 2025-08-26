# 📚 BookAdventure - Library Management System API

A comprehensive library management system backend built with **.NET 9** and **Entity Framework Core**, implementing a layered architecture with Repository and Service patterns. Ready for frontend integration with Angular, React, or any SPA framework.

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
- ✅ ASP.NET Identity Framework with role-based access (Admin/User roles)
- ✅ Comprehensive authorization policies on all endpoints
- ✅ User registration with automatic customer profile creation
- ✅ Login with secure password hashing
- ✅ Swagger UI with JWT integration for testing
- ✅ Protected endpoints with ownership validation

### 📚 Book Management

- ✅ Complete CRUD operations for books
- ✅ Stock control and availability tracking
- ✅ Book search and filtering capabilities
- ✅ Pagination support for large datasets
- ✅ Genre categorization
- ✅ Image URL support for book covers

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

### 🎯 Advanced Rental Features

- **Flexible ordering**: Choose strict mode (all-or-nothing) or partial orders
- **Real-time stock validation**: Prevents over-booking of books
- **Detailed responses**: Know exactly which books are available/unavailable
- **HTTP status codes**: 201 (success), 206 (partial), 400 (failed)

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

```
GET    /api/books              # List books with pagination
GET    /api/books/{id}         # Get book details
POST   /api/books              # Create new book
PUT    /api/books/{id}         # Update book
DELETE /api/books/{id}         # Soft delete book
```

### 🏷️ Genres API

```
GET    /api/genres             # List all genres
GET    /api/genres/{id}        # Get genre details
POST   /api/genres             # Create new genre
PUT    /api/genres/{id}        # Update genre
DELETE /api/genres/{id}        # Soft delete genre
```

### 👥 Customers API

```
GET    /api/customers          # List customers with metrics
GET    /api/customers/{id}     # Get customer details
PUT    /api/customers/{id}     # Update customer
DELETE /api/customers/{id}     # Soft delete customer
GET    /api/customers/{dni}/rented-books  # Get rented books by DNI
```

### 📋 Rental Orders API

```http
GET    /api/rentalorders                     # List rental orders [Admin]
GET    /api/rentalorders/{id}               # Get order details [Admin]
POST   /api/rentalorders                    # Create rental order [User - own orders]
POST   /api/rentalorders/create-for-me      # Create order for current user [User]
POST   /api/rentalorders/rent-single-book   # Rent single book [User - own orders]
PUT    /api/rentalorders/{id}               # Update order [Admin]
DELETE /api/rentalorders/{id}               # Cancel order [Admin]
POST   /api/rentalorders/{id}/return        # Return books [User - own orders/Admin]
GET    /api/rentalorders/my-orders          # Get user's rental orders [User]
GET    /api/rentalorders/overdue            # Get overdue rentals [Admin]
```

### 🔐 Users API

```http
POST   /api/users/register           # Register new user (creates customer) [Public]
POST   /api/users/login              # User authentication [Public]
GET    /api/users/profile            # Get user profile [User]
PUT    /api/users/profile            # Update user profile [User]
GET    /api/users/my-rental-orders   # Get user's rental orders [User]
```

### ⚕️ Health Checks

```
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

### JWT Settings

```json
{
  "JWT": {
    "JWTKey": "your-super-secret-key-here-at-least-256-bits",
    "LifetimeInSeconds": 86400
  }
}
```

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
