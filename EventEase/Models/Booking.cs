using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Models;

[Table("Booking")]
public partial class Booking
{
    [Key]
    [Column("bookingid")]
    public int Bookingid { get; set; }

    [Column("VenueID")]
    public int? VenueId { get; set; }

    [Column("EventID")]
    public int? EventId { get; set; }

    [ForeignKey("EventId")]
    [InverseProperty("Bookings")]
    public virtual Event? Event { get; set; }

    [ForeignKey("VenueId")]
    [InverseProperty("Bookings")]
    public virtual Venue? Venue { get; set; }
}
