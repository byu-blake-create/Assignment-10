using System;
using System.Collections.Generic;

namespace BowlingApi.Models;

// Represents one row in the Bowlers table.
public partial class Bowler
{
    // Primary key for each bowler.
    public int BowlerID { get; set; }

    // Bowler name fields.
    public string? BowlerLastName { get; set; }

    public string? BowlerFirstName { get; set; }

    public string? BowlerMiddleInit { get; set; }

    // Contact/address fields required by the assignment output.
    public string? BowlerAddress { get; set; }

    public string? BowlerCity { get; set; }

    public string? BowlerState { get; set; }

    public string? BowlerZip { get; set; }

    public string? BowlerPhoneNumber { get; set; }

    // Foreign key to Teams.TeamID.
    public int? TeamID { get; set; }

    // Navigation property for the related team row.
    public virtual Team? Team { get; set; }
}
