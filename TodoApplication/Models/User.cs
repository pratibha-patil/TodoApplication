using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApplication.Models
{
    public class User
    {
      
        public int Id { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }
        public virtual List<Todo> Todos { get; set; }
    }
}
