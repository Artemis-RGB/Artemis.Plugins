using System;
using System.Globalization;
using System.Windows.Data;

namespace Artemis.Plugins.LayerBrushes.Ambilight.UI
{
    [ValueConversion(typeof(bool), typeof(int))]
    public class RenderScaleConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (value as bool?) == true ? -1 : 1;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => (value as int?) == -1;

        #endregion
    }
}
