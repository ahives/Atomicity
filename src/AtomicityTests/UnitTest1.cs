namespace AtomicityTests;

using Atomicity;
using Atomicity.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[TestFixture]
public class Tests
{
    private ServiceProvider _services;

    [OneTimeSetUp]
    public void Init()
    {
        _services = new ServiceCollection()
            .AddAtomicity()
            .BuildServiceProvider();
    }
    
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        var op1 = Operations.Create<Operation1>();
        var op2 = Operations.Create<Operation2>();
        var op3 = Operations.Create<Operation3>();

        new Atomicity(new TransactionDurability())
            .Configure(x =>
            {
                x.TurnOnConsoleLogging();
                x.Retry();
            })
            .AddOperations(op1, op2, op3)
            .Execute();
        
        Assert.Pass();
    }

    [Test]
    public void Test2()
    {
        var op1 = Operations.Create<Operation1>();
        var op2 = Operations.Create<Operation2>();
        var op3 = Operations.Create<Operation3>();

        _services.GetService<IAtomicity>()
            .Configure(x =>
            {
                x.TurnOnConsoleLogging();
                x.Retry();
            })
            .AddOperations(op1, op2, op3)
            .Execute();
        
        Assert.Pass();
    }

    class Operation1 :
        AtomicityOperation
    {
        protected override Func<bool> Work()
        {
            return () => true;
        }
    }

    class Operation2 :
        AtomicityOperation
    {
        protected override Func<bool> Work()
        {
            return () => true;
        }
    }

    class Operation3 :
        AtomicityOperation
    {
        protected override Func<bool> Work()
        {
            return () => true;
        }
    }
}