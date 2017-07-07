using Plugin.Media;
using System;
using System.Text;
using Xamarin.Forms;
using System.IO;
using ExifLib;
using Plugin.Media.Abstractions;
using EasyCamera.Views;
using System.Collections.Generic;
using EasyCamera.Data;

namespace EasyCamera
{
    public partial class MainPage : ContentPage
    {
        List<PhotoMetadata> photos;

        public MainPage()
        {
            InitializeComponent();

            photos = new List<PhotoMetadata>();
        }

        private async void takePhoto_Clicked(object sender, EventArgs e)
        {
            if(!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera detected", "No Camera detected", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                Directory = "Sample",
                Name = "test.jpg"
            });

            var modal = new PhotoDetailsView();
            //await Navigation.PushModalAsync(modal, false);
            
            //await Navigation.PopModalAsync(true);

            if (file == null)
                return;

            GetPhotoLocation(file);

            await DisplayAlert("File Location", file.Path, "OK");
            
            image.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                file.Dispose();
                return stream;
            });

            photos.Add(new PhotoMetadata { Latitude = Convert.ToDouble(latitude.Text), Longitude = Convert.ToDouble(longitude.Text), FileName = "Photo.jpg", Timestamp = DateTime.Now });
        }

        private void GetPhotoLocation(MediaFile file)
        {
            using (Stream photo = file.GetStream())
            {
                var picture = ExifReader.ReadJpeg(photo);
                ExifOrientation orientation = picture.Orientation;
                ExifGpsLatitudeRef latRef = picture.GpsLatitudeRef;
                ExifGpsLongitudeRef longRef = picture.GpsLongitudeRef;

                latitude.Text = GetLatitude(latRef, picture.GpsLatitude).ToString();
                longitude.Text = GetLongitude(picture.GpsLongitude).ToString();
            }
        }

        private double GetLatitude(ExifGpsLatitudeRef latRef, double [] data)
        {
            double degrees = data[0];
            double minutes = data[1];
            double seconds = data.Length > 2 ? data[2] : 0.0;

            double result = ConvertDegreeToAngle(degrees, minutes, seconds);

            if (latRef == ExifGpsLatitudeRef.South)
                result *= -1;

            return result;
        }

        private double GetLongitude(double[] data)
        {
            double degrees = data[0];
            double minutes = data[1];
            double seconds = data.Length > 2 ? data[2] : 0.0;

            double result = ConvertDegreeToAngle(degrees, minutes, seconds);

            return result;
        }

        private double ConvertDegreeToAngle(double degrees, double minutes, double seconds)
        {
            return degrees + (minutes / 60) + (seconds / 3600);
        }

        private async void pickPhoto_Clicked(object sender, EventArgs e)
        {
            if(!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Photos Not Supported", "Permission not granted for picking photos.", "OK");
                return;
            }

            var file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
            });

            if (file == null)
                return;

            GetPhotoLocation(file);

            image.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                file.Dispose();
                return stream;
            });
            
            //using (StreamReader sr = new StreamReader(file.GetStream()))
            //{
            //    var picStr = sr.ReadToEnd();
            //    var bytes = Encoding.UTF8.GetBytes(picStr);
            //}

            /*using (Stream photo = file.GetStream())
            {
                var picture = ExifReader.ReadJpeg(photo);
                ExifOrientation orientation = picture.Orientation;

                latitude.Text = GetGpsCoordinate(picture.GpsLatitude);
                longitude.Text = GetGpsCoordinate(picture.GpsLongitude);
            }*/
        }

        //private string GetGpsCoordinate(double[] data)
        //{
        //    if (!data.Any())
        //        return string.Empty;
        //    int index = 0;

        //    string str = data.First() + ".";

        //    for (int i = 1; i < data.Length; i++)
        //    {
        //        str += BitConverter.ToUInt32(data[i], index).ToString();
        //    }

        //    return str;
        //}

        private async void takeVideo_Clicked(object sender, EventArgs e)
        {
            if(!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakeVideoSupported)
            {
                await DisplayAlert("Video not available", "Video not available", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakeVideoAsync(new Plugin.Media.Abstractions.StoreVideoOptions { Name = "video.mp4", Directory = "DefaultVideos" });

            if (file == null)
                return;

            await DisplayAlert("Video Recorded", "Location: " + file.Path, "OK");

            file.Dispose();
        }

        private async void pickVideo_Clicked(object sender, EventArgs e)
        {
            if(!CrossMedia.Current.IsPickVideoSupported)
            {
                await DisplayAlert("Videos Not Supported", ":( Permission not granted to videos.", "OK");
                return;
            }

            var file = await CrossMedia.Current.PickVideoAsync();

            if (file == null)
                return;

            await DisplayAlert("Video Selected", "Location: " + file.Path, "OK");
            file.Dispose();
        }
    }
}
