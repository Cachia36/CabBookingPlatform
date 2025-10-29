# 🚖 Cab Booking Platform

A distributed, event-driven cab booking system built using Microservices Architecture in .NET. This project was developed as part of the **ITSFT-606-2101 Distributed Programming Assignment**.

## 🌐 Live Deployment (Hosted on Microsoft Azure)

### 🔗 Frontend Web Application
- 🌐 [https://cabbookingfrontendkc.azurewebsites.net/](https://cabbookingfrontendkc.azurewebsites.net/)

### 🔌 Gateway API
> Central entry point that routes client requests to the appropriate microservice.
- 📍 [https://cabbooking-gateway-api.azurewebsites.net/swagger](https://cabbooking-gateway-api.azurewebsites.net/swagger/index.html)

### 🧩 Microservices

| Service             | Swagger Link                                                                 |
|---------------------|-------------------------------------------------------------------------------|
| Customer Service    | [https://cabbooking-customer-api.azurewebsites.net/swagger/index.html](https://cabbooking-customer-api.azurewebsites.net/swagger/index.html)   |
| Booking Service     | [https://cabbooking-booking-api.azurewebsites.net/swagger/index.html](https://cabbooking-booking-api.azurewebsites.net/swagger/index.html)     |
| Fare Estimation     | [https://cabbooking-fare-api.azurewebsites.net/swagger/index.html](https://cabbooking-fare-api.azurewebsites.net/swagger/index.html)           |
| Location Service    | [https://cabbooking-location-api.azurewebsites.net/swagger/index.html](https://cabbooking-location-api.azurewebsites.net/swagger/index.html)   |
| Payment Service     | [https://cabbooking-payment-api.azurewebsites.net/swagger/index.html](https://cabbooking-payment-api.azurewebsites.net/swagger/index.html)     |

## 🛠️ Tech Stack

### Backend (Microservices)
- **.NET 6/7**
- **ASP.NET Core Web API**
- **MongoDB Atlas** (cloud NoSQL database)
- **RabbitMQ** (for asynchronous event-driven communication)
- **Azure App Services** (for deployment)

### Frontend
- **ASP.NET Core MVC** / **Blazor**
- **Bootstrap / Tailwind CSS**

### External APIs
- [🚕 Taxi Fare Calculator API](https://rapidapi.com/3b-data-3b-data-default/api/taxi-fare-calculator)
- [☀️ WeatherAPI](https://rapidapi.com/weatherapi/api/weatherapi-com)
- [📍 OpenCage Geocoding API](https://opencagedata.com/api)

## 🔄 Gateway API Usage

The **Gateway API** acts as the single point of access to the backend, simplifying communication between the frontend and various microservices. It improves:
- **Security**: Abstracts and protects internal microservices
- **Scalability**: Routes requests efficiently
- **Maintainability**: Simplifies frontend logic by consolidating endpoints

## ✨ Features

- **Customer Microservice**
  - User registration, login, inbox, user details
- **Booking Microservice**
  - Book rides, view current/past bookings
- **Payment Microservice**
  - Create process payments for MongoDb, view payment history
- **Fare Estimation Microservice**
  - Get real-time taxi fare from external API and calculate total price
- **Location Microservice**
  - Manage favorite pickup locations and fetch weather & coordinates
- **Event-Driven System (via RabbitMQ)**
  - Notify users when:
    - A cab is ready (3 minutes post-booking)
    - A discount becomes available (after 3 bookings)

