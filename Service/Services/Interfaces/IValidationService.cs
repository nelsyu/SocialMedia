using Service.ViewModels;

namespace Service.Services.Interfaces
{
    public interface IValidationService
    {
        Task<List<string>> ValidateRegisterAsync(UserViewModel userVM);
        Task<List<string>> ValidateLoginAsync(UserViewModel userVM);
        Task<List<string>> ValidatePostAsync(PostViewModel postVM);
    }
}
