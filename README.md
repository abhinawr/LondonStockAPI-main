# London Stock API

This API allows authorized brokers to notify the London Stock Exchange (LSE) of trade transactions and expose updated stock prices. This version uses JWT for authentication and defaults to an In-Memory database for quick setup and demonstration, with a clear path to more scalable solutions.

## Core Functionality (MVP)

*   **Trade Submission:** Authenticated brokers can submit trades (ticker symbol, price, shares). Broker identity is derived from the JWT token.
*   **Stock Price Exposure:**
    *   Retrieve the current value of a specific stock (calculated as the average price of all its transactions).
    *   Retrieve values for all stocks on the market.
    *   Retrieve values for a specified range of stocks.
*   **JWT Authentication:** Endpoints are protected using JWT Bearer tokens. A demo token generation endpoint (`/api/auth/token`) is provided.
*   **Data Persistence:** Defaults to EF Core In-Memory provider for rapid demonstration. Easily configurable for SQL Server (or other relational databases).
*   **API Documentation:** Documented via Swagger/OpenAPI, accessible at the application root.

## Technology Stack

*   .NET 8 (or specified version)
*   ASP.NET Core Web API
*   Entity Framework Core 8 (In-Memory, SQL Server)
*   JWT (JSON Web Tokens) for Authentication
*   Swagger (Swashbuckle)

## Prerequisites

*   .NET SDK (version compatible with the project)
*   (Optional, for SQL Server) SQL Server LocalDB or another SQL Server instance.

## Setup and Running (In-Memory Default)

1.  **Clone the repository:**
    ```bash
    git clone <repository-url>
    cd LondonStockApi
    ```

2.  **Restore .NET dependencies:**
    ```bash
    dotnet restore
    ```

3.  **Configure JWT Settings (Critical for Security):**
    *   Open `appsettings.json`.
    *   **Replace the placeholder `Jwt:Key` with a strong, random secret key (at least 32 characters for HMACSHA256 is good practice).**
        ```json
        "Jwt": {
          "Issuer": "https://your-issuer.com", // Customize
          "Audience": "https://your-audience.com/api", // Customize
          "Key": "YOUR_ACTUAL_STRONG_RANDOM_SECRET_KEY_GOES_HERE",
          "ExpiryInMinutes": 60
        }
        ```
        *Note: In a production environment, this key must be managed securely (e.g., Azure Key Vault, HashiCorp Vault, .NET User Secrets for local dev).*
    *   The `DemoUsers` section is for the demo token endpoint. **Real applications must use a secure identity management system with hashed passwords.**

4.  **Run the application:**
    ```bash
    dotnet run
    ```
    Access the API at `https://localhost:7xxx` or `http://localhost:5xxx`. Swagger UI is at the root.

## API Usage Flow (JWT Authentication)

1.  **Obtain JWT Token:**
    *   `POST /api/auth/token` with credentials from `DemoUsers` in `appsettings.json`.
        ```json
        // Request Body
        { "username": "broker1", "password": "Password123!" }
        ```
    *   Response includes the JWT token.

2.  **Access Protected Endpoints:**
    *   Include the token in the `Authorization` header: `Authorization: Bearer <your_jwt_token>`.
    *   All `Trades` and `Stocks` controller endpoints require authentication.

## Key Design Patterns & Practices Demonstrated

*   **Layered Architecture:** Clear separation between Controllers (API layer), Services (business logic), and Data Access (EF Core).
*   **Dependency Injection (DI):** Extensively used for loose coupling and testability (e.g., `DbContext`, services, `IConfiguration` injected).
*   **Repository Pattern (Implicit via EF Core):** `StockDbContext` and `DbSet<T>` abstract data persistence.
*   **Data Transfer Objects (DTOs):** `TradeInputModel`, `StockValueViewModel`, `LoginModel`, `TokenResponseModel`, `ErrorViewModel` define clear API contracts.
*   **Asynchronous Programming (`async/await`):** Used for I/O-bound operations to ensure non-blocking behavior and API responsiveness.
*   **JWT Authentication & Authorization:** Standard-based, stateless authentication.
*   **Centralized Configuration:** API settings managed via `appsettings.json`.
*   **API Documentation:** Swagger/OpenAPI for discoverability and ease of testing.

## Scalability Considerations: From MVP to High-Traffic System

The current MVP, especially with its default In-Memory database and synchronous processing, is designed for demonstration and initial development. For a production system handling high-volume, real-time stock exchange traffic, significant architectural enhancements are necessary.

**This section is designed to showcase an understanding of how to evolve the MVP.**

### Identified MVP Bottlenecks for High Traffic:

1.  **Database Write Contention (Trade Ingestion):**
    *   **MVP:** Each `POST /api/trades` results in a direct, synchronous write to the database.
    *   **Impact:** High trade volumes will quickly overwhelm the database's write capacity, leading to increased latency, timeouts, and potential data loss if the API or DB fails.
2.  **Database Read Load & Complex Calculations (Price Exposure):**
    *   **MVP:** Stock values are calculated by querying *all* relevant trades and performing an `AVG()` aggregation *on-demand* for each request to `/api/stocks/.../value`.
    *   **Impact:** As the number of trades grows, these queries become increasingly slow and resource-intensive, severely impacting API response times for price lookups. The database will be under constant heavy read load.
3.  **Synchronous API Processing:**
    *   **MVP:** The API waits for database operations to complete before responding to the client.
    *   **Impact:** Limits overall throughput and makes the API feel less responsive under load.
4.  **Single Point of Failure / Limited Throughput (Single Instances):**
    *   **MVP:** Assumes single instances of the API and database.
    *   **Impact:** No resilience to instance failure and limited by the capacity of a single server.

### Proposed Architectural Enhancements for Scalability & Resilience:

This outlines a more robust, distributed architecture capable of handling high throughput and providing low-latency price updates:

To scale, we'd move from direct database interactions to an asynchronous, event-driven architecture. Trades would flow through a message queue for robust ingestion, with separate services for processing, real-time price aggregation, and fast lookups from a dedicated cache, ensuring high throughput and low latency.


# LondonStockApi Structure

![image](https://github.com/user-attachments/assets/1ead5ad9-7a37-443b-be34-54f5efa853d2)

![Screenshot 2025-06-02 021140](https://github.com/user-attachments/assets/aac2ec7c-1597-4813-a43a-efa2cef37f50)

![image](https://github.com/user-attachments/assets/7c1f9d0d-281c-4b11-bb73-1cc4b8f1444c)

![image](https://github.com/user-attachments/assets/5733890f-b2ae-43ac-9bfb-6e135b621017)


