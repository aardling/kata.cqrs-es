using CQRS.UpHealth.Commands;

namespace CQRS.UpHealth
{
    public interface ICommandHandler<T> where T : class , ICommand
    {
        void Handle(T command);
    }
}
