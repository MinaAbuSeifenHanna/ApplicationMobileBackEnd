using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StayHub.Backend.Domain.Entities;

namespace StayHub.Backend.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<UnitReservation> UnitReservations => Set<UnitReservation>();
    public DbSet<UnitImage> UnitImages => Set<UnitImage>();
    public DbSet<Amenity> Amenities => Set<Amenity>();
    public DbSet<UnitAmenity> UnitAmenities => Set<UnitAmenity>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Wishlist> Wishlists => Set<Wishlist>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<AdminAction> AdminActions => Set<AdminAction>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Offer> Offers => Set<Offer>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<ClientRequest> ClientRequests => Set<ClientRequest>();
    public DbSet<Finance> Finances => Set<Finance>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Unit>(entity =>
        {
            entity.Property(u => u.PricePerNight)
                .HasColumnType("decimal(18,2)");

            entity.HasMany(u => u.UnitReservations)
                .WithOne(r => r.Unit)
                .HasForeignKey(r => r.UnitId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(u => u.OwnerUser)
                .WithMany(u => u.Units)
                .HasForeignKey(u => u.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<UnitReservation>(entity =>
        {
            entity.HasKey(r => r.Id);
            
            entity.Property(r => r.BasePrice).HasColumnType("decimal(18,2)");
            entity.Property(r => r.ServiceFee).HasColumnType("decimal(18,2)");
            entity.Property(r => r.Taxes).HasColumnType("decimal(18,2)");
            entity.Property(r => r.TotalPrice).HasColumnType("decimal(18,2)");

            entity.HasOne(r => r.Unit)
                .WithMany(u => u.UnitReservations)
                .HasForeignKey(r => r.UnitId);

            entity.HasOne(r => r.Guest)
                .WithMany()
                .HasForeignKey(r => r.GuestId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Reservation>()
            .Property(r => r.TotalPrice)
            .HasColumnType("decimal(18,2)");

        builder.Entity<Reservation>()
            .Property(r => r.Deposit)
            .HasColumnType("decimal(18,2)");

        builder.Entity<UnitImage>()
            .HasOne(ui => ui.Unit)
            .WithMany(u => u.Images)
            .HasForeignKey(ui => ui.UnitId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Reservation>()
            .HasOne(r => r.Unit)
            .WithMany(u => u.Reservations)
            .HasForeignKey(r => r.UnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Review>()
            .HasOne(r => r.Reservation)
            .WithMany(r => r.Reviews)
            .HasForeignKey(r => r.ReservationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UnitAmenity>()
            .HasKey(ua => new { ua.UnitId, ua.AmenityId });

        builder.Entity<UnitAmenity>()
            .HasOne(ua => ua.Unit)
            .WithMany(u => u.UnitAmenities)
            .HasForeignKey(ua => ua.UnitId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UnitAmenity>()
            .HasOne(ua => ua.Amenity)
            .WithMany(a => a.UnitAmenities)
            .HasForeignKey(ua => ua.AmenityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Wishlist>()
            .HasIndex(w => new { w.UserId, w.UnitId })
            .IsUnique();

        builder.Entity<Wishlist>()
            .HasOne(w => w.User)
            .WithMany(u => u.Wishlists)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Wishlist>()
            .HasOne(w => w.Unit)
            .WithMany(u => u.Wishlists)
            .HasForeignKey(w => w.UnitId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Report>(entity =>
        {
            entity.HasOne(r => r.Reporter)
                .WithMany(u => u.ReportsSubmitted)
                .HasForeignKey(r => r.ReporterId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.TargetUser)
                .WithMany(u => u.ReportsAgainstMe)
                .HasForeignKey(r => r.TargetUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.Unit)
                .WithMany(u => u.Reports)
                .HasForeignKey(r => r.UnitId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<AdminAction>(entity =>
        {
            entity.HasOne(aa => aa.Report)
                .WithMany(r => r.AdminActions)
                .HasForeignKey(aa => aa.ReportId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(aa => aa.AdminUser)
                .WithMany(u => u.AdminActionsPerformed)
                .HasForeignKey(aa => aa.AdminId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Payment>(entity =>
        {
            entity.Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            entity.HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.Reservation)
                .WithMany(r => r.Payments)
                .HasForeignKey(p => p.ReservationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Location>(entity =>
        {
            entity.Property(l => l.Latitude)
                .HasColumnType("decimal(18,10)");

            entity.Property(l => l.Longitude)
                .HasColumnType("decimal(18,10)");

            entity.HasOne(l => l.Unit)
                .WithOne(u => u.Location)
                .HasForeignKey<Location>(l => l.UnitId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Offer>(entity =>
        {
            entity.Property(o => o.DiscountPercentage)
                .HasColumnType("decimal(18,2)");

            entity.HasOne(o => o.Unit)
                .WithMany(u => u.Offers)
                .HasForeignKey(o => o.UnitId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Notification>(entity =>
        {
            entity.HasIndex(n => new { n.UserId, n.CreatedAt });

            entity.HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ClientRequest>(entity =>
        {
            entity.Property(cr => cr.MinPrice)
                .HasColumnType("decimal(18,2)");

            entity.Property(cr => cr.MaxPrice)
                .HasColumnType("decimal(18,2)");

            entity.HasOne(cr => cr.User)
                .WithMany(u => u.ClientRequests)
                .HasForeignKey(cr => cr.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Finance>(entity =>
        {
            entity.Property(f => f.Amount)
                .HasColumnType("decimal(18,2)");

            entity.HasOne(f => f.User)
                .WithMany(u => u.Finances)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
