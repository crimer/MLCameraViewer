using System.Collections.ObjectModel;
using System.Linq;
using CameraViewer.Models;
using GalaSoft.MvvmLight;

namespace CameraViewer.Dialogs.CreateFrameDialog
{
    /// <summary>
    /// ViewModel создания фрейма камеры
    /// </summary>
    public class CreateFrameVM : ObservableObject
    {
        /// <summary>
        /// Коллекция камер 
        /// </summary>
        public ObservableCollection<WebCameraInfo> CamerasCollection { get; set; }
        
        /// <summary>
        /// Выбранная камера
        /// </summary>
        public WebCameraInfo SelectedCamera { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="webCameraCollections">КОллекция камер</param>
        public CreateFrameVM(ObservableCollection<WebCameraInfo> webCameraCollections)
        {
            CamerasCollection = webCameraCollections;
            SelectedCamera = webCameraCollections.FirstOrDefault();
        }
    }
}