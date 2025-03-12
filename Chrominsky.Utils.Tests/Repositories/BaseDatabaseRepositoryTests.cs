using System.Linq.Expressions;
using System.Text.Json;
using Chrominsky.Utils.Enums;
using Chrominsky.Utils.Models;
using Chrominsky.Utils.Models.Base;
using Chrominsky.Utils.Repositories.ObjectVersioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;

namespace Chrominsky.Utils.Tests.Repositories
{
    public class BaseDatabaseRepositoryTests
    {
        private readonly Mock<DbContext> _dbContextMock;
        private readonly Mock<DbSet<TestEntity>> _dbSetMock;
        private readonly TestDatabaseRepository _repository;
        private readonly Mock<TestObjectVersioningRepository> _testObjectVersioningRepository;
        private readonly Mock<ObjectVersioningRepository> _objectVersioningRepositoryMock;

        public BaseDatabaseRepositoryTests()
        {
            _dbContextMock = new Mock<DbContext>();
            _dbSetMock = new Mock<DbSet<TestEntity>>();
            _dbContextMock.Setup(m => m.Set<TestEntity>()).Returns(_dbSetMock.Object);

            _objectVersioningRepositoryMock = new Mock<ObjectVersioningRepository>();
            _testObjectVersioningRepository = new Mock<TestObjectVersioningRepository>(_dbContextMock.Object);
            _repository = new TestDatabaseRepository(_dbContextMock.Object, _testObjectVersioningRepository.Object);
        }

