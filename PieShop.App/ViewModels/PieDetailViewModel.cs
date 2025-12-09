using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PieShop.App.Message;
using PieShop.App.Models;
using PieShop.App.Services;

namespace PieShop.App.ViewModels
{
    [QueryProperty(nameof(SelectedPie),"selectedPie")]
    public partial class PieDetailViewModel : ObservableObject
    {
        [ObservableProperty]
        private Pie selectedPie = new Pie();

        [ObservableProperty]
        private ImageSource _imagePreview;

        private IPieRepository _repository;
        private byte[] _photoBytes;
        private readonly INavigationService _navigation;
        private readonly IMessenger _messenger;
        private readonly IMediaPicker _mediaPicker;
        private readonly IDialogService _dialog;

        // public ICommand SaveCommand { get; }

        public PieDetailViewModel(
            IPieRepository pieRepository, 
            INavigationService navigation, 
            IMessenger messenger,
            IMediaPicker mediaPicker,
            IDialogService dialog) 
        {
            _repository = pieRepository;
            _navigation = navigation;
            _messenger = messenger;
            _mediaPicker = mediaPicker;
            _dialog = dialog;
        }

        [RelayCommand]
        private async Task OnSave()
        {
            if (SelectedPie.Id == Guid.Empty)
            {
                await _repository.AddPie(SelectedPie);
                await _repository.Upload
                _messenger.Send(new PieCreatedMessage(SelectedPie));
            }
            else
            {
                await _repository.UpdatePie(SelectedPie);
                _messenger.Send(new PieUpdatedMessage(SelectedPie));
            }

            await _navigation.GoBackAsync();
        }

        [RelayCommand]
        private async Task TakePhotoAsync()
        {
            try
            {
                if (!_mediaPicker.IsCaptureSupported)
                {
                    await _dialog.ShowAlertAsync("Camera", "Foto nemen wordt niet ondersteund op dit toestel.", "OK");
                    return;
                }

                FileResult? photo = await _mediaPicker.CapturePhotoAsync();

                if (photo == null)
                {
                    return;
                }

                using (Stream sourceStream = await photo.OpenReadAsync())
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        await sourceStream.CopyToAsync(memoryStream);
                        _photoBytes = memoryStream.ToArray();
                    }

                    ImagePreview = ImageSource.FromStream(() =>
                    {
                        return new MemoryStream(_photoBytes);
                    });
                }
            }
            catch (Exception ex)
            {
                await _dialog.ShowAlertAsync("Fout", $"Foto nemen is mislukt: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task PickPhotoAsync()
        {
            try
            {
                FileResult? photo = await _mediaPicker.PickPhotoAsync();

                if (photo == null)
                {
                    return; 
                }

                using (Stream sourceStream = await photo.OpenReadAsync())
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        await sourceStream.CopyToAsync(memoryStream);
                        _photoBytes = memoryStream.ToArray();
                    }

                    ImagePreview = ImageSource.FromStream(() =>
                    {
                        return new MemoryStream(_photoBytes);
                    });
                }
            }
            catch (Exception ex)
            {
                await _dialog.ShowAlertAsync("Fout", $"Foto kiezen is mislukt: {ex.Message}", "OK");
            }
        }
    }
}
