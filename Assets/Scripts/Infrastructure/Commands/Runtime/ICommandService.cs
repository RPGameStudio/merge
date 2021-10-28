using System.Threading.Tasks;
using RX;

namespace Messages
{
    public interface ICommand
    {
        bool Important { get; }
    }

    public interface ICommandService : IAsyncObservable<ICommand>
    {
        Task Apply<TMessage>(TMessage message) where TMessage : ICommand;
    }

    public interface ICommandHandler
    {
        void SetupCommandHandlers(ICommandService commandService);
    }
}