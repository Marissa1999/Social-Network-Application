using SocialNetworkApplication.ActionFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SocialNetworkApplication.Controllers
{
    [LoginFilter]
    public class CommentController : Controller
    {
        Models.ClientsEntities database = new Models.ClientsEntities();

        // GET: Comment
        public ActionResult Index(int id)
        {
            Session["picture_id"] = id;
            Models.Picture thePicture = database.Pictures.SingleOrDefault(p => p.picture_id == id);
            int profile_id = (int)Session["user_id"];
            ViewBag.theCommentLike = database.Comment_Like.Where(c=>c.profile_id == profile_id);
            return View(thePicture);
        }

        public ActionResult CommentIndex()
        {
            int id = int.Parse(Session["user_id"].ToString());
            Models.Profile theProfile = database.Profiles.SingleOrDefault(p => p.user_id == id);
            return View(theProfile);
        }

        public ActionResult CommentDetails(int id)
        {
            Models.Comment theComment = database.Comments.SingleOrDefault(c => c.comment_id == id);
            theComment.read = 1;
            database.SaveChanges();
            return RedirectToAction("CommentIndex", new { id = theComment.comment_id });
        }

        public ActionResult CommentLikeDetails(int id)
        {
            Models.Comment_Like theComment = database.Comment_Like.SingleOrDefault(c => c.comment_id == id);
            theComment.read = 1;
            database.SaveChanges();
            return RedirectToAction("LikeCommentIndex", new { id = theComment.comment_id });
        }

        public ActionResult LikeCommentIndex()
        {
            int id = int.Parse(Session["user_id"].ToString());       
            Models.Profile thePicture = database.Profiles.SingleOrDefault(p => p.user_id == id);
            ViewBag.theCommentLike = database.Comment_Like;
            return View(thePicture);
        }

        public ActionResult LikeComment(int id)
        {
            // TODO: Add insert logic here  
            int picture_id = int.Parse(Session["picture_id"].ToString());
            Models.Picture thePicture = database.Pictures.SingleOrDefault(p => p.picture_id == picture_id);
            Models.Comment theComment = database.Comments.SingleOrDefault(c => c.comment_id == id);

            Models.Comment_Like likeComment = new Models.Comment_Like()
            {
                comment_id = theComment.comment_id,
                profile_id = thePicture.profile_id,
                timestamp = DateTime.Now,
                read = 0
            };

            database.Comment_Like.Add(likeComment);
            database.SaveChanges();

            return RedirectToAction("Index", new { id = theComment.picture_id});
        }


        public ActionResult UnLikeComment(int id)
        {
            Models.Comment_Like likeComment = database.Comment_Like.SingleOrDefault(p => p.comment_id == id);
            Models.Comment theComment = database.Comments.SingleOrDefault(p => p.comment_id == id);
            database.Comment_Like.Remove(likeComment);
            database.SaveChanges();

            return RedirectToAction("Index", new { id = theComment.picture_id });
        }


        // GET: Comment/Edit/5
        public ActionResult Details(int id)
        {
            Models.Comment theComment = database.Comments.SingleOrDefault(c => c.comment_id == id);
            theComment.read = 1;
            database.SaveChanges();
            return View(theComment);
        }


        // GET: Comment/Create
        public ActionResult Create()
        {
            int id = int.Parse(Session["user_id"].ToString());
            ViewBag.theProfile = database.Profiles.SingleOrDefault(p => p.user_id == id);
            return View();
        }

        // POST: Comment/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                int id = int.Parse(Session["picture_id"].ToString());
                Models.Picture thePicture = database.Pictures.SingleOrDefault(p => p.picture_id == id);

                Models.Comment newComment = new Models.Comment()
                {
                    picture_id = thePicture.picture_id,
                    profile_id = thePicture.profile_id,               
                    comment1 = collection["comment1"],
                    timestamp = DateTime.Now,
                    read = 0
                };
                database.Comments.Add(newComment);
                database.SaveChanges();

                return RedirectToAction("Index", new { id = id });

            }
            catch
            {
                return View();
            }
        }

        // GET: Comment/Edit/5
        public ActionResult Edit(int id)
        {
            Models.Comment theComment = database.Comments.SingleOrDefault(c => c.comment_id == id);
            return View(theComment);
        }

        // POST: Comment/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here              
                Models.Comment theComment = database.Comments.SingleOrDefault(c => c.comment_id == id);
        
                theComment.comment1 = collection["comment1"];
                theComment.timestamp = DateTime.Now;
                theComment.read = 1;

                database.SaveChanges();

                return RedirectToAction("Index", new { id = theComment.picture_id });

            }
            catch
            {
                return View();
            }
        }

        // GET: Comment/Delete/5
        public ActionResult Delete(int id)
        {
            Models.Comment theComment = database.Comments.SingleOrDefault(c => c.comment_id == id);
            return View(theComment);
        }

        // POST: Comment/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                Models.Comment theComment = database.Comments.SingleOrDefault(c => c.comment_id == id);
                database.Comments.Remove(theComment);
                database.SaveChanges();

                return RedirectToAction("Index", new { id = theComment.picture_id });
            }
            catch
            {
                return View();
            }
        }
    }
}
