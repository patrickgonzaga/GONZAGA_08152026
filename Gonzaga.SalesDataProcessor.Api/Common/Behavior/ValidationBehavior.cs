using FluentValidation;
using MediatR;

namespace Gonzaga.SalesDataProcessor.Api.Common.Behavior
{
    /// <summary>
    /// MediatR pipeline behavior that runs all registered FluentValidation validators
    /// for a request before it reaches its handler.
    /// </summary>
    public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// Validates <paramref name="request"/> and throws <see cref="ValidationException"/>
        /// if any validator fails; otherwise forwards the request to the next handler in the pipeline.
        /// </summary>
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!validators.Any())
            {
                return await next(cancellationToken);
            }

            var failures = new List<FluentValidation.Results.ValidationFailure>();

            foreach (var validator in validators)
            {
                var result = await validator.ValidateAsync(request, cancellationToken);
                failures.AddRange(result.Errors);
            }

            if (failures.Any())
            {
                throw new ValidationException(failures);
            }

            return await next(cancellationToken);
        }
    }
}
