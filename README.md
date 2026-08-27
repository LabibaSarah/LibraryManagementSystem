📚 IUBAT Central Library Management System

A web-based Library Management System developed for the IUBAT Central Library.
The system provides role-based access for Admin, Student, and Faculty users and manages books, borrowing, reservations, users, notices, e-books, and library-related activities.

🔗 Project Repository

GitHub:
https://github.com/LabibaSarah/LibraryManagementSystem

🛠️ Technologies Used

ASP.NET Core MVC

C#

.NET 10

Entity Framework Core

ASP.NET Core Identity

SQL Server / SQL Server LocalDB

Bootstrap

HTML, CSS, JavaScript

Visual Studio

SQL Server Management Studio (SSMS)

The repository currently contains the solution file, the main LibraryManagementSystem project, controllers, data layer, models, views, migrations, Identity pages, and static web assets.

👥 User Roles

👨‍💼 Admin

Admin can manage the main library operations, including:

Book management

Categories

Issue and return management

User management

Notices

E-books

Library settings/status

Reports and analytics

Reservations

👩‍🎓 Student

Students can:

Register and log in

Search books

Use category/advanced search

View book details

Check availability

Borrow books

View borrowed books

Check issue and due dates

Track overdue books

View fines

Manage profile

Change password

Reserve unavailable books

👨‍🏫 Faculty

Faculty members can:

Securely log in and log out

Search and browse books

View detailed book information

Check real-time availability

Borrow up to 5 books

Keep books for 14 days

View borrowed books and due dates

Manage profile

Change password

Reserve unavailable books

💻 Setup on a New Computer

Follow these steps if you are a team member and want to run the project on your own computer.

Step 1 — Install Visual Studio

Install Visual Studio with the:

ASP.NET and web development workload.

Make sure your Visual Studio installation supports .NET 10.

Step 2 — Install SQL Server LocalDB

The project uses SQL Server LocalDB for development.

The SQL Server instance used by the project is:

(localdb)\MSSQLLocalDB

Step 3 — Install SQL Server Management Studio (Optional but Recommended)

Install SQL Server Management Studio (SSMS) if you want to view and manage the database manually.

Official Microsoft page:

https://learn.microsoft.com/en-us/ssms/install/install

📥 Step 4 — Clone the GitHub Repository

Open Command Prompt, PowerShell, or the Visual Studio terminal.

Run:

git clone https://github.com/LabibaSarah/LibraryManagementSystem.git

Then enter the project folder:

cd LibraryManagementSystem

The repository contains:

LibraryManagementSystem.slnx

and the main application folder:

LibraryManagementSystem/

📂 Step 5 — Open the Project

Open:

LibraryManagementSystem.slnx

in Visual Studio.

Alternatively, open the cloned folder in Visual Studio and select the solution.

Wait for Visual Studio to restore the required NuGet packages.

🗄️ Step 6 — Database Configuration

The project uses the following database:

Database Server:
(localdb)\MSSQLLocalDB

Database Name:
LibraryManagementSystemDb

The main connection string is stored in:

LibraryManagementSystem/appsettings.json

The DefaultConnection points to:

Server=(localdb)\MSSQLLocalDB;
Database=LibraryManagementSystemDb;
Trusted_Connection=True;
TrustServerCertificate=True;
MultipleActiveResultSets=true

⚠️ If another team member already has a different SQL Server setup, they may need to change the connection string to match their local environment.

🧱 Step 7 — Create/Update the Database

The project contains Entity Framework Core migrations in:

LibraryManagementSystem/Migrations/

This means team members do not need to manually create every table.

Using Visual Studio

Open:

Tools
→ NuGet Package Manager
→ Package Manager Console

Make sure the Default Project is:

LibraryManagementSystem

Then run:

Update-Database

Entity Framework Core will apply the existing migrations and create/update:

LibraryManagementSystemDb

🖥️ Step 8 — Check the Database in SSMS

Open SQL Server Management Studio.

Use:

Server type

Database Engine

Server name

(localdb)\MSSQLLocalDB

Authentication

Windows Authentication

Then click Connect.

After connecting:

Databases
    ↓
LibraryManagementSystemDb
    ↓
Tables

You should see tables such as:

