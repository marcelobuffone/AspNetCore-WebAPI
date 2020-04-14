using System.Threading.Tasks;

namespace MBAM.Annotations.Infra.CrossCutting.Identity.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
