using System;
using System.Globalization;
using System.Windows.Data;

namespace FireRescueCommand.Converters
{
    public class LongitudeToXConverter : IMultiValueConverter
    {
        private const double MinLon = 19.688292482742394;
        private const double MaxLon = 20.02470275868903;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is double longitude && values[1] is double canvasWidth)
            {
                var normalizedLon = (longitude - MinLon) / (MaxLon - MinLon);
                return normalizedLon * canvasWidth - 15; // -15 - wyśrodkowanie 
            }
            return 0.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LatitudeToYConverter : IMultiValueConverter
    {
        private const double MinLat = 49.95855025648944;
        private const double MaxLat = 50.154564013341734;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is double latitude && values[1] is double canvasHeight)
            {
                var normalizedLat = (latitude - MinLat) / (MaxLat - MinLat);
                return (1 - normalizedLat) * canvasHeight - 15; // wyśrodkować element
            }
            return 0.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
