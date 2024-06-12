using Chrominsky.Utils.Enums;
using Chrominsky.Utils.Models;
using Chrominsky.Utils.Models.Base;
using Chrominsky.Utils.Models.Base.Interfaces;
using Chrominsky.Utils.Repositories.Base;
using Chrominsky.Utils.Repositories.ObjectVersioning;
using Microsoft.EntityFrameworkCore;

namespace Chrominsky.Utils.Tests.Repositories
{
    public class TestDatabaseRepository : BaseDatabaseRepository<TestEntity>
    {
        public TestDatabaseRepository(DbContext dbContext, IObjectVersioningRepository testObjectVersioningRepository) : base(dbContext, testObjectVersioningRepository)
        {
        }
    }

    public class TestObjectVersioningRepository : ObjectVersioningRepository, IObjectVersioningRepository
    {
        public TestObjectVersioningRepository(DbContext dbContext) : base(dbContext)
        {
            
        }

        public new virtual async Task<Guid> AddAsync(ObjectVersion entity)
        {
            return Guid.NewGuid();      
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