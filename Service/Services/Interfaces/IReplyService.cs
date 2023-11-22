using Service.ViewModels;

namespace Service.Services.Interfaces
{
    public interface IReplyService
    {
        Task CreateReplyAsync(ReplyViewModel replyVM);
    }
}
