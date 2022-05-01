using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Daybot
{
  class BackgroundWorker
  {
    Thread WorkerThread;
    bool Stop = false;

    public void Start()
    {
      WorkerThread = new Thread(new ThreadStart(Run));
      WorkerThread.Start();
      Thread.Sleep(1);
    }

    public void Run()
    {
      if (Stop) {
        return;
      }

      while (!Stop) {
        Thread.Sleep(100);
        Tick();
      }
    }

    public void DoStop()
    {
      Stop = true;
      WorkerThread.Join();
    }

    public void Tick()
    {
      //will be called every 100ms;
      // -> Do stuff here
      Program.NumberHandlerInstance.Tick();
    }
  }
}
