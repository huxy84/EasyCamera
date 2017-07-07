using System;

namespace EasyCamera.Services.Interfaces
{
    public interface IPhotoMetadata
    {
        string FileName { get; set; }
        string FilePath { get; set; }
        DateTime Timestamp { get; set; }
        double Latitude { get; set; }
        double Longitude { get; set; }

        event EventHandler<IPhotoMetadata> PhotoTaken;
    }
}