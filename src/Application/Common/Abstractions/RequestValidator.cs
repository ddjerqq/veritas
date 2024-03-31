using FluentValidation;

namespace Application.Common.Abstractions;

public abstract class RequestValidator<T> : AbstractValidator<T>
{
    protected RequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
    }
}