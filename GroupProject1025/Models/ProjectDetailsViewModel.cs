using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroupProject1025.Models
{
    public class ProjectDetailsViewModel
    {
        public DevProject DevProject { get; set; }
        public List<DevTask> DevTasks { get; set; }
    }
}