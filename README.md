# 🏋️ Body Factory — Gym Management System

An ASP.NET Core MVC web application for managing a gym: members, trainers, class sessions, subscription plans, memberships, and session bookings — built on a clean 3-tier architecture (PL → BLL → DAL) with ASP.NET Core Identity, Entity Framework Core, and AutoMapper.

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)
![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core-MVC-5C2D91?logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/EF%20Core-9.0-512BD4)
![SQL Server](https://img.shields.io/badge/SQL%20Server-Express-CC2927?logo=microsoftsqlserver&logoColor=white)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5-7952B3?logo=bootstrap&logoColor=white)

---

## 📑 Table of Contents

- [Features](#-features)
- [Architecture](#-architecture)
- [Tech Stack](#-tech-stack)
- [Project Structure](#-project-structure)
- [Getting Started](#-getting-started)
- [Default Seeded Accounts](#-default-seeded-accounts)
- [Database & Migrations](#-database--migrations)
- [Data Seeding](#-data-seeding)
- [Screens](#-screens)
- [Roadmap](#-roadmap)
- [Author](#-author)

---

## ✨ Features

### 👥 Members
- Full CRUD for gym members with server-side and client-side validation.
- Profile photo upload handled by a dedicated `AttachmentService` (validates size/extension, stores under `wwwroot/images/MembersPictures`).
- Per-member **health record** (height, weight, blood type, medical notes) as a one-to-one related entity.
- Restricted to the `SuperAdmin` role.

### 🏋️ Trainers
- CRUD for trainers including professional details (specialty, certification, years of experience) and address fields.
- Trainers are linked to the sessions they run.

### 📅 Sessions & Categories
- Create, edit, and delete class sessions with a category, an assigned trainer, capacity, schedule, and price.
- Session listing shows remaining capacity computed from active bookings.

### 🎟️ Session Scheduling (Bookings)
- Book a member into a session and cancel bookings.
- Capacity and duplicate-booking rules enforced in the service layer before any write reaches the database.

### 💳 Plans & Memberships
- Subscription plans seeded from a JSON file (`wwwroot/Files/plans.json`) with edit support.
- Memberships link a member to a plan with start/end dates and status, so active vs. expired subscriptions are visible at a glance.

### 📊 Dashboard
- `AnalyticsService` powers a home dashboard with live counts of members, trainers, sessions, and active memberships.

### 🔐 Authentication & Authorization
- ASP.NET Core Identity with a custom `ApplicationUser` (first name, last name, phone).
- Two roles: **SuperAdmin** and **Admin**, seeded on first run.
- Password policy (digit, upper, lower, non-alphanumeric, min length 6) and lockout after 5 failed attempts for 2 minutes.
- Custom `Login` and `AccessDenied` pages; the app's default route is the login screen.

### 🧰 Cross-Cutting
- **Unit of Work + Generic Repository** over EF Core, with specialized repositories for sessions, memberships, and bookings.
- A `Result` type in the BLL carries success/failure and messages up to the controllers, so views can render friendly alerts instead of exceptions.
- **AutoMapper** profiles map entities ↔ view models in one place.
- Automatic migration + seeding on startup via `MigrateAndSeedDataAsync()`.

---

## 🏗 Architecture

A classic 3-tier separation — the presentation layer never touches `DbContext` directly.

```
┌──────────────────────────────────────────────┐
│  GymMangmentSystem.PL   (Presentation)       │
│  Controllers · Razor Views · wwwroot         │
└──────────────────┬───────────────────────────┘
                   │ interfaces + view models
┌──────────────────▼───────────────────────────┐
│  GymMangmentSystem.BLL  (Business Logic)     │
│  Services · ViewModels · AutoMapper · Result │
└──────────────────┬───────────────────────────┘
                   │ IUnitOfWork / repositories
┌──────────────────▼───────────────────────────┐
│  GymMangmentSystem.DAL  (Data Access)        │
│  EF Core DbContext · Models · Configurations │
│  Repositories · Migrations · Seeding         │
└──────────────────────────────────────────────┘
```

**Request flow:** `Controller → IService → IUnitOfWork → IRepository<T> → GymDbContext → SQL Server`

---

## 🧪 Tech Stack

| Area | Technology |
|---|---|
| Framework | .NET 9 / ASP.NET Core MVC |
| ORM | Entity Framework Core 9 (Code First) |
| Database | SQL Server Express |
| Identity | ASP.NET Core Identity + EF Core stores |
| Mapping | AutoMapper 16 |
| Frontend | Razor Views, Bootstrap 5, custom CSS |
| Patterns | 3-Tier, Repository, Unit of Work, DI, Result pattern |

---

## 📂 Project Structure

```
GymMangmentSystem/
├── GymMangmentSystem.slnx
├── GymMangmentSystem.DAL/
│   ├── Data/
│   │   ├── DbContexts/GymDbContext.cs
│   │   ├── Models/              # Member, Trainer, Session, Booking, Plan,
│   │   │                        # Membership, Category, HealthRecord,
│   │   │                        # ApplicationUser, Enums
│   │   ├── Configurations/      # Fluent API entity configurations
│   │   └── DataSeeding/         # GymDataSeeding · IdentityDataSeeding
│   ├── Repositories/            # Generic + specialized repos, UnitOfWork
│   └── Migrations/
├── GymMangmentSystem.BLL/
│   ├── Services/                # Member, Trainer, Session, Plan,
│   │                            # Membership, Booking, Analytics, Attachment
│   ├── ViewModels/              # Grouped per feature
│   ├── MappingProfiles.cs
│   └── Common/Result.cs
└── GymMangmentSystem.PL/
    ├── Controllers/             # Account, Home, Members, Trainer, Session,
    │                            # SessionSchedule, Membership, Plan
    ├── Views/
    ├── wwwroot/                 # css, js, images, Files/plans.json
    ├── Program.cs
    └── ProgramExtensions.cs
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server Express (or any SQL Server instance)
- Visual Studio 2022 / VS Code / Rider

### 1. Clone

```bash
git clone https://github.com/omar0930/Gym-Mangment-System-MvcProject.git
cd Gym-Mangment-System-MvcProject
```

### 2. Configure the connection string

Edit `GymMangmentSystem.PL/appsettings.Development.json` and point it at your SQL Server instance:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=GymManagementSystem;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

### 3. Restore & run

```bash
dotnet restore
dotnet run --project GymMangmentSystem.PL
```

On first launch the app **applies any pending migrations and seeds the database automatically** — no manual `dotnet ef database update` needed.

Then open the URL printed in the console (typically `https://localhost:7xxx`) and you'll land on the login page.

---

## 🔑 Default Seeded Accounts

Created on first run by `IdentityDataSeeding`:

| Role | Username | Email | Password |
|---|---|---|---|
| SuperAdmin | `Omar25` | superadmin@example.com | `P@ssw0rd!` |
| Admin | `Admin25` | admin@example.com | `P@ssw0rd!` |

> ⚠️ These are development-only defaults. Change or remove them before deploying anywhere real.

---

## 🗄 Database & Migrations

Migrations live in `GymMangmentSystem.DAL/Migrations`. To manage them manually:

```bash
# Add a migration
dotnet ef migrations add <Name> --project GymMangmentSystem.DAL --startup-project GymMangmentSystem.PL

# Apply migrations
dotnet ef database update --project GymMangmentSystem.DAL --startup-project GymMangmentSystem.PL
```

Or from the Visual Studio Package Manager Console (default project: `GymMangmentSystem.DAL`):

```powershell
Add-Migration <Name>
Update-Database
```

**Current migrations**

| Migration | Purpose |
|---|---|
| `InitialCreate` | Core schema — members, trainers, sessions, bookings, plans, memberships, categories, health records |
| `AddTrainerProfessionalAndAddressFields` | Trainer specialty, certification, experience, address |
| `AddApplicationUser` | ASP.NET Core Identity tables + custom user fields |

---

## 🌱 Data Seeding

- **`GymDataSeeding`** — loads subscription plans from `GymMangmentSystem.PL/wwwroot/Files/plans.json` so the catalog is editable without a code change.
- **`IdentityDataSeeding`** — creates the `SuperAdmin` and `Admin` roles and their default users.

Both are idempotent: they no-op when data already exists, so restarts won't duplicate rows.

---

## 🖥 Screens

| Screen | Description |
|---|---|
| **Login / Access Denied** | Branded auth pages, the app's entry point |
| **Dashboard** | Live counts of members, trainers, sessions, and active memberships |
| **Members** | List, create (with photo upload), details, edit, delete, health record |
| **Trainers** | List, create, details, edit, delete |
| **Sessions** | List, create, details, edit, delete with capacity tracking |
| **Session Schedule** | Book a member into a session, view details, cancel a booking |
| **Memberships** | List, create, details, edit, delete with plan + date range |
| **Plans** | Browse and edit subscription plans |

---

## 🗺 Roadmap

- [ ] Member self-service portal (members log in and book their own sessions)
- [ ] Payment / invoicing for memberships
- [ ] Attendance check-in tracking
- [ ] Reporting and export (revenue, attendance, retention)
- [ ] Unit and integration test coverage

---

## 👤 Author

**Omar Mohamed**
Route Academy — Back-End Track (C44), MVC Session Project

- GitHub: [@omar0930](https://github.com/omar0930)

---

⭐ If this project helped you, consider starring the repo.
