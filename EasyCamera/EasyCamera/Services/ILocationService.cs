namespace EasyCamera.Services
{
    public interface ILocationService
    {
        void GetMyLocation();

        double GetDistanceTravelled(double latitude, double longitude);
    }
}