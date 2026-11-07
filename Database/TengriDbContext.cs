using Microsoft.EntityFrameworkCore;

namespace TengriTrails.Database;

public sealed class TengriDbContext(DbContextOptions<TengriDbContext> options) : DbContext(options)
{
    public DbSet<RegionEntity> Regions => Set<RegionEntity>();
    public DbSet<TourEntity> Tours => Set<TourEntity>();
    public DbSet<TourTranslationEntity> TourTranslations => Set<TourTranslationEntity>();
    public DbSet<TourPhotoEntity> TourPhotos => Set<TourPhotoEntity>();
    public DbSet<DepartureEntity> Departures => Set<DepartureEntity>();
    public DbSet<BookingEntity> Bookings => Set<BookingEntity>();
    public DbSet<PaymentEntity> Payments => Set<PaymentEntity>();
    public DbSet<BookingStatusLogEntity> BookingStatusLogs => Set<BookingStatusLogEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RegionEntity>(entity =>
        {
            entity.HasIndex(region => region.Code).IsUnique();
            entity.Property(region => region.Code).HasMaxLength(40);
        });

        modelBuilder.Entity<TourEntity>(entity =>
        {
            entity.HasOne(tour => tour.Region).WithMany(region => region.Tours)
                .HasForeignKey(tour => tour.RegionId).OnDelete(DeleteBehavior.Restrict);
            entity.Property(tour => tour.PriceFromKgs).HasPrecision(18, 2);
            entity.Property(tour => tour.Latitude).HasPrecision(9, 6);
            entity.Property(tour => tour.Longitude).HasPrecision(9, 6);
        });

        modelBuilder.Entity<TourTranslationEntity>(entity =>
        {
            entity.HasKey(translation => new { translation.TourId, translation.Language });
            entity.HasIndex(translation => new { translation.Language, translation.Slug }).IsUnique();
            entity.Property(translation => translation.Language).HasMaxLength(2);
            entity.Property(translation => translation.Slug).HasMaxLength(160);
            entity.HasOne(translation => translation.Tour).WithMany(tour => tour.Translations)
                .HasForeignKey(translation => translation.TourId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TourPhotoEntity>(entity =>
        {
            entity.HasIndex(photo => new { photo.TourId, photo.SortOrder }).IsUnique();
            entity.HasOne(photo => photo.Tour).WithMany(tour => tour.Photos)
                .HasForeignKey(photo => photo.TourId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DepartureEntity>(entity =>
        {
            entity.Property(departure => departure.PricePerPersonKgs).HasPrecision(18, 2);
            entity.HasOne(departure => departure.Tour).WithMany(tour => tour.Departures)
                .HasForeignKey(departure => departure.TourId).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(departure => new { departure.TourId, departure.StartAtUtc });
        });

        modelBuilder.Entity<BookingEntity>(entity =>
        {
            entity.HasIndex(booking => booking.BookingNumber).IsUnique();
            entity.Property(booking => booking.TotalAmountKgs).HasPrecision(18, 2);
            entity.Property(booking => booking.Status).HasConversion<string>().HasMaxLength(20);
            entity.HasOne(booking => booking.Departure).WithMany(departure => departure.Bookings)
                .HasForeignKey(booking => booking.DepartureId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PaymentEntity>(entity =>
        {
            entity.Property(payment => payment.Amount).HasPrecision(18, 2);
            entity.Property(payment => payment.Status).HasConversion<string>().HasMaxLength(20);
            entity.HasOne(payment => payment.Booking).WithMany(booking => booking.Payments)
                .HasForeignKey(payment => payment.BookingId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BookingStatusLogEntity>(entity =>
        {
            entity.Property(log => log.OldStatus).HasConversion<string>().HasMaxLength(20);
            entity.Property(log => log.NewStatus).HasConversion<string>().HasMaxLength(20);
            entity.HasOne(log => log.Booking).WithMany(booking => booking.StatusLogs)
                .HasForeignKey(log => log.BookingId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
