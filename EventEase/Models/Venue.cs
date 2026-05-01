using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Models;

[Table("Venue")]
public partial class Venue
{
    [Key]
    [Column("VenueID")]
    public int VenueId { get; set; }

    [Column("Venue_Location")]
    [StringLength(100)]
    [Unicode(false)]
    public string VenueLocation { get; set; } = null!;

    [Column("capacity")]
    public int Capacity { get; set; }

    [Column("Venue_name")]
    [StringLength(100)]
    [Unicode(false)]
    public string VenueName { get; set; } = null!;
    [Column("ImageUrl")]
    [StringLength(600)]
    [Unicode(false)]
    public string ImageUrl { get; set; } = null!;


    [InverseProperty("Venue")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [InverseProperty("Venue")]
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
    
}
