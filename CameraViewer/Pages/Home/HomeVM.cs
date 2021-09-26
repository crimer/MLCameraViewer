using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using CameraViewer.Dialogs.AlertDialog;
using CameraViewer.Dialogs.CreateFrameDialog;
using CameraViewer.Models;
using CameraViewer.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Logging;

namespace CameraViewer.Pages.Home
{
    /// <summary>
    /// ViewModel главной страницы
    /// </summary>
    public class HomeVM :  ObservableObject, IDisposable
    {
        #region Private fields
        
        private BitmapImage _image;
        private string _ipCameraUrl;
        private bool _isDesktopSource;
        private bool _isIpCameraSource;
        private bool _isWebcamSource;

        #endregion
        public BitmapImage BitmapImage { get => _image; set { Set(ref _image, value); } }
        public ObservableCollection<Camera> CamerasCollection { get; set; }
        private readonly CreateFrameVM _createFrameVM;
        private readonly AlertDialog _alertDialog;
        private readonly CreateFrameDialog _createFrameDialog;
        private bool _frameCreationParameterResult;
        public ObservableCollection<WebCameraInfo> WebCameraCollections { get; set; }
        public ICommand OpenCreateFrameDialogCommand { get; private set; }
        public ICommand ConnectToCameraCommand { get; private set; }
        public ICommand DisconnectToCameraCommand { get; private set; }
   
        private readonly VideoService _videoService;
        private readonly ILogger<HomeVM> _logger;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="videoService">Сервис видео</param>
        /// <param name="logger">Логгер</param>
        public HomeVM(VideoService videoService, ILogger<HomeVM> logger)
        {
            _logger = logger;
            _videoService = videoService;

            CamerasCollection = new ObservableCollection<Camera>();
            WebCameraCollections = new ObservableCollection<WebCameraInfo>(_videoService.GetCameras());
            
            OpenCreateFrameDialogCommand = new RelayCommand(OpenCreateFrameDialog);
            ConnectToCameraCommand = new RelayCommand<object>(ConnectToCamera);
            DisconnectToCameraCommand = new RelayCommand<object>(DisconnectToCamera);

            _alertDialog = new AlertDialog();
            _createFrameVM = new CreateFrameVM();
            _createFrameDialog = new CreateFrameDialog
            {
                DataContext = _createFrameVM,
            };
        }

        private void DisconnectToCamera(object obj)
        {
            var camera = (Camera)obj;
            camera.Handler.Disconnect();
            CamerasCollection.Remove(camera);
        }

        private void ConnectToCamera(object obj)
        {
            var camera = (Camera)obj;
            camera.Handler.Connect(camera);
        }
        
        /// <summary>
        /// Открытие диалога создания фрейма
        /// </summary>
        private async void OpenCreateFrameDialog()
        {
            try
            {
                var res = await DialogHost.Show(
                    _createFrameDialog, 
                    "RootDialog", (sender, args) =>
                    {
                        if (!bool.TryParse((string) args.Parameter, out _frameCreationParameterResult))
                            return;
                    });
                
                if(!_frameCreationParameterResult)
                    return;
                
                var isValid = await Validate(_createFrameVM.Name, _createFrameVM.Ip, _createFrameVM.Port);
                if(!isValid)
                    return;

                var camera = new Camera(_createFrameVM.Name, _createFrameVM.Ip, _createFrameVM.Port);
                
                
                camera.MonikerString = WebCameraCollections[0].CameraMonikerString;
                CamerasCollection.Add(camera);
            }
            catch (Exception ex)
            {
                _logger.LogError($"При создании камеры произошла ошибка: {ex}");
            }
        }

        /// <summary>
        /// Валидация данных с модального окна
        /// </summary>
        /// <param name="name">Имя фрейма</param>
        /// <param name="ipaddress">IP адрес камеры</param>
        /// <param name="port">Порт камеры</param>
        /// <returns>Результат валидации</returns>
        private async Task<bool> Validate(string name, string ipaddress, string port)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    _alertDialog.ModalText.Text = "Некорректное имя";
                    await DialogHost.Show(_alertDialog, "RootDialog");
                    return false;
                }
                if (!IPAddress.TryParse(ipaddress, out var ip))
                {
                    _alertDialog.ModalText.Text = "Некорректное IP адрес";
                    await DialogHost.Show(_alertDialog, "RootDialog");
                    return false;
                }
                if (!int.TryParse(port, out var parsedPort) || parsedPort <= 0 || parsedPort > 65000)
                {
                    _alertDialog.ModalText.Text = "Некорректное порт";
                    await DialogHost.Show(_alertDialog, "RootDialog");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Во время валидации произошла ошибка: {ex}");
                return false;
            }
        }

        public void Dispose()
        {
        }
    }
}