using System;
using System.Collections.Generic;

namespace BowlingApi.Models;

// Represents one row in the Teams table.
public partial class Team
{
    // Primary key for each team.
    public int TeamID { get; set; }

    // Human-readable team name (Marlins, Sharks, etc.).
    public string TeamName { get; set; } = null!;

    // Optional reference to the captain bowler ID in this schema.
    public int? CaptainID { get; set; }

    // Navigation collection for all bowlers assigned to this team.
    public virtual ICollection<Bowler> Bowlers { get; set; } = new List<Bowler>();
}
