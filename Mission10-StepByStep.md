# Mission #10 Step-by-Step Guide (React + ASP.NET API + Prettier)

This guide is now based on your **actual SQLite file** at:
- `backend/BowlingApi/data/BowlingLeague.sqlite`

From that DB, we know:
- Table `Bowlers` has bowler fields + `TeamID`
- Table `Teams` has `TeamID`, `TeamName`, `CaptainID`
- `Bowlers.TeamID` links to `Teams.TeamID`
- Team names include `Marlins` and `Sharks` (4 bowlers each, so filtered result should be 8 bowlers)

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
- A **table** component that displays bowlers
- Use both in `App`

---

## 2) Prerequisites

Install before starting:
- [.NET SDK](https://dotnet.microsoft.com/en-us/download)
- [Node.js LTS](https://nodejs.org/)
- Git
- VS Code or Rider

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

### 4.1 Add EF Core packages (SQLite)

```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

### 4.2 Add the database file

1. Download DB from assignment link.
2. Place it here:
   - `backend/BowlingApi/data/BowlingLeague.sqlite`

### 4.3 Add the connection string

In `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "BowlingConnection": "Data Source=data/BowlingLeague.sqlite"
  }
}
```

### 4.4 Create models + DbContext from actual schema

Create folder `Models` and add these files.

`Models/Bowler.cs`

```csharp
namespace BowlingApi.Models;

public class Bowler
{
    public int BowlerID { get; set; }
    public string? BowlerLastName { get; set; }
    public string? BowlerFirstName { get; set; }
    public string? BowlerMiddleInit { get; set; }
    public string? BowlerAddress { get; set; }
    public string? BowlerCity { get; set; }
    public string? BowlerState { get; set; }
    public string? BowlerZip { get; set; }
    public string? BowlerPhoneNumber { get; set; }
    public int? TeamID { get; set; }

    public Team? Team { get; set; }
}
```

`Models/Team.cs`

```csharp
namespace BowlingApi.Models;

public class Team
{
    public int TeamID { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public int? CaptainID { get; set; }

    public ICollection<Bowler> Bowlers { get; set; } = new List<Bowler>();
}
```

`Models/BowlingLeagueContext.cs`

```csharp
using Microsoft.EntityFrameworkCore;

namespace BowlingApi.Models;

public class BowlingLeagueContext : DbContext
{
    public BowlingLeagueContext(DbContextOptions<BowlingLeagueContext> options) : base(options)
    {
    }

    public DbSet<Bowler> Bowlers => Set<Bowler>();
    public DbSet<Team> Teams => Set<Team>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bowler>().ToTable("Bowlers").HasKey(b => b.BowlerID);
        modelBuilder.Entity<Team>().ToTable("Teams").HasKey(t => t.TeamID);

        modelBuilder.Entity<Bowler>()
            .HasOne(b => b.Team)
            .WithMany(t => t.Bowlers)
            .HasForeignKey(b => b.TeamID);
    }
}
```

Now replace `Program.cs` with:

```csharp
using BowlingApi.Models;
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

### 4.5 Create DTO and API endpoint

Create file `Models/BowlerDto.cs`:

```csharp
namespace BowlingApi.Models;

public class BowlerDto
{
    public string? BowlerFirstName { get; set; }
    public string? BowlerMiddleInit { get; set; }
    public string? BowlerLastName { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public string? BowlerAddress { get; set; }
    public string? BowlerCity { get; set; }
    public string? BowlerState { get; set; }
    public string? BowlerZip { get; set; }
    public string? BowlerPhoneNumber { get; set; }
}
```

Create folder `Controllers` and add `Controllers/BowlersController.cs`:

```csharp
using BowlingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BowlingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BowlersController : ControllerBase
{
    private readonly BowlingLeagueContext _context;

    public BowlersController(BowlingLeagueContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BowlerDto>>> GetBowlers()
    {
        var bowlers = await _context.Bowlers
            .Include(b => b.Team)
            .Where(b => b.Team != null &&
                        (b.Team.TeamName == "Marlins" || b.Team.TeamName == "Sharks"))
            .OrderBy(b => b.Team!.TeamName)
            .ThenBy(b => b.BowlerLastName)
            .Select(b => new BowlerDto
            {
                BowlerFirstName = b.BowlerFirstName,
                BowlerMiddleInit = b.BowlerMiddleInit,
                BowlerLastName = b.BowlerLastName,
                TeamName = b.Team!.TeamName,
                BowlerAddress = b.BowlerAddress,
                BowlerCity = b.BowlerCity,
                BowlerState = b.BowlerState,
                BowlerZip = b.BowlerZip,
                BowlerPhoneNumber = b.BowlerPhoneNumber
            })
            .ToListAsync();

        return Ok(bowlers);
    }
}
```

