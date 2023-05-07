using BlazorSozluk.Common.Models.RequestModels;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSozluk.Api.Application.Features.Commands.User.Create
{
    public class CreateUserCommandValidator:AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(a => a.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(a=>a.LastName).NotEmpty().MaximumLength(50);
            RuleFor(a=>a.UserName).NotEmpty().MaximumLength(12);
            RuleFor(a => a.EmailAddress).NotNull().EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible);
            RuleFor(a => a.Password).NotNull().WithMessage("{PropertyName} should at least be {MinimumLength} characters ");
        }
    }
}
