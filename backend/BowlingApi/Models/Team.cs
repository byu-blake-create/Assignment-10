using System;
using System.Collections.Generic;

namespace BowlingApi.Models;

public partial class Team
{
    public int TeamID { get; set; }

    public string TeamName { get; set; } = null!;

    public int? CaptainID { get; set; }

    public virtual ICollection<Bowler> Bowlers { get; set; } = new List<Bowler>();
}
