[![downloads][downloads-shield]][downloads-url] [![Contributors][contributors-shield]][contributors-url] [![Forks][forks-shield]][forks-url] [![Stargazers][stars-shield]][stars-url] [![Issues][issues-shield]][issues-url] [![License][license-shield]][license-url] [![LinkedIn][linkedin-shield]][linkedin-url]

# ![Logo][Logo] Portfolie
A high‑performance portfolio application built with C# and WebAssembly, running entirely in the browser.

## Table of Contents
- [Features](#features)
- [Getting Started](#getting-started)
  - [Clone the repository](#clone-the-repository-and-build)
  - [Run the Application](#run-the-application)
- [Configuration](#configuration)
- [Using the Portfolio](#using-the-portfolio)
- [Roadmap / Future Ideas](#roadmap--future-ideas)
- [Test & Code Coverage](#test--code-coverage)
- [API & Code Documentation](#-api--code-documentation)

The project is primarily aimed at software engineers who want to showcase their work to potential employers, but it is flexible enough to be adapted for other professions as well (designers, data specialists, etc.).
The application supports certificate-based login out of the box and is designed so that additional authentication methods can be added in the future if requested.

## ✨ Features

> See Milestones section for details what is planning.

## 🚀 Getting Started

Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- [Docker](https://www.docker.com/get-started) (optional, but recommended for easier setup)

### Clone the Repository and Build

1. Clone the repository:
   ```bash
   git clone https://github.com/TirsvadWeb/DotNet.Portfolio.git
   cd DotNet.Portfolio
   ```
  
2. Restore .NET workloads:
   ```bash
   dotnet workload restore
   ```

3. Build the application (optional if you plan to run with Docker):
   ```bash
   dotnet build
   ```

### Run the Application

1. Run with Docker (recommended)

   If the repository contains a `docker-compose.yml` at the repository root you can build and run all services with:
   ```bash
   docker compose up --build
   ```

   Notes:
   - Adjust ports and environment variables to match your configuration.
   - If you prefer to run locally without Docker, continue with the `dotnet run` instructions below.

### Alternatively, run locally without Docker

1. Apply any pending database migrations:
   ```bash
   dotnet ef database update --project src/Portfolio.Infrastructure --startup-project src/Portfolio/Portfolio --context ApplicationDbContext
   ``` 

2. Run the application (when not using Docker):
   ```bash
   dotnet run --project src/Portfolio/Portfolio
   ```

### Add Database Migrations
Create and apply separate migrations for Development and Production.
The migration files are added to the infrastructure project and the
startup project (which provides configuration/connection strings) is the
`src/Portfolio/Portfolio` project. The ASPNETCORE_ENVIRONMENT setting
controls which environment configuration is used when creating/applying migrations.

For Windows PowerShell:

```powershell
# Windows PowerShell
# Development migration
$Env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet ef migrations add InitialCreate.Development --project src/Portfolio.Infrastructure --startup-project src/Portfolio/Portfolio --context ApplicationDbContext
# dotnet ef database update --project src/Portfolio.Infrastructure --startup-project src/Portfolio/Portfolio --context ApplicationDbContext

# Production migration
$Env:ASPNETCORE_ENVIRONMENT = 'Production'
dotnet ef migrations add InitialCreate --project src/Portfolio.Infrastructure --startup-project src/Portfolio/Portfolio --context ApplicationDbContext
dotnet ef database update --project src/Portfolio.Infrastructure --startup-project src/Portfolio/Portfolio --context ApplicationDbContext
```

For Bash / macOS:

```bash
# Bash / macOS
# Development migration
ASPNETCORE_ENVIRONMENT=Development dotnet ef migrations add InitialCreate.Development --project src/Portfolio.Infrastructure --startup-project src/Portfolio/Portfolio --context ApplicationDbContext
ASPNETCORE_ENVIRONMENT=Development dotnet ef database update --project src/Portfolio.Infrastructure --startup-project src/Portfolio/Portfolio --context ApplicationDbContext

# Production migration
ASPNETCORE_ENVIRONMENT=Production dotnet ef migrations add InitialCreate.Production --project src/Portfolio.Infrastructure --startup-project src/Portfolio/Portfolio --context ApplicationDbContext
ASPNETCORE_ENVIRONMENT=Production dotnet ef database update --project src/Portfolio.Infrastructure --startup-project src/Portfolio/Portfolio --context ApplicationDbContext
```

<!-- Notes: keep secrets (connection strings, PFX passwords) out of source control; use user-secrets or environment variables -->

---

### Configuration

Configuration for the application is provided in the project appsettings files. See `src/Portfolio/Portfolio/appsettings.json` and `src/Portfolio/Portfolio/appsettings.Development.json` for the exact values used by the project.

Important keys to review or override:

- `ClientCertificateAuth`
  - `Enabled` (bool) — enable/disable automatic client-certificate support
  - `Namespace` (string) — certificate namespace used by the preloaded certificate lookup (e.g. `TirsvadWebCert`, `TirsvadWebCertDevelopment`)
- `DataProtection:KeyPath` — optional path where data-protection keys are persisted (useful for Docker volumes)
- `Kestrel:Certificates:Default` — file-based certificate configuration (Path / Password) when running Kestrel with a PFX

User secrets (secrets.json)

During local development we use the .NET user-secrets feature to keep sensitive values (connection strings, PFX passwords, API keys, etc.) out of source control. The secrets are stored in a `secrets.json` file managed by the secret manager. The on-disk location depends on the OS and the `UserSecretsId` in the project file:

- Windows: `%APPDATA%/Microsoft/UserSecrets/{userSecretsId}/secrets.json` => {source_path}
- Linux/macOS: `~/.microsoft/usersecrets/{userSecretsId}/secrets.json` => {source_path}

The application reads these values via the `UserSecrets` configuration provider when running in the `Development` environment.

Exposing secrets to Docker Compose

Containers do not automatically have access to your host's user-secrets store, so when running with Docker Compose you must provide the secret values to the container. 
There are three common approaches:

1) Mount the `secrets.json` file into the container

```yaml
services:
  portfolio:
    # ... other settings ...
    volumes:
      # Mount the secrets.json file into the container
      # Replace {Yours user} and {userSecretsId} accordingly
      - "{source_path}:/root/.microsoft/usersecrets/{userSecretsId}/secrets.json:ro"
```

Replace `UserSecretsId` with actual `UserSecretsId` (example `0cf4f171-3a18-4cbc-a691-09a51dbb2c5e`).
Adjust source path according to your OS. 
This makes the secrets available to the `UserSecrets` configuration provider inside the container.

Notes

- For local development you can continue to use the Secret Manager (`dotnet user-secrets`) and run the app without Docker; when you move to containers choose one of the approaches above so the container receives the same configuration values.

For the concrete configuration values used in this repository, open the two appsettings files referenced above.

---

### Database Configuration

You can configure the database connection for Release, Development, and Test environments using one of the following methods:

#### 1. appsettings.json / appsettings.{Environment}.json
Edit the appropriate file (e.g., `appsettings.Development.json`) and set the connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=YOUR_SERVER;Initial Catalog=YOUR_DB;User ID=YOUR_USER;Password=YOUR_PASSWORD;MultipleActiveResultSets=True;TrustServerCertificate=True;Encrypt=False"
  }
}
```
Available variables for the connection string:
- Data Source
- Initial Catalog
- User ID
- Password
- MultipleActiveResultSets
- TrustServerCertificate
- Encrypt

#### 2. secrets.json (User Secrets, recommended for local development)
Set the connection string for a specific environment using the .NET user-secrets tool:
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=YOUR_SERVER;Initial Catalog=YOUR_DB;User ID=YOUR_USER;Password=YOUR_PASSWORD;MultipleActiveResultSets=True;TrustServerCertificate=True;Encrypt=False"
```
This is stored in `secrets.json` and is not committed to source control.

#### 3. Environment Variables (Recommended for CI/CD and container setups)
You can override any part of the connection string using environment variables with the prefix `DOTNET_PORTFOLIO_` (all uppercase) and double underscores for nesting. Only set the variables you want to override; others will be read from config files or secrets.

For example, to override just the password and server for Development:
- `DOTNET_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION__PASSWORD`
- `DOTNET_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION__DATASOURCE`
- `DOTNET_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION__INITIALCATALOG`
- `DOTNET_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION__USERID`
- `DOTNET_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION__PASSWORD`
- `DOTNET_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION__MULTIPLEACTIVERESULTSETS`
- `DOTNET_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION__TRUSTSERVER`
- `DOTNET_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION__ENCRYPT`

Example (PowerShell):
```powershell
$Env:DOTNET_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION__PASSWORD = "NEW_PASSWORD"
$Env:DOTNET_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION__DATASOURCE = "NEW_SERVER"
dotnet run --project src/Portfolio/Portfolio
```

Example (Bash):
```bash
export DOTNET_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION__PASSWORD="NEW_PASSWORD"
export DOTNET_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION__DATASOURCE="NEW_SERVER"
dotnet run --project src/Portfolio/Portfolio
```

#### Using Environment Variables in Docker Compose

To override database connection settings in Docker Compose, add environment variables to the `environment:` section of your service. Use all uppercase names and double underscores for nesting. You only need to set the variables you want to override.

Example:
```yaml
services:
  portfolio:
    # ... other settings ...
    environment:
      - DOTNET_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION__DATASOURCE=NEW_SERVER
      - DOTNET_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION__USERID=NEW_USER
      - DOTNET_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION__PASSWORD=NEW_PASSWORD
      # Add other overrides as needed
```

This will override the specified parts of the connection string for the container. Other values will be read from config files or secrets.

**Precedence:**
1. Environment variables (with prefix, per part; only those set are overridden)
2. User secrets (in Development)
3. appsettings files

> **Note:** Never commit sensitive connection strings or secrets to source control. Use user-secrets or environment variables for local and CI/CD setups.

## 🧑‍💼 Using the Portfolio
Once running, you can:
- Log in with your certificate
- Customize your profile: name, title, summary
- Add projects: title, description, tech stack, links, screenshots
- Optionally upload or link your CV (PDF or other supported format)
- Share the URL with potential employers as your personal portfolio
- On Windows, use `certmgr.msc` or the MMC Certificates snap-in to copy the certificate to `Trusted Root Certification Authorities` (only for local dev).
- On macOS, add the certificate to Keychain and mark it as trusted.
- `mkcert` automates trust for local development on macOS and Windows.

---

## 🗺️ Roadmap / Future Ideas
- [ ] v0.1 Basic profile with certificate login
  - [ ] User profile
  - [ ] Profile details (name, title, summary)
  - [ ] Certificate login
- [ ] v0.2 Portfolio basics
  - [ ] Portfolio project management (CRUD operations)
  - [ ] Tags / tech stack
  - [ ] Project details (description, links, screenshots)
- [ ] v0.3 Localization
  - [ ] Localization / multi-language support
  - [ ] English
  - [ ] Danish
- [ ] v0.4 Blog and articles
  - [ ] Blog post management (CRUD operations)
  - [ ] Blog post details (title, content, author, date)
  - [ ] Blog post comments
- [ ] v0.5 Add multiple login options and Role-based access control
  - [ ] Add support for OAuth2 / OpenID Connect providers
  - [ ] Add roles (Admin, Editor and Guest)
- [ ] v0.6 Add export and import functionality
  - [ ] Export portfolio data as JSON
  - [ ] Import portfolio data from JSON
- [ ] v1.0 Stable release
  - [ ] Polish UI/UX
  - [ ] Comprehensive testing (unit, integration, e2e)
  - [ ] Documentation and user guides
  - [ ] Custom themes (light/dark)

---

## 🧪 Test & Code Coverage

Automated tests are included. Code coverage is measured as part of the CI/CD pipeline.

- View the latest code coverage report: [Code covergae][CodeCoverageResults-url]

Coverage reports are generated in HTML format and published to the link above.

---

## 📚 API & Code Documentation

Doxygen is used to generate API and code documentation for this project.

### Generate Doxygen Documentation

1. Install [Doxygen](https://www.doxygen.nl/download.html) if you don't have it.
2. From the repository root, run:
   ```bash
   doxygen
   ```
   This will generate documentation in `docs/doxygen/html`.
3. Open `docs/doxygen/html/index.html` in your browser to view the documentation.

- View the latest online Doxygen documentation: [Doxygen Documentation][doxygen-url]

---

<!-- LINK REFERENCES -->
[contributors-shield]: https://img.shields.io/github/contributors/TirsvadWeb/DotNet.Portfolio?style=for-the-badge
[contributors-url]: https://github.com/TirsvadWeb/DotNet.Portfolio/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/TirsvadWeb/DotNet.Portfolio?style=for-the-badge
[forks-url]: https://github.com/TirsvadWeb/DotNet.Portfolio/network/members
[stars-shield]: https://img.shields.io/github/stars/TirsvadWeb/DotNet.Portfolio?style=for-the-badge
[stars-url]: https://github.com/TirsvadWeb/DotNet.Portfolio/stargazers
[issues-shield]: https://img.shields.io/github/issues/TirsvadWeb/DotNet.Portfolio?style=for-the-badge
[issues-url]: https://github.com/TirsvadWeb/DotNet.Portfolio/issues
[license-shield]: https://img.shields.io/github/license/TirsvadWeb/DotNet.Portfolio?style=for-the-badge
[license-url]: https://github.com/TirsvadWeb/DotNet.Portfolio/blob/master/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://www.linkedin.com/in/jens-tirsvad-nielsen-13b795b9/
[logo]: https://raw.githubusercontent.com/TirsvadCLI/Logo/main/images/logo/32x32/logo.png "Logo"

[downloads-shield]: https://img.shields.io/github/downloads/TirsvadWeb/DotNet.Portfolio/total?style=for-the-badge
[downloads-url]: https://github.com/TirsvadWeb/DotNet.Portfolio/releases

<!-- Github Links -->
[githubIssue-url]: https://github.com/TirsvadWeb/DotNet.Portfolio/issues/
[githubProjectTasks-url]: https://github.com/orgs/TirsvadWeb/projects/7

<!-- Project Links -->
[CodeCoverageResults-url]: https://dev.tirsvad.dk/projects/TirsvadWeb/DotNet.Portfolio/codecoverage/ "Code Coverage Results"
[doxygen-url]: https://dev.tirsvad.dk/projects/TirsvadWeb/DotNet.Portfolio/doxygen/ "doxygen"