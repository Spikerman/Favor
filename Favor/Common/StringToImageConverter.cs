using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Favor.Common
{
    public class StringToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                string text = value as string;
                if (string.IsNullOrEmpty(text) == false)
                {
                    Uri uri;
                    if (Uri.TryCreate((string)value, UriKind.Absolute, out uri))
                    {
                        return uri;
                    }
                }
            }

            return null;
        }

        //public object Convert(object value, Type targetType, object parameter, string language)
        //{
        //    throw new NotImplementedException();
        //}

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();

        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //throw new NotImplementedException();
            if (value != null)
            {
                string text = value as string;
                if (string.IsNullOrEmpty(text) == false)
                {
                    Uri uri;
                    if (Uri.TryCreate((string)value, UriKind.Absolute, out uri))
                    {
                        return uri;
                    }
                }
            }

            return null;
        }
    }
}
