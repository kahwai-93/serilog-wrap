using Common.Utilities.Extensions;
using FluentValidation;

namespace Common.Utilities.MediatrBehaviours
{
    public class SampleCommandValidator : AbstractValidator<SampleCommand>
    {
        public SampleCommandValidator()
        {

            RuleFor(p => p.Email).FieldsMustRequired("Email")
                .EmailAddress()
                .WithErrorCode("Sample Error Code")
                .WithMessage("Smaple Error Message");

            RuleFor(p => p.NewPassword).FieldsMustRequired("New Password");
        }
    }

    public class SampleCommand
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }
}
