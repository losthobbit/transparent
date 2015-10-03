using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Transparent.Models
{
    public class FacebookUser
    {
        public FacebookUser(dynamic facebookUser)
        {
            Id = (string)facebookUser.id;
            Email = (string)facebookUser.email;
        }

        public string Id { get; set; }
        public string Email { get; set; }
    }
}