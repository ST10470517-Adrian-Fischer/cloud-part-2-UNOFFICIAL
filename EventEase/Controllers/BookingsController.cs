using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;

namespace EventEase.Controllers
{
    public class BookingsController : Controller
    {
        private readonly EventEaseManagingSystem _context;

        public BookingsController(EventEaseManagingSystem context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var eventEaseManagingSystem = _context.Bookings.Include(b => b.Event).Include(b => b.Venue);
            return View(await eventEaseManagingSystem.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.Bookingid == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId");
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Bookingid,VenueId,EventId")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                // Get the selected event with its dates
                var selectedEvent = await _context.Events
                    .FirstOrDefaultAsync(e => e.EventId == booking.EventId);

                if (selectedEvent == null)
                {
                    ModelState.AddModelError("", "Selected event does not exist.");
                    ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId", booking.EventId);
                    ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", booking.VenueId);
                    return View(booking);
                }

                if (selectedEvent.Startdate == null || selectedEvent.Enddate == null)
                {
                    ModelState.AddModelError("", "The selected event does not have start/end dates. Please update the event first.");
                    ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId", booking.EventId);
                    ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", booking.VenueId);
                    return View(booking);
                }

                // Pull all bookings at this venue into memory first, then check overlap
                var bookingsAtVenue = await _context.Bookings
                    .Include(b => b.Event)
                    .Where(b => b.VenueId == booking.VenueId && b.EventId != booking.EventId)
                    .ToListAsync();

                // Now check for date overlaps in memory (avoids DateOnly EF Core issues)
                var conflict = bookingsAtVenue.FirstOrDefault(b =>
                    b.Event != null &&
                    b.Event.Startdate != null &&
                    b.Event.Enddate != null &&
                    b.Event.Startdate <= selectedEvent.Enddate &&
                    b.Event.Enddate >= selectedEvent.Startdate
                );

                if (conflict != null)
                {
                    ModelState.AddModelError("",
                        $"⚠️ Booking conflict! '{conflict.Event!.EventName}' is already booked at this venue " +
                        $"from {conflict.Event.Startdate} to {conflict.Event.Enddate}. " +
                        $"Please choose a different venue or a non-overlapping date.");
                    ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId", booking.EventId);
                    ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", booking.VenueId);
                    return View(booking);
                }

                // No conflict - save the booking
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", booking.VenueId);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", booking.VenueId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Bookingid,VenueId,EventId")] Booking booking)
        {
            if (id != booking.Bookingid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Get the event being booked
                var selectedEvent = await _context.Events
                    .FirstOrDefaultAsync(e => e.EventId == booking.EventId);

                if (selectedEvent == null)
                {
                    ModelState.AddModelError("", "Selected event does not exist.");
                    ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId", booking.EventId);
                    ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", booking.VenueId);
                    return View(booking);
                }

                // Check for overlapping bookings (exclude current booking)
                if (selectedEvent.Startdate != null && selectedEvent.Enddate != null)
                {
                    var overlappingBooking = await _context.Bookings
                        .Include(b => b.Event)
                        .Where(b =>
                            b.Bookingid != booking.Bookingid &&        // Exclude current booking
                            b.VenueId == booking.VenueId &&            // Same venue
                            b.EventId != booking.EventId &&             // Different event
                            b.Event.Startdate != null &&
                            b.Event.Enddate != null &&
                            b.Event.Startdate <= selectedEvent.Enddate &&
                            b.Event.Enddate >= selectedEvent.Startdate
                        )
                        .FirstOrDefaultAsync();

                    if (overlappingBooking != null)
                    {
                        var conflictingEvent = overlappingBooking.Event;
                        ModelState.AddModelError("",
                            $"Booking conflict! '{conflictingEvent.EventName}' is already booked at this venue " +
                            $"from {conflictingEvent.Startdate} to {conflictingEvent.Enddate}. " +
                            $"Please choose a different venue or date.");
                        ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId", booking.EventId);
                        ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", booking.VenueId);
                        return View(booking);
                    }
                }

                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Bookingid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", booking.VenueId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.Bookingid == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.Bookingid == id);
        }
    }
}
