using ExifLib;
using System.Linq;

namespace EasyCamera.Data.Helpers
{
    public static class GpsHelper
    {
        /*private double degrees;
        public double Degrees { get; set; }
        private double minutes;
        public double Minutes { get; set; }
        private double seconds;
        public double Seconds { get; set; }*/

        private static double degrees;
        private static double minutes;
        private static double seconds;

        public static double GetLatitude(ExifGpsLatitudeRef latRef, double[] data)
        {
            SplitGpsArray(data);

            var result = ConvertDegreeToAngle();

            if (latRef == ExifGpsLatitudeRef.South)
                result *= -1;

            return result;
        }

        public static double GetLongitude(double[] data)
        {
            SplitGpsArray(data);

            return ConvertDegreeToAngle();
        }

        private static void SplitGpsArray(double[] data)
        {
            if (!data.Any())
                return;

            int count = data.Count();

            degrees = data.First();
            minutes = count > 1 ? data[1] : 0;
            seconds = count > 2 ? data[2] : 0;
        }

        private static double ConvertDegreeToAngle()
        {
            return degrees + (minutes / 60) + (seconds / 3600);
        }
    }
}