using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using Moq;
using TodoApi.Data;

namespace TodoApiTests.Data
{
    public class IdentityManagerTests
    {

        [Fact]
        public async Task EnsureCreatedAsync_OnlyCreates_Table_Once()
        {
            // Setup Test
            TableServiceClient tableServiceClient = Mock.Of<TableServiceClient>();
            Mock.Get(tableServiceClient)
                .Setup(t => t.CreateTableIfNotExistsAsync(
                    It.Is<string>(v => v.Equals("identity")),
                    It.IsAny<CancellationToken>()))
                .Verifiable(Times.Once());
            ILogger<IdentityManager> logger = Mock.Of<ILogger<IdentityManager>>();


            // Create Repository and call it twice
            var repository = new IdentityManager(tableServiceClient, logger);
            await repository.EnsureCreatedAsync();
            await repository.EnsureCreatedAsync();

            // Verify service was only called once
            Mock.Get(tableServiceClient).Verify();
        }

        [Theory]
        [InlineData("000001", "00000002")]
        [InlineData("000000", "00000001")]
        [InlineData("-000001", "00000001")]
        [InlineData("99999999", "100000000")]
        [InlineData(" alpha only", "00000001")]
        public void IncrementId_Works_As_Expected(string value, string expected)
        {
            // Setup Test
            TableServiceClient tableServiceClient = Mock.Of<TableServiceClient>();
            ILogger<IdentityManager> logger = Mock.Of<ILogger<IdentityManager>>();
            var repository = new IdentityManager(tableServiceClient, logger);


            // Execute our Test
            var result = repository.IncrementId(value);

            // Verify service was only called once
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task GetNextIdAsync_Updates_Existing()
        {
            // Setup Test
            TableServiceClient tableServiceClient = Mock.Of<TableServiceClient>();
            TableClient tableClient = Mock.Of<TableClient>();
            Mock.Get(tableServiceClient)
                .Setup(tsc => tsc.GetTableClient(It.Is<string>(v => "identity".Equals(v))))
                .Returns(tableClient)
                .Verifiable(Times.Once());

            NullableResponse<TodoIdentity> getResponse = Mock.Of<NullableResponse<TodoIdentity>>();
            Mock.Get(getResponse).Setup(t => t.HasValue).Returns(true);
            Mock.Get(getResponse).Setup(t => t.Value).Returns(new TodoIdentity { NextId = "00000005", ETag = new ETag("TAG123") });

            Mock.Get(tableClient)
               .Setup(tsc => tsc.GetEntityIfExistsAsync<TodoIdentity>(
                   It.Is<string>(v => "list1".Equals(v)),
                   It.Is<string>(v => "next_id".Equals(v)),
                   It.Is<IEnumerable<string>>(v => v == null),
                   It.IsAny<CancellationToken>()))
               .ReturnsAsync(getResponse)
               .Verifiable(Times.Once());

            Response updateResponse = Mock.Of<Response>();
            Mock.Get(updateResponse).SetupGet(r => r.IsError).Returns(false);

            Mock.Get(tableClient)
               .Setup(tsc => tsc.UpdateEntityAsync(
                   It.Is<TodoIdentity>(v => "00000006".Equals(v.NextId)),
                   It.Is<ETag>(v => new ETag("TAG123").Equals(v)),
                   It.IsAny<TableUpdateMode>(),
                   It.IsAny<CancellationToken>()))
               .ReturnsAsync(updateResponse)
               .Verifiable(Times.Once());

            ILogger<IdentityManager> logger = Mock.Of<ILogger<IdentityManager>>();

            var repository = new IdentityManager(tableServiceClient, logger);


            // Execute our Test
            var result = await repository.GetNextIdAsync("list1");

            // Verify service was only called once
            Assert.Equal("00000006", result);
            Mock.VerifyAll();
        }

        [Fact]
        public async Task GetNextIdAsync_First_Id()
        {
            // Setup Test
            TableServiceClient tableServiceClient = Mock.Of<TableServiceClient>();
            TableClient tableClient = Mock.Of<TableClient>();
            Mock.Get(tableServiceClient)
                .Setup(tsc => tsc.GetTableClient(It.Is<string>(v => "identity".Equals(v))))
                .Returns(tableClient)
                .Verifiable(Times.Once());

            NullableResponse<TodoIdentity> getResponse = Mock.Of<NullableResponse<TodoIdentity>>();
            Mock.Get(getResponse).Setup(t => t.HasValue).Returns(false);

            Mock.Get(tableClient)
               .Setup(tsc => tsc.GetEntityIfExistsAsync<TodoIdentity>(
                   It.Is<string>(v => "list3".Equals(v)),
                   It.Is<string>(v => "next_id".Equals(v)),
                   It.Is<IEnumerable<string>>(v => v == null),
                   It.IsAny<CancellationToken>()))
               .ReturnsAsync(getResponse)
               .Verifiable(Times.Once());

            Response addResponse = Mock.Of<Response>();
            Mock.Get(addResponse).SetupGet(r => r.IsError).Returns(false);

            Mock.Get(tableClient)
               .Setup(tsc => tsc.AddEntityAsync(
                   It.Is<TodoIdentity>(v => "00000001".Equals(v.NextId)),
                   It.IsAny<CancellationToken>()))
               .ReturnsAsync(addResponse)
               .Verifiable(Times.Once());

            ILogger<IdentityManager> logger = Mock.Of<ILogger<IdentityManager>>();

            var repository = new IdentityManager(tableServiceClient, logger);


            // Execute our Test
            var result = await repository.GetNextIdAsync("list3");

            // Verify service was only called once
            Assert.Equal("00000001", result);
            Mock.VerifyAll();
        }

    }
}
