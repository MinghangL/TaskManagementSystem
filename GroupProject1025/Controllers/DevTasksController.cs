using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GroupProject1025.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Specialized;

namespace GroupProject1025.Controllers
{
    //[Authorize(Roles = "SD")]
    public class DevTasksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DevTasks
        public ActionResult Index()
        {
            var devTasks = db.DevTasks.Include(d => d.ApplicationUser);
            return View(devTasks.ToList());
        }

        // GET: DevTasks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DevTask devTask = db.DevTasks.Find(id);
            if (devTask == null)
            {
                return HttpNotFound();
            }
            return View(devTask);
        }

        // GET: DevTasks/Create
        public ActionResult Create()
        {
            ViewBag.ApplicationUserID = new SelectList(db.Users, "Id", "Email");
            return View();
        }

        // POST: DevTasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TaskTitle,Content,DevProjectId,ApplicationUserID,PlanStartDate,PlanEndDate,ActualStartDate,ActualEndDate,Deadline,Comment,Percentage,IsCompleted,Priority,IsNoticed,NoticeDate,NoticeTimes")] DevTask devTask)
        {
            if (ModelState.IsValid)
            {
                db.DevTasks.Add(devTask);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ApplicationUserID = new SelectList(db.Users, "Id", "Email", devTask.ApplicationUserID);
            return View(devTask);
        }

        // GET: DevTasks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DevTask devTask = db.DevTasks.Find(id);
            if (devTask == null)
            {
                return HttpNotFound();
            }
            ViewBag.ApplicationUserID = new SelectList(db.Users, "Id", "Email", devTask.ApplicationUserID);
            return View(devTask);
        }

        // POST: DevTasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TaskTitle,Content,DevProjectId,ApplicationUserID,PlanStartDate,PlanEndDate,ActualStartDate,ActualEndDate,Deadline,Comment,Percentage,IsCompleted,Priority,IsNoticed,NoticeDate,NoticeTimes")] DevTask devTask)
        {
            if (ModelState.IsValid)
            {
                db.Entry(devTask).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ApplicationUserID = new SelectList(db.Users, "Id", "Email", devTask.ApplicationUserID);
            return View(devTask);
        }

        // GET: DevTasks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DevTask devTask = db.DevTasks.Find(id);
            if (devTask == null)
            {
                return HttpNotFound();
            }
            return View(devTask);
        }

        // POST: DevTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DevTask devTask = db.DevTasks.Find(id);
            db.DevTasks.Remove(devTask);
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

        [Authorize(Roles = "SD")]
        public ActionResult Desktop()
        {

            DesktopViewModel desktopView = new DesktopViewModel();
            var userId = User.Identity.GetUserId();
            SystemNoticeToSD(userId);
            int unreadCount = db.DevNotices.Where(n => n.ApplicationUserID == userId && n.IsRead != true).Count();
            var devTasks = db.DevTasks.Where(t => t.ApplicationUserID == userId);

            ViewBag.UserId = userId;
            desktopView.UnreadNumber = unreadCount;
            desktopView.DevTasks = devTasks.ToList();

            return View(desktopView);
        }

        // GET: DevTasks/Create
        public ActionResult CreateTask(int? id)
        {
            //ViewBag.ApplicationUserID = new SelectList(db.Users.Where(u=>u.Email != "admin@test.com"), "Id", "Email");

            List<ApplicationUser> developers = new List<ApplicationUser>();
            foreach (var user in db.Users)
            {
                var isRole = new AccountController().CheckIfUserInRole(user.Id, "SD");
                if (isRole)
                {
                    developers.Add(user);
                }
            }
            ViewBag.ApplicationUserID = new SelectList(developers, "Id", "Email");

            ViewBag.ProjectId = id;

            return View();
        }

        // POST: DevTasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTask([Bind(Include = "Id,TaskTitle,Content,DevProjectId,ApplicationUserID,PlanStartDate,PlanEndDate,ActualStartDate,ActualEndDate,Deadline,Comment,Percentage,IsCompleted,Priority,IsNoticed,NoticeDate,NoticeTimes")] DevTask devTask)
        {
            devTask.ActualStartDate = devTask.PlanStartDate;
            devTask.ActualEndDate = devTask.PlanEndDate;
            devTask.Comment = "";
            devTask.Percentage = 0;
            devTask.IsCompleted = false;
            devTask.IsNoticed = false;
            devTask.NoticeDate = devTask.PlanEndDate;
            devTask.NoticeTimes = 0;

            if (ModelState.IsValid)
            {
                db.DevTasks.Add(devTask);
                db.SaveChanges();
                //return RedirectToAction( "Dashboard", "DevProjects");
                return RedirectToAction("ProjectDetails", "DevProjects", new { id = devTask.DevProjectId });
            }

            List<ApplicationUser> developers = new List<ApplicationUser>();
            foreach (var user in db.Users)
            {
                var isRole = new AccountController().CheckIfUserInRole(user.Id, "SD");
                if (isRole)
                {
                    developers.Add(user);
                }
            }
            ViewBag.ApplicationUserID = new SelectList(developers, "Id", "Email");
            //ViewBag.ApplicationUserID = new SelectList(db.Users, "Id", "Email", devTask.ApplicationUserID);
            return View(devTask);
        }

        // GET: DevTasks/Details/5
        public ActionResult TaskDetails(int? id, int? projectId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DevTask devTask = db.DevTasks.Find(id);
            if (devTask == null)
            {
                return HttpNotFound();
            }
            ViewBag.TaskId = id;
            ViewBag.ProjectId = projectId;
            return View(devTask);
        }

        // GET: DevTasks/Edit/5
        public ActionResult ChangePercentage(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DevTask devTask = db.DevTasks.Find(id);
            if (devTask == null)
            {
                return HttpNotFound();
            }
            if(devTask.IsCompleted == true)
            {
                // show an error page
                return View(devTask);
            }

            return View(devTask);
        }

        // POST: DevTasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePercentage([Bind(Include = "Id,TaskTitle,Content,DevProjectId,ApplicationUserID,PlanStartDate,PlanEndDate,ActualStartDate,ActualEndDate,Deadline,Comment,Percentage,IsCompleted,Priority,IsNoticed,NoticeDate,NoticeTimes")] DevTask devTask)
        {

            if (devTask == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                db.Entry(devTask).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Desktop");
            }

            return View(devTask);
        }

        // GET: DevTasks/Details/5
        public ActionResult CompleteTask(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DevTask devTask = db.DevTasks.Find(id);
            if (devTask == null)
            {
                return HttpNotFound();
            }
            if (devTask.IsCompleted==true)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            devTask.Percentage = 100;
            devTask.ActualEndDate = DateTime.Now;
            devTask.IsCompleted = true;

            // Send message to Manager?=>no, we did it when manager login in

            if (ModelState.IsValid && devTask.IsCompleted != true)
            {
                db.Entry(devTask).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Desktop");
            }

            return View(devTask);
        }

        //CommentTask
        // GET: DevTasks/Edit/5
        public ActionResult CommentTask(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DevTask devTask = db.DevTasks.Find(id);
            if (devTask == null)
            {
                return HttpNotFound();
            }
            if (devTask.IsCompleted != true)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            return View(devTask);
        }

        // POST: DevTasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CommentTask([Bind(Include = "Id,TaskTitle,Content,DevProjectId,ApplicationUserID,PlanStartDate,PlanEndDate,ActualStartDate,ActualEndDate,Deadline,Comment,Percentage,IsCompleted,Priority,IsNoticed,NoticeDate,NoticeTimes")] DevTask devTask)
        {

            if (devTask == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                db.Entry(devTask).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Desktop");
            }

            return View(devTask);
        }


        // GET: DevTasks/Delete/5
        public ActionResult DeleteTask(int? id, int? projectId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DevTask devTask = db.DevTasks.Find(id);
            if (devTask == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProjectId = projectId;
            return View(devTask);
        }
        // Notice the SD
        public void SystemNoticeToSD(string userId)
        {
            var today = DateTime.Now;
            var devTasks = db.DevTasks.Where(t => t.ApplicationUserID == userId).ToList();
            foreach (var devTask in devTasks)
            {
                DevNotice devNotice = new DevNotice();
                devNotice.ApplicationUserID = userId;
                devNotice.DevProjectId = devTask.DevProjectId;
                devNotice.IsRead = false;
                devNotice.NoticeDate = DateTime.Now;
                devNotice.DevTaskId = devTask.Id;
                NoticeType noticeType = NoticeType.TaskToPM;
                devNotice.MessageType = noticeType;
                devNotice.Title = "Task is delay";
                devNotice.Message = "Task passed the deadline!";
                
                // Task add a notice pass deadline
                if (devTask.Deadline <= today.AddDays(1) && DateTime.Now.Date != devTask.NoticeDate.Date)
                {
                    var result = new DevNoticesController().SystemCallToCreateNotice(devNotice);
                    // change the task's notice info
                    var result2 = UpdateTaskNoticeInfo(devTask.Id);
                }
            }
        }

        //ShowTasksPassedDeadline
        public ActionResult ShowTasksPassedDeadline()
        {
            var today = DateTime.Now;
            var devTasks = db.DevTasks.Where(t => t.IsCompleted != true && t.Deadline <= today);

            return View(devTasks.ToList());
        }

        public bool UpdateTaskNoticeInfo(int taskId)
        {
            var devTask = db.DevTasks.Find(taskId);
            devTask.NoticeDate = DateTime.Now;
            devTask.NoticeTimes += 1;
            devTask.IsNoticed = true;
            if (ModelState.IsValid)
            {
                db.Entry(devTask).State = EntityState.Modified;
                db.SaveChanges();
            }

            return true;
        }
    }
}
