using MBAM.Annotations.Domain.Core.Commands;
using MBAM.Annotations.Domain.Core.Events;
using System.Threading.Tasks;

namespace MBAM.Annotations.Domain.Interfaces
{
    public interface IMediatorHandler
    {
        Task SendCommand<T>(T comando) where T : Command;
        Task RaiseEvent<T>(T evento) where T : Event;
    }
}
