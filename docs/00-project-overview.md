# 00 Project Overview

## Objetivo del Proyecto
MesaFacil.API es el backend para un sistema de Punto de Venta (POS) y gestión de restaurantes, facilitando la administración de mesas, pedidos, productos, menús, cuentas, pagos y tickets de cocina.

## Tecnologías
- **Framework:** .NET 9
- **Arquitectura:** Clean Architecture
- **ORM:** Entity Framework Core
- **Base de Datos:** PostgreSQL (Npgsql)
- **Autenticación:** JWT (JSON Web Tokens)
- **Documentación de API:** OpenAPI con Scalar
- **Logging/Monitoreo:** WatchDog

## Solución y Proyectos
La solución `MesaFacil.API.sln` está dividida en las siguientes capas (proyectos):
- **Domain:** Entidades del núcleo del negocio y abstracciones base (`BaseEntity`, `BaseAuditableEntity`).
- **UseCases:** Lógica de aplicación, orquestación de flujos (CQRS o Servicios de Aplicación).
- **Interface:** Contratos (Interfaces) para repositorios, UnitOfWork y UseCases.
- **DTO:** Data Transfer Objects para la comunicación entre capas.
- **Validator:** Lógica de validación usando FluentValidation.
- **Persistence:** Implementación de acceso a datos (EF Core, `ApplicationDbContext`, Repositories).
- **WebApi:** Controladores de la API REST, configuración de DI y middlewares.
- **Common / Logging:** Utilidades transversales y configuración de registros.

## Dependencias Principales
- `Microsoft.EntityFrameworkCore` & `Npgsql.EntityFrameworkCore.PostgreSQL`
- `Scrutor` para inyección de dependencias dinámica.
- `Scalar.AspNetCore` para la interfaz gráfica de la API.
- `WatchDog` para captura de excepciones y logs.

## Cómo iniciar el proyecto
1. Configurar la cadena de conexión `mesafacil_db` en `appsettings.Development.json` hacia una instancia de PostgreSQL.
2. Ejecutar las migraciones con `dotnet ef database update --project Persistence --startup-project WebApi`.
3. Ejecutar el proyecto WebApi: `dotnet run --project WebApi`.
4. Acceder a la documentación de la API generada por Scalar en el endpoint expuesto (típicamente `/scalar/v1`).
