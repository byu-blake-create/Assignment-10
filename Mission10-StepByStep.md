# Mission #10 Step-by-Step Guide (React + ASP.NET API + Prettier)

Use this checklist to build and submit the assignment exactly as required.

## 1) What you are building

Build a React site that displays bowlers from the Bowling League database using an ASP.NET API.

Required displayed fields:
- Bowler Name (First, Middle, Last)
- Team Name
- Address
- City
- State
- Zip
- Phone Number

Important filter:
- Show **only bowlers on the Marlins or Sharks teams**.

Required React components:
- A **Heading** component (page description/title)
- A **table/list** component that displays bowlers
- Use both in `App`

---

## 2) Prerequisites

Install before starting:
- [.NET SDK 8](https://dotnet.microsoft.com/en-us/download)
- [Node.js LTS](https://nodejs.org/)
- Git
- VS Code or Rider (recommended)

Verify installs:

```bash
dotnet --version
node --version
npm --version
```

---

## 3) Create project folders

From your assignment folder:

```bash
mkdir backend frontend
```

---

## 4) Create the ASP.NET Web API (backend)

```bash
cd backend
dotnet new webapi -n BowlingApi
cd BowlingApi
```

### 4.1 Add EF Core packages

If your database is SQLite:

```bash
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
```

If your instructor database is SQL Server instead, use:

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
```

### 4.2 Add database file

1. Download database from assignment link.
2. Put it in the API project (example):
   - `backend/BowlingApi/Data/BowlingLeague.sqlite`

### 4.3 Add connection string

In `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "BowlingConnection": "Data Source=Data/BowlingLeague.sqlite"
  }
}
```

(If SQL Server, your connection string will look different.)

### 4.4 Create model + DbContext

Create models that match your DB tables (usually `Bowlers` and `Teams`) and relationships.

Suggested files:
- `Models/Bowler.cs`
- `Models/Team.cs`
- `Models/BowlingLeagueContext.cs`

In `Program.cs`, register context and CORS:

```csharp
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BowlingLeagueContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("BowlingConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReact");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

### 4.5 Create DTO and controller endpoint

Create DTO file (example):
- `Models/BowlerDto.cs`

Return only needed fields and filter to Marlins/Sharks in controller.

Example endpoint behavior:
- Route: `GET /api/bowlers`
- Query DB with team include/join
- Filter: `TeamName == "Marlins" || TeamName == "Sharks"`
- Project into DTO fields for React

Example LINQ idea:

```csharp
var bowlers = _context.Bowlers
    .Include(b => b.Team)
    .Where(b => b.Team.TeamName == "Marlins" || b.Team.TeamName == "Sharks")
    .Select(b => new BowlerDto
    {
        BowlerFirstName = b.BowlerFirstName,
        BowlerMiddleInit = b.BowlerMiddleInit,
        BowlerLastName = b.BowlerLastName,
        TeamName = b.Team.TeamName,
        BowlerAddress = b.BowlerAddress,
        BowlerCity = b.BowlerCity,
        BowlerState = b.BowlerState,
        BowlerZip = b.BowlerZip,
        BowlerPhoneNumber = b.BowlerPhoneNumber
    })
    .ToList();
```

### 4.6 Run backend

```bash
dotnet run
```

Confirm API works at:
- Swagger page (shown in terminal)
- `GET /api/bowlers` returns JSON

---

## 5) Create React app (frontend)

Open a second terminal in assignment root:

```bash
cd frontend
npm create vite@latest bowling-client -- --template react
cd bowling-client
npm install
npm install axios
```

### 5.1 Create components

Create:
- `src/components/Heading.jsx`
- `src/components/BowlerTable.jsx`

`Heading` should describe the page (example title/subtitle).

`BowlerTable` should:
- fetch from `http://localhost:xxxx/api/bowlers` (use your API port)
- store data in state
- render a table with required columns

Use components in `src/App.jsx`.

---

## 6) Set up Prettier

From `frontend/bowling-client`:

```bash
npm install -D prettier
```

Create `.prettierrc`:

```json
{
  "semi": true,
  "singleQuote": true,
  "tabWidth": 2,
  "trailingComma": "es5",
  "printWidth": 100
}
```

Create `.prettierignore`:

```txt
node_modules
dist
build
coverage
```

Update `package.json` scripts:

```json
"scripts": {
  "dev": "vite",
  "build": "vite build",
  "preview": "vite preview",
  "format": "prettier . --write",
  "format:check": "prettier . --check"
}
```

Run formatter:

```bash
npm run format
```

(Optional VS Code format-on-save settings):

```json
{
  "editor.defaultFormatter": "esbenp.prettier-vscode",
  "editor.formatOnSave": true
}
```

---

## 7) Connect frontend to backend

In `BowlerTable.jsx` use your backend URL.

If backend is, for example, `http://localhost:5000`:
- frontend request URL: `http://localhost:5000/api/bowlers`

If you run HTTPS in backend, use the HTTPS URL shown by `dotnet run`.

---

## 8) Run both apps

Terminal 1 (backend):

```bash
cd backend/BowlingApi
dotnet run
```

Terminal 2 (frontend):

```bash
cd frontend/bowling-client
npm run dev
```

Open the Vite URL (usually `http://localhost:5173`).

Verify:
- Page has heading component
- Table component shows bowlers
- Only Marlins and Sharks entries appear
- All required fields display

---

## 9) Final check against assignment

Before submitting, confirm all are true:
- ASP.NET API created and returns bowler data
- React consumes API and displays table
- Heading component exists and is used in App
- Table/list component exists and is used in App
- Only Marlins/Sharks shown
- Prettier installed and formatting script works

---

## 10) GitHub submission

From assignment root:

```bash
git init
git add .
git commit -m "Mission 10 complete"
git branch -M master
git remote add origin <your-public-repo-url>
git push -u origin master
```

Submit your **public GitHub repo link** in Learning Suite.

---

## Common issues

- CORS error: verify `AllowReact` policy and frontend origin port.
- API returns empty list: check team filter names exactly (`Marlins`, `Sharks`) and DB path.
- HTTPS certificate warning: use HTTP URL locally while developing.
- Wrong DB format: if not SQLite, switch provider/connection string to SQL Server.
