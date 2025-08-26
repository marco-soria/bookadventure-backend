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

#### Basic Operations

```http
GET    /api/books              # List books with pagination
GET    /api/books/{id}         # Get book details
POST   /api/books              # Create new book [Admin]
PUT    /api/books/{id}         # Update book [Admin]
DELETE /api/books/{id}         # Soft delete book [Admin]
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
- ✅ **Advanced search and filtering system**
- ✅ **Multiple filtering options** (genre, author, stock, text search)
- ✅ **Alphabetical sorting** (A-Z and Z-A)
- ✅ **Genre filtering by name** (user-friendly URLs)
- ✅ **Combined filters** with pagination support
- ✅ **Frontend-ready endpoints** with comprehensive examples

### 🆕 Latest Features (August 2025)

- **Genre Name Filtering**: Search by genre names like "Fantasy", "Science Fiction"
- **Advanced Search Endpoint**: Combine multiple filters in a single request
- **Alphabetical Sorting**: Sort books A-Z or Z-A by title, author, or genre
- **Stock Filtering**: Filter by book availability (in stock/out of stock)
- **Author Filtering**: Find books by specific authors
- **Text Search**: Search across title, author, and description fields
- **Flexible Pagination**: Efficient handling of large datasets
- **Frontend Examples**: Complete integration examples for React, Angular, Vue

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
