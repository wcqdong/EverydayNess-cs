using System.Diagnostics;
using Core.Support;

namespace Core.Core;

public class ThreadHandler
{
    private Thread _thread;
    private IThread _case;
    private readonly Stopwatch _watch;
    private volatile bool running;

    public ThreadHandler(IThread @case)
    {
        _case = @case;
        _watch = new Stopwatch();
    }

    public void StartUp()
    {
        if (running)
        {
            return;
        }

        running = true;
        start();
    }

    private void start()
    {
        _thread = new Thread(Run);
        _thread.Start();
    }

    private void Run()
    {
        _case.OnStart();
        while (running)
        {
            try
            {
                _watch.Start();
                _case.OnTick();;
                _watch.Stop();

                long runningTime = _watch.ElapsedMilliseconds;
                Console.WriteLine($"running time {runningTime}ms");
                if (runningTime < CoreConst.TickInterval)
                {
                    Thread.Sleep((int)(CoreConst.TickInterval - runningTime));
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                _watch.Reset();
            }

        }

        _case.OnStop();
    }
}