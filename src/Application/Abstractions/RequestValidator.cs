using FluentValidation;

namespace Application.Abstractions;

public abstract class RequestValidator<T> : AbstractValidator<T>
{
    protected RequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
    }
}