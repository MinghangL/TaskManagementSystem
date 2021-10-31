using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroupProject1025.Models
{
    public class UserDetailsViewModel
    {
        public ApplicationUser User { get; set; }
        public List<string> Roles { get; set; }
    }
}