using EasyCamera.Services;
using EasyCamera.Services.Interfaces;
using System;

namespace EasyCamera.Data
{
    public class PhotoMetadata : EventArgs, IPhotoMetadata
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime Timestamp { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public event EventHandler<IPhotoMetadata> PhotoTaken;
    }
}