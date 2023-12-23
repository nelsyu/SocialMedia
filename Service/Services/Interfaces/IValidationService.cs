using Service.ViewModels;

namespace Service.Services.Interfaces
{
    public interface IValidationService
    {
        Task<List<(string key, string errorMessage)>> ValidateRegisterAsync(UserViewModel userVM);
        Task<List<(string key, string errorMessage)>> ValidateLoginAsync(UserViewModel userVM);
        Task<List<(string key, string errorMessage)>> ValidatePostAsync(PostViewModel postVM);
    }
}
