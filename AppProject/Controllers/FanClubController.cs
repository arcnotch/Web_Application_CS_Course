using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AppProject.Models;

namespace AppProject.Controllers
{
    public class FanClubController : Controller
    {
        private FanDbContext db = new FanDbContext();

        // GET: Fans Search by name gender or/and seniority  OR First view (show all fans)
        public ActionResult Index(string Namesearch, int? Senioritysearch, string Gendersearch) //Search function
        {
            var fans = from a in db.Fans select a; //LINQ get all fans
            if (!String.IsNullOrEmpty(Namesearch))  //seach with name
            {
                if (!String.IsNullOrEmpty(Gendersearch)) //seach with gender
                {
                    if (Senioritysearch != null && int.MaxValue > Senioritysearch && int.MinValue < Senioritysearch) //seach with name, gender and seniority
                    {
                        fans = from c in db.Fans
                               where (c.FirstName == Namesearch || c.LastName == Namesearch) && c.Venteran == Senioritysearch && c.Gender == Gendersearch
                               select c;
                        return View(fans.ToList());
                    }
                    else //search with name and gender
                    {
                        fans = from c in db.Fans
                               where (c.FirstName == Namesearch || c.LastName == Namesearch) && c.Gender == Gendersearch
                               select c;
                        return View(fans.ToList());
                    }
                }
                else //seach without gender
                {
                    if (Senioritysearch != null && int.MaxValue > Senioritysearch && int.MinValue < Senioritysearch) //seach with name and seniority
                    {
                        fans = from c in db.Fans
                               where (c.FirstName == Namesearch || c.LastName == Namesearch) && c.Venteran == Senioritysearch
                               select c;
                        return View(fans.ToList());
                    }
                    else //seach with name only
                    {
                        fans = from c in db.Fans
                               where c.FirstName == Namesearch || c.LastName == Namesearch
                               select c;
                        return View(fans.ToList());
                    }
                }
            }
            else //seach without name
            {
                if (!String.IsNullOrEmpty(Gendersearch)) //seach with gender
                {
                    if (Senioritysearch != null && int.MaxValue > Senioritysearch && int.MinValue < Senioritysearch) //seach with gender and seniority
                    {
                        fans = from c in db.Fans
                               where c.Venteran == Senioritysearch && c.Gender == Gendersearch
                               select c;
                        return View(fans.ToList());
                    }
                    else //seach with gender only
                    {
                        fans = from c in db.Fans
                               where c.Gender == Gendersearch
                               select c;
                        return View(fans.ToList());
                    }
                }
                else //seach without gender
                {
                    if (int.MaxValue > Senioritysearch && int.MinValue < Senioritysearch) //seach with seniority only
                    {
                        fans = from c in db.Fans
                               where c.Venteran == Senioritysearch
                               select c;
                        return View(fans.ToList());
                    }

                }
            }

            return View(fans.ToList());
        }

        // GET: Fans/Details/5
        public ActionResult Details(int? id) //Show fan's details
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fan fan = db.Fans.Find(id);
            if (fan == null)
            {
                return HttpNotFound();
            }
            return View(fan);
        }

        // GET: Fans/Create
        public ActionResult Create() //Create a new fan
        {
            return View();
        }

        // POST: Fans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken] //Create a new fan
        public ActionResult Create([Bind(Include = "ID,FirstName,LastName,Gender,Birthday,Venteran")] Fan fan)
        {
            if (ModelState.IsValid)
            {
                db.Fans.Add(fan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(fan);
        }

        // GET: Fans/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fan fan = db.Fans.Find(id);
            if (fan == null)
            {
                return HttpNotFound();
            }
            return View(fan);
        }

        // POST: Fans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FirstName,LastName,Gender,Birthday,Venteran")] Fan fan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fan).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fan);
        }

        // GET: Fans/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fan fan = db.Fans.Find(id);
            if (fan == null)
            {
                return HttpNotFound();
            }
            return View(fan);
        }

        // POST: Fans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Fan fan = db.Fans.Find(id);
            db.Fans.Remove(fan);
            db.SaveChanges();
            return RedirectToAction("Index");
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
