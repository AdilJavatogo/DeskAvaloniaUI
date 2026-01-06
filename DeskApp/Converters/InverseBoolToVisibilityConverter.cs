using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace DeskApp.Converters
{
    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Hvis værdien er en bool, returner det modsatte
            if (value is bool booleanValue)
            {
                return !booleanValue;
            }

            // Default fallback (hvis input er null eller ikke en bool)
            // Du kan ændre dette til 'true', hvis du vil have elementet synligt som standard ved fejl.
            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Det samme gælder den anden vej
            if (value is bool booleanValue)
            {
                return !booleanValue;
            }
            return false;
        }
    }
}