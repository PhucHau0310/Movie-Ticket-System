# Movie Ticket Booking System

A full-stack movie ticket booking system with FastAPI backend and Windows Forms frontend.

## Table of Contents

-   [Overview](#overview)
-   [Features](#features)
-   [Technology Stack](#technology-stack)
-   [Screenshots](#screenshots)
-   [Getting Started](#getting-started)
    -   [API Setup](#api-setup)
    -   [Desktop App Setup](#desktop-app-setup)
-   [Project Structure](#project-structure)
-   [Author](#author)

## Overview

This project implements a movie ticket booking system with two main components:

-   RESTful API built with FastAPI
-   Windows Forms desktop application for user interface

## Features

-   User authentication (Login/Register)
-   Role-based access control (Admin/Staff/Customer)
-   Movie management
-   Ticket booking with VNPay integration
-   Movie recommendations using Pinecone vector similarity
-   User profile and preferences management
-   Dashboard with statistics
-   Ticket history tracking

## Technology Stack

### Backend (API)

-   Python 3.11
-   FastAPI
-   PostgreSQL
-   SQLAlchemy
-   Alembic
-   Pinecone
-   Docker

### Frontend (Desktop App)

-   C# Windows Forms
-   .NET Framework
-   Visual Studio 2022

## Screenshots

![Login Screen](/Screenshots/auth/signin.png)
![Customer Recommandations](/Screenshots/customer/recommandations.png)
![Admin Dashboard](/Screenshots/admin/dashboard.png)
![Movie Management](/Screenshots/admin/managementMovie.png)

## Getting Started

### API Setup

#### Option 1: Using Virtual Environment

```bash
# Clone repository
git clone <repository-url>
cd MovieTicketSystem/MovieTicketAPI

# Create virtual environment
python -m venv .venv
.venv\Scripts\activate

# Install dependencies
pip install -r requirements.txt

# Set up environment variables
copy .env.example .env
# Edit .env with your configurations

# Run migrations
alembic upgrade head

# Start API server
uvicorn app.main:app --reload
```

#### Option 2: Using Docker

```bash
# Clone repository
git clone <repository-url>
cd MovieTicketSystem/MovieTicketAPI

# Build and run with Docker Compose
docker-compose up --build
```

The API will be available at `http://localhost:8000`  
Swagger documentation: `http://localhost:8000/docs`

### Desktop App Setup

1. Open `MovieTicketSystem/MovieTicketApp/MovieTicketApp.sln` in Visual Studio
2. Restore NuGet packages
3. Build the solution
4. Run the application

## Project Structure

```
MovieTicketSystem/
├── MovieTicketAPI/         # FastAPI Backend
│   ├── app/
│   │   ├── models/        # Database models
│   │   ├── routers/       # API endpoints
│   │   ├── schemas/       # Pydantic schemas
│   │   └── utils/         # Utility functions
│   ├── alembic/           # Database migrations
│   └── tests/             # API tests
│
└── MovieTicketApp/        # Windows Forms Frontend
    ├── Forms/             # UI Forms
    ├── Models/            # Data models
    ├── Services/          # API services
    └── Utils/             # Utility classes
```

## Author

**Nguyễn Phúc Hậu**

-   Email: haunhpr024@gmail.com
-   GitHub: [Your GitHub Profile](https://github.com/yourusername)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details
