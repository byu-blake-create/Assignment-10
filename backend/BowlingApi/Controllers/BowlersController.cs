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
    public async Task<IActionResult> GetBowlers()
    {
        var bowlers = await _context.Bowlers
            .Include(b => b.Team)
            .Where(b => b.Team != null &&
                        (b.Team.TeamName == "Marlins" || b.Team.TeamName == "Sharks"))
            .OrderBy(b => b.Team!.TeamName)
            .ThenBy(b => b.BowlerLastName)
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

        return Ok(bowlers);
    }
}
