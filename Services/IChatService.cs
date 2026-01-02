using System.Threading.Tasks;
using PhapClinicX.Models.ViewModels;

namespace PhapClinicX.Services
{
    public interface IChatService
    {
        Task<ChatReply> AskAsync(ChatQuery query);
    }
}
