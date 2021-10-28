using RX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messages
{
    public class CommandService : ICommandService
    {
        private SortedDictionary<int, LinkedList<IAsyncObserver<ICommand>>> _observers = new SortedDictionary<int, LinkedList<IAsyncObserver<ICommand>>>();

        public async Task Apply<TMessage>(TMessage message) where TMessage : ICommand
        {
            foreach (var list in _observers.Values)
            {
                foreach (var observer in list)
                {
                    await observer.OnNext(message);
                }
            }
        }

        public IDisposable Subscribe(IAsyncObserver<ICommand> observer)
        {
            if (!_observers.ContainsKey(observer.Priority))
            {
                _observers[observer.Priority] = new LinkedList<IAsyncObserver<ICommand>>();
            }
            _observers[observer.Priority].AddLast(observer);

            return new DisposeToken(() =>
            {
                _observers[observer.Priority].Remove(observer);
                observer.OnCompleted();
            });
        }

        private struct DisposeToken : IDisposable
        {
            private Action DisposeAction { get; }

            public DisposeToken(Action dispose) => DisposeAction = dispose;
            public void Dispose() => DisposeAction?.Invoke();
        }
    }

    internal class AsyncObserverComparer<T> : IComparer<IAsyncObserver<T>>
    {
        public int Compare(IAsyncObserver<T> x, IAsyncObserver<T> y)
        {
            if (x.Priority > y.Priority)
                return 1;
            else if (x.Priority < y.Priority)
                return -1;
            return 0;
        }
    }
}