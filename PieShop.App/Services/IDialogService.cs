
namespace PieShop.App.Services
{
    public interface IDialogService
    {
        Task ShowAlertAsync(string title, string message, string buttonText);
    }
}