using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroupProject1025.Models
{
    public class ProjectCoastViewModel
    {
        public int DevProjectId { get; set; }
        public string Name { get; set; }
        public DateTime Deadline { get; set; }
        public int Budget { get; set; }
        public int FinalCoat { get; set; }
    }
}