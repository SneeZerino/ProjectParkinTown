using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ParkhausManager.Models;
using ParkhausManager.Helpers;

namespace ParkhausManager.Controllers
{
    public class AutomatenController : Controller
    {
        private ParkhausEntities db = new ParkhausEntities();
        private ParkplatzHelper parkplatzHelper = new ParkplatzHelper();

        public ActionResult Index(int? id)
        {
            var automatenVM = new AutomatenViewModel();
            var ticket = TempData["Ticket"] as Ticket;
            automatenVM.Ticket = ticket;

            ViewBag.Error = TempData["Error"];

            ViewBag.Parkhaeuser = db.Parkhaus.ToList();
            ViewBag.SelectedParkhaus = (id == null) ? db.Parkhaus.ToList().FirstOrDefault().Id : id;


            return View(automatenVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EinfahrtScan(AutomatenViewModel automatenVM)
        {
            if (ModelState.IsValid)
            {
                // Für Parkhaus-Auswahl Dropdown
                var parkhausId = Convert.ToInt32(Request["SelectedParkhaus"]);

                // Dauermieter Einfahrt Scan:
                var dauermieter = db.Dauermieter.FirstOrDefault(d => d.Stockwerk.Parkhaus_Id == parkhausId && d.Code == automatenVM.MieterCode);

                if (dauermieter == null)
                {
                    ViewBag.DauermieterError = "Kein Dauermieter mit diesem Code gefunden!";
                }
                else if (dauermieter.Gesperrt == false)
                {

                    // Genereate Einfahrt Log (Dauermieter)
                    var log = new Log()
                    {
                        Event = false, //Einfahrt
                        ParkplatzNummer = dauermieter.ParkplatzNummer,
                        Typ = true,
                        Stockwerk = dauermieter.Stockwerk,
                        Stockwerk_Id = dauermieter.Stockwerk_Id,
                        Dauermieter = dauermieter,
                        Dauermieter_Id = dauermieter.Id,
                        Zeit = DateTime.Now
                    };
                    db.Log.Add(log);

                    try
                    {
                        db.SaveChanges();
                        automatenVM.Dauermieter = dauermieter;
                        automatenVM.Eintrittszeit = DateTime.Now;

                    }
                    catch (Exception e)
                    {
                        ViewBag.DauermieterError = "Fehler beim Erstellen der Logs.";
                        throw e;
                    }

                } 
                else if (dauermieter.Gesperrt == true)
                {
                    ViewBag.DauermieterError = "Dieser Dauermieter ist gesperrt!";
                }

                // Daten in Input Feldern leeren
                FelderLeeren(automatenVM);

                // Für Parkhaus-Auswahl Dropdown
                ViewBag.Parkhaeuser = db.Parkhaus.ToList();
                ViewBag.SelectedParkhaus = (parkhausId == null) ? db.Parkhaus.ToList().FirstOrDefault().Id : Convert.ToInt32(parkhausId);

            }
            return View("Index", automatenVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AusfahrtTicketScan(AutomatenViewModel automatenVM)
        {
            // Für Parkhaus-Auswahl Dropdown
            var parkhausId = Convert.ToInt32(Request["SelectedParkhaus"]);

            if (ModelState.IsValid)
            {

                // Ticket entwerten
                if (automatenVM.TicketNummerAusfahrt != null)
                {
                    var ticket = db.Ticket.Find(automatenVM.TicketNummerAusfahrt);

                    if (ticket == null)
                    {
                        // Ticket nicht gefunden
                        ViewBag.AusfahrtTicketError = "Kein Ticket mit dieser Nummer gefunden!";
                    }
                    else if (ticket.Bezahlt == false)
                    {
                        // Leere Nummer
                        ViewBag.AusfahrtTicketError = "Das Ticket wurde noch nicht entwertet!";
                    } 
                    else if (ticket.Benutzt == true)
                    {
                        // Ticket bereits benutzt
                        ViewBag.AusfahrtTicketError = "Das Ticket wurde bereits einmal benutzt. Wenden Sie sich bitte an die Parkhaus-Administration.";
                    }
                    else
                    {
                        // Valides Ausfahrts-Ticket

                        var log = new Log()
                        {
                            Event = true, //Ausfahrt
                            ParkplatzNummer = ticket.ParkplatzNummer,
                            Typ = false,
                            Stockwerk = ticket.Stockwerk,
                            Stockwerk_Id = ticket.Stockwerk.Id,
                            Zeit = DateTime.Now
                        };
                        db.Log.Add(log);

                        // Setze Ticket auf "Benutzt"
                        ticket.Benutzt = true;

                        //Try to save changes or throw exception
                        try
                        {
                            db.SaveChanges();
                            ViewBag.AusfahrtTicketSuccess = "Besten Dank für Ihren Besuch. Wir wünschen einen angenehmen Tag.";
                        }
                        catch (Exception e){
                            ViewBag.AusfahrtTicketError = "Unbekannter Fehler! Wenden Sie sich bitte an die Parkhaus-Administration.";
                            throw e;
                        }
                    }
                }
                else
                {
                    // Leere Nummer
                    ViewBag.AusfahrtTicketError = "Die Ticketnummer darf nicht leer sein!";
                }

            }

            // Daten in Input Feldern leeren
            FelderLeeren(automatenVM);

            // Für Parkhaus-Auswahl Dropdown
            ViewBag.Parkhaeuser = db.Parkhaus.ToList();
            ViewBag.SelectedParkhaus = (parkhausId == null) ? db.Parkhaus.ToList().FirstOrDefault().Id : Convert.ToInt32(parkhausId);

            return View("Index", automatenVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AusfahrtScan(AutomatenViewModel automatenVM)
        {
            if (ModelState.IsValid)
            {
                // Für Parkhaus-Auswahl Dropdown
                var parkhausId = Convert.ToInt32(Request["SelectedParkhaus"]);

                // Dauermieter Einfahrt Scan:
                var dauermieter = db.Dauermieter.FirstOrDefault(d => d.Stockwerk.Parkhaus_Id == parkhausId && d.Code == automatenVM.MieterCodeAusfahrt);
                if(dauermieter == null)
                {
                    ViewBag.AusfahrtDauermieterError = "Kein Dauermieter mit diesem Code gefunden!";
                }
                else if (dauermieter.Gesperrt == false)
                {

                    // Genereate Einfahrt Log (Dauermieter)
                    var log = new Log()
                    {
                        Event = true, //Ausfahrt
                        ParkplatzNummer = dauermieter.ParkplatzNummer,
                        Typ = true,
                        Stockwerk = dauermieter.Stockwerk,
                        Stockwerk_Id = dauermieter.Stockwerk_Id,
                        Dauermieter = dauermieter,
                        Dauermieter_Id = dauermieter.Id,
                        Zeit = DateTime.Now
                    };
                    db.Log.Add(log);

                    try
                    {
                        db.SaveChanges();
                        ViewBag.AusfahrtDauermieterSuccess = "Besten Dank für Ihren Besuch. Wir wünschen einen angenehmen Tag.";
                    }
                    catch (Exception e)
                    {
                        ViewBag.AusfahrtDauermieterError = "Fehler beim Erstellen der Logs.";
                        throw e;
                    }

                } 
                else if (dauermieter.Gesperrt == true)
                {
                    ViewBag.AusfahrtDauermieterError = "Dieser Dauermieter ist gesperrt!";
                }

                // Daten in Input Feldern leeren
                FelderLeeren(automatenVM);

                // Für Parkhaus-Auswahl Dropdown
                ViewBag.Parkhaeuser = db.Parkhaus.ToList();
                ViewBag.SelectedParkhaus = (parkhausId == null) ? db.Parkhaus.ToList().FirstOrDefault().Id : Convert.ToInt32(parkhausId);

            }
            return View("Index", automatenVM);
        }

       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TicketEntwerten(AutomatenViewModel automatenVM)
        {

            if (ModelState.IsValid)
            {
                // Für Parkhaus-Auswahl Dropdown
                var parkhausId = Convert.ToInt32(Request["SelectedParkhaus"]);

                // Ticket entwerten
                if (automatenVM.TicketNummer != null)
                {
                    var ticket = db.Ticket.Find(automatenVM.TicketNummer);

                    if (ticket != null && ticket.Bezahlt == false)
                    {
                        var eintritt = (DateTime)ticket.Eintrittszeit;
                        var austritt = DateTime.Now;
                        var parkhaus = db.Parkhaus.Find(parkhausId);
                        var tarife = db.Tarif.Where(t => t.Parkhaus.Id == ticket.Stockwerk.Parkhaus.Id).ToList();
                        var tagestarif = (double)parkhaus.Tagestarif;
                        var monatsmiete = (double)parkhaus.Monatsmiete;

                        var zeit = (DateTime)eintritt;
                        var kosten = 0.0;

                        // Ist die Gesammtdauer über 24h --> Tagestarif = Anzahl Tage
                        var tage = (austritt - eintritt).TotalDays;

                        // Über 24h?
                        if (tage > 1)
                        {
                            //Angebrochene Tage werden voll verrechnet
                            //1.01 Tage = 2 Tage --> Aufrunden
                            var anzahlTage = Math.Ceiling(tage);
                            kosten = anzahlTage * tagestarif;
                        } 
                        else
                        {
                            while (zeit < austritt)
                            {
                                // Preis für 15 Minuten anhand Stundentarif berechnen

                                // 1) Ist es ein Wochentag oder Feiertag/Wochenende
                                // So = 0, Mo = 1, Di = 2, Mi = 3, Do = 4, Fr = 5, Sa = 6
                                // false = Wochentag, true = Feiertag/Wochenende
                                var typ = ((int)zeit.DayOfWeek > 0 && (int)zeit.DayOfWeek < 6) ? false : true;

                                // 2) Stundentarif finden
                                var tarif = tarife.FirstOrDefault(t => t.Von <= zeit.TimeOfDay && t.Bis > zeit.TimeOfDay && t.Typ == typ);

                                // 3) Viertelstunde berechnen
                                if (tarif != null)
                                {
                                    kosten += (double)tarif.Preis * 0.25;
                                }

                                // 15 Minuten vorspringen
                                zeit = zeit.AddMinutes(15);
                            }
                        }

                        try
                        {
                            // Ticket = Bezahlt
                            ticket.Bezahlt = true;

                            // Eintrag in Zahlungen
                            var zahlung = new Zahlung()
                            {
                                Kosten = kosten,
                                Stockwerk = ticket.Stockwerk,
                                Stockwerk_Id = ticket.Stockwerk_Id,
                                Zeit = DateTime.Now,
                                Ticket = ticket,
                                Ticket_Id = ticket.Id,
                                Typ = false //Gelegenheitsnutzer
                            };
                            db.Zahlung.Add(zahlung);

                            // Speichern
                            db.SaveChanges();

                            // Daten für View
                            var tick = new Ticket()
                            {
                                Bezahlt = true,
                                BezahltZeit = DateTime.Now,
                                Eintrittszeit = eintritt,
                                Id = (int)automatenVM.TicketNummer,
                                Stockwerk = ticket.Stockwerk,
                                Stockwerk_Id = ticket.Stockwerk_Id,
                                ParkplatzNummer = ticket.ParkplatzNummer
                            };

                            var entwertetesTicket = new EntwertetesTicket()
                            {
                                Ticket = tick,
                                Kosten = kosten,
                                Austrittszeit = austritt
                            };

                            automatenVM.EntwertetesTicket = entwertetesTicket;



                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }
                    else if (ticket == null)
                    {
                        // Kein Ticket gefunden
                        ViewBag.TicketEntwertenError = "Es wurde kein Ticket mit dieser Nummer gefunden!";
                    }
                    else
                    {
                        // Ticket wurde bereits bezahlt
                        ViewBag.TicketEntwertenError = "Ticket wurde bereits bezahlt!";
                    }
                } 
                else
                {
                    // Ungültig
                    ViewBag.TicketEntwertenError = "Ungültige Ticketnummer! Sie darf nicht leer sein.";
                }


                // Für Parkhaus-Auswahl Dropdown
                ViewBag.Parkhaeuser = db.Parkhaus.ToList();
                ViewBag.SelectedParkhaus = (parkhausId == null) ? db.Parkhaus.ToList().FirstOrDefault().Id : Convert.ToInt32(parkhausId);
            }

            return View("Index", automatenVM);
        }

        public ActionResult GetTicket(int parkhausId)
        {
            var parkhaus = db.Parkhaus.Find(parkhausId);

            if(parkhaus.Stockwerk.Count <= 0)
            {
                TempData["Error"] = "Das Parkhaus hat keine Stockwerke!";
                return this.RedirectToAction("Index", "Automaten", new { id = parkhausId });
            }

            // Wähle Stockwerk mit meisten freien Plätzen
            Stockwerk freiesStockwerk = null;
            var count = 0;
            var stockwerkFloor = 0;
            var index = 0;

            foreach (var stockwerk in parkhaus.Stockwerk)
            {
                var anzahlFrei = parkplatzHelper.GetAnzahlFreieParkplaetzeAufStockwerk(stockwerk);

                if(anzahlFrei > count)
                {
                    stockwerkFloor = index;
                    count = anzahlFrei;
                    freiesStockwerk = stockwerk;
                }
                index++;
            }

            if(freiesStockwerk == null)
            {
                // Parkhaus voll
                TempData["Error"] = "Das Parkhaus ist leider voll besetzt!";
                return this.RedirectToAction("Index", "Automaten", new { id = parkhausId });
            }


            // Parkplatz wählen
            // EG: 1-99
            // 1. OG 100-199
            // 2. OG 200-299  usw.

            // Gesucht: Tiefste freie Nummer auf gewähltem Stockwerk
            var nummern = parkplatzHelper.GetBesetzteParkplatzNummernAufStockwerk(freiesStockwerk);

            var parkplatzNummer = -1;

            stockwerkFloor = stockwerkFloor * 100;
            for(var i = stockwerkFloor; i < stockwerkFloor + freiesStockwerk.AnzahlParkplaetze; i++)
            {
                if (!nummern.Contains(i))
                {
                    // Parkplatz gefunden
                    parkplatzNummer = i;
                    break;
                }
            }




            // Generate Ticket
            var ticket = new Ticket()
            {
                Stockwerk = freiesStockwerk,
                Eintrittszeit = DateTime.Now,
                ParkplatzNummer = parkplatzNummer,
                Stockwerk_Id = freiesStockwerk.Id,
                Bezahlt = false
            };
            db.Ticket.Add(ticket);

            // Genereate Einfahrt Log
            var log = new Log()
            {
                Event = false, //Einfahrt
                ParkplatzNummer = parkplatzNummer,
                Typ = false,
                Stockwerk = freiesStockwerk,
                Stockwerk_Id = freiesStockwerk.Id,
                Zeit = DateTime.Now
            };
            db.Log.Add(log);

            //Try to save changes or throw exception
            try
            {
                db.SaveChanges();
            }
            catch (Exception e) { 
                throw e; 
            }

            TempData["Ticket"] = db.Ticket.Find(ticket.Id);

            return this.RedirectToAction("Index", "Automaten", new { id = parkhausId });
        }

        private void FelderLeeren(AutomatenViewModel automatenVM)
        {

            automatenVM.TicketNummer = null;
            automatenVM.TicketNummerAusfahrt = null;
            automatenVM.MieterCode = null;
            automatenVM.MieterCodeAusfahrt = null;
        }

    }
}