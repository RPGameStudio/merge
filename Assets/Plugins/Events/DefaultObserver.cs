using System;
using System.Threading.Tasks;

namespace RX
{
    internal class DefaultObserver<T> : IAsyncObserver<T>
    {
        public Func<T, Task> OnNext;
        public Func<Exception, Task> OnError;
        public Func<Task> OnCompleted;

        public bool SkipLatestOnSubscribe { get; set; }
        public int Priority { get; set; }

        async Task IAsyncObserver<T>.OnCompleted()
        {
            if (OnCompleted != null)
                await OnCompleted();
        }

        async Task IAsyncObserver<T>.OnError(Exception exception)
        {
            if (OnError != null)
                await OnError(exception);
        }

        async Task IAsyncObserver<T>.OnNext(T next)
        {
            if (OnNext != null)
                await OnNext(next);
        }
    }
}