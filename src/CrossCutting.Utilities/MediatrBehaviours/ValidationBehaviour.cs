using FluentValidation;
using MediatR;

namespace Common.Utilities.MediatrBehaviours
{
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if (failures.Count != 0)
                {
                    //if (request is CustomCommand)
                    //{
                    //    throw new CustomException(failures.First().ErrorCode, failures.First().ErrorMessage);
                    //}

                    //this will only throw the first failures, can improve to show all
                    throw new DefaultValidationException(failures.First().ErrorCode, failures.First().ErrorMessage);
                }

            }
            return await next();
        }
    }
}
