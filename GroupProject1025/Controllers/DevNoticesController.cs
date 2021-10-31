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
    public class DevNoticesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DevNotices
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var devNotices = db.DevNotices.Include(d => d.ApplicationUser);
            return View(devNotices.ToList());
        }

        // GET: DevNotices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DevNotice devNotice = db.DevNotices.Find(id);
            if (devNotice == null)
            {
                return HttpNotFound();
            }
            return View(devNotice);
        }

        // GET: DevNotices/Create
        public ActionResult Create()
        {
            ViewBag.ApplicationUserID = new SelectList(db.Users, "Id", "Email");
            return View();
        }

        // POST: DevNotices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ApplicationUserID,MessageType,SenderId,Title,Message,DevProjectId,DevTaskId,IsRead,NoticeDate")] DevNotice devNotice)
        {
            if (ModelState.IsValid)
            {
                db.DevNotices.Add(devNotice);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ApplicationUserID = new SelectList(db.Users, "Id", "Email", devNotice.ApplicationUserID);
            return View(devNotice);
        }

        // GET: DevNotices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DevNotice devNotice = db.DevNotices.Find(id);
            if (devNotice == null)
            {
                return HttpNotFound();
            }
            ViewBag.ApplicationUserID = new SelectList(db.Users, "Id", "Email", devNotice.ApplicationUserID);
            return View(devNotice);
        }

        // POST: DevNotices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ApplicationUserID,MessageType,SenderId,Title,Message,DevProjectId,DevTaskId,IsRead,NoticeDate")] DevNotice devNotice)
        {
            if (ModelState.IsValid)
            {
                db.Entry(devNotice).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ApplicationUserID = new SelectList(db.Users, "Id", "Email", devNotice.ApplicationUserID);
            return View(devNotice);
        }

        // GET: DevNotices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DevNotice devNotice = db.DevNotices.Find(id);
            if (devNotice == null)
            {
                return HttpNotFound();
            }
            return View(devNotice);
        }

        // POST: DevNotices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DevNotice devNotice = db.DevNotices.Find(id);
            db.DevNotices.Remove(devNotice);
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

        // GET: DevNotices/Create
        //SendUrgentMessage
        public ActionResult SendUrgentMessage(int? id)
        {
            var devTask = db.DevTasks.Find(id);
            var projectId = devTask.DevProjectId;
            var devProject = db.DevProjects.Find(projectId);

            ViewBag.ProjectId = devTask.DevProjectId;
            ViewBag.ManagerId = devProject.ApplicationUserID;
            ViewBag.SenderId = devTask.ApplicationUserID;
            ViewBag.TaskId = id;

            return View();
        }

        // POST: DevNotices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendUrgentMessage([Bind(Include = "Id,ApplicationUserID,MessageType,SenderId,Title,Message,DevProjectId,DevTaskId,IsRead,NoticeDate")] DevNotice devNotice)
        {
            NoticeType messageType = NoticeType.SDToPM;

            devNotice.IsRead = false;
            devNotice.NoticeDate = DateTime.Now;
            devNotice.MessageType = messageType;
            
            if (ModelState.IsValid)
            {
                db.DevNotices.Add(devNotice);
                db.SaveChanges();
                return RedirectToAction("Desktop", "DevTasks");
            }

            return View(devNotice);
        }

        public bool SystemCallToCreateNotice(DevNotice devNotice)
        {
            if (ModelState.IsValid)
            {
                db.DevNotices.Add(devNotice);
                db.SaveChanges();
                return true;
            }

            return false;
        }

        // ShowNoticeList
        // GET: DevNotices
        public ActionResult ShowNoticeList(string userId, string actionName, string controllerName)
        {
            var devNotices = db.DevNotices.Where(n=>n.ApplicationUserID == userId);

            ViewBag.ActionName = actionName;
            ViewBag.ControllerName = controllerName;
            List<DevNotice> tmpNotices = new List<DevNotice>();
            foreach (var notice in devNotices)
            {
                DevNotice tmpN = notice;
                if(notice.SenderId != null)
                {
                    var user = db.Users.Find(notice.SenderId);
                    tmpN.SenderId = user.Email;
                }else {
                    tmpN.SenderId = "System Notification";
                }
                tmpNotices.Add(tmpN);
            }

            return View(tmpNotices);
            //return View(devNotices.ToList());
        }

        // NoticeDetails
        // GET: DevNotices/Details/5
        public ActionResult NoticeDetails(int? id, string userId, string actionName, string controllerName)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DevNotice devNotice = db.DevNotices.Find(id);
            if (devNotice == null)
            {
                return HttpNotFound();
            }
            devNotice.IsRead = true;
            if (ModelState.IsValid)
            {
                db.Entry(devNotice).State = EntityState.Modified;
                db.SaveChanges();
                //return RedirectToAction("ShowNoticeList", "DevNotices");
            }

            ViewBag.UserId = userId;
            ViewBag.ActionName = actionName;
            ViewBag.ControllerName = controllerName;

            return View(devNotice);
        }

    }
}
