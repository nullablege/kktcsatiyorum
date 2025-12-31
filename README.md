# KKTCSatiyorum

**KKTCSatiyorum** is a modern, responsive classified ads platform tailored for the TRNC (KKTC) market. It supports Real Estate, Vehicles, and Electronics categories with a focus on user experience and robust architecture.

## Architecture

The project follows a **Layered Architecture** (Onion/Clean Architecture principles) to ensure separation of concerns and maintainability.

```mermaid
graph TD
    UI[Presentation Layer (MVC)] --> BL[Business Layer]
    BL --> DAL[Data Access Layer]
    BL --> EL[Entity Layer]
    DAL --> EL
    DAL --> DB[(SQL Server)]
```

- **Entity Layer**: Defines POCO entities and Enums (e.g., `Ilan`, `Kategori`, `UygulamaKullanicisi`).
- **Data Access Layer (DAL)**: Handles database interactions using **EntityFramework Core** with Repository Pattern and Unit of Work.
- **Business Layer (BL)**: Contains business logic, validation (FluentValidation), and DTO mappings (AutoMapper).
- **Presentation Layer**: ASP.NET Core MVC application with **SignalR** for real-time notifications.

## Tech Stack

- **Framework**: .NET 8.0 (ASP.NET Core MVC)
- **Database**: Entity Framework Core (Code First) / SQL Server
- **Identity**: ASP.NET Core Identity (Authentication/Authorization)
- **Real-time**: SignalR (Notifications)
- **Validation**: FluentValidation
- **Mapping**: AutoMapper
- **Frontend**: Bootstrap 5, jQuery, Custom CSS

## Setup & installation

1.  **Clone the Repository**
    ```bash
    git clone https://github.com/yourusername/KKTCSatiyorum.git
    cd KKTCSatiyorum
    ```

2.  **Configuration**
    Update the connection string in `appsettings.json` if necessary (defaults to LocalDB or SQL Express).
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=...;Database=KKTCSatiyorumDb;..."
    }
    ```

3.  **Database Migration & Seeding**
    The application automatically applies migrations and seeds demo data on the first run.
    ```bash
    dotnet build
    dotnet run --project KKTCSatiyorum
    ```

## Demo Flow

To verify the robust features of the application, follow this scenario:

1.  **Login as Admin**
    - **Email**: `admin@kktcsatiyorum.com`
    - **Password**: `Admin123!`
    - **Action**: Go to Admin Panel -> Listings. View "Onay Bekliyor" (Pending) listings.

2.  **User Interaction (Open Incognito Window)**
    - **Login**: `user@kktcsatiyorum.com` / `User123!`
    - **Action**: `İlan Ver` (Post Ad) -> Create a new listing (e.g., "Sony PlayStation 5").

3.  **Real-Time Approval**
    - Switch back to **Admin**.
    - Approve the new listing.
    - Switch to **User**. You should receive a **SignalR Notification** instantly: "İlanınız onaylandı!"

4.  **Favorites**
    - As User, browse the homepage.
    - Click the Heart icon on any listing to add to Favorites.

## Key Features

- **Category Attributes (EAV)**: Dynamic attributes for categories (e.g., Square Meters for Real Estate).
- **Admin Approval Workflow**: Listings require admin approval before going live.
- **Seo Friendly URLs**: Slug generation for listings and categories.
- **Performance**: Caching mechanisms for categories and configuration.
