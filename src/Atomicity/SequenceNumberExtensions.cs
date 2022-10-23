namespace Atomicity;

public static class SequenceNumberExtensions
{
    public static Operation SetSequenceNumber(this Operation operation, int sequenceNumber)
    {
        return new()
        {
            Name = operation.Name,
            SequenceNumber = sequenceNumber,
            Compensation = operation.Compensation,
            Work = operation.Work,
            Config = operation.Config
        };
    }
}