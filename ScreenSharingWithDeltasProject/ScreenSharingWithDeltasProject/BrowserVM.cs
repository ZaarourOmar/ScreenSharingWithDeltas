using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ScreenSharingWithDeltasProject
{
    public class BrowserVM : INotifyPropertyChanged
    {

        ObservableCollection<BrowserItem> _screensList = null;
        ObservableCollection<BrowserItem> _applicationsList = null;
        public ObservableCollection<BrowserItem> ScreensList
        {
            get
            {
                return _screensList;
            }

            set
            {
                _screensList = value;
                NotifyPropertyChanged();
            }
        }
        public ObservableCollection<BrowserItem> ApplicationsList
        {
            get
            {
                return _applicationsList;
            }

            set
            {
                _applicationsList = value;
                NotifyPropertyChanged();

            }
        }

        DispatcherTimer _timer = null;
        CapturingManager _capturingManager = null;
        public BrowserVM(CapturingManager manager, DispatcherTimer timer)
        {
            ScreensList = new ObservableCollection<BrowserItem>();
            ApplicationsList = new ObservableCollection<BrowserItem>();

            _capturingManager = manager;

            _timer = timer;
            _timer.Interval = TimeSpan.FromMilliseconds(1200);
            _timer.Tick += (s, e) =>
           {
               UpdateScreensList();
               UpdateApplicationsList();
           };
        }



        private void UpdateApplicationsList()
        {
            var listOfApplications = NativeCallsHelper.GetValidApplicationList();
            foreach (var app in listOfApplications)
            {
                IntPtr appID = app.Key;
                string title = app.Value;

                BitmapSource appShot = _capturingManager.GetAppCapture(appID);

                var existingApp = ApplicationsList.FirstOrDefault(x => x.Id == appID);
                if (existingApp != null)
                {
                    existingApp.Title = title;
                    existingApp.Image = appShot;
                }
                else
                {
                    ApplicationsList.Add(new BrowserItem(appID, title, appShot));
                }
            }
        }

        private void UpdateScreensList()
        {
            var screens = Screen.AllScreens;
            foreach (var screen in screens)
            {
                string title = screen.DeviceName;
                BitmapSource sourceImage = _capturingManager.GetScreenCapture(screen);

                var existingScreen = ScreensList.FirstOrDefault(x => x.Title == title);
                if (existingScreen != null)
                {
                    existingScreen.Image = sourceImage;
                }
                else
                {
                    ScreensList.Add(new BrowserItem(IntPtr.Zero, title, sourceImage));
                }
            }
        }


        public void Start()
        {
            _timer.Start();
        }
        public void Stop()
        {
            _timer.Stop();
        }
        #region PropertyChanged implementation
        /// <summary>
        /// A view model needs to implement INotifyPropertyChanged interface. In this way, we can notify any change of the properties and lists in this view model to the View and the view will be updated accordingly.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }


    public class BrowserItem
    {
        IntPtr _id;
        string _title;
        ImageSource _image;

        public BrowserItem(IntPtr id, string title, ImageSource image)
        {
            this.Id = id;
            this.Title = title;
            this.Image = image;
        }

        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
            }
        }

        public ImageSource Image
        {
            get
            {
                return _image;
            }

            set
            {
                _image = value;
            }
        }

        public IntPtr Id { get; internal set; }
    }
}
