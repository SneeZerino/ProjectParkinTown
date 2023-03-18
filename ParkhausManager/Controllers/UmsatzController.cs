using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ParkhausManager.Controllers
{
    public class UmsatzController : Controller
    {
        private ParkhausEntities db = new ParkhausEntities();

        private SelectList TypList = new SelectList(
        new Dictionary<bool, string>
        {
                    { false, "Gelegenheitsnutzer" },
                    { true, "Dauermieter" }
        }, "Key", "Value");

        // GET: Umsatz
        public ActionResult Index(int? parkhausId, int? jahr, int? monat, bool? typ)
        {
            // Listen für Dropdowns
            ViewBag.ParkhausList = new SelectList(db.Parkhaus.ToList(), "Id", "Name");
            ViewBag.JahrList = new SelectList(db.Zahlung.Select(z => z.Zeit.Value.Year).Distinct().ToList());
            ViewBag.MonatList = getMonatListe();
            ViewBag.TypList = TypList;

            // ViewBag Daten für View
            ViewBag.ParkhausId = parkhausId;
            ViewBag.Jahr = jahr;


            var zahlungen = db.Zahlung.AsQueryable();

            // Filter Besuchertyp
            if (typ.HasValue)
            {
                zahlungen = zahlungen.Where(z => z.Typ == typ);
            }

            // Filter Parkhaus
            if (parkhausId.HasValue)
            {
                zahlungen = zahlungen.Where(z => z.Stockwerk.Parkhaus_Id == parkhausId);

                ViewBag.Parkhaus = db.Parkhaus.Find(parkhausId);
                ViewBag.Monat = monat.ToString();
            } else
            {
                ViewBag.ParkhausError = "Es muss ein Parkhaus gewählt werden!";
                return View(zahlungen);
            }

            // Filter Jahr
            if (jahr.HasValue)
            {
                zahlungen = zahlungen.Where(z => z.Zeit.Value.Year == jahr);
            } else
            {
                ViewBag.JahrError = "Es muss ein Jahr gewählt werden!";
                return View(zahlungen);
            }

            // Filter Monat
            if (monat.HasValue)
            {
                // Monatsumsatz
                zahlungen = zahlungen.Where(z => z.Zeit.Value.Month == monat);
                ViewBag.TotalMonatsUmsatz = zahlungen.ToList().Sum(z => z.Kosten);
            } else
            {
                // Jahresumsatz
                ViewBag.TotalJahresUmsatz = zahlungen.ToList().Sum(z => z.Kosten);

                var umsatzPerMonatList = new List<Double>();

                for (var i = 1; i < 13; i++)
                {
                    // Zahlungen mit Parkhaus und Jahr (sind immer gesetzt) und Monat aus Loop
                    var zahl = db.Zahlung.Where(z => z.Zeit.Value.Month == i && z.Zeit.Value.Year == jahr && z.Stockwerk.Parkhaus_Id == parkhausId);

                    // Filter für Besuchertyp wenn gesetzt
                    zahl = typ.HasValue ? zahl.Where(z => z.Typ == typ) : zahl;

                    // Umsatz berechnen
                    umsatzPerMonatList.Add(zahl.ToList().Sum(z => z.Kosten.Value));
                }

                ViewBag.UmsatzPerMonatList = umsatzPerMonatList;
            }


            return View(zahlungen.ToList().OrderBy(z => z.Zeit));
        }

        // Liste mit Monaten für UI Dropdown
        private SelectList getMonatListe()
        {
            var dict = new Dictionary<int, string>();
            
            dict.Add(1, "Januar");
            dict.Add(2, "Februar");
            dict.Add(3, "März");
            dict.Add(4, "April");
            dict.Add(5, "Mai");
            dict.Add(6, "Juni");
            dict.Add(7, "Juli");
            dict.Add(8, "August");
            dict.Add(9, "September");
            dict.Add(10, "Oktober");
            dict.Add(11, "November");
            dict.Add(12, "Dezember");

            return new SelectList(dict, "Key", "Value");
        }
    }
}