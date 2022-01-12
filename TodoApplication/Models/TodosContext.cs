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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasMany(c => c.Todos).WithOne(a => a.User).HasForeignKey(a => a.UserId);
        }

        public DbSet<Todo> Todos { get; set; }
    }
}
