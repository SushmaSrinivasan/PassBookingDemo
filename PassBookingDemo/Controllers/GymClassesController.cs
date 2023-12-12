using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PassBookingDemo.Data;
using PassBookingDemo.Models;
using PassBookingDemo.Models.ViewModels;

namespace PassBookingDemo.Controllers
{
    [Authorize]
    public class GymClassesController : Controller

    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public GymClassesController(IMapper mapper,ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
          _mapper = mapper;
            _context = context;
            _userManager = userManager;
        }

        [AllowAnonymous]
        // GET: GymClasses
        public async Task<IActionResult> Index()
        {
            var gymClasses = await _context.GymClasses
                .Include(g => g.AttendingMembers)
                .Where(g => g.StartTime > DateTime.Now)
                .ToListAsync();

            var model = _mapper.Map<IndexViewModel>(gymClasses);

            return View(model);

            //return View(await _context.GymClasses.ToListAsync());
        }

        // GET: GymClasses/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return RedirectToAction("Index");

            var gymClassWithAttendees = await _context.GymClasses
                .Where(g=>g.Id == id)
                .Include(c=>c.AttendingMembers)
                .ThenInclude(u=>u.user).FirstOrDefaultAsync();

            if(gymClassWithAttendees == null) 
            return RedirectToAction(nameof(Index));
           

            return View(gymClassWithAttendees);
        }
        [Authorize(Roles ="Admin")]
        // GET: GymClasses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GymClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,StartTime,Duration")] GymClass gymClass)
        {
            //ModelState.Remove("AttendingMembers");
            if (ModelState.IsValid)
            {
                _context.Add(gymClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        }

        [Authorize(Roles = "Admin")]
        // GET: GymClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClasses.FindAsync(id);
            if (gymClass == null)
            {
                return NotFound();
            }
            return View(gymClass);
        }

        // POST: GymClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,StartTime,Duration")] GymClass gymClass)
        {
            if (id != gymClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gymClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GymClassExists(gymClass.Id))
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
            return View(gymClass);
        }

        // GET: GymClasses/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // POST: GymClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gymClass = await _context.GymClasses.FindAsync(id);
            if (gymClass != null)
            {
                _context.GymClasses.Remove(gymClass);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GymClassExists(int id)
        {
            return _context.GymClasses.Any(e => e.Id == id);
        }

        [Authorize]
        public async Task<ActionResult> BookingToggle(int? id)
        {
            if (id == null) { return NotFound(); }

            var userId = _userManager.GetUserId(User);

            var attending = await _context.ApplicationUserGymClasses.FindAsync(userId, id);

            if(attending == null)
            {
                var booking = new ApplicationUserGymClass
                {
                    ApplicationUserId = userId!,
                    GymClassId = (int)id
                };

                _context.ApplicationUserGymClasses.Add(booking);
            }
            else
            {
                _context.Remove(attending);
            }

          await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }
        [AllowAnonymous]
        public async Task<IActionResult> History()
        {
            var gymClasses = await _context.GymClasses
                .Include(g => g.AttendingMembers)
                .Where(g => g.StartTime < DateTime.Now)
                .ToListAsync();

            var model = _mapper.Map<IndexViewModel>(gymClasses);

            return View(model);
        }
        public async Task<IActionResult> MyHistory()
        {
            var userId = _userManager.GetUserId(User);
            var historicalClasses = await _context.Users
                .Where(u => u.Id == userId)
                .SelectMany(u=>u.AttendedClasses)
                .Select(ac=>ac.GymClass)
                .Where(g=>g.StartTime < DateTime.Now)
                .ToListAsync();

            var model = _mapper.Map<IndexViewModel>(historicalClasses);

            return View();
        }
        public async Task<IActionResult> MyBookings()
        {
            var userId = _userManager.GetUserId(User);
            var bookedClasses = await _context.Users
                .Where(u => u.Id == userId)
                .SelectMany(u => u.AttendedClasses)
                .Select(ac => ac.GymClass)
                .Where(g => g.StartTime < DateTime.Now)
                .ToListAsync();

            var model = _mapper.Map<IndexViewModel>(bookedClasses);

            return View();
        }

    }
}
