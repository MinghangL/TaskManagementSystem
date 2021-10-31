using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GroupProject1025.Models
{
    public enum TaskPriority
    {
        UrgentAndImportment,
        ImportmentButNotUrgent,
        UrgentButNotImportment,
        NeitherUrgentNorImportment,
    }

    public class DevTask
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string TaskTitle {get;set;}
        public string Content { get; set; }
        public int DevProjectId {get;set;}
        public string ApplicationUserID {get;set;}
        public DateTime PlanStartDate {get;set;}
        public DateTime PlanEndDate {get;set;}
        public DateTime ActualStartDate {get;set;}
        public DateTime ActualEndDate {get;set;}
        public DateTime Deadline {get;set;}
        public string Comment {get;set;}
        public int Percentage {get;set;}
        public bool IsCompleted {get;set;}
        public TaskPriority Priority {get;set;}
        public bool IsNoticed {get;set;}
        public DateTime NoticeDate {get;set;}
        public int NoticeTimes {get;set;}

        public virtual ApplicationUser ApplicationUser { get; set; }
        //public virtual DevProject DevProject { get; set; }
        //public virtual ICollection<DevNotice> DevNotices { get; set; }
    }
}