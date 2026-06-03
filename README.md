# AuctionHouse — Fullstack Auction Platform

A fullstack web application built with **React** (Vite + TypeScript) and **ASP.NET Web API** (.NET 10).

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Frontend | React 19, TypeScript, Vite, React Router, Axios, Context API |
| Backend | ASP.NET Web API, .NET 10, Entity Framework Core |
| Database | SQL Server (LocalDB), Code First |
| Auth | JWT (JSON Web Tokens), BCrypt hashing |
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
- Repository pattern — controller → interface → implementation

## Project Statistics

| Metric | Value |
|--------|-------|
| Total commits | 24 |
| Development period | May 4 — June 2, 2026 (28 days) |
| Frontend files | 25 |
| Backend files | 25 |
| Total files | 50 |
| Frontend lines | 1,721 |
| Backend lines | 1,031 |
| Combined codebase | **2,752 lines of code** |

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
├── AuctionApi/                  # ASP.NET Backend
│   ├── Controllers/             # API endpoints (Auth, Auctions, Bids, Admin)
│   ├── DTOs/                    # Request/Response data transfer objects
│   ├── Models/                  # Database entities (User, Auction, Bid)
│   ├── Repositories/            # Data access layer (Repository Pattern)
│   ├── Services/                # JWT token generation
│   ├── Utils/                   # JSON converters (UTC DateTime)
│   ├── Data/                    # DbContext and database seeder
│   └── Program.cs               # App configuration, DI, middleware
├── auction-client/              # React Frontend
│   └── src/
│       ├── components/          # Reusable UI (Navbar, AuctionCard, ProtectedRoute)
│       ├── contexts/            # AuthContext — global auth state
│       ├── pages/               # Route pages (Home, Login, AuctionDetail, Admin)
│       ├── services/            # Axios API client with JWT interceptor
│       └── types/               # TypeScript interfaces
```

## Architecture

```
React (Vite)  ──HTTP/JSON──►  ASP.NET Web API  ──EF Core──►  SQL Server
  │  React Router                 │  JWT Middleware             Users
  │  Context API                  │  Controllers                Auctions
  └── Axios (JWT header)          └── Repository Pattern        Bids
```

## Git Commit History

Incremental delivery: 1 backend commit → 11 frontend commits (G-krav) → 3 VG phases → refactoring + fixes.

```
May 4   14:12  Initial commit
May 4   14:15  Projectplan in place
May 25  11:42  Phase 1: Backend (EF Core, JWT, DTOs)         ← DB locked
May 26  13:08  Scaffold React + Vite                          ← G start
May 26  14:08  TypeScript types
May 26  14:26  Axios API + JWT interceptor
May 26  16:18  AuthContext
May 26  22:19  Navbar component
May 26  23:05  Home page
May 27  00:30  AuctionCard component
May 27  13:10  Login + Register pages
May 27  14:33  AuctionDetail (bid form, history)
May 27  15:27  CreateAuction + ProtectedRoute
May 27  15:43  Wire up App routing                           ← G done
May 28  00:22  Phase 3a: Closed auctions, winner, password   ← VG 1
May 31  19:32  Phase 3b: Update auction, undo bid            ← VG 2
May 31  22:37  Phase 3c: Admin panel                         ← VG done
Jun  2  12:18  Repository Pattern refactor
Jun  2  12:39  UTC timezone fix
Jun  2  12:58  Admin panel IsActive fix
Jun  2  13:05  Navbar logo + avatar
Jun  2  13:15  CreateAuction button rename
Jun  2  13:17  DateTime-local timezone fix
```
