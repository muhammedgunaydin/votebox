# votebox

Voting managament/usage system written in [.NET](https://learn.microsoft.com/tr-tr/dotnet/welcome) MVC model and [PostgreSql](https://www.postgresql.org/) database.

## Requirements
- [.NET](https://learn.microsoft.com/tr-tr/dotnet/welcome)
- [PostgreSql](https://www.postgresql.org/)
- Code Editor

## Instructions

- Clone the repository and move to the project directory:
  ```bash
    git clone https://github.com/muhammedgunaydin/votebox.git
    cd votebox
  ```
- To load the dependencies:
  ```bash
    dotnet restore
  ```
- To load the migrations:
  ```bash
    dotnet ef migrations add InitialCreate
  ```
- And:
  ```bash
    dotnet ef database update
  ```
- Start:
  ```bash
    dotnet run
  ```
  
## Usage
The application has two sides. Admin panel and users. You can manage users, start and end voting via the admin panel. And you can vote via user and see the progress of the votes. However, in practice there is no method of obtaining admin authority. After creating a user, you need to manually change the Role field in the database to "Admin". After making the change, log in with an admin authorized account from the login screen, it will direct you admin panel.
