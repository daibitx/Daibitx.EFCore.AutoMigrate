[English](README.md) | 简体中文

- # Daibitx.EFCore.AutoMigrate

  一个基于 Entity Framework Core 的智能自动迁移库，提供安全、可控的数据库迁移方案。

  ## 项目概述

  Daibitx.EFCore.AutoMigrate 是一个针对 EF Core 的增强库，它提供了在生产环境中安全执行数据库迁移的能力。与传统的 EF Core 迁移不同，本库允许精细控制迁移操作，避免意外的数据丢失和破坏性更改。

  ## 项目结构

  ```
  Daibitx.EFCore.AutoMigrate/
  ├── Abstraction/                   # 抽象接口层
  │   ├── IDesignTimeService.cs      # 设计时服务接口
  │   ├── IMigrateBuilder.cs         # 迁移构建器接口
  │   ├── IMigrationRunner.cs        # 迁移运行器接口
  │   └── IMigrationStepProcessor.cs # 迁移步骤处理器接口
  ├── Core/                          # 核心实现层
  │   ├── AutoMigrationOptions.cs    # 迁移配置选项
  │   ├── DesignTimeService.cs       # 设计时服务实现
  │   ├── MigrateBuilder.cs          # 迁移构建器实现
  │   ├── MigrationRunner.cs         # 迁移运行器实现
  │   └── MigrationStepProcessor.cs  # 迁移步骤处理器实现
  ├── Exceptions/                    # 异常处理层
  │   ├── AutoMigrationException.cs      # 基础迁移异常
  │   ├── DesignTimeServiceException.cs  # 设计时服务异常
  │   └── MigrationScriptException.cs    # 迁移脚本异常
  ├── Extensions/                    # 扩展方法层
  │   └── AutoMigrateExtensions.cs   # 便捷使用的扩展方法
  ```

  ## 功能特性

  - ✅ **安全第一**：默认禁用所有破坏性操作（删除表、列等）
  - ✅ **精细控制**：可配置允许的迁移操作类型
  - ✅ **事务支持**：所有迁移操作在事务中执行，失败自动回滚
  - ✅ **多数据库支持**：支持 SQL Server、MySQL、PostgreSQL 等
  - ✅ **依赖注入**：与 [ASP.NET](https://asp.net/) Core 依赖注入完美集成
  - ✅ **并发安全**：内置信号量防止并发迁移冲突
  - ✅ **异常处理**：详细的异常信息和上下文信息

  ## 快速开始

  ### 安装

  ```
  dotnet add package Daibitx.EFCore.AutoMigrate
  ```

  ### 基本用法

  #### 方式一：通过依赖注入（推荐）

  csharp

  ```
  // Program.cs
  var builder = WebApplication.CreateBuilder(args);
  
  // 注册 DbContext
  builder.Services.AddDbContext<MyDbContext>(options => 
      options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
  
  var app = builder.Build();
  
  // 执行自动迁移（安全模式）
  app.Services.AutoMigrate<MyDbContext>(services => 
  {
      
  }, options => 
  {
      options.AsSafeMode(); // 使用安全模式
  });
  
  app.Run();
  ```

  #### 方式二：直接使用 DbContext

  ```
  using var context = new MyDbContext();
  context.AutoMigrate(services => 
  {
      // 配置设计时服务
  }, options => 
  {
      options.AsSafeMode();
  });
  ```

  #### 方式三：使用构建器模式

  ```
  var builder = new MigrateBuilder<MyDbContext>(dbContext)
      .WithOptions(new AutoMigrationOptions().AsSafeMode())
      .AddDesignTimeServices(services => 
      {
          // 配置设计时服务
      });
  
  var runner = builder.Build();
  await runner.ExecuteAsync();
  ```

  ## 配置选项

  ### 安全模式配置（推荐）

  ```
  var options = new AutoMigrationOptions().AsSafeMode();
  ```

  安全模式配置：

  - ✅ `AllowCreateIndex = true` - 允许创建索引
  - ✅ `AllowCreateForeignKey = true` - 允许创建外键
  - ❌ `AllowDropTable = false` - 禁止删除表
  - ❌ `AllowDropColumn = false` - 禁止删除列
  - ❌ `AllowDropIndex = false` - 禁止删除索引
  - ❌ `AllowDropForeignKey = false` - 禁止删除外键
  - ❌ `AllowDropPrimaryKey = false` - 禁止删除主键
  - ❌ `AllowAlterColumn = false` - 禁止修改列
  - ❌ `AllowRenameTable = false` - 禁止重命名表
  - ❌ `AllowRenameColumn = false` - 禁止重命名列

  ### 完整模式配置（谨慎使用）

  ```
  var options = new AutoMigrationOptions().AsFullMode();
  ```

  ### 自定义配置

  ```
  var options = new AutoMigrationOptions
  {
      AllowCreateIndex = true,
      AllowCreateForeignKey = true,
      AllowDropTable = false,        // 生产环境建议 false
      AllowDropColumn = false,       // 生产环境建议 false
      AllowAlterColumn = true,       // 谨慎开启，可能影响数据
      UseTransactions = true,        // 启用事务
      CommandTimeout = 60            // 60秒超时
  };
  ```

  ## 设计时服务配置

  ```
  services.AutoMigrate<MyDbContext>(designTimeServices => 
  {
       new MySqlDesignTimeServices().ConfigureDesignTimeServices(services);
       //new SqliteDesignTimeServices().ConfigureDesignTimeServices(services);
       //new NpgsqlDesignTimeServices().ConfigureDesignTimeServices(services);
       //new KdbndpDesignTimeServices().ConfigureDesignTimeServices(services);
       //new GaussDBDesignTimeServices().ConfigureDesignTimeServices(services);
  }, options => 
  {
      options.AsSafeMode();
  });
  ```

  

  ## 注意事项

  ### 1. 防止破坏性更改的最佳实践

  **强烈建议在生产环境中使用安全模式：**

  ```
  options.AsSafeMode();
  ```

  安全模式确保：

  - 只允许添加新的表、列、索引、外键
  - 禁止所有删除操作（表、列、索引等）
  - 禁止修改现有列定义
  - 禁止重命名操作

  如果需要修改列结构，建议：

  1. 添加新列
  2. 迁移数据
  3. 删除旧列（仅在开发环境测试后）

  ### 2. 数据库大小写敏感性问题

  **问题描述：**
  直接在不存在的数据库上使用 EF Core 迁移可能会生成字段不敏感的数据库表，导致后续迁移时出现字段索引名称重复等问题。

  **解决方案：**

  #### 方案一：通过 Fluent API 配置（推荐）

  csharp

  ```
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
      // 设置所有字符串列区分大小写
      foreach (var entityType in modelBuilder.Model.GetEntityTypes())
      {
          foreach (var property in entityType.GetProperties())
          {
              if (property.ClrType == typeof(string))
              {
                  property.SetCollation("SQL_Latin1_General_CP1_CS_AS"); // SQL Server
                  // 或者对于 MySQL: property.SetCollation("utf8mb4_bin");
                  // 或者对于 PostgreSQL: property.SetCollation("C");
              }
          }
      }
  }
  ```

  

  #### 方案二：通过数据注解

  csharp

  ```
  public class MyEntity
  {
      public int Id { get; set; }
      
      [Column(TypeName = "varchar(255) COLLATE SQL_Latin1_General_CP1_CS_AS")]
      public string Name { get; set; }
  }
  ```

  

  #### 方案三：数据库层面设置

  在创建数据库时指定区分大小写的排序规则：

  sql

  ```
  -- SQL Server
  CREATE DATABASE MyDatabase COLLATE SQL_Latin1_General_CP1_CS_AS;
  
  -- MySQL
  CREATE DATABASE MyDatabase CHARACTER SET utf8mb4 COLLATE utf8mb4_bin;
  
  -- PostgreSQL
  CREATE DATABASE MyDatabase ENCODING 'UTF8' LC_COLLATE 'C' LC_CTYPE 'C';
  ```

  ### 3. 其他最佳实践

  1. **始终备份数据**：在生产环境执行迁移前备份数据库
  2. **测试环境验证**：先在测试环境验证迁移脚本
  3. **分阶段部署**：大型变更分多个小迁移执行
  4. **监控和回滚计划**：准备迁移失败时的回滚方案

  ## 异常处理

  库提供了详细的异常信息：

  ```
  try
  {
      await context.AutoMigrateAsync(services => { }, options => options.AsSafeMode());
  }
  catch (AutoMigrationException ex)
  {
      Console.WriteLine($"迁移失败: {ex.Message}");
      Console.WriteLine($"DbContext: {ex.DbContextType?.Name}");
      Console.WriteLine($"操作: {ex.MigrationOperation}");
  }
  ```
