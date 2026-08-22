using Gonzaga.SalesDataProcessor.Api.Common.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gonzaga.SalesDataProcessor.Api.Tests.Common.Middlewares
{
    public class ApiKeyMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_MissingHeader_Returns401AndDoesNotCallNext()
        {
            var middleware = CreateMiddleware("expected-key");
            var context = new DefaultHttpContext();
            var nextCalled = false;

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => middleware.InvokeAsync(context, _ => { nextCalled = true; return Task.CompletedTask; }));

            Assert.False(nextCalled);
        }

        [Fact]
        public async Task InvokeAsync_WrongKey_Returns401AndDoesNotCallNext()
        {
            var middleware = CreateMiddleware("expected-key");
            var context = new DefaultHttpContext();
            context.Request.Headers["X-API-KEY"] = "wrong-key";
            var nextCalled = false;

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => middleware.InvokeAsync(context, _ => { nextCalled = true; return Task.CompletedTask; }));

            Assert.False(nextCalled);
        }

        [Fact]
        public async Task InvokeAsync_CorrectKey_CallsNext()
        {
            var middleware = CreateMiddleware("expected-key");
            var context = new DefaultHttpContext();
            context.Request.Headers["X-API-KEY"] = "expected-key";
            var nextCalled = false;

            await middleware.InvokeAsync(context, _ => { nextCalled = true; return Task.CompletedTask; });

            Assert.True(nextCalled);
        }

        private static ApiKeyMiddleware CreateMiddleware(string configuredKey)
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> { ["ApiKey"] = configuredKey })
                .Build();

            return new ApiKeyMiddleware(configuration, NullLogger<ApiKeyMiddleware>.Instance);
        }
    }
}
