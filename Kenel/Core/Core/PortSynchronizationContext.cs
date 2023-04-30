
namespace ConsoleApp63;

public class PortSynchronizationContext : SynchronizationContext
{

    public override void Post(SendOrPostCallback d, object? state)
    {
        d(state);
    }

}