using Demo.Infrastructures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Models
{
    public class AppUser : IdentityUser
    {
        public AppUser() : base() { }

        public virtual ICollection<Project> Projects { get; set; }
    }
}