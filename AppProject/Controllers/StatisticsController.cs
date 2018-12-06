using AppProject.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppProject.Controllers
{
    //========================= Out source d3 scripts ==========================
    //========================= Group by and 2 join LINQs ======================
    public class StatisticsController : Controller
    {
        private PostCommentsContext db = new PostCommentsContext();
        private UserAccountDbContext dbAccount = new UserAccountDbContext();

        // First d3: circle tree sctruced by writters post titles and comment titles
        public ActionResult Index()
        {
            string appPath = Request.PhysicalApplicationPath;
            string filePath = appPath + "/Content/"+"data.csv"; //Write to the file the structure
            StreamWriter file;
            file = System.IO.File.CreateText(filePath);
            var writters = (from writter in db.Posts select writter.WrittenBy);
            writters = writters.Distinct();

            file.WriteLine("id,value"); //Headers
            file.WriteLine("root,"); //The root of the tree
            foreach (var writter in writters)
            {
                file.WriteLine("root." +writter +","); //Root.{Writers},
                foreach (var post in db.Posts)
                {
                    if (writter == post.WrittenBy) //Same writter to the post
                    {
                        file.WriteLine("root." + writter + "." + post.Title + ","); //Root.{Writers}.{Post Titles},
                        foreach (var comment in post.Comments)
                        {
                            if (post.PostID == comment.PostID && writter == post.WrittenBy) //Same writter to the post and same post ID to the comment
                            {
                                file.WriteLine("root." + writter + "." + post.Title + "." + comment.Title + ","); //Root.{Writeers}.{Post Titles}.{Comment titles}
                            }
                        }
                    }
                }
            }
            file.Flush();
            file.Close(); //Close file - no more writing

            return View();
        }

        // Second d3: Spiral comments view - how many comments was posted each day for the last year
        //From https://bl.ocks.org/arpitnarechania/027e163073864ef2ac4ceb5c2c0bf616
        public ActionResult Comments()
        {
                int[] dates = new int[366]; //Count how many comments per day
                for (int i = 1; i <= 365; i++) //Initial array
                {
                    dates[i] = 0;
                }
                foreach (var comment in db.Comments)
                {
                if (comment.PublishDate > DateTime.Now.AddYears(-1)) //Check if the comment was posted this year
                    dates[comment.PublishDate.DayOfYear]++; //Add to the dates array at the DayOfYear number
                }
                ViewBag.Dates = dates;
            ViewBag.PassedArray = dates; //Send to ViewBag
            return View(dates);
        }

        //2 Group by and 2 join LINQs queries
        public ActionResult UsersActivity()
        {
            //Get for each writter the number of the total posts he/she posted
            var CountPosts = db.Posts.ToList().GroupBy(n => n.WrittenBy).
                     Select(group =>
                         new
                         {
                             PostWrittenBy = group.Key,
                             Count = group.Count()
                         });

            //Get for each writter the number of the total comments he/she posted
            var CountComments = db.Comments.ToList().GroupBy(n => n.WrittenBy).
                    Select(group =>
                        new
                        {
                             CommentWrittenBy = group.Key,
                             Count = group.Count()
                         });

            //JOIN each account the post number
            var BeforeEnd = from a in dbAccount.UserAccounts.ToList()
                         join p in CountPosts.ToList()
                         on a.FirstName + " " + a.LastName equals p.PostWrittenBy
                         select new
                         {
                             UserName = p.PostWrittenBy,
                             PostNum = p.Count
                         };

            //JOIN each account the comments number
            var Results = from a in BeforeEnd
                          join p in CountComments.ToList()
                            on a.UserName equals p.CommentWrittenBy
                            select new
                            {
                                UserName = a.UserName,
                                PostNum = a.PostNum,
                                CommentNum = p.Count
                            };

            ViewBag.Results = Results; //A table of Account name , number of posts, number of comments

            return View();
        }
    }
}