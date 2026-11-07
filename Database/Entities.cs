namespace TengriTrails.Database;

public sealed class RegionEntity
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public required string NameKy { get; set; }
    public required string NameRu { get; set; }
    public required string NameEn { get; set; }
    public required string NameEs { get; set; }
    public required string NameFr { get; set; }
    public required string NameZh { get; set; }
    public List<TourEntity> Tours { get; set; } = [];
}

public sealed class TourEntity
{
    public int Id { get; set; }
    public int RegionId { get; set; }
    public required string Type { get; set; }
    public int DurationDays { get; set; }
    public int Difficulty { get; set; }
    public decimal PriceFromKgs { get; set; }
    public int MinimumGroupSize { get; set; }
    public int MaximumGroupSize { get; set; }
    public bool IsPublished { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public RegionEntity Region { get; set; } = null!;
    public List<TourTranslationEntity> Translations { get; set; } = [];
    public List<TourPhotoEntity> Photos { get; set; } = [];
    public List<DepartureEntity> Departures { get; set; } = [];
}

public sealed class TourTranslationEntity
{
    public int TourId { get; set; }
    public required string Language { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public required string ShortDescription { get; set; }
    public required string FullDescription { get; set; }
    public required string ItineraryJson { get; set; }
    public required string IncludedJson { get; set; }
    public required string ExcludedJson { get; set; }
    public required string PackingJson { get; set; }
    public TourEntity Tour { get; set; } = null!;
}

public sealed class TourPhotoEntity
{
    public int Id { get; set; }
    public int TourId { get; set; }
    public required string FilePath { get; set; }
    public required string ThumbnailPath { get; set; }
    public int SortOrder { get; set; }
    public bool IsCover { get; set; }
    public string? Caption { get; set; }
    public TourEntity Tour { get; set; } = null!;
}

public sealed class DepartureEntity
{
    public required string Id { get; set; }
    public int TourId { get; set; }
    public DateTime StartAtUtc { get; set; }
    public required string MeetingPlace { get; set; }
    public decimal PricePerPersonKgs { get; set; }
    public int SeatsTotal { get; set; }
    public int SeatsTaken { get; set; }
    public string? Guide { get; set; }
    public TourEntity Tour { get; set; } = null!;
    public List<BookingEntity> Bookings { get; set; } = [];
}

public enum BookingStatus
{
    New,
    Paid,
    Confirmed,
    Completed,
    Cancelled,
    Refunded
}

public enum PaymentStatus
{
    Pending,
    Succeeded,
    Failed,
    Refunded
}

public sealed class BookingEntity
{
    public long Id { get; set; }
    public string? BookingNumber { get; set; }
    public required string DepartureId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public required string Citizenship { get; set; }
    public required string Messenger { get; set; }
    public int Adults { get; set; }
    public int Children { get; set; }
    public decimal TotalAmountKgs { get; set; }
    public required string RequestLanguage { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime ReservedUntilUtc { get; set; }
    public bool PersonalDataConsent { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public DepartureEntity Departure { get; set; } = null!;
    public List<PaymentEntity> Payments { get; set; } = [];
    public List<BookingStatusLogEntity> StatusLogs { get; set; } = [];
}

public sealed class PaymentEntity
{
    public long Id { get; set; }
    public long BookingId { get; set; }
    public string? GatewayPaymentId { get; set; }
    public decimal Amount { get; set; }
    public required string Currency { get; set; }
    public PaymentStatus Status { get; set; }
    public bool IsDeposit { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public BookingEntity Booking { get; set; } = null!;
}

public sealed class BookingStatusLogEntity
{
    public long Id { get; set; }
    public long BookingId { get; set; }
    public BookingStatus? OldStatus { get; set; }
    public BookingStatus NewStatus { get; set; }
    public required string ChangedBy { get; set; }
    public DateTime ChangedAtUtc { get; set; }
    public BookingEntity Booking { get; set; } = null!;
}
