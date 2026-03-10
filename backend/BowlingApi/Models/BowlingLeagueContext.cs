using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BowlingApi.Models;

// EF Core database context for the BowlingLeague.sqlite schema.
public partial class BowlingLeagueContext : DbContext
{
    // Accept configured options from Program.cs (connection string, provider, etc.).
    public BowlingLeagueContext(DbContextOptions<BowlingLeagueContext> options)
        : base(options)
    {
    }

    // Table accessors used in LINQ queries.
    public virtual DbSet<Bowler> Bowlers { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    // Configure table mappings, column types, indexes, and relationships.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bowler>(entity =>
        {
            // DB indexes that already exist in the SQLite file.
            entity.HasIndex(e => e.BowlerLastName, "BowlerLastName");

            entity.HasIndex(e => e.TeamID, "BowlersTeamID");

            // Match EF property types to the existing DB column types.
            entity.Property(e => e.BowlerID).HasColumnType("INT");
            entity.Property(e => e.BowlerAddress).HasColumnType("nvarchar (50)");
            entity.Property(e => e.BowlerCity).HasColumnType("nvarchar (50)");
            entity.Property(e => e.BowlerFirstName).HasColumnType("nvarchar (50)");
            entity.Property(e => e.BowlerLastName).HasColumnType("nvarchar (50)");
            entity.Property(e => e.BowlerMiddleInit).HasColumnType("nvarchar (1)");
            entity.Property(e => e.BowlerPhoneNumber).HasColumnType("nvarchar (14)");
            entity.Property(e => e.BowlerState).HasColumnType("nvarchar (2)");
            entity.Property(e => e.BowlerZip).HasColumnType("nvarchar (10)");
            entity.Property(e => e.TeamID).HasColumnType("INT");

            // Set Bowler.TeamID -> Team.TeamID relationship.
            entity.HasOne(d => d.Team).WithMany(p => p.Bowlers).HasForeignKey(d => d.TeamID);
        });

        modelBuilder.Entity<Team>(entity =>
        {
            // TeamID is unique in the source database.
            entity.HasIndex(e => e.TeamID, "TeamID").IsUnique();

            // Keep team mappings aligned with existing schema types.
            entity.Property(e => e.TeamID).HasColumnType("INT");
            entity.Property(e => e.CaptainID).HasColumnType("INT");
            entity.Property(e => e.TeamName).HasColumnType("nvarchar (50)");
        });

        // Allows extending mappings in another partial class if needed.
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
