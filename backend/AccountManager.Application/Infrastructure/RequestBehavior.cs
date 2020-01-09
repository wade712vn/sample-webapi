using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using AccountManager.Common.Extensions;
using FluentValidation;
using MediatR;

namespace AccountManager.Application.Infrastructure
{
    public class RequestBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public RequestBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var currentUserName = HttpContext.Current?.User?.Identity.Name;

            var command = request as ICommand;
            if (!currentUserName.IsNullOrWhiteSpace() && command != null)
            {
                command.User = currentUserName;
            }

            var context = new ValidationContext(request);

            var failures = _validators
                .Select(v => v.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }

            return next();
        }
    }
}
