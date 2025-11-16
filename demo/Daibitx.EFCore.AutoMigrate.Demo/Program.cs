using Daibitx.EFCore.AutoMigrate.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite.Design.Internal;
using Npgsql.EntityFrameworkCore.PostgreSQL.Design.Internal;
using Pomelo.EntityFrameworkCore.MySql.Design.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Daibitx.EFCore.AutoMigrate.Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Daibitx.EFCore.AutoMigrate Multi-Database Demo ===");

            //await DemoWithSqlite();
            //await DemoWithPostgreSql();
            await DemoWithMySql();

            Console.WriteLine("\nDemo completed successfully!");
        }

        // -----------------------------------------
        // SQLite Demo
        // -----------------------------------------
        static async Task DemoWithSqlite()
        {
            Console.WriteLine("\n--- SQLite Demo ---");

            var options = new DbContextOptionsBuilder<BlogContext>()
                .UseSqlite("Data Source=blogdemo.sqlite")
                .Options;

            using var context = new BlogContext(options);

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            context.AutoMigrate(services =>
            {
                new SqliteDesignTimeServices().ConfigureDesignTimeServices(services);
            });

            Console.WriteLine("SQLite migration completed!");
        }

        // -----------------------------------------
        // PostgreSQL Demo
        // -----------------------------------------
        static async Task DemoWithPostgreSql()
        {
            Console.WriteLine("\n--- PostgreSQL Demo ---");

            var options = new DbContextOptionsBuilder<BlogContext>()
                .UseNpgsql("Host=localhost;Database=blogdemo_pg;Username=postgres;Password=123456")
                .Options;

            using var context = new BlogContext(options);

            await context.Database.EnsureCreatedAsync();

            context.AutoMigrate(services =>
            {
                new NpgsqlDesignTimeServices().ConfigureDesignTimeServices(services);
            });

            Console.WriteLine("PostgreSQL migration completed!");
        }

        // -----------------------------------------
        // MySQL Demo (Pomelo)
        // -----------------------------------------
        static async Task DemoWithMySql()
        {
            Console.WriteLine("\n--- MySQL Demo ---");

            var options = new DbContextOptionsBuilder<BlogContext>()
                .UseMySql(
                    "Server=localhost;Port=33600;Database=blogdemo_mysql;User=root;Password=123456;",
                    new MySqlServerVersion(new Version(8, 0))
                )
                .Options;

            using var context = new BlogContext(options);

            context.AutoMigrate(services =>
            {
                new MySqlDesignTimeServices().ConfigureDesignTimeServices(services);
            });

            Console.WriteLine("MySQL migration completed!");
        }
    }

    // ----------------------------------------------------------
    // Shared DbContext + Models
    // ----------------------------------------------------------

    public class BlogContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        public BlogContext(DbContextOptions<BlogContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Url).IsRequired().HasMaxLength(500);
                entity.HasIndex(e => e.Url).IsUnique();
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Content).HasColumnType("text");

                entity.HasOne(e => e.Blog)
                    .WithMany(b => b.Posts)
                    .HasForeignKey(e => e.BlogId);
            });
        }
    }
    [Table("blogs")]
    public class Blog
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Xml { get; set; }
        public string Title { get; set; }

        public List<Post> Posts { get; set; } = new();
    }
    [Table("posts")]
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
