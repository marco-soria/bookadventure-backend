# BookAdventure - Sistema de Gestión de Biblioteca

Sistema completo de gestión de biblioteca desarrollado con **.NET 9** y **Entity Framework Core**, implementando una arquitectura en capas con patrones Repository y Service.

## 🏗️ Arquitectura del Proyecto

### Estructura de Capas

```
BookAdventure/
├── src/
│   ├── BookAdventure.Api/          # 🌐 Capa de Presentación (Controllers)
│   ├── BookAdventure.Services/     # 💼 Capa de Lógica de Negocio
│   ├── BookAdventure.Repositories/ # 🗃️ Capa de Acceso a Datos
│   ├── BookAdventure.Persistence/  # 🗄️ Contexto de Base de Datos
│   ├── BookAdventure.Entities/     # 📋 Modelos de Dominio
│   └── BookAdventure.Dto/          # 📦 Objetos de Transferencia de Datos
```

## ✨ Funcionalidades Implementadas

### 🔐 Autenticación y Autorización

- ✅ JWT Token Authentication
- ✅ Identity Framework con roles
- ✅ Registro y login de usuarios
- ✅ Recuperación de contraseña

### 📚 Gestión de Libros

- ✅ CRUD completo de libros
- ✅ Control de disponibilidad (`IsAvailable`)
- ✅ Búsqueda y filtrado
- ✅ Paginación
- ✅ Subida de imágenes

### 🏷️ Gestión de Géneros

- ✅ CRUD completo de géneros
- ✅ Validaciones
- ✅ Soft delete

### 👥 Gestión de Clientes

- ✅ CRUD completo de clientes
- ✅ Métricas de alquiler (total, activos, vencidos)
- ✅ Perfil completo con AutoMapper

### 📋 Sistema de Alquiler

- ✅ Creación de órdenes de alquiler
- ✅ Gestión de detalles de alquiler
- ✅ Fechas de vencimiento (`DueDate`)
- ✅ Control de devoluciones (`IsReturned`, `ReturnDate`)
- ✅ Reportes de alquileres

## 🛠️ Tecnologías Utilizadas

- **.NET 9** - Framework principal
- **ASP.NET Core Web API** - API REST
- **Entity Framework Core** - ORM
- **SQL Server** - Base de datos
- **AutoMapper** - Mapeo objeto-objeto
- **JWT Bearer** - Autenticación
- **Identity Framework** - Gestión de usuarios
- **Serilog** - Logging

## 🗄️ Base de Datos

### Entidades Principales

#### 📖 Book

```csharp
- Id, Title, Author, ISBN
- PublicationDate, Price
- IsAvailable (nuevo) ✨
- GenreId, ImageUrl
```

#### 📋 RentalOrder

```csharp
- Id, CustomerId, RentalDate
- TotalPrice, Status
- RentalOrderDetails (colección)
```

#### 📝 RentalOrderDetail

```csharp
- Id, RentalOrderId, BookId
- Quantity, UnitPrice, TotalPrice
- DueDate (nuevo) ✨
- ReturnDate (nuevo) ✨
- IsReturned (nuevo) ✨
```

## 🚀 Endpoints Implementados

### 📚 Books API

- `GET /api/books` - Listar libros (con paginación)
- `GET /api/books/{id}` - Obtener libro por ID
- `POST /api/books` - Crear libro
- `PUT /api/books/{id}` - Actualizar libro
- `DELETE /api/books/{id}` - Eliminar libro (soft delete)

### 🏷️ Genres API

- `GET /api/genres` - Listar géneros
- `GET /api/genres/{id}` - Obtener género por ID
- `POST /api/genres` - Crear género
- `PUT /api/genres/{id}` - Actualizar género
- `DELETE /api/genres/{id}` - Eliminar género

### 👥 Customers API

- `GET /api/customers` - Listar clientes
- `GET /api/customers/{id}` - Obtener cliente por ID
- `POST /api/customers` - Crear cliente
- `PUT /api/customers/{id}` - Actualizar cliente
- `DELETE /api/customers/{id}` - Eliminar cliente

### 📋 Rental Orders API

- `GET /api/rental-orders` - Listar órdenes
- `GET /api/rental-orders/{id}` - Obtener orden por ID
- `POST /api/rental-orders` - Crear orden
- `PUT /api/rental-orders/{id}` - Actualizar orden
- `DELETE /api/rental-orders/{id}` - Eliminar orden

### 🔐 Users API

- `POST /api/users/register` - Registrar usuario
- `POST /api/users/login` - Iniciar sesión
- `POST /api/users/reset-password` - Solicitar reset
- `POST /api/users/new-password` - Nueva contraseña

## 🏃‍♂️ Cómo Ejecutar

1. **Clonar el repositorio**

```bash
git clone <repository-url>
cd BookAdventure
```

2. **Configurar la base de datos**

```bash
# Actualizar cadena de conexión en appsettings.json
# Aplicar migraciones
dotnet ef database update --project src/BookAdventure.Persistence
```

3. **Ejecutar la aplicación**

```bash
dotnet run --project src/BookAdventure.Api/BookAdventure.Api.csproj
```

4. **Acceder a la API**

- API: `https://localhost:7260`
- Swagger: `https://localhost:7260/swagger`

## 📊 Características Técnicas

### 🔧 Patrones Implementados

- **Repository Pattern** - Abstracción de acceso a datos
- **Service Layer** - Lógica de negocio centralizada
- **DTO Pattern** - Transferencia de datos optimizada
- **Dependency Injection** - Inversión de dependencias
- **Unit of Work** - Gestión de transacciones

### 📈 Funcionalidades Avanzadas

- **Paginación** automática en listados
- **Soft Delete** en todas las entidades
- **AutoMapper** para mapeo automático
- **Logging** estructurado con Serilog
- **Validaciones** personalizadas
- **Filtros de excepción** globales
- **Seeding** automático de datos

## 🧪 Datos de Prueba

Al ejecutar la aplicación por primera vez, se crearán automáticamente:

- ✅ Roles de usuario (Admin, Customer)
- ✅ Usuario administrador
- ✅ Géneros literarios
- ✅ Libros de muestra
- ✅ Clientes de prueba
- ✅ Órdenes de alquiler

## 🔧 Configuración

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=BookAdventureDb;..."
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key",
    "Issuer": "BookAdventure",
    "Audience": "BookAdventure"
  }
}
```

## 🎯 Estado del Proyecto

✅ **COMPLETADO** - Proyecto totalmente funcional con:

- Arquitectura en capas implementada
- Todos los controllers, services y repositories
- Base de datos migrada y funcionando
- API REST completa
- Autenticación JWT
- Seeding de datos
- Documentación completa

---

**Autor**: Desarrollado con .NET 9 y Entity Framework Core
**Fecha**: Enero 2025
