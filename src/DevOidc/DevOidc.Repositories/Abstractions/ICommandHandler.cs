using System.Threading.Tasks;

namespace DevOidc.Repositories.Abstractions
{
    public interface ICommandHandler
    {
    }

    public interface ICommandHandler<TCommand> : ICommandHandler
        where TCommand : ICommand
    {
        Task HandleAsync(TCommand command);
    }
}
