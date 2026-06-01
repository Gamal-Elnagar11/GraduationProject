 
namespace E_Commerce_API.Data
{
    public class Application : IdentityDbContext<User>
    {

        public Application() { }


        public Application(DbContextOptions<Application> options) : base(options) { }



       public  DbSet<Cart> Carts { get; set; }
       public  DbSet<Order> Orders { get; set; }
       public  DbSet<CartItem> CartItems { get; set; }
       public  DbSet<Category> Categories { get; set; }
       public  DbSet<OrderItem> OrderItems { get; set; }
       public  DbSet<Product> Products { get; set; }
        public DbSet<FAQ> FAQ { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }




        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);

            // تثبيت صيغة الديسيمال لكل الجداول اللي ظهرت في التحذير
            builder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18,2)");
            builder.Entity<CartItem>().Property(c => c.UnitPrice).HasColumnType("decimal(18,2)");
            builder.Entity<Order>().Property(o => o.TotalPrice).HasColumnType("decimal(18,2)");
            builder.Entity<OrderItem>().Property(oi => oi.Price).HasColumnType("decimal(18,2)");
            builder.Entity<OrderItem>().Property(oi => oi.TotalPrice).HasColumnType("decimal(18,2)");
        
            base.OnModelCreating(builder);
        }

    }
}
