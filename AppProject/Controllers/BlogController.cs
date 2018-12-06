using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AppProject.Models;
using System.Net.Mime;
using BayesSharp;
using System.IO;

namespace AppProject.Controllers
{
    public class BlogController : Controller
    {
        public BayesSimpleTextClassifier c = new BayesSimpleTextClassifier(); //Naive Bayes object https://github.com/afonsof/BayesSharp
        private PostCommentsContext db = new PostCommentsContext();

        public BlogController()
        {
            string appPath = HttpRuntime.AppDomainAppPath; //Application path
            string filePath = appPath + "/Content/" + "training_data.csv"; //Training data

            var reader = new StreamReader(filePath);
            
            List<string> listA = new List<string>();//Text (x)
            List<string> listB = new List<string>();//Label (y)
            while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    listA.Add(values[0]);
                    listB.Add(values[1]);
                }

            for (int i=0;i<listA.Count(); i++)
            {
                c.Train(listB[i], listA[i]); //Trining part
            }

        }

        // GET: Blog - First view or by search
        public ActionResult Index(string searchString, string authorsearch, int? yearsearch) //Search function
        {
            var postsOrigin = from a in db.Posts orderby a.PublishedDate descending select a; //LINQ Select ALL posts and order by date
            var posts = from a in db.Posts select a; //LINQ

            var Authors = new List<string>();
            var AuthorsQry = from d in db.Posts
                             orderby d.WrittenBy//from Posts get the authors's names and order it by name
                             select d.WrittenBy;
            Authors.AddRange(AuthorsQry.Distinct()); //Get all names and remove duplicates
            ViewBag.AuthorsList = Authors; //Put it inside ViewBag

            if (!String.IsNullOrEmpty(searchString)) //seach with text
            {
                if (!String.IsNullOrEmpty(authorsearch)) //search with name
                {
                    if (yearsearch != null && int.MaxValue > yearsearch && int.MinValue < yearsearch) //search with year and input validation for year (int)
                    {
                        posts = from b in db.Posts
                                where b.Text.Contains(searchString) && b.PublishedDate.Year == yearsearch && b.WrittenBy == authorsearch
                                orderby b.PublishedDate descending //Get posts by year, author and context
                                select b;
                        return View(posts.ToList());
                    }
                    else //search without year
                    {
                        posts = from b in db.Posts
                                where b.Text.Contains(searchString) && b.WrittenBy == authorsearch
                                orderby b.PublishedDate descending //Get posts by author and context
                                select b;
                        return View(posts.ToList());
                    }
                }
                else //search without author name
                {
                    if (yearsearch != null && int.MaxValue > yearsearch && int.MinValue < yearsearch) //search with year and input validation for year (int)
                    {
                        posts = from b in db.Posts
                                where b.Text.Contains(searchString) && b.PublishedDate.Year == yearsearch
                                orderby b.PublishedDate descending //Get posts by year and context
                                select b;
                        return View(posts.ToList());
                    }
                    else //search only by context
                    {
                        posts = from b in db.Posts
                                where b.Text.Contains(searchString)
                                orderby b.PublishedDate descending //Get posts only by context 
                                select b;
                        return View(posts.ToList());
                    }
                }
            }
            else //search without context
            {
                if (!String.IsNullOrEmpty(authorsearch)) //search with author name
                {
                    if (yearsearch != null && int.MaxValue > yearsearch && int.MinValue < yearsearch) //search with year
                    {
                        posts = from b in db.Posts
                                where b.PublishedDate.Year == yearsearch && b.WrittenBy == authorsearch
                                orderby b.PublishedDate descending //Get posts by author name and year
                                select b;
                        return View(posts.ToList());
                    }
                    else //search without year
                    {
                        posts = from b in db.Posts
                                where b.WrittenBy == authorsearch
                                orderby b.PublishedDate descending //Get posts only by author name
                                select b;
                        return View(posts.ToList());
                    }
                }
                else //search without author name
                {
                    if (yearsearch != null && int.MaxValue > yearsearch && int.MinValue < yearsearch) //search with year
                    {
                        posts = from b in db.Posts
                                where b.PublishedDate.Year == yearsearch
                                orderby b.PublishedDate descending //Get posts only by year
                                select b;
                        return View(posts.ToList());
                    }
                }
            }
            return View(postsOrigin.ToList()); //If something happend return the 
        }

        //Add comment POST request
        [HttpPost]
        public string AddComment([Bind(Include = "CommentID,PostID,WrittenBy,Title,Website,Text")] Comment comment)
        {
            comment.PublishDate = DateTime.Now; //Take the current time
            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                var result = c.Classify(comment.Text); //!!!!!Classification!!!!! (good or bad review)
                foreach (var item in result)
                {
                    if (item.Key == "1") { return "You post a positive comment"; } //The first class is the most possible classification of the two
                    else { return "You post a negative comment"; }

                }      
            }
            return "Failed";
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
