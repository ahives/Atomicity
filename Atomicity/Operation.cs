namespace Atomicity;

using Configuration;

public record Operation
{
    public Func<bool> Work { get; init; }
    
    public Action Compensation { get; init; }
    
    public OperationConfig Config { get; init; }
}