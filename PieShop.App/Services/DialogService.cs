using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PieShop.App.Services
{
    public class DialogService : IDialogService
    {
        public Task ShowAlertAsync(string title, string message, string buttonText)
        {
            return Shell.Current.DisplayAlert(title, message, buttonText);
        }
    }
}
