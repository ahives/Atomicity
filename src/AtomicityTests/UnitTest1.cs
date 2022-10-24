namespace AtomicityTests;

using Atomicity;
using Atomicity.Extensions.DependencyInjection;
using Atomicity.Persistence;
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
        var op1 = Factory.CreateOperation<Operation1>();
        var op2 = Factory.CreateOperation<Operation2>();
        var op3 = Factory.CreateOperation<Operation3>();

        new TransactionBroker(new TransactionPersistenceProvider())
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
        var op1 = Factory.CreateOperation<Operation1>();
        var op2 = Factory.CreateOperation<Operation2>();
        var op3 = Factory.CreateOperation<Operation3>();

        _services.GetService<ITransactionBroker>()
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
        TransactionOperation<Operation1>
    {
        protected override Func<bool> DoWork()
        {
            return () => true;
        }

        protected override Action Compensate()
        {
            return () =>
            {
                Console.WriteLine("Something went wrong in Operation 1");
            };
        }
    }

    class Operation2 :
        TransactionOperation<Operation2>
    {
        protected override Func<bool> DoWork()
        {
            return () => true;
        }

        protected override Action Compensate()
        {
            return () =>
            {
                Console.WriteLine("Something went wrong in Operation 2");
            };
        }
    }

    class Operation3 :
        TransactionOperation<Operation3>
    {
        protected override Func<bool> DoWork()
        {
            return () => false;
        }
    }
}