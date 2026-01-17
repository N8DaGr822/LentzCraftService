# Production Deployment Guide

This guide covers deploying LentzCraftServices to production environments.

## Prerequisites

- .NET 9.0 Runtime installed on the server
- Database server (SQL Server, PostgreSQL, or Azure SQL)
- SSL certificate for HTTPS
- Domain name configured

## Pre-Deployment Checklist

### 1. Configuration

1. **Update `appsettings.Production.json`**:
   - Set `AllowedHosts` to your domain name(s)
   - Configure connection string for production database
   - Set admin email and password (or use environment variables)

2. **Environment Variables**:
   ```bash
   ASPNETCORE_ENVIRONMENT=Production
   ConnectionStrings__DefaultConnection=<your-production-connection-string>
   Admin__Email=<admin-email>
   Admin__Password=<secure-password>
   Database__EnableSeeding=false
   ```

### 2. Database Setup

#### Option A: Using Migrations (Recommended)

1. **Create initial migration** (if not already created):
   ```bash
   dotnet ef migrations add InitialCreate --project LentzCraftServices.Infrastructure --startup-project LentzCraftServices
   ```

2. **Apply migrations to production database**:
   ```bash
   dotnet ef database update --project LentzCraftServices.Infrastructure --startup-project LentzCraftServices
   ```

#### Option B: Using SQL Scripts

1. **Generate SQL script**:
   ```bash
   dotnet ef migrations script --project LentzCraftServices.Infrastructure --startup-project LentzCraftServices --output migration.sql
   ```

2. **Execute script on production database**

### 3. Security Configuration

1. **Change default admin credentials**:
   - Never use default credentials in production
   - Use strong, unique passwords
   - Consider using environment variables or Azure Key Vault

2. **Configure HTTPS**:
   - Ensure SSL certificate is properly configured
   - HSTS is enabled by default in production

3. **Review security headers**:
   - CSP headers are configured in `Program.cs`
   - Adjust as needed for your requirements

## Deployment Options

### Option 1: Docker Deployment

1. **Build Docker image**:
   ```bash
   docker build -t lentzcraftservices:latest .
   ```

2. **Run container**:
   ```bash
   docker run -d \
     -p 8080:8080 \
     -e ASPNETCORE_ENVIRONMENT=Production \
     -e ConnectionStrings__DefaultConnection="<connection-string>" \
     -e Admin__Email="<admin-email>" \
     -e Admin__Password="<admin-password>" \
     --name lentzcraftservices \
     lentzcraftservices:latest
   ```

3. **Using Docker Compose**:
   ```bash
   docker-compose up -d
   ```

### Option 2: Azure App Service

1. **Create App Service**:
   - Use Azure Portal or Azure CLI
   - Select .NET 9.0 runtime stack

2. **Configure Application Settings**:
   - Add connection string in Connection Strings section
   - Add environment variables in Application Settings

3. **Deploy**:
   ```bash
   az webapp deploy --resource-group <resource-group> --name <app-name> --src-path .
   ```

### Option 3: IIS Deployment

1. **Publish application**:
   ```bash
   dotnet publish -c Release -o ./publish
   ```

2. **Configure IIS**:
   - Create new application pool targeting .NET 9.0
   - Create new website pointing to publish folder
   - Configure bindings with HTTPS

3. **Set environment variables**:
   - Set `ASPNETCORE_ENVIRONMENT=Production` in web.config or system environment

### Option 4: Linux Service (systemd)

1. **Create service file** `/etc/systemd/system/lentzcraftservices.service`:
   ```ini
   [Unit]
   Description=LentzCraftServices Web Application
   After=network.target

   [Service]
   Type=notify
   ExecStart=/usr/bin/dotnet /var/www/lentzcraftservices/LentzCraftServices.dll
   Restart=always
   RestartSec=10
   Environment=ASPNETCORE_ENVIRONMENT=Production
   Environment=ConnectionStrings__DefaultConnection=<connection-string>

   [Install]
   WantedBy=multi-user.target
   ```

2. **Enable and start service**:
   ```bash
   sudo systemctl enable lentzcraftservices
   sudo systemctl start lentzcraftservices
   ```

## Post-Deployment

### 1. Verify Deployment

1. **Health Check**:
   ```bash
   curl https://yourdomain.com/health
   ```

2. **Test Admin Login**:
   - Navigate to `/Account/Login`
   - Verify login works with production credentials

3. **Check Logs**:
   - Review application logs for errors
   - Check database connectivity

### 2. Monitoring Setup

1. **Application Insights** (if using Azure):
   - Add Application Insights SDK
   - Configure instrumentation key

2. **Log Aggregation**:
   - Configure log shipping to centralized logging service
   - Set up alerts for critical errors

### 3. Backup Strategy

1. **Database Backups**:
   - Set up automated daily backups
   - Test restore procedures

2. **Application Backups**:
   - Backup configuration files
   - Document deployment process

## Maintenance

### Updating the Application

1. **Stop application**
2. **Backup database**
3. **Apply database migrations** (if any)
4. **Deploy new version**
5. **Start application**
6. **Verify functionality**

### Monitoring

- Check `/health` endpoint regularly
- Monitor application logs
- Review performance metrics
- Check database performance

## Troubleshooting

### Common Issues

1. **Database Connection Errors**:
   - Verify connection string
   - Check firewall rules
   - Verify database server is accessible

2. **Authentication Issues**:
   - Verify admin credentials are set correctly
   - Check Identity configuration

3. **Performance Issues**:
   - Review caching configuration
   - Check database indexes
   - Monitor memory usage

## Support

For issues or questions, refer to the main README.md or project documentation.