### 4.6 Run and verify backend

```bash
dotnet run
```

From your current project launch settings, API URLs are:
- `http://localhost:5283`
- `https://localhost:7209`

Test endpoint:

```bash
curl http://localhost:5283/api/bowlers
```

Expected:
- JSON array
- 8 records (4 Marlins + 4 Sharks)

---

## 5) Create React app (frontend)

In a second terminal from assignment root:

```bash
cd frontend
npm create vite@latest bowling-client -- --template react
cd bowling-client
npm install
npm install axios
```

### 5.1 Create Heading component

`src/components/Heading.jsx`

```jsx
function Heading() {
  return (
    <header>
      <h1>Bowling League Bowlers</h1>
      <p>Marlins and Sharks roster from the Bowling League database</p>
    </header>
  );
}

export default Heading;
```

### 5.2 Create BowlerTable component

`src/components/BowlerTable.jsx`

```jsx
import { useEffect, useState } from 'react';
import axios from 'axios';

function BowlerTable() {
  const [bowlers, setBowlers] = useState([]);

  useEffect(() => {
    axios
      .get('http://localhost:5283/api/bowlers')
      .then((res) => setBowlers(res.data))
      .catch((err) => console.error('Error loading bowlers:', err));
  }, []);

  return (
    <table>
      <thead>
        <tr>
          <th>Name</th>
          <th>Team</th>
          <th>Address</th>
          <th>City</th>
          <th>State</th>
          <th>Zip</th>
          <th>Phone</th>
        </tr>
      </thead>
      <tbody>
        {bowlers.map((b, i) => (
          <tr key={i}>
            <td>{`${b.bowlerFirstName ?? ''} ${b.bowlerMiddleInit ?? ''} ${b.bowlerLastName ?? ''}`.replace(/\s+/g, ' ').trim()}</td>
            <td>{b.teamName}</td>
            <td>{b.bowlerAddress}</td>
            <td>{b.bowlerCity}</td>
            <td>{b.bowlerState}</td>
            <td>{b.bowlerZip}</td>
            <td>{b.bowlerPhoneNumber}</td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}

export default BowlerTable;
```

### 5.3 Use both components in App

`src/App.jsx`

```jsx
import Heading from './components/Heading';
import BowlerTable from './components/BowlerTable';
import './App.css';

function App() {
  return (
    <>
      <Heading />
      <BowlerTable />
    </>
  );
}

export default App;
```

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

(Optional VS Code settings):

```json
{
  "editor.defaultFormatter": "esbenp.prettier-vscode",
  "editor.formatOnSave": true
}
```

---

## 7) Connect frontend to backend (exact for this project)

Use this URL in React:
- `http://localhost:5283/api/bowlers`

If you prefer HTTPS, use:
- `https://localhost:7209/api/bowlers`

Make sure backend is running before starting frontend.

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

Open Vite URL (usually `http://localhost:5173`).

Verify:
- Heading component renders
- Table renders data from API
- Only Marlins/Sharks bowlers appear
- All required fields are shown

---

## 9) Final check against assignment

Confirm all are true:
- ASP.NET API created and returns bowler data
- React consumes API and displays table
- Heading component exists and is used in App
- Table component exists and is used in App
- Only Marlins/Sharks are shown
- Prettier installed and formatting works

---

## 10) GitHub submission

From assignment root:

```bash
git add .
git commit -m "Mission 10 complete"
git branch -M master
git remote add origin <your-public-repo-url>
git push -u origin master
```

Submit your **public GitHub repo link** in Learning Suite.

---

## Common issues

- CORS error: verify `AllowReact` policy is enabled and React runs at `http://localhost:5173`.
- `no such table` errors: confirm DB file path is exactly `data/BowlingLeague.sqlite`.
- Empty API list: verify filter is exactly `Marlins` or `Sharks`.
- HTTPS warnings: use the HTTP backend URL while developing.
