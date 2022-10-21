namespace AtomicityTests;

using Atomicity;

public class Tests
{
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

        new Atomicity(new Durability())
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