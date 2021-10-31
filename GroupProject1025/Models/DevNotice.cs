using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GroupProject1025.Models
{
    public enum NoticeType
    {
        ProjectToPM,
        TaskToPM,
        TaskToSD,
        SDToPM
    }

    public class DevNotice
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ApplicationUserID { get;set;}
        public NoticeType MessageType {get;set;}
        public string SenderId {get;set;}
        public string Title { get; set; }
        public string Message {get;set;}
        public int DevProjectId {get;set;}
        public int DevTaskId {get;set;}
        public bool IsRead {get;set;}
        public DateTime NoticeDate {get;set;}

        public virtual ApplicationUser ApplicationUser { get; set; }
        //public virtual DevProject DevProject { get; set; }
        //public virtual DevTask Task { get; set; }
    }
}