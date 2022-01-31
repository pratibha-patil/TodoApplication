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
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Todo>().ToTable("Todos");

            // Configure Primary Keys  
            modelBuilder.Entity<User>().HasKey(u => u.Id).HasName("PK_User");
            modelBuilder.Entity<Todo>().HasKey(ti => ti.Id).HasName("PK_Todo");

            // Configure columns  
            modelBuilder.Entity<User>().Property(u => u.Id).HasColumnType("int").UseMySqlIdentityColumn().IsRequired();
            modelBuilder.Entity<User>().Property(u => u.Name).HasColumnType("nvarchar(100)").IsRequired();
            modelBuilder.Entity<User>().Property(u =>u.Password).HasColumnType("nvarchar(12)").IsRequired();

            modelBuilder.Entity<Todo>().Property(ti => ti.Id).HasColumnType("int").UseMySqlIdentityColumn().IsRequired();
            modelBuilder.Entity<Todo>().Property(ti => ti.Title).HasColumnType("nvarchar(100)").IsRequired();
            modelBuilder.Entity<Todo>().Property(ti => ti.Description).HasColumnType("nvarchar(200)").IsRequired(false);
            modelBuilder.Entity<Todo>().Property(ti => ti.IsCompleted).HasColumnType("tinyint");

            modelBuilder.Entity<User>().HasMany(c => c.Todos).WithOne(a => a.User).HasForeignKey(a => a.UserId);
        }

        public DbSet<Todo> Todos { get; set; }
    }
}
