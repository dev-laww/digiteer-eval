# Task Manager API (Backend)

Tech stack:
- .NET 9 Web API
- PostgreSQL
- Entity Framework Core (EF Core)
- JWT Bearer Authentication
- Swagger (OpenAPI)

## Getting started

1) Install prerequisites
- .NET 9 SDK
- PostgreSQL 14+

2) Configure settings
- Update `appsettings.json` connection string `DefaultConnection` for PostgreSQL.
- Set JWT values in `appsettings.json` under `Jwt` section: `Issuer`, `Audience`, `Secret`, `ExpiresMinutes`.

3) Database
```bash
dotnet ef database update
```

4) Run the API
```bash
dotnet run
```
- Swagger UI: `https://localhost:{port}/swagger`

## Authentication

- Register: `POST /auth/register`
  - Body: `{ "email": "user@example.com", "password": "Passw0rd!" }`
  - Response: `{ success, code, message, data: { token } }`

- Login: `POST /auth/login`
  - Body: `{ "email": "user@example.com", "password": "Passw0rd!" }`
  - Response: `{ success, code, message, data: { token } }`

Use the returned JWT in the `Authorization` header:
```
Authorization: Bearer <token>
```

## Tasks API

All tasks endpoints require a valid JWT. Responses never expose `userId` or `user`.

- List tasks: `GET /tasks`
  - 200: `{ success, code, message, data: [ { id, title, isDone } ] }`

- Create task: `POST /tasks`
  - Body:
    ```json
    { "title": "My task" }
    ```
  - 201: `{ success, code, message: "Created", data: { id, title, isDone } }`

- Update task: `PUT /tasks/{id}`
  - Body (partial allowed):
    ```json
    { "title": "New title", "isDone": true }
    ```
  - 200: `{ success, code, message: "Updated", data: { id, title, isDone } }`

- Delete task: `DELETE /tasks/{id}`
  - 204 No Content

### Request/Response shapes

- Create body
```json
{ "title": "Task title" }
```

- Update body (all fields optional)
```json
{ "title": "Task title", "isDone": true }
```

- Task DTO returned by the API
```json
{ "id": 1, "title": "Task title", "isDone": false }
```

### Standard response envelope

All endpoints return this envelope (except 204):
```json
{
  "success": true,
  "code": 200,
  "message": "Success",
  "data": {}
}
```
On error:
```json
{
  "success": false,
  "code": 404,
  "message": "Task not found",
  "error": {
    "type": "",
    "message": "",
    "stackTrace": null,
    "validationErrors": null
  }
}
```

## Notes

- `TasksController` uses DTOs to avoid leaking `userId` or `user` in responses.
- `CreateTaskRequest` requires only `title`; `isDone` defaults to `false`.
- `UpdateTaskRequest` supports partial updates: `title?`, `isDone?`.
- 204 responses intentionally have no response body.