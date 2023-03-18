using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ParkhausManager;
using ParkhausManager.Helpers;

namespace ParkhausManager.Controllers
{
    public class ParkhausController : Controller
    {
        private ParkhausEntities db = new ParkhausEntities();

        private ParkplatzHelper parkplatzHelper = new ParkplatzHelper();

        // Tarifliste für UI Dropdown Texte
        private SelectList TarifTypen = new SelectList(
                new Dictionary<bool, string>
                {
                    { false, "Wochentag" },
                    { true, "Wochenende / Feiertag" }
                }, "Key", "Value");



        // GET: Parkhaus
        public ActionResult Index()
        {
            return View(db.Parkhaus.ToList());
        }

        // GET: Parkhaus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Parkhaus parkhaus = db.Parkhaus.Find(id);
            if (parkhaus == null)
            {
                return HttpNotFound();
            }

            // Liste mit allen Parkplätzen und deren Typen (Frei, Gelgenheitsnutzer, Dauermieter9
            ViewBag.Parkplaetze = parkplatzHelper.GetParkplaetzeUndNummernInParkhaus(parkhaus);


            return View(parkhaus);
        }

        // GET: Parkhaus/Create
        public ActionResult Create()
        {
            ViewBag.TarifTypen = TarifTypen;
            return View();
        }

        // POST: Parkhaus/Create
        // Aktivieren Sie zum Schutz vor Angriffen durch Overposting die jeweiligen Eigenschaften, mit denen eine Bindung erfolgen soll. 
        // Weitere Informationen finden Sie unter https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Ort")] Parkhaus parkhaus)
        {
            if (ModelState.IsValid)
            {
                ViewBag.TarifTypen = TarifTypen;
                db.Parkhaus.Add(parkhaus);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(parkhaus);
        }

        public ActionResult AddStockwerk(int parkhausId)
        {
            //Add new Vorgehensmodell
            db.Stockwerk.Add(new Stockwerk()
            {
                Name = "Neues Stockwerk",
                Parkhaus_Id = parkhausId
            });


            //Try to save changes or throw exception
            try
            {
                db.SaveChanges();
            }
            catch (Exception e) { throw e; }

            //Redirect to the Edit Project View
            return this.RedirectToAction("Edit", new { id = parkhausId });
        }

        public ActionResult RemoveStockwerk(int stockwerkId, int parkhausId)
        {
            //Search phase object in the database
            var stockwerk = db.Stockwerk.FirstOrDefault(p => p.Id == stockwerkId);

            //Remove phase from the context
            db.Stockwerk.Remove(stockwerk);

            //Try to save changes or throw exception
            try
            {
                db.SaveChanges();
            }
            catch (Exception e) { throw e; }

            //Return to Edit Project View
            return this.RedirectToAction("Edit", new { id = parkhausId });
        }

        public ActionResult AddTarif(int parkhausId)
        {
            //Add new Vorgehensmodell
            db.Tarif.Add(new Tarif()
            {
                Parkhaus_Id = parkhausId
            });


            //Try to save changes or throw exception
            try
            {
                db.SaveChanges();
            }
            catch (Exception e) { throw e; }

            //Redirect to the Edit Project View
            return this.RedirectToAction("Edit", new { id = parkhausId });
        }

        public ActionResult RemoveTarif(int tarifId, int parkhausId)
        {
            //Search phase object in the database
            var tarif = db.Tarif.FirstOrDefault(p => p.Id == tarifId);

            //Remove phase from the context
            db.Tarif.Remove(tarif);

            //Try to save changes or throw exception
            try
            {
                db.SaveChanges();
            }
            catch (Exception e) { throw e; }

            //Return to Edit Project View
            return this.RedirectToAction("Edit", new { id = parkhausId });
        }



        // GET: Parkhaus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Parkhaus parkhaus = db.Parkhaus.Find(id);

            ViewBag.TarifTypen = TarifTypen;

            if (parkhaus == null)
            {
                return HttpNotFound();
            }
            return View(parkhaus);
        }

        // POST: Parkhaus/Edit/5
        // Aktivieren Sie zum Schutz vor Angriffen durch Overposting die jeweiligen Eigenschaften, mit denen eine Bindung erfolgen soll. 
        // Weitere Informationen finden Sie unter https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Parkhaus parkhaus)
        {
            if (ModelState.IsValid)
            {

                //Tarife
                foreach (var item in parkhaus.Tarif)
                {
                    var dbitem = db.Tarif.FirstOrDefault(s => s.Id == item.Id);
                    dbitem.Von = item.Von;
                    dbitem.Bis = item.Bis;
                    dbitem.Preis = item.Preis;
                    dbitem.Typ = item.Typ;
                }

                //Stockwerke
                foreach (var item in parkhaus.Stockwerk)
                {
                    var dbitem = db.Stockwerk.FirstOrDefault(s => s.Id == item.Id);
                    dbitem.Name = item.Name;
                    dbitem.AnzahlParkplaetze = item.AnzahlParkplaetze;
                }

                //View Table leeren
                parkhaus.Stockwerk.Clear();
                parkhaus.Tarif.Clear();

                ViewBag.TarifTypen = TarifTypen;

                db.Entry(parkhaus).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Edit");
            }
            return View(parkhaus);
        }

        // GET: Parkhaus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Parkhaus parkhaus = db.Parkhaus.Find(id);


            if (parkhaus == null)
            {
                return HttpNotFound();
            }

            if (parkhaus.Stockwerk.Count > 0)
            {
                ViewBag.Error = "Parkhaus kann nicht gelöscht werden, da noch Stockwerke zugewiesen sind!";
                return View(parkhaus);
            }
            else
            {
                return View(parkhaus);
            }
        }

        // POST: Parkhaus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Parkhaus parkhaus = db.Parkhaus.Find(id);

            if(parkhaus.Stockwerk.Count > 0)
            {
                ViewBag.Error = "Parkhaus kann nicht gelöscht werden, da noch Stockwerke zugewiesen sind!";
                return View(parkhaus);
            } else 
            {
                db.Parkhaus.Remove(parkhaus);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
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
