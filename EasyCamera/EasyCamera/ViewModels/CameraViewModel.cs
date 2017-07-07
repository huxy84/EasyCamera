using EasyCamera.Data;
using EasyCamera.Data.Helpers;
using EasyCamera.Services;
using EasyCamera.Services.Interfaces;
using ExifLib;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace EasyCamera.ViewModels
{
    public class CameraViewModel : INotifyPropertyChanged
    {
        ILocationService locationService;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;

            if (handler == null)
                return;

            handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void GetLocation()
        {
            locationService.GetMyLocation();
        }

        public CameraViewModel()
        {
            locationService = DependencyService.Get<ILocationService>();

            if (locationService == null)
                return;

            locationService.GetMyLocation();
        }

        private Command takePhoto;
        public Command TakePhoto
        {
            get { return takePhoto ?? (takePhoto = new Command(ExecuteTakePhoto)); }
        }

        private async void ExecuteTakePhoto()
        {
            if (!CameraAvailable)
                return;

            var photo = await Camera.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Full,
                Directory = "Sample",
                Name = FileName
            });

            if (photo == null)
                return;
        }

        public IMedia Camera => CrossMedia.Current;

        public bool CameraAvailable
        {
            get
            {
                if (Camera == null)
                    return false;

                return Camera.IsCameraAvailable && Camera.IsTakePhotoSupported;
            }
        }

        private void PreviewPhoto(MediaFile mediaFile)
        {
            using (Stream photo = mediaFile.GetStream())
            {
                var picture = ExifReader.ReadJpeg(photo);

                FilePath = mediaFile.Path;

                ExifOrientation orientation = picture.Orientation;
                ExifGpsLatitudeRef latRef = picture.GpsLatitudeRef;
                ExifGpsLongitudeRef longRef = picture.GpsLongitudeRef;

                Latitude = GpsHelper.GetLatitude(latRef, picture.GpsLatitude);
                Longitude = GpsHelper.GetLongitude(picture.GpsLongitude);
            }
        }

        private List<PhotoMetadata> photos;
        public List<PhotoMetadata> Photos
        {
            get { return photos; }

            set
            {
                photos = value;

                OnPropertyChanged();
            }
        }

        private string fileName;
        public string FileName
        {
            get { return fileName; }

            set
            {
                if (fileName == value)
                    return;

                fileName = value;

                OnPropertyChanged();
            }
        }

        private string filePath;
        public string FilePath
        {
            get { return filePath; }

            set
            {
                if (filePath == value)
                    return;

                filePath = value;

                OnPropertyChanged();
            }
        }

        private double latitude;
        public double Latitude
        {
            get { return latitude; }
            set
            {
                latitude = value;

                OnPropertyChanged();
            }
        }

        private double longitude;
        public double Longitude
        {
            get { return longitude; }
            set
            {
                longitude = value;

                OnPropertyChanged();
            }
        }

        private DateTime timestamp;
        public DateTime Timestamp
        {
            get { return timestamp; }
            set
            {
                timestamp = value;
                OnPropertyChanged();
            }
        }

        private void PhotoTaken(object sender, IPhotoMetadata e)
        {
            FileName = e.FileName;
            FilePath = e.FilePath;
            Latitude = e.Latitude;
            Longitude = e.Longitude;
            Timestamp = e.Timestamp;
            Photos.Add(new PhotoMetadata { FileName = FileName, Timestamp = Timestamp, Latitude = Latitude, Longitude = Longitude });
        }
    }
}