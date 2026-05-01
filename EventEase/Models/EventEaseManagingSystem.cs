using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Models;

public partial class EventEaseManagingSystem : DbContext
{
    public EventEaseManagingSystem()
    {
    }

    public EventEaseManagingSystem(DbContextOptions<EventEaseManagingSystem> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Venue> Venues { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog='EventEaseManagingSystem';Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Bookingid).HasName("PK__Booking__C6D3070532031F57");

            entity.HasOne(d => d.Event).WithMany(p => p.Bookings).HasConstraintName("FK__Booking__EventID__5070F446");

            entity.HasOne(d => d.Venue).WithMany(p => p.Bookings).HasConstraintName("FK__Booking__VenueID__4F7CD00D");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Events__7944C8709FFE9E26");

            entity.HasOne(d => d.Venue).WithMany(p => p.Events).HasConstraintName("FK__Events__VenueID__4CA06362");
        });

        modelBuilder.Entity<Venue>(entity =>
        {
            entity.HasKey(e => e.VenueId).HasName("PK__Venue__3C57E5D29E2AD7AB");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
