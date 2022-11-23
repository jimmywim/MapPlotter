using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MapPlotter.Data;

public partial class ResidencesContext : DbContext
{
    public ResidencesContext()
    {
    }

    public ResidencesContext(DbContextOptions<ResidencesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Residence> Residences { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("DataSource=Data.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Residence>(entity =>
        {
            entity.HasKey(e => e.PrimaryKey);

            entity.Property(e => e.VrannualValue).HasColumnName("VRAnnualValue£");
            entity.Property(e => e.VrannualValued).HasColumnName("VRAnnualValued.");
            entity.Property(e => e.VrannualValues).HasColumnName("VRAnnualValues.");
            entity.Property(e => e.Vrdescription).HasColumnName("VRDescription");
            entity.Property(e => e.VrfeuDuty).HasColumnName("VRFeuDuty£");
            entity.Property(e => e.VrfeuDutyd).HasColumnName("VRFeuDutyd.");
            entity.Property(e => e.VrfeuDutys).HasColumnName("VRFeuDutys.");
            entity.Property(e => e.VrnotRated).HasColumnName("VRNotRated");
            entity.Property(e => e.Vrnumber).HasColumnName("VRNumber");
            entity.Property(e => e.Vroccupier).HasColumnName("VROccupier");
            entity.Property(e => e.Vrproprietor).HasColumnName("VRProprietor");
            entity.Property(e => e.Vrtenant).HasColumnName("VRTenant");
            entity.Property(e => e.VryearlyRent).HasColumnName("VRYearlyRent£");
            entity.Property(e => e.VryearlyRentd).HasColumnName("VRYearlyRentd.");
            entity.Property(e => e.VryearlyRents).HasColumnName("VRYearlyRents.");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
