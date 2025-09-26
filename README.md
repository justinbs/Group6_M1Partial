**Group6_M1Partial – Item Manager**

An ASP.NET Core Web API (.NET 8) with a WinForms client for basic Item CRUD.
Tech used: C#, ASP.NET Web API, EF Core (SQL Server LocalDB), Dependency Injection, Swagger.

**Tech Stack**
Backend: ASP.NET Core Web API (.NET 8), EF Core, SQL Server LocalDB
Client: Windows Forms (.NET)
Patterns: DI, repository/service pattern (lightweight)
Dev tools: Visual Studio 2022, Swagger UI


***********************
## How to Run (Visual Studio 2022)

### Prerequisites
- Visual Studio 2022 with workloads:
  - **.NET desktop development**
  - **ASP.NET and web development**
- .NET 8 SDK
- SQL Server **LocalDB** (installed with VS workloads)

### 1) Clone the repo

2) Open the solution
Open Group6_M1Partial.sln in Visual Studio.

4) Restore & build
VS usually restores packages automatically.
If not: Right-click the solution → Restore NuGet Packages, then Build → Rebuild Solution.

4) Create the database (first run only)
Tools → NuGet Package Manager → Package Manager Console
Set Default project (dropdown) to Api
Run:
  Update-Database
// If you see "no migrations", run: //
  Add-Migration InitialCreate
  Update-Database

5) Ensure API URL and client URL match
Api/Properties/launchSettings.json should contain:
  "applicationUrl": "http://localhost:5238"
Client.WinForms/Services/ItemApi.cs should contain:
  private const string Base = "http://localhost:5238/api/items";

6) Run both projects together
Right-click the solution → Configure Startup Projects…
Choose Multiple startup projects:
  - Api → Start
  - Client.WinForms → Start
- Click OK, then press F5
Expected result:
- Swagger opens at http://localhost:5238/swagger
- WinForms app opens (Item Manager UI)

7) Quick test (WinForms)
Add an item:
Name: Gel Pen Blue
Code: GP-BLU
Brand: Pilot
Unit Price: 24.50

You should see it in the grid. Try Search, Update, and Delete.
***********************

**API**
Base URL: http://localhost:5238/api/items

Verb	  Route	           Description
GET	    /api/items	     List all
GET	    /api/items/{id}	 Get by id
POST	  /api/items	     Create
PUT	    /api/items/{id}	 Update
DELETE	/api/items/{id}	 Delete

Sample POST body
{ "name": "Pencil HB", "code": "P-HB", "brand": "Faber-Castell", "unitPrice": 12.5 }

**Project Structure**
Group6_M1Partial.sln
│
├─ Api/
│  ├─ Controllers/ItemsController.cs
│  ├─ Data/AppDbContext.cs
│  ├─ Migrations/
│  ├─ Models/Item.cs
│  ├─ Services/IItemService.cs, ItemService.cs
│  ├─ Program.cs
│  ├─ appsettings.json
│  └─ Properties/launchSettings.json
│
└─ Client.WinForms/
   ├─ Models/Item.cs
   ├─ Services/ItemApi.cs
   ├─ Form1.cs, Program.cs
   └─ (UI is built in code at runtime; no designer syncing needed)
