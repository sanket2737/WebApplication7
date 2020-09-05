using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication7.Models
{
    public class ExtendedIdentityUserModel : IdentityUser
    {
        public ExtendedIdentityUserModel(string userName) : base(userName)
        {
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

    }
}
