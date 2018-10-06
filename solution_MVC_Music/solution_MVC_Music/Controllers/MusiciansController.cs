using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using solution_MVC_Music.Data;
using solution_MVC_Music.Models;

namespace solution_MVC_Music.Controllers
{
    public class MusiciansController : Controller
    {
        private readonly MusicContext _context;

        public MusiciansController(MusicContext context)
        {
            _context = context;
        }

        // GET: Musicians
        public async Task<IActionResult> Index()
        {
            var musicContext = _context.Musicians.Include(m => m.Instrument);
            return View(await musicContext.ToListAsync());
        }

        // GET: Musicians/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var musician = await _context.Musicians
                .Include(m => m.Instrument)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (musician == null)
            {
                return NotFound();
            }

            return View(musician);
        }

        // GET: Musicians/Create
        public IActionResult Create()
        {
            ddlValues();
            return View();
        }

        // POST: Musicians/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,MiddleName,LastName,Phone,DOB,SIN,InstrumentID")] Musician musician)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(musician);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException dex)
            {
                if (dex.InnerException.Message.Contains("IX_Musicians_SIN")) 
                {
                    ModelState.AddModelError("SIN", "Unable to save changes. Remember, you cannot have duplicate SIN numbers.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            ddlValues(musician);
            return View(musician);
        }

        // GET: Musicians/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var musician = await _context.Musicians.FindAsync(id);
            if (musician == null)
            {
                return NotFound();
            }
            ddlValues(musician);
            return View(musician);
        }

        // POST: Musicians/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var musicianToUpdate = await _context.Musicians.SingleOrDefaultAsync(m => m.ID == id);
            if (musicianToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Musician>(musicianToUpdate, "",
                m => m.ID, m => m.FirstName, m => m.MiddleName, m => m.LastName,
                m => m.Phone, m => m.DOB, m => m.SIN, m => m.InstrumentID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MusicianExists(musicianToUpdate.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException dex)
                {
                    if (dex.InnerException.Message.Contains("IX_Musicians_SIN"))
                    {
                        ModelState.AddModelError("SIN", "Unable to save changes. Remember, you cannot have duplicate SIN numbers.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    }
                }
            }
            ddlValues(musicianToUpdate);
            return View(musicianToUpdate);
        }

        // GET: Musicians/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var musician = await _context.Musicians
                .Include(m => m.Instrument)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (musician == null)
            {
                return NotFound();
            }

            return View(musician);
        }

        // POST: Musicians/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var musician = await _context.Musicians.FindAsync(id);

            try
            {
                _context.Musicians.Remove(musician);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dex)
            {
                if (dex.InnerException.Message.Contains("FK_"))
                {
                    ModelState.AddModelError("", "Unable to save changes. Remember, you cannot delete a Musician that has Performances.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }

            return View(musician);
        }

        private void ddlValues(Musician selItem = null)
        {
            var instruments = from i in _context.Instruments
                              orderby i.Name
                              select i;
            ViewData["InstrumentID"] = new SelectList(instruments, "ID", "Name", selItem?.InstrumentID);
        }

        private bool MusicianExists(int id)
        {
            return _context.Musicians.Any(e => e.ID == id);
        }
    }
}
