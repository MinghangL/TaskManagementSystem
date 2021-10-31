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

    public class DevProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DevProjects
        public ActionResult Index()
        {
            var devProjects = db.DevProjects.Include(d => d.ApplicationUser);
            return View(devProjects.ToList());
        }

        // GET: DevProjects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DevProject devProject = db.DevProjects.Find(id);
            if (devProject == null)
            {
                return HttpNotFound();
            }
            return View(devProject);
        }

        // GET: DevProjects/Create
        public ActionResult Create()
        {
            ViewBag.ApplicationUserID = new SelectList(db.Users, "Id", "Email");
            return View();
        }

        // POST: DevProjects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProjectName,ProjectDesc,ApplicationUserID,Budget,FinalCost,PlanStartDate,PlanEndDate,ActualStartDate,ActualEndDate,Deadline,Priority,IsNoticed,NoticeDate,NoticeTimes,IsCompleted,Percentage")] DevProject devProject)
        {
            if (ModelState.IsValid)
            {
                db.DevProjects.Add(devProject);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ApplicationUserID = new SelectList(db.Users, "Id", "Email", devProject.ApplicationUserID);
            return View(devProject);
        }

        // GET: DevProjects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DevProject devProject = db.DevProjects.Find(id);
            if (devProject == null)
            {
                return HttpNotFound();
            }
            ViewBag.ApplicationUserID = new SelectList(db.Users, "Id", "Email", devProject.ApplicationUserID);
            return View(devProject);
        }

        // POST: DevProjects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProjectName,ProjectDesc,ApplicationUserID,Budget,FinalCost,PlanStartDate,PlanEndDate,ActualStartDate,ActualEndDate,Deadline,Priority,IsNoticed,NoticeDate,NoticeTimes,IsCompleted,Percentage")] DevProject devProject)
        {
            if (ModelState.IsValid)
            {
                db.Entry(devProject).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            ViewBag.ApplicationUserID = new SelectList(db.Users, "Id", "Email", devProject.ApplicationUserID);
            return View(devProject);
        }

        // GET: DevProjects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DevProject devProject = db.DevProjects.Find(id);
            if (devProject == null)
            {
                return HttpNotFound();
            }
            return View(devProject);
        }

        // POST: DevProjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DevProject devProject = db.DevProjects.Find(id);
            db.DevProjects.Remove(devProject);
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

        public void SystemNoticeToPM(string userId)
        {
            // Projects pass the deadline
            List<DevProject> devProjects = db.DevProjects.Where(p => p.ApplicationUserID == userId).ToList();
            foreach (var devProject in devProjects)
            {
                //Id,ApplicationUserID,MessageType,SenderId,Title,Message,DevProjectId,DevTaskId,IsRead,NoticeDate
                DevNotice devNotice = new DevNotice();
                devNotice.ApplicationUserID = userId;
                devNotice.DevProjectId = devProject.Id;
                devNotice.IsRead = false;
                devNotice.NoticeDate = DateTime.Now;

                var today = DateTime.Now;
                if (devProject.NoticeDate.Date != today.Date)
                {
                    if (devProject.IsCompleted == true)
                    {
                        // Project add notice is completed
                        NoticeType noticeType = NoticeType.ProjectToPM;
                        devNotice.MessageType = noticeType;
                        devNotice.Title = "Project is completed.";
                        devNotice.Message = "Project is completed!";
                        var result = new DevNoticesController().SystemCallToCreateNotice(devNotice);
                        // need to change the info in Project of Notice
                        UpdateProjectNoticeInfo(devProject.Id);
                    }
                    else if (devProject.Deadline <= today.AddDays(1))
                    {
                        // Project add notice is delay
                        NoticeType noticeType = NoticeType.ProjectToPM;
                        devNotice.MessageType = noticeType;
                        devNotice.Title = "Project is delay.";
                        devNotice.Message = "Project passed the deadline!";
                        var result = new DevNoticesController().SystemCallToCreateNotice(devNotice);
                        // need to change the info in Project of Notice
                        UpdateProjectNoticeInfo(devProject.Id);
                    }
                }
            }

            // Tasks is completed, releations problem
            var projectIds = db.DevProjects.Where(p => p.ApplicationUserID == userId).Select(p => p.Id);
            foreach (var projectId in projectIds)
            {
                var today = DateTime.Now;
                var devTasks = db.DevTasks.Where(t => t.DevProjectId == projectId);
                foreach (var devTask in devTasks)
                {
                    DevNotice devNotice = new DevNotice();
                    devNotice.ApplicationUserID = userId;
                    devNotice.DevProjectId = projectId;
                    devNotice.IsRead = false;
                    devNotice.NoticeDate = DateTime.Now;
                    devNotice.DevTaskId = devTask.Id;
                    NoticeType noticeType = NoticeType.TaskToPM;
                    devNotice.MessageType = noticeType;

                    if (devTask.IsCompleted == true && devTask.NoticeDate.Date != today.Date)
                    {
                        // Task add a notice is completed
                        devNotice.Title = "Task is completed!";
                        devNotice.Message = "Task is completed!";
                        var result = new DevNoticesController().SystemCallToCreateNotice(devNotice);
                        // need to change the info in Task of Notice
                        var result2 = new DevTasksController().UpdateTaskNoticeInfo(devTask.Id);
                    }
                    else if (devTask.Deadline <= today.AddDays(1) && devTask.NoticeDate.Date != today.Date)
                    {
                        // Task add a notice pass deadline
                        devNotice.Title = "Task is delay";
                        devNotice.Message = "Task passed the deadline!";
                        var result = new DevNoticesController().SystemCallToCreateNotice(devNotice);
                        // need to change the info in Task of Notice
                        var result2 = new DevTasksController().UpdateTaskNoticeInfo(devTask.Id);
                    }
                }

            }

        }

        // Change the project notice info
        public bool UpdateProjectNoticeInfo(int projectId)
        {
            var devProject = db.DevProjects.Find(projectId);
            devProject.IsNoticed = true;
            devProject.NoticeDate = DateTime.Now;
            devProject.NoticeTimes += 1;
            if (ModelState.IsValid)
            {
                db.Entry(devProject).State = EntityState.Modified;
                db.SaveChanges();
            }
            return true;
        }

        // GET: DevProjects
        [Authorize(Roles = "PM")]
        public ActionResult Dashboard()
        {
            var userId = User.Identity.GetUserId();
            // Create Notice for PM
            SystemNoticeToPM(userId);
            int noticeCount = db.DevNotices.Where(n => n.ApplicationUserID == userId && n.IsRead != true).Count();

            var devProjects = db.DevProjects.Where(d => d.ApplicationUserID == userId)
                .OrderByDescending(p => p.Percentage);
            ViewBag.UserId = userId;
            DashboardViewModel dashboardView = new DashboardViewModel();
            dashboardView.UnreadNumber = noticeCount;
            dashboardView.DevProjects = devProjects.ToList();

            return View(dashboardView);
        }

        // GET: DevProjects/Create
        public ActionResult CreateProject()
        {
            return View();
        }

        // POST: DevProjects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProject([Bind(Include = "Id,ProjectName,ProjectDesc,ApplicationUserID,Budget,FinalCost,PlanStartDate,PlanEndDate,ActualStartDate,ActualEndDate,Deadline,Priority,IsNoticed,NoticeDate,NoticeTimes,IsCompleted,Percentage")] DevProject devProject)
        {

            devProject.ApplicationUserID = User.Identity.GetUserId();
            devProject.FinalCost = 0;
            devProject.ActualStartDate = devProject.PlanStartDate;
            devProject.ActualEndDate = devProject.PlanEndDate;
            devProject.IsNoticed = false;
            devProject.NoticeDate = devProject.PlanEndDate;
            devProject.NoticeTimes = 0;
            devProject.IsCompleted = false;
            devProject.Percentage = 0;

            if (ModelState.IsValid)
            {
                db.DevProjects.Add(devProject);
                db.SaveChanges();
                return RedirectToAction("Dashboard");
            }

            return View(devProject);
        }

        // GET: DevProjects/Details/5
        public ActionResult ProjectDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DevProject devProject = db.DevProjects.Find(id);
            if (devProject == null)
            {
                return HttpNotFound();
            }

            ProjectDetailsViewModel projectDetailsView = new ProjectDetailsViewModel();
            projectDetailsView.DevProject = devProject;
            projectDetailsView.DevTasks = db.DevTasks.Where(t => t.DevProjectId == id).ToList();

            return View(projectDetailsView);
        }

        // caculate the coast of a Project
        public int CaculateCost(int? projectId)
        {
            var result = 0;
            var PMCost = 1000;
            var SDCost = 0;

            var devTasks = db.DevTasks.Where(t => t.DevProjectId == projectId);
            foreach (var devTask in devTasks)
            {
                int salary = devTask.ApplicationUser.Salary;
                int subCount = (devTask.PlanEndDate.Date - devTask.PlanStartDate.Date).Days * 200;
                double betweenDays = (devTask.PlanEndDate - devTask.PlanStartDate).TotalDays;
                TimeSpan tt = devTask.PlanEndDate - devTask.PlanStartDate;
                double days = tt.TotalDays;

                SDCost += subCount;
            }

            var devProject = db.DevProjects.Find(projectId);
            result = PMCost + SDCost;
            return result;
        }

        //ShowProjectExceedBudget
        public ActionResult ShowProjectExceedBudget()
        {
            var userId = User.Identity.GetUserId();
            var devProjects = db.DevProjects.Where(d => d.ApplicationUserID == userId);
            List<DevProject> results = new List<DevProject>();
            foreach (var devProject in devProjects)
            {
                if (devProject.Budget < CaculateCost(devProject.Id))
                {
                    results.Add(devProject);
                }
            }

            return View(results);
        }

        //ShowProjectFinalCost
        public ActionResult ShowProjectFinalCost()
        {
            var userId = User.Identity.GetUserId();
            var devProjects = db.DevProjects.Where(d => d.ApplicationUserID == userId);
            //var devProjects = db.DevProjects.Where(d => d.ApplicationUserID == userId && d.IsCompleted ==true);
            List<ProjectCoastViewModel> results = new List<ProjectCoastViewModel>();
            foreach (var devProject in devProjects)
            {
                ProjectCoastViewModel projectCoastView = new ProjectCoastViewModel();
                projectCoastView.DevProjectId = devProject.Id;
                projectCoastView.Budget = devProject.Budget;
                projectCoastView.FinalCoat = CaculateCost(devProject.Id);
                projectCoastView.Name = devProject.ProjectName;
                projectCoastView.Deadline = devProject.Deadline;

                results.Add(projectCoastView);
            }

            return View(results);
        }
    }
}
