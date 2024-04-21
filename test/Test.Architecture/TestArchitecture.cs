using Application;
using Domain;
using MediatR;
using NetArchTest.Rules;

namespace Test.Architecture;

public class TestArchitecture
{
    [Test]
    public void TestAllDomainClassesSealed()
    {
        var result = Types
            .InAssembly(DomainAssembly.Assembly)
            .That()
            .AreClasses()
            .Should()
            .BeSealed()
            .Or()
            .BeAbstract()
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True);
    }

    [Test]
    public void TestDomainDoesNotDependOnApplicationOrInfrastructure()
    {
        var result = Types
            .InAssembly(DomainAssembly.Assembly)
            .ShouldNot()
            .HaveDependencyOnAny("Application", "Infrastructure")
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True);
    }

    [Test]
    public void TestApplicationDoesNotDependOnInfrastructure()
    {
        var result = Types
            .InAssembly(ApplicationAssembly.Assembly)
            .Should()
            .NotHaveDependencyOn("Infrastructure")
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True);
    }

    [Test]
    public void TestCommandHandlersAreProperlyNamedAndSealedAndNotPublic()
    {
        var result = Types
            .InAssembly(ApplicationAssembly.Assembly)
            .That()
            .ImplementInterface(typeof(IRequestHandler<>))
            .Should()
            .HaveNameEndingWith("CommandHandler")
            .Or()
            .HaveNameEndingWith("QueryHandler")
            .And()
            .BeSealed()
            .And()
            .NotBePublic()
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True);
    }

    [Test]
    public void TestEventHandlersAreProperlyNamedAndSealedAndNotPublic()
    {
        var result = Types
            .InAssembly(ApplicationAssembly.Assembly)
            .That()
            .ImplementInterface(typeof(INotificationHandler<>))
            .Should()
            .HaveNameEndingWith("EventHandler")
            .And()
            .BeSealed()
            .And()
            .NotBePublic()
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True);
    }
}