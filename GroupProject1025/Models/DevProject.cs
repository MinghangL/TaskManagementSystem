using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GroupProject1025.Models
{
    public enum ProjectPriority 
    {
        UrgentAndImportment,
        ImportmentButNotUrgent,
        UrgentButNotImportment,
        NeitherUrgentNorImportment
    }

    public class DevProject
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ProjectName {get;set;}
        public string ProjectDesc { get; set; }
        public string ApplicationUserID {get;set;}
        public int Budget {get;set;}
        public int FinalCost {get;set;}
        public DateTime PlanStartDate {get;set;}
        public DateTime PlanEndDate {get;set;}
        public DateTime ActualStartDate {get;set;}
        public DateTime ActualEndDate {get;set;}
        public DateTime Deadline {get;set;}
        public ProjectPriority Priority {get;set;}
        public bool IsNoticed {get;set;}
        public DateTime NoticeDate {get;set;}
        public int NoticeTimes {get;set;}
        public bool IsCompleted {get;set;}
        public int Percentage {get;set;}

        public virtual ApplicationUser ApplicationUser { get; set; }
        //public virtual ICollection<DevTask> devTasks { get; set; }
        //public virtual ICollection<DevNotice> DevNotices { get; set; }
    }
}