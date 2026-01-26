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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Zanr>().ToTable("Zanr");
            modelBuilder.Entity<Sala>().ToTable("Sala");
            modelBuilder.Entity<Predstava>().ToTable("Predstava");
            modelBuilder.Entity<Termin>().ToTable("Termin");
            modelBuilder.Entity<Rezervacija>().ToTable("Rezervacija");
            modelBuilder.Entity<RezervacijaSediste>().ToTable("RezervacijaSediste");

            // Cena - preciznost
            modelBuilder.Entity<Predstava>()
                .Property(p => p.Cena)
                .HasColumnType("decimal(18,2)");

            // Relacija Rezervacija -> Termin
            modelBuilder.Entity<Rezervacija>()
                .HasOne(r => r.Termin)
                .WithMany()
                .HasForeignKey(r => r.TerminId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RezervacijaSediste>()
                .HasOne(rs => rs.Rezervacija)
                .WithMany(r => r.Sedista)
                .HasForeignKey(rs => rs.RezervacijaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RezervacijaSediste>()
                .HasOne(rs => rs.Termin)
                .WithMany()
                .HasForeignKey(rs => rs.TerminId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<RezervacijaSediste>()
                .HasIndex(rs => new { rs.TerminId, rs.Red, rs.Broj })
                .IsUnique();

            // Ako property u klasi jeste KorisnikId, NEMA potrebe za mapiranjem imena kolone.
            // (Samo pazi da ti i tabela ima kolonu KorisnikId.)
        }
    }
}
