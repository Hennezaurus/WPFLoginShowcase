using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFExample
{
    class LoginData
    {
        // Public properties - username and password
        public string Username { get; set; }
        public string Password { get; set; }

        // Simple constructor taking two strings, a username and password
        public LoginData(string user, string pass)
        {
            Username = user;
            Password = pass;
        }
    }
}
