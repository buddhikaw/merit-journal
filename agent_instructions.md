# Product Requirement Document: Coding Agent for Merit Journal Application

## 1. Introduction and Purpose

This document outlines the product requirements for a "Coding Agent" tasked with developing a full-stack **Merit Journal** application. The agent's primary purpose is to generate high-quality, maintainable, and scalable code for both a C# API backend and a React frontend, adhering to specified architectural principles, technologies, and best practices. The application will allow users to record and manage **meritorious acts or significant things done on a particular day**.

---

## 2. Goals and Objectives

The primary objective for the Coding Agent is to produce two integrated applications that collectively form the Merit Journal application.

**Key Goals:**

* **Deliver a functional Merit Journal Application:** Users must be able to create, view, edit, and delete journal entries.

* **Ensure data persistence:** Journal entries and user data must be stored and retrieved reliably.

* **Implement secure user management:** User identity must be managed through industry-standard authentication.

* **Provide a responsive and intuitive user interface:** The application should be usable across various devices.

* **Adhere to modern development standards:** Code must follow specified architectural patterns and include testing and containerization capabilities.

---

## 3. User Stories / Features

The Coding Agent must generate code that supports the following core functionalities:

### 3.1. User Management (Authentication & Authorization)

* As a user, I can log in to the application using external OpenID Connect (OIDC) providers (e.g., Google, Microsoft).

* As a user, my identity from the OIDC provider (e.g., `sub` claim) is securely linked to my journal entries in the backend.

* As an authenticated user, I can only access and manage my own journal entries.

* As an authenticated user, I can securely log out of the application.

### 3.2. Merit Journal Entry Management

* As an authenticated user, when I log in, I can see my most recent journal entries, categorized chronologically by date, reflecting **the meritorious things I did on those days**.

* As an authenticated user, I can create a new journal entry with a title, formatted text content (e.g., HTML text), one or more associated image data, and multiple descriptive tags.

* As an authenticated user, I can view all my existing journal entries, displayed with their titles, formatted content, embedded images (if any), creation date, and associated tags.

* As an authenticated user, I can edit the title, formatted content, associated images, and tags of my existing journal entries.

* As an authenticated user, I can delete my existing journal entries.

* As an authenticated user, I can search my journal entries using specific tags.

---

## 4. Technical Requirements

The Coding Agent must generate code that strictly adheres to the following technical specifications:

### 4.1. Backend API (C#)

* **Language**: C#

* **Framework**: ASP.NET Core Minimal APIs

* **Architecture**: Clean Architecture with distinct Domain, Application, Infrastructure, and Presentation (API) layers.

* **Database**: **PostgreSQL** for data persistence.

    * EF Core must be configured to use **Npgsql** for PostgreSQL interaction.

    * EF Core Migrations must be defined and integrated for schema management.

    * Journal entries will include fields for formatted text (HTML content) and binary image data.

    * Journal entries must support categorization by multiple tags.

* **Command/Query Handling**: MediatR for implementing a Command Query Responsibility Segregation (CQRS) pattern within the Application layer.

* **Object Mapping**: AutoMapper for mapping between domain entities and Data Transfer Objects (DTOs).

* **API Documentation**: Swagger (OpenAPI) for interactive API documentation.

* **Authentication**: Configured to accept JWTs from OIDC providers via Bearer token authentication (`Microsoft.AspNetCore.Authentication.JwtBearer`). The API must identify users based on claims (e.g., `NameIdentifier` or `sub` claim) from these validated tokens.

* **Containerization**: Dockerfile provided for building a Linux-based Docker image of the API.

### 4.2. Frontend Application (React)

* **Framework**: Latest React version with functional components and hooks.

* **UI Library**: Material-UI (MUI) for a consistent and responsive design.

* **State Management**: Redux Toolkit for predictable state management, including:

    * Auth slice for OIDC user state, JWT, and authentication status.

    * Journal entry slice for managing journal entry data.

    * Async Thunks for handling API interactions.

    * Redux Persist for persisting authentication state in local storage.

* **Authentication**: `oidc-client-ts` library for handling OIDC flows with external identity providers (Google, Microsoft).

* **API Interaction**: Uses standard `fetch` API calls, sending JWTs in the `Authorization: Bearer` header.

    * Must handle sending and receiving formatted HTML text for journal entry content.

    * Must support uploading and displaying images (binary data) directly from the database.

    * Must support managing and searching by tags.

* **Responsive Design**: Implemented using Material-UI's responsive features (e.g., `Grid` system, breakpoints) to ensure optimal viewing and usability across mobile, tablet, and desktop devices.

* **Error Handling**: Basic UI feedback for API call errors and authentication failures.

### 4.3. Testing

* **Unit Tests**: Dedicated project (`MeritJournal.UnitTests`) using a standard framework (e.g., xUnit, NUnit) with mocking (e.g., Moq) for isolated testing of Application layer components (e.g., MediatR handlers).

* **Integration Tests**: Dedicated project (`MeritJournal.IntegrationTests`) using `WebApplicationFactory` for in-memory API testing and in-memory SQLite (or Testcontainers for full realism) for database interactions.

### 4.4. Deployment

* **Containerization**: The backend must be containerizable and ready for Linux containers.

* **Frontend Deployment**: The frontend application will be deployed as static assets on **AWS CloudFront**.

* **CI/CD Pipeline Readiness**: The generated solution structure and Dockerfiles should support automation for:

    * Building applications.

    * Running all tests (unit and integration).

    * Building Docker images.

    * Pushing Docker images to Docker Hub.

    * Automating deployment to a Linux-compatible hosting environment for the backend and **AWS CloudFront for the frontend**.

## 5. Non-Functional Requirements

* **Performance**: API responses should be timely, and the frontend should render smoothly, especially when handling image data. Display of recent journal entries and search by tags must be performant.

* **Security**: Authentication must adhere to OIDC standards. No sensitive information (e.g., client secrets) should be exposed in frontend code. Backend API must validate JWTs.

* **Maintainability**: Code must be clean, well-structured, and follow Clean Architecture principles to facilitate future development and debugging.

* **Scalability**: The architectural choices (Clean Architecture, MediatR, containerization, PostgreSQL) should provide a foundation for future scaling.

* **Testability**: Code structure must maximize testability, enabling comprehensive unit and integration testing.

* **Usability**: The frontend application should be intuitive and easy to navigate for end-users, supporting a diary-like experience for recording merits.

* **Internationalization (i18n)**: Journal entry content (text) must support multiple languages (e.g., English or any other language) and should be stored and displayed correctly.

---

## 6. Out of Scope

The following aspects are explicitly out of scope for the current generation by the Coding Agent:

* Advanced authorization policies (e.g., role-based access control beyond user ownership of journal entries and basic tag access).

* Complex database schemas (e.g., relationships beyond User-JournalEntries-Tags, advanced querying beyond specified tag search).

* Real-time features (e.g., WebSockets for instant journal entry updates).

* Server-side rendering (SSR) or Static Site Generation (SSG) for the React app.

* Advanced logging/monitoring beyond basic console output and error alerts.

* User registration directly within the API (it relies solely on OIDC provider authentication).

* Custom UI frameworks or extensive custom CSS/styling beyond Material-UI's capabilities.

* Advanced image manipulation (e.g., resizing, compression) on the backend or frontend beyond basic storage and display.