using AppProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AppProject.Controllers
{
    public class AccountController : Controller
    {
        //Session["UserID"] != null is a session check = user is logged in
        PostCommentsContext db = new PostCommentsContext();

        // GET: AccountTest
        public ActionResult Index() //Login page
        {
            if (Session["UserID"] != null) //If user has session = logged in so redirect him to menu
            {
                return RedirectToAction("Menu");
            }
            else
            {
                return View();
            }
        }

        //Login Post request
        [HttpPost]
        public ActionResult Index(UserAccount AccountP)
        {

                using (UserAccountDbContext dbn = new UserAccountDbContext())
                {
                    var usr = dbn.UserAccounts.Where(u => u.Username == AccountP.Username && u.Password == AccountP.Password).FirstOrDefault();
                    if (AccountP != null && usr!=null)
                    {
                        Session["UserID"] = usr.UserID.ToString(); //Session stores the UserID
                        Session["Username"] = AccountP.Username.ToString(); //Session stores the username
                        Session["Firstname"] = usr.FirstName.ToString(); //Session stores the First name for writing posts
                        Session["Lastname"] = usr.LastName.ToString(); //Session stores the Last name for writing posts
                    return RedirectToAction("Menu");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Username or password is invalid");
                    }
                }
            return View();
        }

        //Kill session
        public ActionResult Logout()
        {
            if (Session["UserID"] != null)
            {
                do
                {
                    Session.Remove("UserID");    // removes the key
                    Session.Remove("Username");
                } while (Session["UserID"] != null);// tests that the remove worked
            }
            return RedirectToAction("Index");
        }


        //After login page
        public ActionResult Menu()
        {
            if (Session["UserID"] != null)
            {

                return View((from a in db.Posts orderby a.PublishedDate descending select a).ToList()); //Show list of all posts + actions
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult Register() //REGISTRATION (NEW AUTHENTICATION USER)
        {
            return View();
        }


        //Registeration page
        [HttpPost]
        public ActionResult Register(UserAccount AccountP)
        {
            if (ModelState.IsValid)
            {
                using (UserAccountDbContext dbn = new UserAccountDbContext())
                {
                    bool canRegister = true; //defualt
                    foreach (var account in dbn.UserAccounts)
                    {
                        if (account.Username == AccountP.Username) //Check if such username exists
                        {
                            canRegister = false; //Username is taken
                        }
                    }
                    if (canRegister) //If no one has this username
                    {
                        dbn.UserAccounts.Add(AccountP);
                        dbn.SaveChanges();
                    }
                }
                ModelState.Clear();
                ViewBag.Message = AccountP.FirstName + " " + AccountP.LastName + " seccessfully registered";
            }
            return View();
        } 


        // GET: Account/Details/5 Show posts as they look like in the Blog
        public ActionResult Details(int? id)
        {
            if (Session["UserID"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Post post = db.Posts.Find(id);
                if (post == null)
                {
                    return HttpNotFound();
                }
                return View(post);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }



        // GET: Account/Create Add a new Post to the web
        public ActionResult Create()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // POST: Account/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PostID,Title,Website,Text,Image,Video")] Post post)
        {
            if (Session["UserID"] != null)
            {
                post.PublishedDate = DateTime.Now;
                post.WrittenBy = (Session["Firstname"].ToString() +" "+ Session["Lastname"].ToString());
                if (post.Image != null)
                {
                    post.IsImage = true;
                }
                else
                {
                    post.IsImage = false;
                }
                if (post.Video != null)
                {
                    post.IsVideo = true;
                }
                else
                {
                    post.IsVideo = false;
                }
                if (ModelState.IsValid)
                {
                    db.Posts.Add(post);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(post);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Account/Edit/5 Edit a post
        public ActionResult Edit(int? id)
        {
            if (Session["UserID"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Post post = db.Posts.Find(id);
                if (post == null)
                {
                    return HttpNotFound();
                }
                return View(post);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // POST: Account/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PostID,Title,Website,Text,Image,Video")] Post post)
        {
            if (Session["UserID"] != null)
            {
                post.PublishedDate = DateTime.Now;
                post.WrittenBy = (Session["Firstname"].ToString() + " " + Session["Lastname"].ToString());
                if (post.Image != null)
                {
                    post.IsImage = true;
                }
                else
                {
                    post.IsImage = false;
                }
                if (post.Video != null)
                {
                    post.IsVideo = true;
                }
                else
                {
                    post.IsVideo = false;
                }
                if (ModelState.IsValid)
                {
                    db.Entry(post).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Edit");
                }
                return View(post);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Account/Delete/5 Delete a post for admin only
        public ActionResult Delete(int? id)
        {
            if (Session["Username"].ToString() == "admin") //Only admin is allowed to delete
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Post post = db.Posts.Find(id);
                if (post == null)
                {
                    return HttpNotFound();
                }
                return View(post);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // POST: Account/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Username"].ToString() == "admin") //Only admin is allowed to delete
            {
                Post post = db.Posts.Find(id);
                db.Posts.Remove(post);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }



        public ActionResult ViewComments(int? id) //View (edit or delete for admin) all comments for spesific post
        {
            if (Session["UserID"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Post post = db.Posts.Find(id);
                if (post == null)
                {
                    return HttpNotFound();
                }
                return View(post);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult EditComment(int? id)
        {
            if (Session["UserID"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Comment comment = db.Comments.Find(id);
                if (comment == null)
                {
                    return HttpNotFound();
                }
                return View(comment);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // POST: Account/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditComment([Bind(Include = "CommentID,PostID,Title,WrittenBy,Website,Text")] Comment comment)
        {
            if (Session["UserID"] != null)
            {
                comment.PublishDate = DateTime.Now;
                if (ModelState.IsValid)
                {
                    var postidReturn = comment.PostID;
                    db.Entry(comment).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("ViewComments/"+ postidReturn);
                }
                return View(comment);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }


        public ActionResult DeleteComment(int? id)
        {
            if (Session["UserID"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Comment comment = db.Comments.Find(id);
                if (comment == null)
                {
                    return HttpNotFound();
                }
                return View(comment);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // POST: Comments/Delete/5 Only admin is allowed to delete
        [HttpPost, ActionName("DeleteComment")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCommentConfirmed(int id)
        {
            if (Session["Username"].ToString() == "admin") //Only admin is allowed to delete
            {
                Comment comment = db.Comments.Find(id);
                var postidReturn = comment.PostID;
                db.Comments.Remove(comment);
                db.SaveChanges();
                return RedirectToAction("ViewComments/"+postidReturn); //Return to view comments of the comment's post
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
    }
}