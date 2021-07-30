using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Linq;




namespace RI.DesktopServices.Settings.Converters
{
    /// <summary>
    ///     Implements a default setting converter which can convert to and from the basic types used in .NET.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The types supported by this setting converter are:
    ///         <see cref="bool" />, <see cref="char" />, <see cref="string" />, <see cref="sbyte" />, <see cref="byte" />,
    ///         <see cref="short" />, <see cref="ushort" />, <see cref="int" />, <see cref="uint" />, <see cref="long" />,
    ///         <see cref="ulong" />, <see cref="float" />, <see cref="double" />, <see cref="decimal" />,
    ///         <see cref="DateTime" />, <see cref="DateTimeOffset" />, <see cref="TimeSpan" />, <see cref="Guid" />,
    ///         <see cref="Version" />, enumerations (<see cref="Enum" />), arrays of <see cref="byte" />,
    ///         <see cref="XDocument" />, <see cref="XmlDocument" />.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="fale" instance="false" />
    public sealed class DefaultSettingConverter : ISettingConverter
    {
        #region Instance Properties/Indexer

        private List<Type> SupportedTypes { get; } = new List<Type>()
        {
            typeof(bool),
            typeof(char),
            typeof(string),
            typeof(sbyte),
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(Guid),
            typeof(Version),
            typeof(Enum),
            typeof(byte[]),
            typeof(XDocument),
            typeof(XmlDocument),
        };

        #endregion




        #region Interface: ISettingConverter

        /// <inheritdoc />
        public SettingConversionMode ConversionMode => SettingConversionMode.StringConversion;

        /// <inheritdoc />
        public bool CanConvert (Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsEnum)
            {
                return true;
            }

            return this.SupportedTypes.Contains(type);
        }

        /// <inheritdoc />
        public string ConvertFrom (Type type, object value)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if ((type != value.GetType()) && (type.IsEnum && (!value.GetType()
                                                                    .IsEnum)))
            {
                throw new
                    NotSupportedException($"Cannot convert {value.GetType().Name} as {type.Name} to string by {this.GetType().Name}.");
            }

            if (type.IsEnum)
            {
                //TODO: string representation
                return Convert.ToInt32(value)
                              .ToString("D", CultureInfo.InvariantCulture);
            }

            if (type == typeof(bool))
            {
                return ((bool)value).ToString(CultureInfo.InvariantCulture);
            }

            if (type == typeof(char))
            {
                return ((char)value).ToString();
            }

            if (type == typeof(string))
            {
                return (string)value;
            }

            if (type == typeof(sbyte))
            {
                return ((sbyte)value).ToString("D", CultureInfo.InvariantCulture);
            }

            if (type == typeof(byte))
            {
                return ((byte)value).ToString("D", CultureInfo.InvariantCulture);
            }

            if (type == typeof(short))
            {
                return ((short)value).ToString("D", CultureInfo.InvariantCulture);
            }

            if (type == typeof(ushort))
            {
                return ((ushort)value).ToString("D", CultureInfo.InvariantCulture);
            }

            if (type == typeof(int))
            {
                return ((int)value).ToString("D", CultureInfo.InvariantCulture);
            }

            if (type == typeof(uint))
            {
                return ((uint)value).ToString("D", CultureInfo.InvariantCulture);
            }

            if (type == typeof(long))
            {
                return ((long)value).ToString("D", CultureInfo.InvariantCulture);
            }

            if (type == typeof(ulong))
            {
                return ((ulong)value).ToString("D", CultureInfo.InvariantCulture);
            }

            if (type == typeof(float))
            {
                return ((float)value).ToString("F", CultureInfo.InvariantCulture);
            }

            if (type == typeof(double))
            {
                return ((double)value).ToString("F", CultureInfo.InvariantCulture);
            }

            if (type == typeof(decimal))
            {
                return ((decimal)value).ToString("F", CultureInfo.InvariantCulture);
            }

            if (type == typeof(DateTime))
            {
                return ((DateTime)value).ToString("O", CultureInfo.InvariantCulture);
            }

            if (type == typeof(DateTimeOffset))
            {
                return ((DateTimeOffset)value).ToString("O", CultureInfo.InvariantCulture);
            }

            if (type == typeof(TimeSpan))
            {
                return ((TimeSpan)value).ToString("G", CultureInfo.InvariantCulture);
            }

            if (type == typeof(Guid))
            {
                return ((Guid)value).ToString("N", CultureInfo.InvariantCulture)
                                    .ToUpperInvariant();
            }

            if (type == typeof(Version))
            {
                return ((Version)value).ToString();
            }

            if (type == typeof(byte[]))
            {
                return Convert.ToBase64String((byte[])value, Base64FormattingOptions.None);
            }

