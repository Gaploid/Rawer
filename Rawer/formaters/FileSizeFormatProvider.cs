using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rawer.Resources;

namespace Rawer
{
    public class FileSizeFormatProvider : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter)) return this;
            return null;
        }

        private const string fileSizeFormat = "fs";
        private const Decimal OneKiloByte = 1024M;
        private const Decimal OneMegaByte = OneKiloByte * 1024M;
        private const Decimal OneGigaByte = OneMegaByte * 1024M;

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (format == null || !format.StartsWith(fileSizeFormat))
            {
                return defaultFormat(format, arg, formatProvider);
            }

            if (arg is string)
            {
                return defaultFormat(format, arg, formatProvider);
            }

            Decimal size;

            try
            {
                size = Convert.ToDecimal(arg, CultureInfo.InvariantCulture);
            }
            catch (InvalidCastException)
            {
                return defaultFormat(format, arg, formatProvider);
            }

            string suffix;
            if (size > OneGigaByte)
            {
               

                size /= OneGigaByte;
                suffix = AppResources.CommonSizeGB;
            }
            else if (size > OneMegaByte)
            {
                size /= OneMegaByte;
                suffix = AppResources.CommonSizeMB;
            }
            else if (size > OneKiloByte)
            {
                size /= OneKiloByte;
                suffix = AppResources.CommonSizeKB;
            }
            else
            {
                suffix = AppResources.CommonSizeB;
            }

            string precision = format.Substring(2);
            if (String.IsNullOrEmpty(precision)) precision = "2";
            return String.Format("{0:N" + precision + "} {1}", size, suffix);

        }

        private static string defaultFormat(string format, object arg, IFormatProvider formatProvider)
        {
            IFormattable formattableArg = arg as IFormattable;
            if (formattableArg != null)
            {
                return formattableArg.ToString(format, formatProvider);
            }
            return arg.ToString();
        }

    }
}
