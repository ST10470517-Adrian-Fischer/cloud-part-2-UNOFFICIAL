using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Models;

public partial class Event
{
    [Key]
    [Column("EventID")]
    public int EventId { get; set; }
    [Column("EventName")]
    [StringLength(100)]
    [Unicode(false)]
    public string EventName { get; set; } = null!;

    [Column("Event_Location")]
    [StringLength(100)]
    [Unicode(false)]
    public string EventLocation { get; set; } = null!;

    [Column("startdate")]
    public DateOnly? Startdate { get; set; }

    [Column("enddate")]
    public DateOnly? Enddate { get; set; }

    [Column("VenueID")]
    public int? VenueId { get; set; }

    [InverseProperty("Event")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [ForeignKey("VenueId")]
    [InverseProperty("Events")]
    public virtual Venue? Venue { get; set; }
}
