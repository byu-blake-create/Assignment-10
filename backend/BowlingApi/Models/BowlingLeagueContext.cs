using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BowlingApi.Models;

public partial class BowlingLeagueContext : DbContext
{
    public BowlingLeagueContext(DbContextOptions<BowlingLeagueContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bowler> Bowlers { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bowler>(entity =>
        {
            entity.HasIndex(e => e.BowlerLastName, "BowlerLastName");

            entity.HasIndex(e => e.TeamID, "BowlersTeamID");

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

            entity.HasOne(d => d.Team).WithMany(p => p.Bowlers).HasForeignKey(d => d.TeamID);
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasIndex(e => e.TeamID, "TeamID").IsUnique();

            entity.Property(e => e.TeamID).HasColumnType("INT");
            entity.Property(e => e.CaptainID).HasColumnType("INT");
            entity.Property(e => e.TeamName).HasColumnType("nvarchar (50)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
