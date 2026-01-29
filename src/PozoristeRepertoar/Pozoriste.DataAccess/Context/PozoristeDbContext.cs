using Microsoft.EntityFrameworkCore;
using Pozoriste.Models.Entities;

namespace Pozoriste.DataAccess.Context
{
    public class PozoristeDbContext : DbContext
    {
        public PozoristeDbContext(DbContextOptions<PozoristeDbContext> options)
            : base(options)
        {
        }

        public DbSet<Zanr> Zanrovi { get; set; } = default!;
        public DbSet<Sala> Sale { get; set; } = default!;
        public DbSet<Predstava> Predstave { get; set; } = default!;
        public DbSet<Termin> Termini { get; set; } = default!;
        public DbSet<Rezervacija> Rezervacije { get; set; } = default!;
        public DbSet<RezervacijaSediste> RezervacijaSedista { get; set; } = default!;

        public DbSet<Glumac> Glumci { get; set; } = default!;
        public DbSet<PredstavaGlumac> PredstavaGlumci { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tabele
            modelBuilder.Entity<Zanr>().ToTable("Zanr");
            modelBuilder.Entity<Sala>().ToTable("Sala");
            modelBuilder.Entity<Predstava>().ToTable("Predstava");
            modelBuilder.Entity<Termin>().ToTable("Termin");
            modelBuilder.Entity<Rezervacija>().ToTable("Rezervacija");
            modelBuilder.Entity<RezervacijaSediste>().ToTable("RezervacijaSediste");
            modelBuilder.Entity<Glumac>().ToTable("Glumci");
            modelBuilder.Entity<PredstavaGlumac>().ToTable("PredstavaGlumci");

            // Cena - preciznost (Predstava)
            modelBuilder.Entity<Predstava>()
                .Property(p => p.Cena)
                .HasColumnType("decimal(18,2)");

            // UkupnaCena (Rezervacija)
            modelBuilder.Entity<Rezervacija>()
                .Property(x => x.UkupnaCena)
                .HasPrecision(18, 2);

            // Cena (RezervacijaSediste)
            modelBuilder.Entity<RezervacijaSediste>()
                .Property(x => x.Cena)
                .HasPrecision(18, 2);

            // Relacija Rezervacija -> Termin
            modelBuilder.Entity<Rezervacija>()
                .HasOne(r => r.Termin)
                .WithMany()
                .HasForeignKey(r => r.TerminId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacija RezervacijaSediste -> Rezervacija
            modelBuilder.Entity<RezervacijaSediste>()
                .HasOne(rs => rs.Rezervacija)
                .WithMany(r => r.Sedista)
                .HasForeignKey(rs => rs.RezervacijaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacija RezervacijaSediste -> Termin
            modelBuilder.Entity<RezervacijaSediste>()
                .HasOne(rs => rs.Termin)
                .WithMany()
                .HasForeignKey(rs => rs.TerminId)
                .OnDelete(DeleteBehavior.NoAction);

            // Jedinstveno sedište po terminu (TerminId + Red + Broj)
            modelBuilder.Entity<RezervacijaSediste>()
                .HasIndex(rs => new { rs.TerminId, rs.Red, rs.Broj })
                .IsUnique();

            // ===== Glumci / PredstavaGlumci (Many-to-Many preko join entiteta) =====

            modelBuilder.Entity<PredstavaGlumac>()
                .HasKey(x => new { x.PredstavaId, x.GlumacId });

            modelBuilder.Entity<PredstavaGlumac>()
                .HasOne(x => x.Predstava)
                .WithMany(p => p.Glumci)
                .HasForeignKey(x => x.PredstavaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PredstavaGlumac>()
                .HasOne(x => x.Glumac)
                .WithMany(g => g.Predstave)
                .HasForeignKey(x => x.GlumacId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Glumac>()
                .Property(x => x.PunoIme)
                .HasMaxLength(120)
                .IsRequired();

            
            modelBuilder.Entity<Glumac>()
                .HasIndex(x => x.PunoIme)
                .IsUnique();
        }
    }
}
