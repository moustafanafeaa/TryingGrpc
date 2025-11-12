using GrpcGreeter.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcGreeter.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions)
        {
            
        }
        public DbSet<ToDoItem> ToDoItems => Set<ToDoItem>(); 
    }
}
