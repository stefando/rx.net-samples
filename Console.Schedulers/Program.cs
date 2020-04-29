using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;

namespace Console.Schedulers
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Starting on threadId:{0}", Thread.CurrentThread.ManagedThreadId);
            var source = Observable.Create<int>(
                o =>
                {
                    System.Console.WriteLine("Observable.create start on threadId:{0}", Thread.CurrentThread.ManagedThreadId);
                    o.OnNext(1);
                    o.OnNext(2);
                    o.OnNext(3);
                    o.OnCompleted();
                    System.Console.WriteLine("Observable.create end on threadId:{0}", Thread.CurrentThread.ManagedThreadId);
                    return Disposable.Empty;
                });
            source 
                //.SubscribeOn(Scheduler.ThreadPool)
                .Subscribe(
                    o => System.Console.WriteLine("Received {1} on threadId:{0}",
                        Thread.CurrentThread.ManagedThreadId,
                        o),
                    () => System.Console.WriteLine("OnCompleted on threadId:{0}",
                        Thread.CurrentThread.ManagedThreadId));
            System.Console.WriteLine("Finishing on threadId:{0}", Thread.CurrentThread.ManagedThreadId);

            //System.Console.ReadKey();
        }
    }
}
