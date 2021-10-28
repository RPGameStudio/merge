
using System;
using System.Threading.Tasks;

namespace RX
{
    public interface IReadonlyReactiveProperty<out T> : IAsyncObservable<T>
    {
        T Value { get; }
    }

    public interface IReactiveProperty<T> : IReadonlyReactiveProperty<T>
    {
        new T Value { get; set; }
    }

    public interface IAsyncObservable<out T>
    {
        IDisposable Subscribe(IAsyncObserver<T> observer);
    }

    public interface IAsyncObserver<in T>
    {
        bool SkipLatestOnSubscribe { get; }
        int Priority { get; }

        Task OnNext(T value);
        Task OnError(Exception e);
        Task OnCompleted();

    }
}