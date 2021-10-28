using RX;

namespace Messages
{
    public static class MessagesExtentions
    {
        public static IAsyncObservable<TMessage> Filter<TMessage>(this IAsyncObservable<ICommand> observable) where TMessage : ICommand
        {
            return observable.Where(x => x is TMessage).Select(x => (TMessage)x);
        }
    }
}