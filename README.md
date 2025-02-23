# Saving Money Management System
A system for managing savings accounts online to help users easily track and manage their savings.
## Main features

### 1. Account management
- Register, login
- Manage personal information
- Email verification
- Forgot password

### 2. Savings account management
- Open a new savings account
- Track balance and interest rate
- Automatically calculate interest
- Manage maturity
- View transaction history

### 3. Utilities
- Compare savings packages
- Calculate expected interest rate
- Create a savings plan
- Suggest suitable savings packages


### 4. Automatic features
- Automatically update balance
- Automatically calculate interest
- Notify maturity via email

## Technologies used
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Identity Framework
- Bootstrap
- jQuery

## Project Structure

SavingMoneyManagement/

├── Controllers/ # Handle logic
├── Models/ # Data models
├── Views/ # User interface
├── Services/ # Handle business logic
├── Repositories/ # Interact with database
├── Helpers/ # Utility functions
├── Entity/ # Entity framework models
├── Data/ # Database context and migrations
└── wwwroot/ # Static files (CSS, JS, Images)

## Setup and Run 
### Prerequisites
- .NET 6 SDK up to date
- SQL Server
- Visual Studio 2022

### Setup steps

1. Clone the repository:
```
git clone https://github.com/your-username/SavingMoneyManagement.git
```
2. Update the database connection string in this file `appsettings.json`
3. Run migrations to create the database: 
```commandline
dotnet ef database update
```
4. Build and run the project: 
```bash
dotnet build
dotnet run
```
5. Open the browser and navigate to `https://localhost:7272`

### Account demo 
- Admin: `admin@gmail.com` ; Password: `Admin@123`
- User: `user@gmail.com` ; Password: `User@123`

### Note 
- Make sure to update the database connection string in `appsettings.json`
- Make sure to run migrations to create the database
- Make sure to build and run the project
- Make sure to navigate to `https://localhost:7272`
### Contributing
- Le Hieu Nghia 
- Link github: [@lehieungia](https://github.com/lehieungia)
- Email: `lehieungia2002@gmail.com`
