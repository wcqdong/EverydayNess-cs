namespace Core.Core;

public interface IThread
{
    public void OnStart();
    public void OnTick();
    public void OnStop();

}