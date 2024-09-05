using FastEndpoints;
using FastEndpoints.Testing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TodoApi.Data;
using TodoApi.Features.Todo.Get;
using Create = TodoApi.Features.Todo.Create;
using Get = TodoApi.Features.Todo.Get;

namespace TodoApiTests.Features.Todo
{
    public class TodoEndpointTests
    {
        [Fact]
        public async Task Verify_Create()
        {
            // Setup the test
            ITodoRepository mockRepo = Mock.Of<ITodoRepository>();
            Mock.Get(mockRepo)
                .Setup(r => r.AddTodoAsync(It.IsAny<string>(), It.IsAny<TodoEntity>()))
                .Callback<string, TodoEntity>((_, entity) => entity.RowKey = "12345")
                .ReturnsAsync(true);

            var request = new Create.CreateTodoRequest
            {
                Description = "description",
                DueDate = null,
                IsCompleted = false,
                ReminderDays = 0,
                TodoList = "list1"
            };

            // Perform the test
            var endpoint = Factory.Create<Create.Endpoint>(mockRepo);
            await endpoint.HandleAsync(request, CancellationToken.None);

            // Validate
            Assert.NotNull(endpoint.Response);
            Assert.Equal("12345", endpoint.Response.Id);
        }

        [Fact]
        public async Task Verify_Get()
        {
            // Setup the test
            ITodoRepository mockRepo = Mock.Of<ITodoRepository>();
            var results = new List<TodoEntity>()
            {
                new TodoEntity{Description = "desc1"},
                new TodoEntity{Description = "desc3"}
            };
            Mock.Get(mockRepo)
                .Setup(r => r.GetTodosAsync(It.IsAny<string>()))
                .ReturnsAsync(results);


            // Perform the test
            DefaultHttpContext httpContext = new DefaultHttpContext();
            Factory.AddTestServices(httpContext, s =>
            {
                s.AddServicesForUnitTesting();
                s.AddScoped(_ => mockRepo);
            });
            httpContext.Request.RouteValues.Add("TodoList", "test1");
            var endpoint = Factory.Create<Get.Endpoint>(httpContext);
            endpoint.Map = new GetTodoMapper();
            await endpoint.HandleAsync(CancellationToken.None);

            // Validate
            Assert.NotNull(endpoint.Response);
            Assert.Equal(2, endpoint.Response.Count());
            Assert.Contains(endpoint.Response, i => i.Description == "desc1");
            Assert.Contains(endpoint.Response, i => i.Description == "desc3");
        }
    }
}
