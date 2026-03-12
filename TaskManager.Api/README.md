# Task Manager API

A secure REST API for managing personal tasks, built with ASP.NET Core and Entity Framework Core.

Users can register, log in, and manage their own tasks. Each task is private to the user who created it — no user can view or modify another user's data.

## Tech Stack

- **ASP.NET Core 8** — Web API framework
- **Entity Framework Core** — Database ORM
- **SQL Server** — Database
- **JWT Bearer Authentication** — Secure authentication
- **Swagger / OpenAPI** — API documentation and testing

## Features

- User registration and login with hashed passwords
- JWT token-based authentication
- Full CRUD operations for tasks (Create, Read, Update, Delete)
- User-scoped data — users only see their own tasks
- DTO pattern — database entities are never exposed directly
- Global error handling — all errors return clean, consistent responses
- Input validation on all request bodies

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or SQL Server LocalDB

### Setup

1. Clone the repository
```bash
   git clone https://github.com/AhmedIhab454/TaskManager.Api
```

2. Navigate to the project folder
```bash
   cd TaskManager.Api
```

3. Copy the example settings file and fill in your values
```bash
   cp appsettings.example.json appsettings.json
```

4. Update `appsettings.json` with your database connection string and a secure JWT secret key

5. Apply database migrations
```bash
   dotnet ef database update
```

6. Run the project
```bash
   dotnet run
```

7. Open Swagger UI at `https://localhost:{port}/swagger`

## API Endpoints

### Auth

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/register` | Register a new user | No |
| POST | `/api/auth/login` | Login and receive JWT token | No |

### Tasks

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/task` | Get all tasks for logged in user | Yes |
| GET | `/api/task/{id}` | Get a specific task by ID | Yes |
| POST | `/api/task` | Create a new task | Yes |
| PUT | `/api/task/{id}` | Update an existing task | Yes |
| DELETE | `/api/task/{id}` | Delete a task | Yes |

## Authentication

This API uses JWT Bearer authentication. To access protected endpoints:

1. Register a new account via `POST /api/auth/register`
2. Login via `POST /api/auth/login` and copy the token from the response
3. In Swagger, click the **Authorize** button and enter `Bearer your_token_here`
4. All protected endpoints are now accessible

## Security Notes

- Passwords are hashed using SHA256 before storage — plain text passwords are never saved
- JWT tokens expire after 60 minutes
- All task endpoints verify ownership — users can only access their own tasks
- Sensitive configuration values are stored outside of source control
```

