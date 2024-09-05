using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using Moq;
using TodoApi.Data;

namespace TodoApiTests.Data
{

    public class TodoRepositoryTests
    {

        [Fact]
        public async Task EnsureCreatedAsync_OnlyCreates_Table_Once()
        {
            // Setup Test
            TableServiceClient tableServiceClient = Mock.Of<TableServiceClient>();
            Mock.Get(tableServiceClient)
                .Setup(t => t.CreateTableIfNotExistsAsync(
                    It.Is<string>(v => v.Equals("todos")), 
                    It.IsAny<CancellationToken>()))
                .Verifiable(Times.Once());
            ILogger<TodoRepository> logger = Mock.Of<ILogger<TodoRepository>>();

          
            // Create Repository and call it twice
            var repository = new TodoRepository(tableServiceClient, logger, Mock.Of<IIdentityManager>());
            await repository.EnsureCreatedAsync();
            await repository.EnsureCreatedAsync();

            // Verify service was only called once
            Mock.Get(tableServiceClient).Verify();
        }

        [Theory]
        [InlineData("my test data", "mytestdata")]
        [InlineData("$ Super Grocery  ", "supergrocery")]
        [InlineData("097  _#42@#% alpha", "09742alpha")]
        public void GetSafeStorageKey_Values_Are_Safe(string value, string expected)
        {
            // Setup Test
            TableServiceClient tableServiceClient = Mock.Of<TableServiceClient>();
            ILogger<TodoRepository> logger = Mock.Of<ILogger<TodoRepository>>();
            var repository = new TodoRepository(tableServiceClient, logger, Mock.Of<IIdentityManager>());


            // Execute our Test
            var result = repository.GetSafeStorageKey(value);

            // Verify service was only called once
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task AddTodo_CreatesEntity_With_Identity_Set()
        {
            // Setup Test
            TableServiceClient tableServiceClient = Mock.Of<TableServiceClient>();

            TableClient tableClient = Mock.Of<TableClient>();
            Mock.Get(tableServiceClient)
                .Setup(tsc => tsc.GetTableClient(It.Is<string>(v => "todos".Equals(v))))
                .Returns(tableClient)
                .Verifiable(Times.Once());

            Response addResponse = Mock.Of<Response>();
            Mock.Get(addResponse).SetupGet(r => r.IsError).Returns(false);

            Mock.Get(tableClient)
               .Setup(tsc => tsc.AddEntityAsync(
                   It.Is<TodoEntity>(v => "000999".Equals(v.RowKey)),
                   It.IsAny<CancellationToken>()))
               .ReturnsAsync(addResponse)
               .Verifiable(Times.Once());

            ILogger<TodoRepository> logger = Mock.Of<ILogger<TodoRepository>>();
            IIdentityManager identityManager = Mock.Of<IIdentityManager>();
            Mock.Get(identityManager)
                .Setup(i => i.GetNextIdAsync(It.IsAny<string>())).ReturnsAsync("000999")
                .Verifiable(Times.Once());

            // Create Repository and call it twice
            var repository = new TodoRepository(tableServiceClient, logger, identityManager);
            await repository.AddTodoAsync("list9", new TodoEntity { Description= "descr", IsCompleted = false, DueDate = null});

            // Verify service was only called once
            Mock.Get(tableServiceClient).Verify();
        }


        [Fact]
        public async Task UpdateTodo_UpdatesEntity_WithoutIdentity_Change()
        {
            // Setup Test
            TableServiceClient tableServiceClient = Mock.Of<TableServiceClient>();

            TableClient tableClient = Mock.Of<TableClient>();
            Mock.Get(tableServiceClient)
                .Setup(tsc => tsc.GetTableClient(It.Is<string>(v => "todos".Equals(v))))
                .Returns(tableClient)
                .Verifiable(Times.Once());

            Response upsertResponse = Mock.Of<Response>();
            Mock.Get(upsertResponse).SetupGet(r => r.IsError).Returns(false);

            Mock.Get(tableClient)
               .Setup(tsc => tsc.UpsertEntityAsync(
                   It.Is<TodoEntity>(v => "12345".Equals(v.RowKey)),
                   It.IsAny<TableUpdateMode>(),
                   It.IsAny<CancellationToken>()))
               .ReturnsAsync(upsertResponse)
               .Verifiable(Times.Once());

            ILogger<TodoRepository> logger = Mock.Of<ILogger<TodoRepository>>();
            IIdentityManager identityManager = Mock.Of<IIdentityManager>();
            Mock.Get(identityManager)
                .Setup(i => i.GetNextIdAsync(It.IsAny<string>())).ReturnsAsync("000999")
                .Verifiable(Times.Never());

            // Create Repository and call it twice
            var repository = new TodoRepository(tableServiceClient, logger, identityManager);
            await repository.UpdateTodoAsync("list9", new TodoEntity { RowKey="12345", Description = "descr", IsCompleted = false, DueDate = null });

            // Verify service was only called once
            Mock.Get(tableServiceClient).Verify();
        }

        [Fact]
        public async Task DeleteTodo_RemovesEntity_ByPartition_And_RowKey()
        {
            // Setup Test
            TableServiceClient tableServiceClient = Mock.Of<TableServiceClient>();

            TableClient tableClient = Mock.Of<TableClient>();
            Mock.Get(tableServiceClient)
                .Setup(tsc => tsc.GetTableClient(It.Is<string>(v => "todos".Equals(v))))
                .Returns(tableClient)
                .Verifiable(Times.Once());

            Response upsertResponse = Mock.Of<Response>();
            Mock.Get(upsertResponse).SetupGet(r => r.IsError).Returns(false);

            Mock.Get(tableClient)
               .Setup(tsc => tsc.DeleteEntityAsync(
                   It.Is<string>(v => "list9".Equals(v)),
                   It.Is<string>(v => "12345".Equals(v)),
                   It.IsAny<ETag>(),
                   It.IsAny<CancellationToken>()))
               .ReturnsAsync(upsertResponse)
               .Verifiable(Times.Once());

            ILogger<TodoRepository> logger = Mock.Of<ILogger<TodoRepository>>();
            IIdentityManager identityManager = Mock.Of<IIdentityManager>();

            // Create Repository and call it twice
            var repository = new TodoRepository(tableServiceClient, logger, identityManager);
            await repository.DeleteTodoAsync("list9", "12345");

            // Verify service was only called once
            Mock.Get(tableServiceClient).Verify();
        }

    }
}
