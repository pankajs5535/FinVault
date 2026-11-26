using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FinVault.Shared.Responses;

namespace FinVault.API.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            // 1. VALIDATE ACTION ARGUMENTS
            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument == null)
                    continue;

                var validatorType = typeof(IValidator<>)
                    .MakeGenericType(argument.GetType());

                var validators = _serviceProvider.GetServices(validatorType)
                    .Cast<IValidator>();

                // 2. EXECUTE FLUENTVALIDATION
                foreach (var validator in validators)
                {
                    var validationContext = new ValidationContext<object>(argument);

                    var result = await validator.ValidateAsync(
                        validationContext,
                        context.HttpContext.RequestAborted);

                    // 3. COLLECT VALIDATION ERRORS
                    if (!result.IsValid)
                    {
                        foreach (var error in result.Errors)
                        {
                            context.ModelState.AddModelError(
                                error.PropertyName,
                                error.ErrorMessage);
                        }
                    }
                }
            }

            // 4. RETURN VALIDATION RESPONSE
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value!.Errors
                            .Select(e => e.ErrorMessage)
                            .ToArray());

                context.Result = new BadRequestObjectResult(
                 ApiResponse<object>.FailureResponse(
                     "Validation failed.",
                     StatusCodes.Status400BadRequest,
                     errors,
                     context.HttpContext.TraceIdentifier));

                return;
            }

            // 5. CONTINUE REQUEST PIPELINE
            await next();
        }
    }
}
