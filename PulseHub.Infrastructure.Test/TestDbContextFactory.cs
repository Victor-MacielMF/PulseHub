using Microsoft.EntityFrameworkCore;
using PulseHub.Infrastructure.Data;
using System;

namespace PulseHub.Infrastructure.Test
{
    public static class TestDbContextFactory
    {
        public static PulseHubDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<PulseHubDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new PulseHubDbContext(options);
            context.Database.EnsureCreated();

            return context;
        }
    }
}
