using Service.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface IReplyService
    {
        Task CreateReplyAsync(ReplyViewModel replyVM);
    }
}
