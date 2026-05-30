# icmc <img src="frontend/assets/icon.png" align="right" height="138" />

<!-- badges: start -->
[![Deploy API](https://github.com/NoahHellen/icmc/actions/workflows/deploy-backend.yml/badge.svg)](https://github.com/NoahHellen/icmc/actions/workflows/deploy-backend.yml)
[![Deploy Frontend](https://github.com/NoahHellen/icmc/actions/workflows/deploy-frontend.yml/badge.svg)](https://github.com/NoahHellen/icmc/actions/workflows/deploy-frontend.yml)
[![Lifecycle: experimental](https://img.shields.io/badge/lifecycle-experimental-orange.svg)](https://lifecycle.r-lib.org/articles/stages.html#experimental)
<!-- badges: end -->

The goal of icmc is to provide a comprehensive gear tracking and logbook application for the Imperial College Mountaineering Club. It provides:

- **Gear Management**: A centralized system to track club equipment, including its storage location, size, and category.
- **Loan System**: Automated reminders and tracking for gear lent to club members.
- **Logbook**: A digital record for club members to log their climbs and trips.
- **ICU Integration**: Seamless synchronization with Imperial College Union data.

## Tech Stack

- **Backend**: .NET 10 Web API with Entity Framework Core.
- **Frontend**: React Native with Expo and TypeScript.
- **Database**: SQL Server.
- **Integrations**: ImgBB for image hosting, Gmail SMTP for notifications.

## Installation

### Backend

To run the backend locally:

```bash
cd backend/Api
dotnet run
```

### Frontend

To run the frontend locally:

```bash
cd frontend
npm install
npx expo start
```

## Features

- **Automated Reminders**: Sends email notifications for overdue gear using Gmail SMTP.
- **Image Support**: Integrated with ImgBB for gear and logbook photos.
- **Role-based Access**: Different permissions for club members and administrators (Member, Driver, Committe, etc.).

## License

icmc has an MIT license.
