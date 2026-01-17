# Database Migrations

This project uses Entity Framework Core migrations for database schema management.

## Creating a Migration

To create a new migration after making changes to entities:

```bash
dotnet ef migrations add MigrationName --project LentzCraftServices.Infrastructure --startup-project LentzCraftServices
```

## Applying Migrations

### Development
Migrations are automatically applied when the application starts in Development mode.

### Production
Apply migrations manually before deploying:

```bash
dotnet ef database update --project LentzCraftServices.Infrastructure --startup-project LentzCraftServices
```

Or use the migration script in CI/CD pipeline.

## Migration Scripts

To generate a SQL script for migrations:

```bash
dotnet ef migrations script --project LentzCraftServices.Infrastructure --startup-project LentzCraftServices --output migration.sql
```

## Important Notes

- **Never use `EnsureCreated()` in production** - Always use migrations
- Review generated migrations before applying
- Test migrations on a copy of production data first
- Always backup the database before applying migrations
