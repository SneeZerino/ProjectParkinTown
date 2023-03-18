using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ParkhausManager;
using PagedList;
using System.Globalization;
using System.Threading;

namespace ParkhausManager.Controllers
{
    public class LogsController : Controller
    {
        private ParkhausEntities db = new ParkhausEntities();


        private SelectList EventList = new SelectList(
               new Dictionary<bool, string>
               {
                    { false, "Einfahrt" },
                    { true, "Ausfahrt" }
               }, "Key", "Value");

        private SelectList TypList = new SelectList(
               new Dictionary<bool, string>
               {
                    { false, "Gelegenheitsnutzer" },
                    { true, "Dauermieter" }
               }, "Key", "Value");

        // GET: Logs
        [HttpGet]
        public ActionResult Index(int? page, int? id, bool? eventBool, bool? typ, int? dauermieterId, int? stockwerkId, int? parkhausId, string von, string bis)
        {

            var ci = CultureInfo.CurrentCulture.Name;


            // ViewBag Filter wieder abfüllen wenn sie vorhanden waren
            ViewBag.Id = id;
            ViewBag.Event = eventBool;
            ViewBag.Typ = typ;
            ViewBag.DauermieterId = dauermieterId;
            ViewBag.StockwerkId = stockwerkId;
            ViewBag.ParkhausId = parkhausId;
            ViewBag.Von = von;
            ViewBag.Bis = bis;

            // Listen für Dropdowns
            ViewBag.DauermieterList = new SelectList(db.Dauermieter.ToList(), "Id", "Name");

            var stockwerke = db.Stockwerk.ToList();
            foreach(var s in stockwerke)
            {
                s.Name = s.Parkhaus.Name + " - " + s.Name;
            }

            ViewBag.StockwerkList = new SelectList(db.Stockwerk.ToList(), "Id", "Name");
            ViewBag.ParkhausList = new SelectList(db.Parkhaus.ToList(), "Id", "Name");
            ViewBag.EventList = EventList;
            ViewBag.TypList = TypList;

            // Filter anwenden
            var log = db.Log.Include(l => l.Dauermieter).Include(l => l.Stockwerk).AsQueryable();

            // ID
            if (id.HasValue)
            {
                log = log.Where(l => l.Id == id);
            }

            // Event
            if (eventBool.HasValue)
            {
                log = log.Where(l => l.Event == eventBool);
            }

            // Besucher Typ
            if (typ.HasValue)
            {
                log = log.Where(l => l.Typ == typ);
            }

            // Dauermieter
            if (dauermieterId.HasValue)
            {
                log = log.Where(l => l.Dauermieter_Id == dauermieterId);
            }

            // Stockwerk
            if (stockwerkId.HasValue)
            {
                log = log.Where(l => l.Stockwerk_Id == stockwerkId);
            }

            // Parkhaus
            if (parkhausId.HasValue)
            {
                log = log.Where(l => l.Stockwerk.Parkhaus_Id == parkhausId);
            }

            DateTime? vonDate = null;
            if (!string.IsNullOrEmpty(von))
            {
                vonDate = DateTime.Parse(von);
            }
            DateTime? bisDate = null;
            if (!string.IsNullOrEmpty(bis))
            {
                bisDate = DateTime.Parse(bis);
                bisDate = bisDate.Value.AddMilliseconds(999); // Um sicherzutstellen das die Sekunde auch noch miteinbezogen wird
            }

            // Zeit
            if (vonDate.HasValue)
            {
                log = log.Where(l => l.Zeit >= vonDate);
            }
            if (bisDate.HasValue)
            {
                log = log.Where(l => l.Zeit <= bisDate);
            }

            // Anzhal Elemente pro Seite
            int pageSize = 10;

            // pageNummer aus GET Request, Default: 1
            int pageNummer = (page ?? 1);

            // Anzahl Einträge total gezeigt / TODO Add to Configurations
            int anzahl = 100;


            return View(log.OrderBy(l => l.Id).Take(anzahl).ToPagedList(pageNummer, pageSize));
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
