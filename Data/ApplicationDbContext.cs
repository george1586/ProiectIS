using Microsoft.EntityFrameworkCore;
using ProjIS.Models;

namespace ProjIS.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Airline> Airlines => Set<Airline>();
    public DbSet<AirportStaff> AirportStaff => Set<AirportStaff>();
    public DbSet<Flight> Flights => Set<Flight>();
    public DbSet<Discount> Discounts => Set<Discount>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<FlightType>(name: "flight_type");
        modelBuilder.HasPostgresEnum<FlightClass>(name: "flight_class");
        modelBuilder.HasPostgresEnum<PaymentMethod>(name: "payment_method");
        modelBuilder.HasPostgresEnum<PaymentStatus>(name: "payment_status");

        modelBuilder.Entity<Airline>(e =>
        {
            e.ToTable("airlines");
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Name).HasColumnName("name");
            e.Property(x => x.PasswordHash).HasColumnName("password_hash");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
        });

        modelBuilder.Entity<AirportStaff>(e =>
        {
            e.ToTable("airport_staff");
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.PersonalCode).HasColumnName("personal_code");
            e.Property(x => x.FullName).HasColumnName("full_name");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
        });

        modelBuilder.Entity<Flight>(e =>
        {
            e.ToTable("flights");
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.RouteCode).HasColumnName("route_code");
            e.Property(x => x.PlaneModel).HasColumnName("plane_model");
            e.Property(x => x.DepartureCity).HasColumnName("departure_city");
            e.Property(x => x.DestinationCity).HasColumnName("destination_city");

            e.Property(x => x.EconomySeats).HasColumnName("economy_seats");
            e.Property(x => x.BusinessSeats).HasColumnName("business_seats");
            e.Property(x => x.FirstClassSeats).HasColumnName("first_class_seats");

            e.Property(x => x.EconomyPrice).HasColumnName("economy_price");
            e.Property(x => x.BusinessPrice).HasColumnName("business_price");
            e.Property(x => x.FirstClassPrice).HasColumnName("first_class_price");

            e.Property(x => x.FlightType)
    .HasColumnName("flight_type")
    .HasColumnType("flight_type");
            e.Property(x => x.DepartureTime).HasColumnName("departure_time");
            e.Property(x => x.DaysOfWeek).HasColumnName("days_of_week");
            e.Property(x => x.SeasonStart).HasColumnName("season_start");
            e.Property(x => x.SeasonEnd).HasColumnName("season_end");

            e.Property(x => x.AirlineId).HasColumnName("airline_id");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");

            e.HasOne(x => x.Airline)
                .WithMany(a => a.Flights)
                .HasForeignKey(x => x.AirlineId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Discount>(e =>
        {
            e.ToTable("discounts");
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Type).HasColumnName("type");
            e.Property(x => x.Percentage).HasColumnName("percentage");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<Reservation>(e =>
        {
            e.ToTable("reservations");
            e.Property(x => x.Id).HasColumnName("id");

            e.Property(x => x.PassengerName).HasColumnName("passenger_name");
            e.Property(x => x.Phone).HasColumnName("phone");
            e.Property(x => x.AdultsCount).HasColumnName("adults_count");
            e.Property(x => x.ChildrenCount).HasColumnName("children_count");
            e.Property(x => x.SeniorsCount).HasColumnName("seniors_count");

            e.Property(x => x.OutboundFlightId).HasColumnName("outbound_flight_id");
            e.Property(x => x.OutboundDate).HasColumnName("outbound_date");
            e.Property(x => x.OutboundClass)
     .HasColumnName("outbound_class")
     .HasColumnType("flight_class");

            e.Property(x => x.ReturnFlightId).HasColumnName("return_flight_id");
            e.Property(x => x.ReturnDate).HasColumnName("return_date");

            e.Property(x => x.ReturnClass)
                .HasColumnName("return_class")
                .HasColumnType("flight_class");

            e.Property(x => x.HasMeal).HasColumnName("has_meal");
            e.Property(x => x.HasExtraLuggage).HasColumnName("has_extra_luggage");

            e.Property(x => x.TotalPrice).HasColumnName("total_price");
            e.Property(x => x.PaymentMethod)
    .HasColumnName("payment_method")
    .HasColumnType("payment_method");

            e.Property(x => x.PaymentStatus)
    .HasColumnName("payment_status")
    .HasColumnType("payment_status");
            e.Property(x => x.ValidatedByStaff).HasColumnName("validated_by_staff");
            e.Property(x => x.ValidatedAt).HasColumnName("validated_at");

            e.Property(x => x.CreatedAt).HasColumnName("created_at");

            e.HasOne(x => x.OutboundFlight)
                .WithMany()
                .HasForeignKey(x => x.OutboundFlightId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.ReturnFlight)
                .WithMany()
                .HasForeignKey(x => x.ReturnFlightId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.ValidatedByStaffNavigation)
                .WithMany()
                .HasForeignKey(x => x.ValidatedByStaff)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}