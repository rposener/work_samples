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
            var repository = new TodoRepository(tableServiceClient, logger);
            await repository.EnsureCreatedAsync();
            await repository.EnsureCreatedAsync();

            // Verify service was only called once
            Mock.Get(tableServiceClient).Verify();
        }

        [Theory]
        [InlineData("my test data", "mytestdata")]
        [InlineData("$ Super Grocery  ", "supergrocery")]
        [InlineData("097  _#42@#% alpha", "09742alpha")]
        public async Task GetSafeStorageKey_Values_Are_Safe(string value, string expected)
        {
            // Setup Test
            TableServiceClient tableServiceClient = Mock.Of<TableServiceClient>();
            ILogger<TodoRepository> logger = Mock.Of<ILogger<TodoRepository>>();
            var repository = new TodoRepository(tableServiceClient, logger);


            // Execute our Test
            var result = repository.GetSafeStorageKey(value);

            // Verify service was only called once
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("000001", "00000002")]
        [InlineData("000000", "00000001")]
        [InlineData("-000001", "00000001")]
        [InlineData("99999999", "100000000")]
        [InlineData(" alpha only", "00000001")]
        public async Task IncrementId_Works_As_Expected(string value, string expected)
        {
            // Setup Test
            TableServiceClient tableServiceClient = Mock.Of<TableServiceClient>();
            ILogger<TodoRepository> logger = Mock.Of<ILogger<TodoRepository>>();
            var repository = new TodoRepository(tableServiceClient, logger);


            // Execute our Test
            var result = repository.IncrementId(value);

            // Verify service was only called once
            Assert.Equal(expected, result);
        }
    }
}
