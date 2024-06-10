using Chrominsky.Utils.Enums;
using Chrominsky.Utils.Models;
using Chrominsky.Utils.Models.Base.Interfaces;
using Chrominsky.Utils.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Chrominsky.Utils.Tests.Repositories
{
    public class TestDatabaseRepository : BaseDatabaseRepository<TestEntity>
    {
        public TestDatabaseRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }

    // Sample entity class for testing
    public class TestEntity : IBaseDatabaseEntity
    {
        public Guid Id { get; set; }
        public string SomeProperty { get; set; }
        public string? UpdatedBy { get; set; }
        public DatabaseEntityStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}