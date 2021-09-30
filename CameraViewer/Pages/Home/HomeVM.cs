using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
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
        private readonly CreateFrameVM _createFrameVm;
        private readonly CreateFrameDialog _createFrameDialog;
        private bool _frameCreationParameterResult;
        private readonly ILogger<HomeVM> _logger;
        public ObservableCollection<Camera> CamerasCollection { get; }
        public ObservableCollection<WebCameraInfo> WebCameraCollections { get; }
        public ICommand OpenCreateFrameDialogCommand { get; }
        public ICommand DisconnectToCameraCommand { get; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="cameraService">Сервис камер</param>
        /// <param name="logger">Логгер</param>
        public HomeVM(CameraService cameraService, ILogger<HomeVM> logger)
        {
            _logger = logger;

            CamerasCollection = new ObservableCollection<Camera>();
            WebCameraCollections = new ObservableCollection<WebCameraInfo>(cameraService.GetCameras());
            
            OpenCreateFrameDialogCommand = new RelayCommand(OpenCreateFrameDialog);
            DisconnectToCameraCommand = new RelayCommand<object>(DisconnectToCamera);

            _createFrameVm = new CreateFrameVM(WebCameraCollections);
            _createFrameDialog = new CreateFrameDialog
            {
                DataContext = _createFrameVm,
            };
        }

        private void DisconnectToCamera(object obj)
        {
            var camera = (Camera) obj;
            CamerasCollection.Remove(camera);
        }

        /// <summary>
        /// Открытие диалога создания фрейма
        /// </summary>
        private async void OpenCreateFrameDialog()
        {
            try
            {
                await DialogHost.Show(
                    _createFrameDialog, 
                    "RootDialog", (sender, args) =>
                    {
                        if (!bool.TryParse((string) args.Parameter, out _frameCreationParameterResult))
                            return;
                    });
                
                if(!_frameCreationParameterResult)
                    return;
                
                CamerasCollection.Add(new Camera(_createFrameVm.SelectedCamera));
            }
            catch (Exception ex)
            {
                _logger.LogError($"При создании камеры произошла ошибка: {ex}");
            }
        }
    }
}