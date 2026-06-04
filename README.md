# AuctionHouse — Fullstack Auction Platform

A fullstack web application built with **React** (Vite + TypeScript) and **ASP.NET Web API** (.NET 10).

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Frontend | React 19, TypeScript, Vite, React Router, Axios, Context API |
| Backend | ASP.NET Web API, .NET 10, Entity Framework Core |
| Database | SQL Server (LocalDB), Code First |
| Auth | JWT (JSON Web Tokens), BCrypt hashing |
| Architecture | Clean Architecture (4-layer), Service Layer, Repository Pattern, Dependency Injection |
| API Docs | Swagger / OpenAPI |

## Features

### G-krav (Pass)
- Register and login with JWT authentication
- Create, search, and browse open auctions
- Auction detail view with full bid history
- Place bids with validation (higher than current, cannot bid on own auction)
- Auction card grid with image, countdown, and seller info

### VG-krav (Distinction)
- Read-only access for guests (bid form hidden, ProtectedRoute guards)
- Search closed auctions with toggle filter
- Closed auction view — shows only winner and winning bid (no bid history)
- Edit auction (price locked if bids already exist)
- Undo latest bid (must be your own, auction must be open)
- Change password from authenticated user
- Admin panel — deactivate/reactivate users, hide/show auctions
- Responsive design with hamburger menu (mobile-first)
- Clean architecture with 4 project layers and service layer
- Dependency injection throughout (Controller → IService → Service → IRepository → Repository)

## Project Statistics

| Metric | Value |
|--------|-------|
| Total commits | 27 |
| Development period | May 4 — June 4, 2026 (31 days) |
| Frontend files | 31 |
| Backend files | 31 (across 4 projects) |
| Total files | 62 |
| Frontend lines | 1,721 |
| Backend lines | 1,047 |
| Combined codebase | **2,768 lines of code** |

## Dependency Injection Chain

```
Program.cs (DI registration)
    │
    ▼
Controller          injects IService
    │
    ▼
Service             injects IRepository (business logic lives here)
    │
    ▼
Repository          injects DbContext (data access only)
    │
    ▼
EF Core / SQL Server
```

Every layer depends only on the layer directly below it. Interfaces (`IService`, `IRepository`) enable unit testing and loose coupling.

## Service Layer

All business logic moved from controllers into four service classes:

| Service | Responsibility |
|---------|---------------|
| `AuthService` | Register validation, login, BCrypt password verification, password change |
| `AuctionService` | Create/update validation, search, price-lock rule, entity → DTO mapping |
| `BidService` | Bid amount validation, owner check, undo bid, highest-bid recalculation |
| `AdminService` | User listing, auction listing, deactivate/reactivate toggles |

## Getting Started

### Prerequisites

- .NET 10 SDK
- Node.js 24+
- SQL Server LocalDB

### Backend

```bash
dotnet run --project AuctionApi --launch-profile https
```

- API runs on `https://localhost:7066`
- Swagger UI at `https://localhost:7066/swagger`

### Frontend

```bash
cd auction-client
npm install
npm run dev
```

- App runs on `http://localhost:5173`

### Test Accounts

> **Note:** The seeded test accounts and passwords below exist only for demonstration and grading purposes. In a production environment, seed data should never be committed to source control, and credentials should never be stored in plaintext in code.

| Username | Password | Role |
|----------|----------|------|
| `admin` | `admin123` | Admin |
| `erik` | `erik123` | User |
| `lisa` | `lisa123` | User |

## Project Structure

```
├── Auction.Domain/              # Entities (User, Auction, Bid)
│   └── Entities/
├── Auction.Data/                # Data access layer
│   ├── Interfaces/              # Repository interfaces
│   ├── Repositories/            # EF Core implementations
│   └── DbSeeder.cs              # Test data seeder
├── Auction.Core/                # Business logic layer
│   ├── Services/                # Auth, Auction, Bid, Admin services + JWT
│   ├── Interfaces/              # Service interfaces
│   ├── DTOs/                    # Request/Response transfer objects
│   └── Utils/                   # UTC DateTime converter
├── Auction.Api/                 # Web API host
│   ├── Controllers/             # Thin controllers (Auth, Auctions, Bids, Admin)
│   └── Program.cs               # App config, DI registrations, middleware
├── auction-client/              # React Frontend
│   └── src/
│       ├── components/          # Reusable UI (Navbar, AuctionCard, ProtectedRoute, AdminRoute)
│       ├── contexts/            # AuthContext — login/register/logout state
│       ├── pages/               # Routes (Home, Login, AuctionDetail, CreateAuction, Admin)
│       ├── services/            # Axios API client with JWT interceptor
│       └── types/               # TypeScript interfaces
```

## Architecture

```
React (Vite)  ──HTTP/JSON──►  ASP.NET Web API  ──Service──►  Repository  ──EF Core──►  SQL Server
  │  React Router                 │  JWT Middleware    (business)     (data access)
  │  Context API                  │  Thin Controllers
  └── Axios (JWT header)          └── DI Chain: IService → Service → IRepository → Repository
```

## Git Commit History

27 commits over 31 days — incremental delivery.

```
May 4   14:12  Initial commit
May 4   14:15  Project plan
May 25  11:42  Phase 1: Backend (EF Core, JWT, DTOs)              ← DB locked
May 26  13:08  Scaffold React + Vite                              ← G start
May 26  14:08  TypeScript types
May 26  14:26  Axios API + JWT interceptor
May 26  16:18  AuthContext
May 26  22:19  Navbar component
May 26  23:05  Home page
May 27  00:30  AuctionCard component
May 27  13:10  Login + Register pages
May 27  14:33  AuctionDetail (bid form, history)
May 27  15:27  CreateAuction + ProtectedRoute
May 27  15:43  Wire up App routing                               ← G done
May 28  00:22  Phase 3a: Closed auctions, winner, password       ← VG 1
May 31  19:32  Phase 3b: Update auction, undo bid                ← VG 2
May 31  22:37  Phase 3c: Admin panel                             ← VG done
Jun  2  12:13  Repository Pattern refactor
Jun  2  12:21  UTC timezone fix
Jun  2  12:51  Admin panel IsActive fix
Jun  2  12:59  Navbar logo + avatar
Jun  2  13:07  Double image sizes, adjust navbar height
Jun  2  13:14  Rename Sell to Create Auction
Jun  2  13:17  DateTime-local timezone fix
Jun  3  11:20  Project README
Jun  3  11:52  Clean up conflicting packages
Jun  4  23:32  Clean architecture: 4 projects with service layer and DI  ← final
```
