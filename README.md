
# GuardianGate Microservice

## Description
GuardianGate is a microservice responsible for protecting and controlling access to resources within the application. It handles the issuance and validation of authentication tokens.

## Features

- User registration and authentication.
- Token-based authentication and authorization.
- Secure handling of user credentials.
- Integration with other microservices for seamless authentication.

## Technologies Used

- .NET Core (version 7.0)
- ASP.NET Core Identity
- Entity Framework Core
- JWT (JSON Web Tokens) for authentication

## Setup and Installation

1. Clone the repository.
   ```
    git clone https://github.com/Tecnizium/GuardianGate.git
   ```
2. Navigate to the project directory:
   ```
   cd GuardianGate
   ```
3. Install dependencies:
   ```
   dotnet restore
   ```
4. Configure your database connection in `appsettings.json`.

5. Run the application:
   ```
   dotnet run
   ```
6. The service will be accessible at `http://localhost:your_port`.

## API Documentation

For detailed API documentation and usage, refer to the [API Documentation](API_DOCS.md) file.

## Contributing

Contributions are welcome! If you encounter issues or have suggestions, please open an issue or submit a pull request.

## License

This project is licensed under the [MIT License](LICENSE).