        private Mock<DbSet<T>> CreateDbSetMock<T>(IEnumerable<T> elements) where T : class
        {
            var elementsAsQueryable = elements.AsQueryable();
            var dbSetMock = new Mock<DbSet<T>>();

            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<T>(elementsAsQueryable.Provider));
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression)
                .Returns(elementsAsQueryable.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType)
                .Returns(elementsAsQueryable.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator())
                .Returns(elementsAsQueryable.GetEnumerator());
            dbSetMock.As<IAsyncEnumerable<T>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(elementsAsQueryable.GetEnumerator()));

            return dbSetMock;
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEntities()
        {
            // Arrange
            var entities = new List<TestEntity>
                { new TestEntity { Id = Guid.NewGuid() }, new TestEntity { Id = Guid.NewGuid() } };
            var mockSet = CreateDbSetMock(entities);
            _dbContextMock.Setup(m => m.Set<TestEntity>()).Returns(mockSet.Object);

            // Act
            var result = await _repository.GetAllAsync<TestEntity>();

            // Assert
            Assert.Equal(entities, result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnEntity_WhenEntityExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entity = new TestEntity { Id = id };
            _dbSetMock.Setup(m => m.FindAsync(id)).ReturnsAsync(entity);

            // Act
            var result = await _repository.GetByIdAsync<TestEntity>(id);

            // Assert
            Assert.Equal(entity, result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenEntityDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            _dbSetMock.Setup(m => m.FindAsync(id)).ReturnsAsync((TestEntity)null);

            // Act
            var result = await _repository.GetByIdAsync<TestEntity>(id);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_ShouldAddEntityAndReturnId()
        {
            // Arrange
            var entity = new TestEntity();
            _dbSetMock.Setup(m => m.AddAsync(entity, It.IsAny<CancellationToken>()))
                .ReturnsAsync((EntityEntry<TestEntity>)null);

            // Act
            var result = await _repository.AddAsync(entity);

            // Assert
            Assert.Equal(entity.Id, result);
            _dbContextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _testObjectVersioningRepository.Verify(m => m.AddAsync(It.IsAny<ObjectVersion>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateEntity_WhenEntityExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var existingEntity = new TestEntity { Id = id };
            var updatedEntity = new TestEntity { Id = id, SomeProperty = "Updated" };

            _dbSetMock.Setup(m => m.FindAsync(id)).ReturnsAsync(existingEntity);

            // Act
            var result = await _repository.UpdateAsync(updatedEntity);

            // Assert
            Assert.Equal(updatedEntity.SomeProperty, existingEntity.SomeProperty);
            _dbContextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _testObjectVersioningRepository.Verify(m => m.AddAsync(It.IsAny<ObjectVersion>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowKeyNotFoundException_WhenEntityDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            var updatedEntity = new TestEntity { Id = id, SomeProperty = "Updated" };
            _dbSetMock.Setup(m => m.FindAsync(id)).ReturnsAsync((TestEntity)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _repository.UpdateAsync(updatedEntity));

            _testObjectVersioningRepository.Verify(m => m.AddAsync(It.IsAny<ObjectVersion>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteEntity_WhenEntityExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entity = new TestEntity { Id = id, Status = DatabaseEntityStatus.Active };
            _dbContextMock.Setup(m => m.FindAsync<TestEntity>(id)).ReturnsAsync(entity);
            _dbContextMock.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _repository.DeleteAsync<TestEntity>(id);

            // Assert
            Assert.True(result);
            Assert.Equal(DatabaseEntityStatus.Deleted, entity.Status);
            _dbContextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _testObjectVersioningRepository.Verify(m => m.AddAsync(It.IsAny<ObjectVersion>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenEntityDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            _dbSetMock.Setup(m => m.FindAsync(id)).ReturnsAsync((TestEntity)null);

            // Act
            var result = await _repository.DeleteAsync<TestEntity>(id);

            // Assert
            Assert.False(result);
            _testObjectVersioningRepository.Verify(m => m.AddAsync(It.IsAny<ObjectVersion>()), Times.Never);
        }

        [Fact]
        public async Task SearchAsync_ShouldReturnMatchingEntities_WhenGreaterThanOperatorIsUsed()
        {
            // Arrange
            var entities = new List<TestEntity>
            {
                new TestEntity { Id = Guid.NewGuid(), SomeProperty = "5" },
                new TestEntity { Id = Guid.NewGuid(), SomeProperty = "10" }
            };
            var mockSet = CreateDbSetMock(entities);
            _dbContextMock.Setup(m => m.Set<TestEntity>().AsQueryable()).Returns(mockSet.Object);

            var request = new SearchParameterRequest
            {
                SearchParameters = new List<SearchParameter>
                {
                    new SearchParameter { Key = "SomeProperty", Value = "7", Operator = SearchOperator.GreaterThan }
                },
                Page = 1,
                PageSize = 10
            };

            // Act
            var result = await _repository.SearchAsync<TestEntity>(request);

            // Assert
            Assert.Single(result);
            Assert.Equal("10", result.First().SomeProperty);
        }

        [Fact]
        public async Task SearchAsync_ShouldReturnMatchingEntities_WhenLessThanOperatorIsUsed()
        {
            // Arrange
            var entities = new List<TestEntity>
            {
                new TestEntity { Id = Guid.NewGuid(), SomeProperty = "5" },
                new TestEntity { Id = Guid.NewGuid(), SomeProperty = "10" }
            };
            var mockSet = CreateDbSetMock(entities);
            _dbContextMock.Setup(m => m.Set<TestEntity>().AsQueryable()).Returns(mockSet.Object);

            var request = new SearchParameterRequest
            {
                SearchParameters = new List<SearchParameter>
                {
                    new SearchParameter { Key = "SomeProperty", Value = "7", Operator = SearchOperator.LessThan }
                },
                Page = 1,
                PageSize = 10
            };

            // Act
            var result = await _repository.SearchAsync<TestEntity>(request);

            // Assert
            Assert.Single(result);
            Assert.Equal("5", result.First().SomeProperty);
        }

        [Fact]
        public async Task SearchAsync_ShouldApplyPaginationCorrectly()
        {
            // Arrange
            var entities = new List<TestEntity>
            {
                new TestEntity { Id = Guid.NewGuid(), SomeProperty = "Value1" },
                new TestEntity { Id = Guid.NewGuid(), SomeProperty = "Value2" },
                new TestEntity { Id = Guid.NewGuid(), SomeProperty = "Value3" }
            };
            var mockSet = CreateDbSetMock(entities);
            _dbContextMock.Setup(m => m.Set<TestEntity>().AsQueryable()).Returns(mockSet.Object);

            var request = new SearchParameterRequest
            {
                SearchParameters = new List<SearchParameter>(),
                Page = 2,
                PageSize = 1
            };

            // Act
            var result = await _repository.SearchAsync<TestEntity>(request);

            // Assert
            Assert.Single(result);
            Assert.Equal("Value2", result.First().SomeProperty);
        }

        [Fact]
        public async Task GetPaginatedAsync_ShouldReturnCorrectPage()
        {
            // Arrange
            var entities = new List<TestEntity>
            {
                new TestEntity { Id = Guid.NewGuid(), SomeProperty = "Value1" },
                new TestEntity { Id = Guid.NewGuid(), SomeProperty = "Value2" },
                new TestEntity { Id = Guid.NewGuid(), SomeProperty = "Value3" },
                new TestEntity { Id = Guid.NewGuid(), SomeProperty = "Value4" },
                new TestEntity { Id = Guid.NewGuid(), SomeProperty = "Value5" }
            };
            var mockSet = CreateDbSetMock(entities);
            _dbContextMock.Setup(m => m.Set<TestEntity>().AsQueryable()).Returns(mockSet.Object);

            // Act
            var result = await _repository.GetPaginatedAsync<TestEntity>(page: 2, pageSize: 2);

            // Assert
            Assert.Equal(2, result.Page);
            Assert.Equal(2, result.PageSize);
            Assert.Equal(5, result.TotalCount);
            Assert.Equal(2, result.Data.Count());
            Assert.Contains(result.Data, e => e.SomeProperty == "Value3");
            Assert.Contains(result.Data, e => e.SomeProperty == "Value4");
        }

        [Fact]
        public async Task GetPaginatedAsync_NoItemsReturnedFromRepo_EmptyResponseWithPagePageSize()
        {
            // Arrange
            var entities = new List<TestEntity>();
            var mockSet = CreateDbSetMock(entities);
            _dbContextMock.Setup(m => m.Set<TestEntity>().AsQueryable()).Returns(mockSet.Object);

            // Act
            var result = await _repository.GetPaginatedAsync<TestEntity>(page: 2, pageSize: 2);

            // Assert
            Assert.Equal(2, result.Page);
            Assert.Equal(2, result.PageSize);
            Assert.Equal(0, result.TotalCount);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task GetTableColumnsAsync_ShouldReturnTableColumns_WhenEntityExists()
        {
            // Arrange
            var columns = new List<TableColumn>
            {
                new() { ColumnName = "Id", Type = "int", Order = 1, DefaultValue = null, MaxLength = 0, IsNullable = 0 },
                new()
                {
                    ColumnName = "SomeProperty", Type = "string", Order = 2, DefaultValue = null, MaxLength = 255,
                    IsNullable = 1,
                },
            };
            
            var tableColumns = new TableColumns
            {
                TableName = "TestEntity",
                Json = JsonSerializer.Serialize(columns)
            };
            var mockSet = CreateDbSetMock([tableColumns]);
            _dbContextMock.Setup(m => m.Set<TableColumns>().AsQueryable()).Returns(mockSet.Object);

            // Act
            var result = _repository.GetTableColumnsAsync<TestEntity>();

            // Assert
            Assert.Equal(tableColumns, result);
        }

        [Fact]
        public async Task GetTableColumnsAsync_ShouldReturnTableColumns_WhenEntityExistsTableNamePassed()
        {
            // Arrange
            var columns = new List<TableColumn>
            {
                new()
                {
                    ColumnName = "Id", Type = "int", Order = 1, DefaultValue = null, MaxLength = 0, IsNullable = 0
                },
                new()
                {
                    ColumnName = "SomeProperty", Type = "string", Order = 2, DefaultValue = null, MaxLength = 255,
                    IsNullable = 1,
                },
            };

            var tableColumns = new TableColumns
            {
                TableName = "TestEntity2",
                Json = JsonSerializer.Serialize(columns)
            };
            var mockSet = CreateDbSetMock([tableColumns]);
            _dbContextMock.Setup(m => m.Set<TableColumns>().AsQueryable()).Returns(mockSet.Object);

            // Act
            var result = _repository.GetTableColumnsAsync<TestEntity>("TestEntity2");

            // Assert
            Assert.Equal(tableColumns, result);
        }

        [Fact]
        public async Task GetTableColumnsAsync_ShouldReturnEmptyTableColumns_WhenEntityDoesNotExist()
        {
            // Arrange
            var mockSet = CreateDbSetMock<TableColumns>([]);
            _dbContextMock.Setup(m => m.Set<TableColumns>().AsQueryable()).Returns(mockSet.Object);

            // Act
            var result = _repository.GetTableColumnsAsync<TestEntity>();

            // Assert
            Assert.Null(result);
        }

        private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;

            public TestAsyncEnumerator(IEnumerator<T> inner)
            {
                _inner = inner;
            }

            public ValueTask DisposeAsync()
            {
                _inner.Dispose();
                return ValueTask.CompletedTask;
            }

            public ValueTask<bool> MoveNextAsync()
            {
                return new ValueTask<bool>(_inner.MoveNext());
            }

            public T Current => _inner.Current;
        }

        private class TestAsyncQueryProvider<T> : IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;

            internal TestAsyncQueryProvider(IQueryProvider inner)
            {
                _inner = inner;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return new TestAsyncEnumerable<T>(expression);
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new TestAsyncEnumerable<TElement>(expression);
            }

            public object Execute(Expression expression)
            {
                return _inner.Execute(expression);
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return _inner.Execute<TResult>(expression);
            }

            public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
            {
                return new TestAsyncEnumerable<TResult>(expression);
            }

            public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            {
                return _inner.Execute<TResult>(expression);
            }
        }

        private class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public TestAsyncEnumerable(IEnumerable<T> enumerable)
                : base(enumerable)
            {
            }

            public TestAsyncEnumerable(Expression expression)
                : base(expression)
            {
            }

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            }

            IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
        }
    }
}