AspNetUsers
AspNetRoles
AspNetUserRoles
Books
Categories
EBooks
LibrarySettings
LibraryStatuses
Notices
Reservations
Transactions

The exact table list may change if new migrations are added later.

▶️ Step 9 — Run the Website

In Visual Studio:

Build
→ Build Solution

Then run the project using:

Ctrl + F5

or press the green Run button.

The browser will open with a local HTTPS address similar to:

https://localhost:xxxx

The port number can be different on each computer.

👤 Step 10 — Create a Student or Faculty Account

From the website, open:

Register

Select:

Student

or:

Faculty

Then provide the required information and create the account.

Faculty University ID

University ID is optional for Faculty.

🔐 Admin Account

The application seeds the required roles and the default Admin account through the project's database initialization code.

Before using the Admin account, check:

LibraryManagementSystem/Data/DbInitializer.cs

for the configured Admin credentials.

⚠️ Do not use credentials from this README unless they are explicitly configured in DbInitializer.cs.

🗃️ Important: GitHub vs Database

GitHub stores the project source code and migrations.

Your local SQL Server database is not the same thing as the GitHub repository.

The setup works like this:

GitHub Repository
       ↓
     Clone
       ↓
ASP.NET Core Project
       ↓
Entity Framework Migrations
       ↓
Update-Database
       ↓
SQL Server LocalDB
       ↓
LibraryManagementSystemDb

Therefore, when a teammate clones the project, their existing local database data will not automatically appear.

For example, users, books, reservations, and transactions created on your computer are not automatically copied to another teammate's LocalDB.

The database structure is recreated through the migrations.

🔄 When New Database Changes Are Added

If a developer changes the database model and creates a new migration, teammates should pull the latest code:

git pull

Then run:

Update-Database

This applies the new migration to their local database.

🔁 Updating the Project from GitHub

Before starting work:

git pull

After making changes:

git add .
git commit -m "Describe your changes"
git push

Example:

git add .
git commit -m "Update student dashboard"
git push

📁 Important Project Structure

LibraryManagementSystem/
│
├── Areas/
│   └── Identity/
│       └── Pages/
│
├── Controllers/
│
├── Data/
│
├── Migrations/
│
├── Models/
│
├── Views/
│
├── wwwroot/
│
├── Program.cs
├── appsettings.json
└── LibraryManagementSystem.csproj

Main folders

Folder

Purpose

Controllers

Handles application requests and business flow

Models

Application/data models

Data

Database context, Identity user and initialization

Migrations

Entity Framework Core database migrations

Views

MVC user interface

Areas/Identity

Login, registration and Identity pages

wwwroot

CSS, JavaScript, images and other static files

🔧 Common Problems

Problem 1 — Database does not exist

Run:

Update-Database

Problem 2 — Cannot connect to LocalDB

Check that the server name is exactly:

(localdb)\MSSQLLocalDB

For SSMS, use:

Authentication: Windows Authentication

Problem 3 — Project does not build

Try:

Build
→ Rebuild Solution

Then run:

Ctrl + F5

Problem 4 — NuGet packages are missing

In Visual Studio:

Build
→ Rebuild Solution

Visual Studio should restore the packages defined in the .csproj file.

Problem 5 — Database schema is outdated

Pull the latest project:

git pull

Then run:

Update-Database

🔗 Useful Links

GitHub Repository

https://github.com/LabibaSarah/LibraryManagementSystem

SQL Server Management Studio

https://learn.microsoft.com/en-us/ssms/install/install

Visual Studio

https://visualstudio.microsoft.com/

✅ Quick Setup Checklist

For a new team member:

Install Visual Studio

Install ASP.NET and web development workload

Install .NET 10

Install SQL Server LocalDB

Install SSMS (recommended)

Clone the GitHub repository

Open LibraryManagementSystem.slnx

Restore/build the project

Check appsettings.json

Open Package Manager Console

Run Update-Database

Connect to (localdb)\MSSQLLocalDB in SSMS

Check LibraryManagementSystemDb

Run the application

Register/Login

Start working

👩‍💻 Project Repository

IUBAT Central Library Management System

GitHub:
https://github.com/LabibaSarah/LibraryManagementSystem
