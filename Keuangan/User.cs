using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keuangan
{
    public class User
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }

        public User(int id, string username, string role, string token=null)
        {
            this.ID = id;
            this.Username = username;
            this.Role = role;
            this.Token = token;
        }
    }
}
