English | [简体中文](README.zh-CN.md)
- # Daibitx.EFCore.AutoMigrate

  An intelligent, safe, and controllable database migration toolkit built on **Entity Framework Core**, providing enhanced protection and fine-grained migration control for production environments.

  ## Overview

  **Daibitx.EFCore.AutoMigrate** is an enhancement library for EF Core that enables **safe database migrations** in production. Unlike standard EF Core migrations, this library allows precise control over which migration operations are permitted, preventing unintended data loss or destructive schema changes.

  ------

  ## Project Structure

  ```
  Daibitx.EFCore.AutoMigrate/
  ├── Abstraction/                   # Abstraction layer
  │   ├── IDesignTimeService.cs      # Design-time services interface
  │   ├── IMigrateBuilder.cs         # Migration builder interface
  │   ├── IMigrationRunner.cs        # Migration runner interface
  │   └── IMigrationStepProcessor.cs # Migration step processor interface
  ├── Core/                          # Core implementation layer
  │   ├── AutoMigrationOptions.cs    # Migration configuration options
  │   ├── DesignTimeService.cs       # Design-time service implementation
  │   ├── MigrateBuilder.cs          # Migration builder implementation
  │   ├── MigrationRunner.cs         # Migration runner implementation
  │   └── MigrationStepProcessor.cs  # Migration step processor implementation
  ├── Exceptions/                    # Exception layer
  │   ├── AutoMigrationException.cs      # Base migration exception
  │   ├── DesignTimeServiceException.cs  # Design-time service exception
  │   └── MigrationScriptException.cs    # Migration script exception
  ├── Extensions/                    # Extension methods
  │   └── AutoMigrateExtensions.cs   # Convenient extension methods
  ```

  ------

  ## Features

  - ✅ **Safety First** — All destructive operations (drop table, drop column, etc.) disabled by default
  - ✅ **Fine-grained control** — Configure exactly which migration actions are allowed
  - ✅ **Transactional execution** — All operations run inside a transaction, fully rolled back on failure
  - ✅ **Multi-database support** — Works with SQL Server, MySQL, PostgreSQL, and more
  - ✅ **Dependency Injection support** — Fully integrated with **ASP.NET Core DI**
  - ✅ **Concurrency-safe** — Built-in semaphore to prevent concurrent migration conflicts
  - ✅ **Rich exception information** — Detailed context and error reporting

  ------

  ## Getting Started

  ### Installation

  ```
  dotnet add package Daibitx.EFCore.AutoMigrate
  ```

  ------

  ## Usage

  ### Option 1: Via Dependency Injection (Recommended)

  ```
  // Program.cs
  var builder = WebApplication.CreateBuilder(args);
  
  // Register DbContext
  builder.Services.AddDbContext<MyDbContext>(options => 
      options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
  
  var app = builder.Build();
  
  // Execute automatic migration (safe mode)
  app.Services.AutoMigrate<MyDbContext>(services => 
  {
      // Register design-time services
  }, options => 
  {
      options.AsSafeMode(); // Enable safe mode
  });
  
  app.Run();
  ```

  ------

  ### Option 2: Using DbContext directly

  ```
  using var context = new MyDbContext();
  context.AutoMigrate(services => 
  {
      // Configure design-time services
  }, options => 
  {
      options.AsSafeMode();
  });
  ```

  ------

  ### Option 3: Builder Pattern

  ```
  var builder = new MigrateBuilder<MyDbContext>(dbContext)
      .WithOptions(new AutoMigrationOptions().AsSafeMode())
      .AddDesignTimeServices(services => 
      {
          // Configure design-time services
      });
  
  var runner = builder.Build();
  await runner.ExecuteAsync();
  ```

  ------

  ## Configuration Options

  ### Safe Mode (Recommended)

  ```
  var options = new AutoMigrationOptions().AsSafeMode();
  ```

  Safe mode configuration:

  | Option                | Default | Description                       |
  | --------------------- | ------- | --------------------------------- |
  | AllowCreateIndex      | ✅ true  | Allow creating indexes            |
  | AllowCreateForeignKey | ✅ true  | Allow creating foreign keys       |
  | AllowDropTable        | ❌ false | Prevent dropping tables           |
  | AllowDropColumn       | ❌ false | Prevent dropping columns          |
  | AllowDropIndex        | ❌ false | Prevent dropping indexes          |
  | AllowDropForeignKey   | ❌ false | Prevent dropping foreign keys     |
  | AllowDropPrimaryKey   | ❌ false | Prevent dropping primary keys     |
  | AllowAlterColumn      | ❌ false | Prevent altering existing columns |
  | AllowRenameTable      | ❌ false | Prevent renaming tables           |
  | AllowRenameColumn     | ❌ false | Prevent renaming columns          |

  ------

  ### Full Mode (Use Carefully)

  ```
  var options = new AutoMigrationOptions().AsFullMode();
  ```

  ------

  ### Custom Configuration

  ```
  var options = new AutoMigrationOptions
  {
      AllowCreateIndex = true,
      AllowCreateForeignKey = true,
      AllowDropTable = false,         // Recommended false in production
      AllowDropColumn = false,        // Recommended false in production
      AllowAlterColumn = true,        // Use with caution — may affect data
      UseTransactions = true,         // Enable transactions
      CommandTimeout = 60             // 60-second timeout
  };
  ```

  ------

  ## Configuring Design-Time Services

  ```
  services.AutoMigrate<MyDbContext>(designTimeServices => 
  {
       new MySqlDesignTimeServices().ConfigureDesignTimeServices(services);
       // new SqliteDesignTimeServices().ConfigureDesignTimeServices(services);
       // new NpgsqlDesignTimeServices().ConfigureDesignTimeServices(services);
       // new KdbndpDesignTimeServices().ConfigureDesignTimeServices(services);
       // new GaussDBDesignTimeServices().ConfigureDesignTimeServices(services);
  }, options => 
  {
      options.AsSafeMode();
  });
  ```

  ------

  ## Notes & Best Practices

  ### 1. Preventing Destructive Changes

  **Strongly recommended for production environments:**

  ```
  options.AsSafeMode();
  ```

  Safe mode ensures:

  - Only allows *adding* tables, columns, indexes, foreign keys
  - Blocks *all* drop operations
  - Blocks altering or renaming columns/tables
  - Minimizes unexpected destructive changes

  For modifying a column:

  1. Add a new column
  2. Migrate data
  3. Drop the old column **only after testing**

  ------

  ### 2. Case-Sensitivity Issues in Database Creation

  **Issue:**
   Creating a database directly via EF Core on systems with case-insensitive collations may cause index or column name collisions in later migrations.

  **Solutions:**

  #### Option A — Configure via Fluent API (Recommended)

  ```
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
      foreach (var entityType in modelBuilder.Model.GetEntityTypes())
      {
          foreach (var property in entityType.GetProperties())
          {
              if (property.ClrType == typeof(string))
              {
                  property.SetCollation("SQL_Latin1_General_CP1_CS_AS"); // SQL Server
                  // MySQL: property.SetCollation("utf8mb4_bin");
                  // PostgreSQL: property.SetCollation("C");
              }
          }
      }
  }
  ```

  ------

  #### Option B — Using Data Annotations

  ```
  public class MyEntity
  {
      public int Id { get; set; }
  
      [Column(TypeName = "varchar(255) COLLATE SQL_Latin1_General_CP1_CS_AS")]
      public string Name { get; set; }
  }
  ```

  ------

  #### Option C — Set Collation at Database Creation

  ```
  -- SQL Server
  CREATE DATABASE MyDatabase COLLATE SQL_Latin1_General_CP1_CS_AS;
  
  -- MySQL
  CREATE DATABASE MyDatabase CHARACTER SET utf8mb4 COLLATE utf8mb4_bin;
  
  -- PostgreSQL
  CREATE DATABASE MyDatabase ENCODING 'UTF8' LC_COLLATE 'C' LC_CTYPE 'C';
  ```

  ------

  ### 3. Additional Best Practices

  1. **Always back up the database** before production migrations
  2. **Test migrations in staging** before deploying
  3. **Deploy large changes in phases**
  4. **Have a rollback plan** in case a migration fails

  ------

  ## Exception Handling

  The library provides detailed exceptions with context information.

  ```
  try
  {
      await context.AutoMigrateAsync(services => { }, options => options.AsSafeMode());
  }
  catch (AutoMigrationException ex)
  {
      Console.WriteLine($"Migration failed: {ex.Message}");
      Console.WriteLine($"DbContext: {ex.DbContextType?.Name}");
      Console.WriteLine($"Operation: {ex.MigrationOperation}");
  }
  ```
