# Inventario Ferretería

Este proyecto es un sistema de gestión de inventario para una ferretería, desarrollado con ASP.NET Core MVC. Permite la administración de artículos, control de stock y autenticación de usuarios. Una característica clave es la exposición de funcionalidades a través de servicios SOAP.

## Descripción del Proyecto

El sistema "Inventario Ferretería" es una aplicación web diseñada para optimizar la gestión de productos en una ferretería. Ofrece una interfaz de usuario para realizar operaciones CRUD (Crear, Leer, Actualizar, Eliminar) sobre los artículos, así como una sección para visualizar artículos con bajo stock. La autenticación de usuarios se maneja a través de ASP.NET Core Identity.

### Servicios SOAP

El proyecto expone funcionalidades específicas a través de servicios SOAP, permitiendo la integración con otros sistemas. Las operaciones SOAP disponibles incluyen:

*   **`InsertarArticuloSoap`**: Permite registrar un nuevo artículo en el inventario.
*   **`ConsultarArticuloPorCodigoSoap`**: Permite consultar los detalles de un artículo utilizando su código.

Estos servicios están implementados utilizando la librería `SoapCore` y el modelo `Articulo` está configurado con `DataContract` y `DataMember` para su correcta serialización y deserialización.

El endpoint del servicio SOAP se encuentra en la ruta `/ArticuloService.asmx` (o similar, dependiendo de la configuración exacta de `SoapCore` en `Program.cs`).

## Características Principales

*   Gestión completa de artículos (CRUD).
*   Visualización de artículos con stock bajo.
*   Sistema de autenticación y autorización de usuarios (ASP.NET Core Identity).
*   Exposición de funcionalidades clave a través de servicios SOAP.
*   Interfaz de usuario intuitiva basada en ASP.NET Core MVC.

## Tecnologías Utilizadas

*   **Backend**: ASP.NET Core 8.0
*   **Base de Datos**: PostgreSQL (Entity Framework Core)
*   **Servicios Web**: SOAP (con `SoapCore`)
*   **Frontend**: HTML, CSS (Bootstrap), JavaScript (jQuery)
*   **ORM**: Entity Framework Core
*   **Autenticación**: ASP.NET Core Identity

## Prerrequisitos

Antes de ejecutar el proyecto, asegúrate de tener instalado lo siguiente:

*   **.NET SDK 8.0** o superior.
*   **Visual Studio Code** (o Visual Studio 2022).
*   **PostgreSQL**: Servidor de base de datos.
*   **Cliente PostgreSQL**: Como pgAdmin o DBeaver (opcional, para gestionar la base de datos).

## Guía de Instalación y Ejecución

Sigue estos pasos para configurar y ejecutar el proyecto en tu entorno local:

### 1. Clonar el Repositorio

```bash
git clone https://github.com/BryanAndresO/inventario_ferreteria.git
cd inventario_ferreteria
```

### 2. Configuración de la Base de Datos

1.  **Crear la Base de Datos**: Asegúrate de tener un servidor PostgreSQL en ejecución.
2.  **Ejecutar Script SQL**: Abre tu cliente PostgreSQL (pgAdmin, DBeaver, etc.) y ejecuta el script `Base de datos/inventario_ferreteria.sql` para crear la base de datos y sus tablas.
3.  **Actualizar Cadena de Conexión**: Abre el archivo `appsettings.json` y verifica que la cadena de conexión `DefaultConnection` apunte a tu instancia de PostgreSQL. La configuración por defecto es:

    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Host=localhost;Port=5432;Database=inventario_ferreteria;Username=postgres;Password=1234"
    }
    ```

    Asegúrate de que el `Host`, `Port`, `Username` y `Password` coincidan con tu configuración de PostgreSQL.

### 3. Restaurar Dependencias

Abre una terminal en la raíz del proyecto (`inventario_ferreteria`) y ejecuta:

```bash
dotnet restore
```

### 4. Ejecutar la Aplicación en Visual Studio Code

1.  Abre la carpeta del proyecto en Visual Studio Code.
2.  Abre una terminal integrada (Ctrl+ñ o View > Terminal).
3.  Ejecuta la aplicación con el siguiente comando:

    ```bash
    dotnet run --project inventario_ferreteria_Servidor.csproj
    ```

4.  Una vez que la aplicación se inicie, podrás acceder a ella a través de tu navegador web. La URL principal será similar a:

    *   **Aplicación Web**: `https://192.168.1.35:7208` (o la que se muestre en la consola al iniciar).
    *   **Endpoint SOAP**: `https://192.168.1.35:7208/ArticuloService.asmx` (o la ruta configurada para el servicio SOAP).

### 5. Acceder a la Aplicación

Abre tu navegador y navega a la URL proporcionada en el paso anterior. Podrás registrarte, iniciar sesión y comenzar a gestionar el inventario de la ferretería.

---

**Desarrollado por**: Kevin Lechon, Bryan Ortiz
**Proyecto**: Arquitectura N-Capas con Servicios SOAP
