# MultiTenantApi

## Description

This project is a multi-tenant application built with ASP.NET Core and Finbuckle.MultiTenant. It demonstrates how to implement multi-tenancy at the database level, 
where each tenant has its own database.

another project for shared db uses Finbuckle.MultiTenant.
and alst one for manual implementaion 
ِمسخ شىخفاثق ش
## Setup

### Prerequisites

- .NET Core 8.0
- SQL Server

### Installation

1. Clone the repository
2. Navigate to the project directory
3. Run `dotnet restore`
4. Update the connection strings in `appsettings.json`
5. Run `dotnet ef database update` to apply the migrations
6. Run `dotnet run` to start the application

## Usage

The application exposes several endpoints:

- `GET /product/list`: Returns a list of all products for the current tenant.
- `GET /product/{id}`: Returns the product with the specified ID for the current tenant.
- `POST /product`: Adds a new product for the current tenant.

The current tenant is determined by the `tenant` header in the request.

## Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

