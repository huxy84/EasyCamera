using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.IO;
using ExifLib;

namespace EasyCamera
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
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

            if (file == null)
                return;
            
            

            //var test = Encoding.UTF8.GetBytes(file.Path);

            //var strTest = Encoding.UTF8.GetString(test, 0, test.Length);

            //var start = strTest.IndexOf("xmpmeta");
            //var end = strTest.IndexOf("</x:xmpmeta>") + 12;

            //var justTheMeta = strTest.Substring(start, end - start);

            await DisplayAlert("File Location", file.Path, "OK");
            
            image.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                file.Dispose();
                return stream;
            });
        }

        /*private string GetPhotoGps(FileInfo fi)
        {
            FileStream
            return "";
        }*/

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

            image.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                file.Dispose();
                return stream;
            });

            using (Stream photo = file.GetStream())
            {
                var picture = ExifReader.ReadJpeg(photo);
                ExifOrientation orientation = picture.Orientation;

                latitude.Text = GetGpsCoordinate(picture.GpsLatitude);
                longitude.Text = GetGpsCoordinate(picture.GpsLongitude);
            }
        }

        private string GetGpsCoordinate(double[] data)
        {
            if (!data.Any())
                return string.Empty;

            string str = data.First() + ".";

            for (int i = 1; i < data.Length; i++)
            {
                str += data[i];
            }

            return str;
        }

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
