# TruckLoading-Api-x-Client

## Overview
TruckLoading-Api-x-Client is a C# based application designed to facilitate and optimize the loading process for trucks. The project provides an API and client implementation to manage and streamline the truck loading operations effectively.

## Features
- **Load Optimization:** Calculate the optimal loading arrangement for various goods in a truck.
- **API Integration:** Seamless integration with other systems via API endpoints.
- **User Management:** Manage users and their roles in the loading process.
- **Data Analytics:** Generate reports and analytics on loading operations and efficiency.

## Installation

To set up the project locally, follow these steps:

1. **Clone the repository:**
    ```bash
    git clone https://github.com/OLEMUKAN/TruckLoading-Api-x-Client.git
    ```

2. **Navigate to the project directory:**
    ```bash
    cd TruckLoading-Api-x-Client
    ```

3. **Build the project:**
    ```bash
    dotnet build
    ```

4. **Run the project:**
    ```bash
    dotnet run
    ```

## Requirements
- **.NET Version:** net8.0
- **Packages and Versions:**
  - `Asp.Versioning.Http` Version: 8.1.0
  - `Asp.Versioning.Mvc` Version: 8.1.0
  - `Asp.Versioning.Mvc.ApiExplorer` Version: 8.1.0
  - `Microsoft.AspNetCore.Authentication.JwtBearer` Version: 8.0.12
  - `Microsoft.AspNetCore.SignalR.Core` Version: 1.2.0
  - `Microsoft.EntityFrameworkCore.Design` Version: 8.0.12
  - `Microsoft.EntityFrameworkCore.Tools` Version: 8.0.12
  - `Swashbuckle.AspNetCore` Version: 6.6.2

## Usage

### API Endpoints
- **GET /api/loading-plan:** Retrieve the current loading plan.
- **POST /api/loading-plan:** Submit a new loading plan.
- **GET /api/users:** List all users.
- **POST /api/users:** Create a new user.

### Example Request
To get the current loading plan:
```bash
curl -X GET https://yourapiurl.com/api/loading-plan
```

## Contributing
We welcome contributions to improve TruckLoading-Api-x-Client. To contribute:

1. Fork the repository.
2. Create a new branch (`git checkout -b feature-branch`).
3. Make your changes.
4. Commit your changes (`git commit -m 'Add some feature'`).
5. Push to the branch (`git push origin feature-branch`).
6. Create a new Pull Request.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contact
For any questions or feedback, please open an issue on the repository or contact the project owner at [OLEMUKAN](https://github.com/OLEMUKAN).
