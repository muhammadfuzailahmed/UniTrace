# UniTrace

**A shared place to report found belongings and help people find what they have lost.**

UniTrace is a university-focused lost-and-found web application built with ASP.NET Core MVC and SQL Server. Someone who finds an item can publish its details, while someone looking for a missing belonging can browse reports, search by title and submit a claim.

> **Project status:** Early prototype. Core screens and database workflows are implemented, with known issues and improvements still needed before deployment.

## Features

- **Accounts:** Registration, email/password login and logout, with session-based user tracking.
- **Found-item reports:** Capture a title, category, date found, location and description.
- **Shared feed:** Browse unresolved items and identify your own posts on the dashboard.
- **Title search:** Find unresolved reports using a partial item title.
- **Claim submission:** Submit identifying details and the location where an item was lost.
- **Claim review:** View incoming requests for your posted items, with approval and rejection controls. Approval marks an item resolved and removes it from the active feed.

The **I Lost Something** button opens the search page. A separate lost-item posting workflow is not currently implemented.

## Technology stack

| Layer | Technology |
| --- | --- |
| Application | C#, ASP.NET Core MVC, .NET 10 |
| Views | Razor (`.cshtml`), HTML, CSS, JavaScript |
| UI libraries | Bootstrap and jQuery |
| Database | Microsoft SQL Server |
| Data access | ADO.NET through `Microsoft.Data.SqlClient` 7.0.1 |
| User state | ASP.NET Core sessions |

## Architecture

The application follows the Model-View-Controller structure. Controllers handle requests and execute SQL directly; models represent item and claim data; Razor views render the interface. There is currently no separate service or repository layer and no Entity Framework dependency.

```text
UniTrace/
├── Controllers/
│   ├── AuthController.cs         # Registration, login and logout
│   ├── DashboardController.cs    # Item feed and incoming claim requests
│   ├── ItemController.cs         # Found-item reporting and title search
│   └── ClaimController.cs        # Claim submission and review actions
├── Models/                      # Item, claim and error view models
├── Views/                       # Razor pages and shared layout
├── wwwroot/                     # Styles, scripts and bundled UI libraries
├── Properties/launchSettings.json
├── Program.cs                   # Services, middleware and MVC routing
├── Lab_project.sql              # Database schema and development queries
└── Database_lab_project.csproj   # Application project
```

The database contains three main tables:

- `users`: Account information and a reference to another user involved in a claim.
- `items`: Report details, the posting user, resolution status and the current requesting user.
- `claimItem`: Claim submissions, identifying details and lost-location information.

Internally, `is_found = 'not found'` represents an unresolved report, and `is_found = 'found'` represents an approved claim. The `lost_date` column is used by the found-item form to store the date found.

## Local setup

### Prerequisites

- .NET 10 SDK
- SQL Server or SQL Server Express
- A SQL client such as SQL Server Management Studio
- Git

### 1. Clone the repository

```sh
git clone https://github.com/muhammadfuzailahmed/UniTrace.git
cd UniTrace
```

### 2. Create the database

Open `Lab_project.sql` in your SQL client. Execute `CREATE DATABASE dbms_lab_project` as its own batch, then switch to that database and execute the `CREATE TABLE` statements in this order:

1. `users`
2. `items`
3. `claimItem`

The file also includes sample accounts, inspection queries and an `UPDATE` statement. Review these separately; they are not required to create the schema. Use a fresh local development database.

### 3. Configure the connection

Connection strings are currently hardcoded in all four controllers. Replace the server name in each connection string with your local SQL Server instance, keeping the database name consistent with the database you created.

For example, a local SQL Server Express connection in C# can look like:

```csharp
string connectionString = @"Server=.\SQLEXPRESS;Database=dbms_lab_project;Trusted_Connection=True;TrustServerCertificate=True;";
```

This example uses Windows authentication. Ensure your Windows account has access to the database. Changing `appsettings.json` alone will not change the connections used by the controllers.

### 4. Check session configuration

`Program.cs` registers sessions with `AddSession()`. If startup reports a missing `IDistributedCache` dependency, register an in-memory cache before that call:

```csharp
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
```

### 5. Restore and run

```sh
dotnet restore
dotnet run --launch-profile https
```

The checked-in HTTPS launch profile uses `https://localhost:7291` and `http://localhost:5115`. Open the HTTPS address to reach the login page. If your local development certificate is not trusted, run:

```sh
dotnet dev-certs https --trust
```

Local setup depends on your SQL Server instance and permissions. These instructions describe the source configuration; they do not imply that every workflow has passed end-to-end testing.

## Typical workflow

1. Sign in and choose **Report Found** to publish an item.
2. Browse the feed or choose **I Lost Something** to search reports by title.
3. Submit a claim for a matching item with identifying details.
4. The posting user opens **Claim Requests** to review incoming requests.
5. Approving a request removes that item from the unresolved feed.

## Current limitations

- Passwords are currently stored and compared in plain text. Password hashing and stronger authentication are needed before real use.
- Session checks, server-side ownership authorization, anti-forgery protection and input validation need strengthening.
- Registration leaves its email lookup reader open before attempting an insert, which can fail with the default SQL connection settings.
- The rejection action reuses an update query where a user lookup was intended, so its cleanup logic needs correction.
- Claim details are stored but are not displayed in the claim review screen. The item stores only one current requesting user, so a later claim can replace the earlier requester shown there.
- Connection settings are duplicated in controllers. Centralized configuration and clearer error handling remain improvements.
- Image uploads, automated ownership verification, messaging and notifications are not implemented.

## Planned MERN version

The next major version of UniTrace is planned as a rebuild using **MongoDB, Express.js, React and Node.js (MERN)**. The goal is to expand this prototype into a more complete campus lost-and-found experience.

Planned features and improvements include:

- **Item photos:** Upload pictures with found-item reports to make belongings easier to identify.
- **New-item email alerts:** Notify users when new found-item reports are posted.
- **Claim submission emails:** Notify the person who posted an item when someone submits a claim.
- **Claim decision emails:** Notify the claimant when their request is approved or rejected.
- **Improved claim review:** Present identifying details clearly and support multiple pending claims per item.
- **Stronger validation and access controls:** Improve account security, input validation and authorization throughout the application.

These are future plans and are **not implemented in the current ASP.NET Core version**. The technology stack and setup instructions above describe the current repository. No release date has been set for the MERN version.

## Current version improvements

- Fix registration and claim rejection behavior.
- Improve account security and enforce authorization on every protected action.
- Show identifying details during claim review and support multiple pending claims per item.
- Move database access into a dedicated layer and read connection settings from configuration.
- Add meaningful workflow tests and improve setup documentation as the application evolves.

## Author

[Muhammad Fuzail Ahmed](https://github.com/muhammadfuzailahmed)
