# Cab Booking Platform

A full-stack cab booking platform built with **ASP.NET Core 8**, using a **microservices architecture**, **MongoDB**, **RabbitMQ**, Docker, and an API Gateway.

The application supports user registration and login, saved locations, live weather information, fare estimation, cab bookings, simulated payments, booking history, payment history, and asynchronous booking notifications.

> **Live demo:** `https://cabbookingplatform.duckdns.org/`

---

## Features

- User registration and login
- Session-based authentication
- Save and manage favourite locations
- Current weather for saved locations
- Fare estimation between locations in Malta
- Multiple cab types and passenger counts
- Cab booking creation
- Simulated payment processing
- Current and previous booking history
- Payment history
- Asynchronous booking notifications using RabbitMQ
- Swagger/OpenAPI endpoints for the backend services
- Docker-based production deployment
- Nginx reverse proxy with HTTPS support

---

## Architecture

The platform is split into independent ASP.NET Core services:

| Service | Responsibility |
| --- | --- |
| **WebFrontend** | MVC web interface presented to the user |
| **GatewayAPI** | Single API entry point and request routing |
| **CustomerService** | Registration, login, users and inbox notifications |
| **BookingService** | Creates and retrieves cab bookings |
| **FareEstimationService** | Geocodes locations and calculates estimated fares |
| **LocationService** | Saved locations and current weather |
| **PaymentService** | Simulated payments and payment history |

Production traffic flows through Nginx:

```text
Browser
   |
   | HTTPS
   v
Nginx
   |
   +---- / ----------------------> WebFrontend
   |
   +---- /api/gateway/* ---------> GatewayAPI
                                       |
             +-------------------------+------------------------+
             |             |            |           |          |
             v             v            v           v          v
         Customer       Booking       Payment     Location    Fare
             |             |
             +------ RabbitMQ / CloudAMQP

Services requiring persistent data use MongoDB Atlas.
```

---

## Tech Stack

### Backend

- .NET 8
- ASP.NET Core Web API
- ASP.NET Core MVC
- C#
- MassTransit
- RabbitMQ
- MongoDB
- Swagger / OpenAPI

### External APIs

- **OpenCage Geocoding API** — converts Maltese locations into coordinates
- **RapidAPI Taxi Fare Calculator** — fare estimation
- **RapidAPI WeatherAPI** — current weather information

### Infrastructure

- Docker
- Docker Compose
- Nginx
- Oracle Cloud VM
- MongoDB Atlas
- CloudAMQP
- DuckDNS
- Let's Encrypt / Certbot

---

## Repository Structure

```text
CabBookingPlatform/
├── BookingService/
├── CustomerService/
├── FareEstimationService/
├── GatewayAPI/
├── LocationService/
├── PaymentService/
├── Shared/
├── WebFrontend/
├── Dockerfile
├── docker-compose.yml
├── .dockerignore
├── .gitignore
└── CabBookingPlatform.sln
```

---

## Local Development

### Requirements

Install:

- .NET 8 SDK
- Visual Studio 2022+ or another .NET IDE
- MongoDB Atlas account
- CloudAMQP RabbitMQ instance
- RapidAPI key
- OpenCage API key

### Clone the repository

```bash
git clone https://github.com/Cachia36/CabBookingPlatform.git
cd CabBookingPlatform
```

### Configuration

Sensitive values are intentionally not stored in `appsettings.json`.

For local development, configure them using **.NET User Secrets**.

#### BookingService

```bash
dotnet user-secrets set "ConnectionStrings:MongoDb" "YOUR_MONGODB_URI" --project BookingService/BookingService.csproj
dotnet user-secrets set "RabbitMq:Uri" "YOUR_RABBITMQ_URI" --project BookingService/BookingService.csproj
```

#### CustomerService

```bash
dotnet user-secrets set "ConnectionStrings:MongoDb" "YOUR_MONGODB_URI" --project CustomerService/CustomerService.csproj
dotnet user-secrets set "RabbitMq:Uri" "YOUR_RABBITMQ_URI" --project CustomerService/CustomerService.csproj
```

#### LocationService

```bash
dotnet user-secrets set "ConnectionStrings:MongoDb" "YOUR_MONGODB_URI" --project LocationService/LocationService.csproj
dotnet user-secrets set "RapidApi:Key" "YOUR_RAPIDAPI_KEY" --project LocationService/LocationService.csproj
dotnet user-secrets set "OpenCage:Key" "YOUR_OPENCAGE_KEY" --project LocationService/LocationService.csproj
```

