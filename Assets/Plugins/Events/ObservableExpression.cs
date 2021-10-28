using System;
using System.Threading.Tasks;

namespace RX
{
    internal abstract class ObservableExpression<TSource, TResult> : IAsyncObservable<TResult>, IAsyncObserver<TSource>
    {
        protected IAsyncObservable<TSource> _observable;
        protected IAsyncObserver<TResult> _observer;

        public bool SkipLatestOnSubscribe => _observer.SkipLatestOnSubscribe;
        public int Priority => _observer.Priority;

        public ObservableExpression(IAsyncObservable<TSource> observable)
        {
            _observable = observable;
        }

        public IDisposable Subscribe(IAsyncObserver<TResult> observer)
        {
            _observer = observer;
            var subscribtion = _observable.Subscribe(this);

            return new DisposeToken
            {
                DisposeAction = async () => subscribtion.Dispose()
            };
        }

        async Task IAsyncObserver<TSource>.OnCompleted() => await _observer.OnCompleted();
        async Task IAsyncObserver<TSource>.OnError(Exception exception) => await _observer.OnError(exception);
        public abstract Task OnNext(TSource value);
    }
}