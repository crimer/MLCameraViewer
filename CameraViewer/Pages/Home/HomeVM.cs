using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
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
    public class HomeVM :  ObservableObject
    {
        public ObservableCollection<Camera> CamerasCollection { get; set; }
        private readonly CreateFrameVM _createFrameVM;
        private readonly CreateFrameDialog _createFrameDialog;
        private bool _frameCreationParameterResult;
        public ObservableCollection<WebCameraInfo> WebCameraCollections { get; set; }
        public ICommand OpenCreateFrameDialogCommand { get; private set; }
        public ICommand ConnectToCameraCommand { get; private set; }
        public ICommand DisconnectToCameraCommand { get; private set; }
        public HomeWindow View { get; set; }

        private readonly CameraService _cameraService;
        private readonly ILogger<HomeVM> _logger;
        
        private Image _image;
        private Canvas _canvas;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="cameraService">Сервис видео</param>
        /// <param name="logger">Логгер</param>
        public HomeVM(CameraService cameraService, ILogger<HomeVM> logger)
        {
            _logger = logger;
            _cameraService = cameraService;

            CamerasCollection = new ObservableCollection<Camera>();
            WebCameraCollections = new ObservableCollection<WebCameraInfo>(_cameraService.GetCameras());
            
            OpenCreateFrameDialogCommand = new RelayCommand(OpenCreateFrameDialog);
            ConnectToCameraCommand = new RelayCommand<object>(ConnectToCamera);
            DisconnectToCameraCommand = new RelayCommand<object>(DisconnectToCamera);

            _createFrameVM = new CreateFrameVM(WebCameraCollections);
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
                
                CamerasCollection.Add(new Camera(_createFrameVM.SelectedCamera));
            }
            catch (Exception ex)
            {
                _logger.LogError($"При создании камеры произошла ошибка: {ex}");
            }
        }
    }
}