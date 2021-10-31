using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroupProject1025.Models
{
    public class DesktopViewModel
    {
        public int UnreadNumber { get; set; }
        public List<DevTask> DevTasks { get; set; }
    }
}