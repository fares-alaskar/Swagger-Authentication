# Swagger Authentication Middleware for ASP.NET Core

A lightweight middleware for **ASP.NET Core** to protect Swagger UI with **Basic Authentication** and **brute-force protection**.  
This allows you to restrict access to your Swagger documentation in development or production environments.

---

## Features

- Protects `/swagger` routes with **Basic Auth**.
- Tracks failed login attempts per IP to prevent **brute-force attacks**.
- Automatically blocks IPs after configurable failed attempts.
- Easy to integrate into any ASP.NET Core project.
- Thread-safe using `ConcurrentDictionary`.

---

## Installation

Clone the repository or add the source files to your project:

```bash
git clone https://github.com/github.com/fares-alaskar/Swagger-Authentication.git
```

---

## Usage

In your Program.cs (ASP.NET Core 6/7/8/9):

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Add Swagger first
app.UseSwagger();
app.UseSwaggerUI();

// Use Swagger Authentication middleware
app.UseSwaggerAuthentication();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

---

## Configuration

Default settings in the middleware:

Max failed attempts: 5
Lockout time: 5 minutes
Username: admin
Password: admin

You can modify these directly in SwaggerAuthentication.cs or extend the middleware to read from configuration.

---

## Contributing

Contributions are welcome! Feel free to submit pull requests or open issues for improvements or bug fixes.

---

## Example

When accessing /swagger in the browser:
- Browser prompts for username and password.
- Enter the credentials defined in the middleware.
- If correct, Swagger UI loads.
- If wrong credentials are entered multiple times, the IP is temporarily blocked.
  
---

# Notes
- Middleware only protects Swagger UI routes (/swagger and /swagger/v1/swagger.json).

---
