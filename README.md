# TruckLoading-Api-x-Client

## Overview
TruckLoading-Api-x-Client is a C# based application designed to facilitate and optimize the loading process for trucks. The project provides an API and client implementation to manage and streamline the truck loading operations effectively.

## Features
- **Load Optimization:** Calculate the optimal loading arrangement for various goods in a truck.
- **API Integration:** Seamless integration with other systems via API endpoints.
- **User Management:** Manage users and their roles in the loading process.
- **Data Analytics:** Generate reports and analytics on loading operations and efficiency.
- **Driver Management:** Comprehensive driver tracking, compliance, and scheduling system.
- **Recurring Schedules:** Create and manage recurring driver schedules with advanced compliance validation.

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

#### Driver Schedule Endpoints
- **GET /api/driverschedule/{scheduleId}:** Get a specific schedule by ID.
- **GET /api/driverschedule/driver/{driverId}:** Get a driver's schedules for a specific date range.
- **GET /api/driverschedule/driver/{driverId}/recurring:** Get all recurring schedules for a driver.
- **GET /api/driverschedule/recurring/{recurringScheduleId}/instances:** Get all instances of a recurring schedule.
- **POST /api/driverschedule:** Create a new schedule.
- **POST /api/driverschedule/recurring:** Create a new recurring schedule.
- **PUT /api/driverschedule/{scheduleId}:** Update an existing schedule.
- **PUT /api/driverschedule/recurring/{scheduleId}:** Update a recurring schedule.
- **DELETE /api/driverschedule/{scheduleId}:** Delete a schedule.
- **DELETE /api/driverschedule/recurring/{scheduleId}:** Delete a recurring schedule.
- **GET /api/driverschedule/available:** Get available drivers for a time slot.
- **GET /api/driverschedule/driver/{driverId}/available:** Check if a driver is available for a time slot.

### Recurring Schedules

The system supports creating recurring driver schedules with the following patterns:
- **Daily:** Schedule repeats every day
- **Weekly:** Schedule repeats every week on the same day
- **Bi-Weekly:** Schedule repeats every two weeks on the same day
- **Monthly:** Schedule repeats every month on the same day

All recurring schedules undergo advanced compliance validation to ensure:
- Drivers receive required rest periods
- Daily driving time limits are not exceeded
- Continuous driving without breaks is within regulations

### Example Request
To get the current loading plan:
```bash
curl -X GET https://yourapiurl.com/api/loading-plan
```

To create a recurring schedule:
```bash
curl -X POST https://yourapiurl.com/api/driverschedule/recurring \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {your_token}" \
  -d '{
    "driverId": 1,
    "startTime": "2023-06-01T08:00:00",
    "endTime": "2023-06-01T17:00:00",
    "notes": "Weekly delivery route",
    "recurrencePattern": 1,
    "recurrenceEndDate": "2023-08-01T00:00:00",
    "maxOccurrences": 8
  }'
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

![CodeRabbit Pull Request Reviews](https://img.shields.io/coderabbit/prs/github/OLEMUKAN/TruckLoading-Api-x-Client?labelColor=171717&color=FF570A&link=https%3A%2F%2Fcoderabbit.ai&label=CodeRabbit%20Reviews)

## Contact
For any questions or feedback, please open an issue on the repository or contact the project owner at [OLEMUKAN](https://github.com/OLEMUKAN).
