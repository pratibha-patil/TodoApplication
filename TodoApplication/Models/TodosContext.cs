using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApplication.Models
{
    public class TodosContext:DbContext
    {
        public TodosContext(DbContextOptions<TodosContext> options):base(options)
        {}

        public DbSet<Todo> Todos { get; set; }
    }
}