#### FareEstimationService

```bash
dotnet user-secrets set "RapidApi:Key" "YOUR_RAPIDAPI_KEY" --project FareEstimationService/FareEstimationService.csproj
dotnet user-secrets set "OpenCage:Key" "YOUR_OPENCAGE_KEY" --project FareEstimationService/FareEstimationService.csproj
```

#### PaymentService

```bash
dotnet user-secrets set "ConnectionStrings:MongoDb" "YOUR_MONGODB_URI" --project PaymentService/PaymentService.csproj
```

---

## Local Service URLs

When running with the development HTTP profiles:

| Application | URL |
| --- | --- |
| WebFrontend | `http://localhost:5000` |
| CustomerService | `http://localhost:5001` |
| BookingService | `http://localhost:5002` |
| FareEstimationService | `http://localhost:5003` |
| LocationService | `http://localhost:5004` |
| PaymentService | `http://localhost:5005` |
| GatewayAPI | `http://localhost:7000` |
| Gateway route | `http://localhost:7000/api/gateway` |

In Visual Studio, configure the seven application projects as **Multiple Startup Projects** to run the complete platform.

---

## Build

```bash
dotnet restore
dotnet build CabBookingPlatform.sln
```

---

## Docker

The production deployment is built using Docker Compose.

Create a `.env` file in the repository root:

```env
MONGODB_URI=YOUR_MONGODB_CONNECTION_STRING
RABBITMQ_URI=YOUR_CLOUDAMQP_URI
RAPIDAPI_KEY=YOUR_RAPIDAPI_KEY
OPENCAGE_KEY=YOUR_OPENCAGE_KEY

PUBLIC_FRONTEND_URL=https://YOUR-DUCKDNS-DOMAIN.duckdns.org
PUBLIC_GATEWAY_URL=https://YOUR-DUCKDNS-DOMAIN.duckdns.org/api/gateway
```

> `.env` is ignored by Git and must never be committed.

Build and start the platform:

```bash
docker compose build
docker compose up -d
```

Check the containers:

```bash
docker compose ps
```

View logs:

```bash
docker compose logs -f
```

---

## Production Deployment

The live deployment uses:

```text
Internet
   |
   v
DuckDNS + HTTPS
   |
   v
Nginx
   |
   +---- WebFrontend container
   |
   +---- GatewayAPI container
             |
             +---- CustomerService
             +---- BookingService
             +---- FareEstimationService
             +---- LocationService
             +---- PaymentService

MongoDB Atlas <---- application services
CloudAMQP    <---- BookingService / CustomerService
```

Nginx exposes the frontend and API through the same public hostname:

```text
https://YOUR-DUCKDNS-DOMAIN.duckdns.org/
https://YOUR-DUCKDNS-DOMAIN.duckdns.org/api/gateway/
```

Internal microservices are not exposed directly to the internet.

### Updating the live deployment

Make and test changes locally, then push them:

```bash
git add .
git commit -m "Your commit message"
git push origin main
```

On the production VM:

```bash
cd ~/CabBookingPlatform
git pull origin main
docker compose up -d --build
```

---

## Keeping the Deployment Running

The Oracle VM must remain running for the website to remain online.

Docker only needs to run on the **server**. Docker Desktop does **not** need to remain open on the developer PC.

For production, each Compose service should use:

```yaml
restart: unless-stopped
```

This allows containers to restart automatically if they crash or if the VM/Docker service restarts.

Docker itself should also be enabled at boot:

```bash
sudo systemctl enable docker
```

Nginx should similarly remain enabled:

```bash
sudo systemctl enable nginx
```

---

## Security

The repository intentionally does not store:

- MongoDB credentials
- RabbitMQ credentials
- RapidAPI keys
- OpenCage API keys
- Production `.env` files

Local secrets should use .NET User Secrets and production secrets should be supplied through environment variables.

Only the reverse proxy should be publicly exposed. Individual microservice ports should remain private.

---

## Notes

- Payment handling in this project is **simulated** and stored by `PaymentService`; it is not connected to Stripe, PayPal, or another real payment processor.
- Fare estimation and weather features depend on third-party APIs and their availability/usage limits.
- Geocoding is restricted to Malta for more accurate location matching.

---

## Author

**Kyle Cachia**

GitHub: [Cachia36](https://github.com/Cachia36)

---

## License

Add a licence file if you intend to distribute or allow reuse of the project.
