# Contributing to Azure Price Calculator Agent

Thank you for your interest in contributing to the Azure Price Calculator Agent! This document provides guidelines for contributing to the project.

## Getting Started

1. Fork the repository
2. Clone your fork
3. Create a new branch for your feature or bug fix
4. Make your changes
5. Test your changes
6. Submit a pull request

## Development Setup

### Prerequisites

- .NET 10 SDK or later
- Docker Desktop (for local development with Aspire)
- Azure CLI (for deployment)
- Azure Developer CLI (azd)
- Visual Studio 2022 or VS Code

### Local Development

1. Clone the repository:
   ```bash
   git clone https://github.com/marconsilva/apca.git
   cd apca
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the solution:
   ```bash
   dotnet build
   ```

4. Run with Aspire:
   ```bash
   cd AzurePriceCalculatorAgent.AppHost
   dotnet run
   ```

## Code Style

- Follow standard .NET coding conventions
- Use meaningful variable and method names
- Add XML documentation comments for public APIs
- Keep methods focused and concise

## Testing

- Write unit tests for new features
- Ensure all existing tests pass
- Test locally before submitting PR

## Pull Request Process

1. Update the README.md with details of changes if applicable
2. Update the version number in relevant files
3. Ensure your code builds without warnings
4. Submit your pull request with a clear description

## Code of Conduct

- Be respectful and inclusive
- Welcome newcomers and help them learn
- Focus on constructive feedback
- Report unacceptable behavior

## Questions?

Feel free to open an issue for questions or discussions!
