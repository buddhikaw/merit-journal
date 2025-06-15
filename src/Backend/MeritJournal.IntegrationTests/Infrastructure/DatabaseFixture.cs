using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MeritJournal.Infrastructure.Persistence;
using System;
using System.Threading.Tasks;
using Xunit;
using Testcontainers.PostgreSql;
using Microsoft.AspNetCore.Mvc.Testing;
using MeritJournal.API;
using MeritJournal.Domain.Entities;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using MeritJournal.Application.Interfaces;

namespace MeritJournal.IntegrationTests
{
    public class DatabaseFixture : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer;
        public string ConnectionString { get; private set; } = string.Empty;
        public IServiceProvider ServiceProvider { get; private set; } = null!;

        public DatabaseFixture()
        {
            _dbContainer = new PostgreSqlBuilder()
                .WithImage("postgres:14")
                .WithPortBinding(5432, true)
                .WithDatabase("merit_journal_test_db")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .Build();
        }
        
        public async Task InitializeAsync()
        {
            // Start the PostgreSQL container
            await _dbContainer.StartAsync();
            ConnectionString = _dbContainer.GetConnectionString();
            
            // Create a service collection and configure it for testing
            var services = new ServiceCollection();
              // Create configuration with test connection string
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = ConnectionString
                })
                .Build();
            
            services.AddSingleton<IConfiguration>(configuration);
            
            // Add DbContext using the test container
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(ConnectionString));
            
            services.AddScoped<IApplicationDbContext>(provider => 
                provider.GetRequiredService<ApplicationDbContext>());
            
            // Build the service provider
            ServiceProvider = services.BuildServiceProvider();
            
            // Create the database and run migrations
            using var scope = ServiceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync();
            
            // Seed test data
            await SeedTestDataAsync(dbContext);
        }

        private async Task SeedTestDataAsync(ApplicationDbContext dbContext)
        {
            // Create test user
            var userId = "test-user-id";
            
            // Create test tags
            var tag1 = new Tag { Name = "health", UserId = userId };
            var tag2 = new Tag { Name = "work", UserId = userId };
            var tag3 = new Tag { Name = "personal", UserId = userId };
            
            dbContext.Tags.AddRange(tag1, tag2, tag3);
            await dbContext.SaveChangesAsync();
            
            // Create test entries
            var entry1 = new JournalEntry
            {
                Title = "Test Entry 1",
                Content = "This is test content for entry 1",
                EntryDate = DateTime.UtcNow.AddDays(-2),
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UserId = userId
            };
            
            var entry2 = new JournalEntry
            {
                Title = "Test Entry 2",
                Content = "This is test content for entry 2",
                EntryDate = DateTime.UtcNow.AddDays(-1),
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UserId = userId
            };
            
            dbContext.JournalEntries.AddRange(entry1, entry2);
            await dbContext.SaveChangesAsync();
            
            // Create tag relationships
            var journalEntryTag1 = new JournalEntryTag
            {
                JournalEntryId = entry1.Id,
                TagId = tag1.Id
            };
            
            var journalEntryTag2 = new JournalEntryTag
            {
                JournalEntryId = entry1.Id,
                TagId = tag2.Id
            };
            
            var journalEntryTag3 = new JournalEntryTag
            {
                JournalEntryId = entry2.Id,
                TagId = tag3.Id
            };
            
            dbContext.JournalEntryTags.AddRange(journalEntryTag1, journalEntryTag2, journalEntryTag3);
            await dbContext.SaveChangesAsync();
        }

        public async Task DisposeAsync()
        {
            // Stop and dispose the container
            await _dbContainer.StopAsync();
            await _dbContainer.DisposeAsync();
        }
    }
    
    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
    
    public class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _connectionString;
        
        public TestWebApplicationFactory(string connectionString)
        {
            _connectionString = connectionString;
        }
          protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, configurationBuilder) =>
            {
                configurationBuilder.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:DefaultConnection"] = _connectionString
                    });
            });
            
            builder.ConfigureServices(services =>
            {
                // Remove any registered DbContexts
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                
                // Add DbContext using the test container's connection string
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseNpgsql(_connectionString);
                });
                
                services.AddScoped<IApplicationDbContext>(provider => 
                    provider.GetRequiredService<ApplicationDbContext>());
            });
        }
    }
}