            if (type == typeof(XDocument))
            {
                using XmlReader xr = ((XDocument)value).CreateReader();
                XmlDocument doc = new XmlDocument();
                doc.Load(xr);
                using StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);
                doc.Save(sw);
                sw.Flush();
                return sw.ToString();
            }

            if (type == typeof(XmlDocument))
            {
                XmlDocument doc = (XmlDocument)value;
                using StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);
                using XmlWriter xw = new XmlTextWriter(sw);
                doc.Save(xw);
                xw.Flush();
                sw.Flush();
                return sw.ToString();
            }

            throw new
                NotSupportedException($"Cannot convert {value.GetType().Name} as {type.Name} to string by {this.GetType().Name}.");
        }

        /// <inheritdoc />
        public object ConvertTo (Type type, string value)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            object finalValue = null;

            if (type.IsEnum)
            {
                try
                {
                    finalValue = Enum.Parse(type, value, true);
                }
                catch (Exception ex) when (ex is ArgumentException or OverflowException)
                {
                    Trace.TraceWarning($"Invalid enum value for {type.Name} ({ex.Message})");
                    finalValue = null;
                }
            }
            else if (type == typeof(bool))
            {
                bool found = false;

                if (!bool.TryParse(value, out bool boolValue))
                {
                    boolValue = false;

                    switch (value.ToLowerInvariant())
                    {
                        case "true":
                        case "yes":
                        case "on":
                        case "enabled":
                            boolValue = true;
                            found = true;
                            break;

                        case "false":
                        case "no":
                        case "off":
                        case "disabled":
                            found = true;
                            break;
                    }
                }
                else
                {
                    found = true;
                }

                finalValue = found ? boolValue : null;
            }
            else if (type == typeof(char))
            {
                finalValue = value.Length == 1 ? value[0] : null;
            }
            else if (type == typeof(string))
            {
                finalValue = value;
            }
            else if (type == typeof(sbyte))
            {
                if (sbyte.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out sbyte result))
                {
                    finalValue = result;
                }
            }
            else if (type == typeof(byte))
            {
                if (byte.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out byte result))
                {
                    finalValue = result;
                }
            }
            else if (type == typeof(short))
            {
                if (short.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out short result))
                {
                    finalValue = result;
                }
            }
            else if (type == typeof(ushort))
            {
                if (ushort.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out ushort result))
                {
                    finalValue = result;
                }
            }
            else if (type == typeof(int))
            {
                if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
                {
                    finalValue = result;
                }
            }
            else if (type == typeof(uint))
            {
                if (uint.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out uint result))
                {
                    finalValue = result;
                }
            }
            else if (type == typeof(long))
            {
                if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out long result))
                {
                    finalValue = result;
                }
            }
            else if (type == typeof(ulong))
            {
                if (ulong.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out ulong result))
                {
                    finalValue = result;
                }
            }
            else if (type == typeof(float))
            {
                if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
                {
                    finalValue = result;
                }
            }
            else if (type == typeof(double))
            {
                if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
                {
                    finalValue = result;
                }
            }
            else if (type == typeof(decimal))
            {
                if (decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal result))
                {
                    finalValue = result;
                }
            }
            else if (type == typeof(DateTime))
            {
                if (DateTime.TryParseExact(value, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind,
                                           out DateTime result))
                {
                    finalValue = result;
                }
            }
            else if (type == typeof(DateTimeOffset))
            {
                if (DateTimeOffset.TryParseExact(value, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind,
                                                 out DateTimeOffset result))
                {
                    finalValue = result;
                }
            }
            else if (type == typeof(TimeSpan))
            {
                if (TimeSpan.TryParseExact(value, "G", CultureInfo.InvariantCulture,
                                           out TimeSpan result))
                {
                    finalValue = result;
                }
            }
            else if (type == typeof(Guid))
            {
                if (Guid.TryParse(value, out Guid result))
                {
                    finalValue = result;
                }
            }
            else if (type == typeof(Version))
            {
                if (Version.TryParse(value, out Version result))
                {
                    finalValue = result;
                }
            }
            else if (type == typeof(byte[]))
            {
                try
                {
                    finalValue = Convert.FromBase64String(value);
                }
                catch (FormatException ex)
                {
                    Trace.TraceWarning($"Invalid Base64 value for {type.Name} ({ex.Message})");
                    finalValue = null;
                }
            }
            else if (type == typeof(XDocument))
            {
                try
                {
                    finalValue = XDocument.Parse(value, LoadOptions.None);
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning($"Invalid XML value for {type.Name} ({ex.Message})");
                    finalValue = null;
                }
            }
            else if (type == typeof(XmlDocument))
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(value);
                    finalValue = doc;
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning($"Invalid XML value for {type.Name} ({ex.Message})");
                    finalValue = null;
                }
            }
            else
            {
                throw new NotSupportedException($"Cannot convert from string to {value.GetType().Name}.");
            }

            if (finalValue == null)
            {
                throw new FormatException($"Cannot convert value from string to {value.GetType().Name}.");
            }

            return finalValue;
        }

        #endregion
    }
}
