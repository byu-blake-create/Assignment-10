using BowlingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BowlingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BowlersController : ControllerBase
{
    private readonly BowlingLeagueContext _context;

    // Inject the database context so this controller can query bowlers and teams.
    public BowlersController(BowlingLeagueContext context)
    {
        _context = context;
    }

    // GET /api/bowlers returns only Marlins and Sharks with assignment-required fields.
    [HttpGet]
    public async Task<IActionResult> GetBowlers()
    {
        var bowlers = await _context.Bowlers
            // Join each bowler to their Team row so TeamName is available.
            .Include(b => b.Team)
            // Assignment filter: only include bowlers from Marlins or Sharks.
            .Where(b => b.Team != null &&
                        (b.Team.TeamName == "Marlins" || b.Team.TeamName == "Sharks"))
            // Sort output for a stable, predictable table.
            .OrderBy(b => b.Team!.TeamName)
            .ThenBy(b => b.BowlerLastName)
            // Return only the fields needed by the frontend table.
            .Select(b => new
            {
                b.BowlerFirstName,
                b.BowlerMiddleInit,
                b.BowlerLastName,
                TeamName = b.Team!.TeamName,
                b.BowlerAddress,
                b.BowlerCity,
                b.BowlerState,
                b.BowlerZip,
                b.BowlerPhoneNumber
            })
            .ToListAsync();

        // Send the filtered/projected result as JSON.
        return Ok(bowlers);
    }
}
