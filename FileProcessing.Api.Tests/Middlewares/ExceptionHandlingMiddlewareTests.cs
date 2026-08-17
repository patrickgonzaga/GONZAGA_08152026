using CsvHelper;
using CsvHelper.Configuration;
using FileProcessing.Api.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using System.Globalization;

namespace FileProcessing.Api.Tests.Middlewares
{
    public class ExceptionHandlingMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_InvalidDataException_Returns400()
        {
            var middleware = new ExceptionHandlingMiddleware(NullLogger<ExceptionHandlingMiddleware>.Instance);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            await middleware.InvokeAsync(context, _ => throw new InvalidDataException());

            Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_UnexpectedException_Returns500()
        {
            var middleware = new ExceptionHandlingMiddleware(NullLogger<ExceptionHandlingMiddleware>.Instance);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            await middleware.InvokeAsync(context, _ => throw new InvalidOperationException());

            Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_CsvHelperException_Returns400()
        {
            var middleware = new ExceptionHandlingMiddleware(NullLogger<ExceptionHandlingMiddleware>.Instance);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            var csvContext = new CsvContext(new CsvConfiguration(CultureInfo.InvariantCulture));

            await middleware.InvokeAsync(context, _ => throw new CsvHelperException(csvContext, "bad csv"));

            Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_UnauthorizedAccessException_Returns401()
        {
            var middleware = new ExceptionHandlingMiddleware(NullLogger<ExceptionHandlingMiddleware>.Instance);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            await middleware.InvokeAsync(context, _ => throw new UnauthorizedAccessException("no key"));

            Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        }
    }
}
