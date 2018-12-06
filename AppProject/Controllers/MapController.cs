using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AppProject.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace AppProject.Controllers
{
    //===================== Google map and web service =====================
    public class MapController : Controller
    {
        private MarkDbContext db = new MarkDbContext();

        // GET: Map of google
        public ActionResult Index()
        {
            string markers = "[";
            foreach (var mark in db.Marks) //Get all marks from db, build it as json array
            {
                markers += "{";
                markers += string.Format("'lat': '{0}',", mark.Lat); //X cordinate
                markers += string.Format("'lng': '{0}',", mark.Lng); //Y cordinate
                markers += string.Format("'title': '{0}',", mark.Title); //The name/title of the mark
                markers += "},";
            }
            markers += "];";
            ViewBag.Markers = markers;

            return View(db.Marks.ToList());
        }

        // GET: Map/Create 
        public ActionResult Create() //insert a new mark
        {
                return View();
           
        }

        // POST: Map/Create //insert a new mark
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MarkID,Lat,Lng,Title")] Mark mark)
        {
                if (ModelState.IsValid)
                {
                    db.Marks.Add(mark);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(mark);
        }


        // GET: Map/Delete/5 Delete a new mark
        public ActionResult Delete(int? id)
        {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Mark mark = db.Marks.Find(id);
                if (mark == null)
                {
                    return HttpNotFound();
                }
                return View(mark);
        }

        // POST: Map/Delete/5 Delete a new mark
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Username"].ToString() == "admin")
            {
                Mark mark = db.Marks.Find(id);
                db.Marks.Remove(mark);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
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

        public ActionResult Domain()
        {
            return View();

        }

        //================ Web Service ==================
        //Get hostname information by hostname string provided by the client
        [HttpGet]
        public string Domain(string hostname)
        {
            //input validation
            string host = Regex.Replace(hostname, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);

            //Creating Web Service reference object  
            net.webservicex.www.whois objPayRef = new net.webservicex.www.whois();
            //calling and storing web service output into the variable  
            if (host != null)
            {
                string Result = objPayRef.GetWhoIS(host); //Action
                //returning josn result  
                //return Json(Result, JsonRequestBehavior.AllowGet);
                return Result;
            }
            return null;
        }
    }
}
