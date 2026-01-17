# Lentz Crafts & Services - Blazor Web Application

A professional, full-stack .NET Blazor Web application for displaying a portfolio of handcrafted items and managing inventory for a small craft business.

## Project Overview

This application serves three main purposes:
1. **Public Portfolio**: Display handcrafted items (woodworking, engraving, crochet) to potential customers
2. **Private Inventory Management**: Manage items not intended for public display
3. **Professional Portfolio Project**: Demonstrate clean architecture and full-stack .NET development skills

## Architecture

The solution follows **Clean Architecture** principles with the following project structure:

- **LentzCraftServices.Domain**: Core domain entities and enums
  - `Product` entity
  - `ProductImage` entity
  - `ProductCategory` enum (Woodworking, Engraving, Crochet)
  - `ProductStatus` enum (Available, Sold, DisplayOnly)

- **LentzCraftServices.Infrastructure**: Data access and external services
  - Entity Framework Core with SQLite
  - Repository pattern implementation
  - ASP.NET Core Identity configuration
  - Database initialization and seeding

- **LentzCraftServices** (Web): Blazor Server UI
  - Public pages (Home, Products, Product Detail, About, Contact)
  - Admin pages (Dashboard, Product Management)
  - Authentication and authorization

- **LentzCraftServices.API**: Web API project (for future expansion)

## Features

### Public Features
- **Home Page**: Hero section with featured items
- **Product Portfolio**: Browse all public products with filtering by category and status
- **Product Detail Pages**: View individual products with image galleries
- **Category Pages**: Filter products by craft type
- **About Page**: Information about the business
- **Contact Form**: Simple inquiry form (no e-commerce)

### Admin Features (Authenticated)
- **Dashboard**: Summary statistics (total items, available, sold, display only)
- **Product Management**: 
  - List all products with search and filters
  - Add/Edit/Delete products
  - Toggle public visibility
  - Manage product status and quantities

### Technical Features
- **Authentication**: ASP.NET Core Identity with single admin user
- **Database**: SQLite for development (easily switchable to SQL Server/PostgreSQL)
- **Responsive Design**: Mobile-friendly UI with craft-focused aesthetic
- **Clean Code**: Well-documented, maintainable codebase

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- Visual Studio 2022 or VS Code

### Setup Instructions

1. **Clone the repository** (or navigate to the project directory)

2. **Restore packages**:
   ```bash
   dotnet restore
   ```

3. **Build the solution**:
   ```bash
   dotnet build
   ```

4. **Run the application**:
   ```bash
   dotnet run --project LentzCraftServices
   ```

5. **Access the application**:
   - Public site: `https://localhost:7025` (or the port shown in console)
   - Admin login: Navigate to `/Account/Login`
     - Email: `admin@lentzcrafts.com`
     - Password: `Admin@123`

### Database

The SQLite database (`lentzcrafts.db`) will be created automatically on first run. Sample products are seeded automatically.

## Project Structure

```
LentzCraftServices/
├── LentzCraftServices.Domain/          # Domain entities and enums
│   ├── Entities/
│   └── Enums/
├── LentzCraftServices.Infrastructure/  # Data access layer
│   ├── Data/
│   └── Repositories/
├── LentzCraftServices/                 # Blazor Web App
│   ├── Components/
│   │   ├── Layout/
│   │   └── Pages/
│   │       ├── Account/
│   │       └── Admin/
│   └── wwwroot/
└── LentzCraftServices.API/             # Web API (future)
```

## Default Admin Credentials

**⚠️ IMPORTANT**: Change these in production!

- **Email**: `admin@lentzcrafts.com`
- **Password**: `Admin@123`

## Future Enhancements

The application is designed for easy expansion:

- **Image Upload**: Currently uses placeholder images - ready for file upload implementation
- **Cloud Storage**: ImageUrl field can be extended to support Azure Blob Storage, AWS S3, etc.
- **Custom Orders**: Order management system
- **E-commerce**: Payment processing integration
- **Email Notifications**: Contact form email delivery
- **Advanced Search**: Full-text search capabilities
- **Product Variants**: Size, color, material options

## Technologies Used

- **.NET 9.0**: Latest .NET framework
- **Blazor Server**: Interactive web UI
- **Entity Framework Core 9.0**: ORM for data access
- **SQLite**: Development database
- **ASP.NET Core Identity**: Authentication and authorization
- **Clean Architecture**: Separation of concerns

## Styling

The application uses a custom CSS design with a craft-focused aesthetic:
- Warm, natural color palette (wood tones, yarn colors)
- Clean, minimal layout
- Large imagery for product showcase
- Mobile-responsive design

## License

This is a portfolio project for demonstration purposes.

## Production Deployment

For detailed production deployment instructions, see [DEPLOYMENT.md](DEPLOYMENT.md).

### Quick Production Setup

1. **Configure environment variables**:
   ```bash
   export ASPNETCORE_ENVIRONMENT=Production
   export ConnectionStrings__DefaultConnection="<your-connection-string>"
   export Admin__Email="<admin-email>"
   export Admin__Password="<secure-password>"
   ```

2. **Apply database migrations**:
   ```bash
   dotnet ef database update --project LentzCraftServices.Infrastructure --startup-project LentzCraftServices
   ```

3. **Publish and deploy**:
   ```bash
   dotnet publish -c Release -o ./publish
   ```

## Production Features

- ✅ Security hardening (rate limiting, secure cookies, CSP headers)
- ✅ Structured logging with Serilog
- ✅ Health checks endpoint
- ✅ Caching for improved performance
- ✅ Database migrations support
- ✅ Docker containerization
- ✅ CI/CD pipeline ready
- ✅ Error handling and custom error pages
- ✅ SEO optimization (robots.txt, sitemap.xml)

## Notes

- The database is seeded with sample products on first run (Development only)
- Only products with `IsPublic = true` are visible on public pages
- Admin pages require authentication
- All admin functionality is protected with `[Authorize]` attribute
- **Never use default admin credentials in production!**

