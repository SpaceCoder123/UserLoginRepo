
using Microsoft.EntityFrameworkCore;
using EmailVerification.Model;
namespace EmailVerification.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options) 
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=(LocalDB)\\MSSQLLocalDB; Database=RegistrationUserDB; Trusted_Connection=true; TrustServerCertificate=true;");
        }
        
        public DbSet<User> Users => Set<User>();
    }
}
