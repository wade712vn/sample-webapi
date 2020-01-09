using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using AccountManager.Application.Exceptions;
using FluentValidation;
using FluentValidation.Results;

namespace AccountManager.WebApi.Filters
{
    public class CustomExceptionHandler : IExceptionHandler
    {
        public async Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            if (context.Exception is ValidationException)
            {
                var resp = context.Request.CreateResponse(HttpStatusCode.BadRequest,
                    new ResponsePackage<ValidationFailure>("There are validation errors",
                        ((ValidationException)context.Exception).Errors.ToList()));

                context.Result = new ErrorMessageResult(context.Request, resp);
            }
            else if (context.Exception is EntityNotFoundException)
            {
                var resp = context.Request.CreateResponse(HttpStatusCode.NotFound,
                    new ResponsePackage<string>(context.Exception.Message, null));

                context.Result = new ErrorMessageResult(context.Request, resp);
            }
            else if (context.Exception is CommandException)
            {
                var resp = context.Request.CreateResponse(HttpStatusCode.BadRequest,
                    new ResponsePackage<string>(context.Exception.Message, null));

                context.Result = new ErrorMessageResult(context.Request, resp);
            }
        }
    }

    public class ResponsePackage<T>
    {
        public string Message { get; set; }
        public List<T> Errors { get; set; }

        public ResponsePackage(string message, List<T> errors)
        {
            Message = message;
            Errors = errors;
        }
    }

    public class ErrorMessageResult : IHttpActionResult
    {
        private HttpRequestMessage _request;
        private HttpResponseMessage _httpResponseMessage;


        public ErrorMessageResult(HttpRequestMessage request, HttpResponseMessage httpResponseMessage)
        {
            _request = request;
            _httpResponseMessage = httpResponseMessage;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_httpResponseMessage);
        }
    }
}