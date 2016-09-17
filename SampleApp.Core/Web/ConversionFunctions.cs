using System;
using System.Globalization;
using System.Threading;
using Microsoft.VisualBasic;
using System.Data.SqlTypes;

namespace SampleApp.Core.Web
{
    // following class was VB module
    sealed public class ConversionFunctions
    {
        private static CultureInfo DatabaseCulture = new CultureInfo("en-US");
        public static readonly DateTime DatabaseMinimumDate = new DateTime(1753, 1, 1);

        #region Generic Conversion

        public static Type GetTypeByDataTypeCode(string DataTypeCode)
        {
            switch (DataTypeCode)
            {
                case "BIT":
                    return typeof(System.Boolean);
                case "INT":
                    return typeof(System.Int32);
                case "DECIMAL":
                case "CURRENCY":
                    return typeof(System.Decimal);
                case "STRING":
                    return typeof(System.String);
                case "DATETIME":
                    return typeof(System.DateTime);
            }
            return null;
        }

        public static bool ConvertToBoolean(object value)
        {
            return Convert.ToBoolean(value);
        }

        public static decimal ConvertToDecimal(object value)
        {
            return Convert.ToDecimal(value);
        }

        public static string ConvertToString(object value)
        {
            return Convert.ToString(value);
        }

        public static object ConvertTo(object value, Type type)
        {
            return ConvertTo(value, Type.GetTypeCode(type), null);
        }

        public static object ConvertTo(object value, TypeCode typeCode, object defaultValue)
        {
            object o = null;

            try
            {
                o = System.Convert.ChangeType(value, typeCode);
            }
            catch
            {
                if (defaultValue == null)
                {
                    switch (typeCode)
                    {
                        case TypeCode.Boolean:
                            if (!value.Equals("") && !value.Equals("0"))
                                o = true;
                            else
                                o = false;
                            break;
                        case TypeCode.Byte:
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                            o = byte.MinValue;
                            break;
                        case TypeCode.Decimal:
                            o = 0M;
                            break;
                        case TypeCode.String:
                            o = string.Empty;
                            break;
                        case TypeCode.Empty:
                        case TypeCode.Object:
                        case TypeCode.DBNull:
                        case TypeCode.Char:
                        case TypeCode.SByte:
                        case TypeCode.DateTime:
                            o = null;
                            break;
                    }

                }
                else
                {
                    o = defaultValue;
                }
            }

            if (Convert.IsDBNull(defaultValue) && (!(Convert.IsDBNull(o))))
            {
                switch (typeCode)
                {
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                        if ((byte)o == byte.MinValue)
                        {
                            //  if the default value is DBNull (should mean we are populating a datatable), and
                            //  converting to a numeric data type, and the conversion result is 0, return DBNull
                            //  (populating a datatable with DBNull instead of 0 avoids formula divide by zero hard errors)
                            o = DBNull.Value;
                        }
                        break;
                    case TypeCode.Decimal:
                        if ((decimal)o == 0M)
                        {
                            //  if the default value is DBNull (should mean we are populating a datatable), and
                            //  converting to the decimal data type, and the conversion result is 0, return DBNull
                            //  (populating a datatable with DBNull instead of 0 avoids formula divide by zero hard errors)
                            o = DBNull.Value;
                        }
                        break;
                }

            }

            return o;
        }

        #endregion

        #region all retailer cultures in cache


        #endregion

        #region Currency Conversion Methods
        private static CultureInfo CurrentRetailerCurrencyCulture
        {
            get
            {
                return new CultureInfo(RetailerCultureString);
            }
        }

        static string cultureString = "en-US";
        public static string RetailerCultureString
        {
            get { return cultureString; }
            set { cultureString = value; }
        }

        public static int CurrencyDigits
        {
            get
            {
                return CurrentRetailerCurrencyCulture.NumberFormat.CurrencyDecimalDigits;
            }
        }

        public static string FormatAllowanceAmount(decimal dValue, string AllowanceType)
        {
            switch (AllowanceType)
            {
                case "Percent":
                    dValue = dValue / 100;
                    NumberFormatInfo nfi = CurrentRetailerCurrencyCulture.NumberFormat;
                    nfi.PercentDecimalDigits = 4;
                    return (dValue).ToString("p", nfi);
                case "Currency":
                    return dValue.ToString("c");
                case "HighPrecisionCurrency":
                    return ToCurrencyString(dValue, ConversionFunctions.CurrencyDigits, ConversionFunctions.CurrencyDigits + 2);
                default:
                    return ToCurrencyString(dValue, ConversionFunctions.CurrencyDigits, ConversionFunctions.CurrencyDigits + 2);
            }
        }

        public static string ConvertToCurrencyString(object value)
        {
            int iDigits;
            string s = value.ToString();
            if (s.Substring(s.Length - 1, 1) != "0")
            {
                iDigits = ConversionFunctions.CurrencyDigits + 2;
            }
            else
            {
                iDigits = ConversionFunctions.CurrencyDigits;
            }
            return Convert.ToDecimal(value).ToString("f" + iDigits);
        }

        public static string ToCurrencyString(string retailerCulture,string sValue, int Scale)
        {
            return ToCurrencyString(GetDecimalFromString(retailerCulture,sValue), Scale);
        }
        public static string ToCurrencyString(decimal dValue, CultureInfo culture)
        {
            int iMinimumDecimal = culture.NumberFormat.CurrencyDecimalDigits;
            int iMaximumDecimal = culture.NumberFormat.CurrencyDecimalDigits + 3;
            string sValue = dValue.ToString("N" + iMaximumDecimal);
            string[] sParts = sValue.Split(culture.NumberFormat.CurrencyDecimalSeparator[0]);
            string sDecimal = "0";

            if (sParts.Length > 1)
                sDecimal = sParts[1].TrimEnd('0');

            int iDecimalLength = sDecimal.Length;

            if (sDecimal.Length < iMinimumDecimal)
                iDecimalLength = iMinimumDecimal;
            else if (sDecimal.Length > iMaximumDecimal)
                iDecimalLength = iMaximumDecimal;

            return dValue.ToString("c" + iDecimalLength, culture);
        }

        public static string ToCurrencyString(decimal dValue)
        {
            return ToCurrencyString(dValue, ConversionFunctions.CurrencyDigits, ConversionFunctions.CurrencyDigits + 3);
        }

        public static string ToCurrencyString(decimal dValue, int Scale)
        {
            return ToCurrencyString(dValue, Scale, Scale);
        }

        public static string ToCurrencyString(string retailerCulture, decimal dValue)
        {
            CultureInfo pRetailerCurrencyCulture = new CultureInfo(retailerCulture);

            int minimumDecimal = pRetailerCurrencyCulture.NumberFormat.CurrencyDecimalDigits;
            int maximumDecimal = pRetailerCurrencyCulture.NumberFormat.CurrencyDecimalDigits + 3;
            string sValue = dValue.ToString("N" + maximumDecimal);
            string[] sParts = sValue.Split(pRetailerCurrencyCulture.NumberFormat.CurrencyDecimalSeparator[0]);
            string sDecimal = "0";

            if (sParts.Length > 1)
                sDecimal = sParts[1].TrimEnd('0');

            int iDecimalLength = sDecimal.Length;

            if (sDecimal.Length < minimumDecimal)
                iDecimalLength = minimumDecimal;
            else if (sDecimal.Length > maximumDecimal)
                iDecimalLength = maximumDecimal;

            return dValue.ToString("F" + iDecimalLength, pRetailerCurrencyCulture);

        }

        public static string ToCurrencyString(string retailerCulture, decimal dValue,Boolean displayCurrencySymbol)
        {
            if(displayCurrencySymbol)
                return ToCurrencyString(new CultureInfo(retailerCulture),dValue);
            else
                return ToCurrencyString(retailerCulture, dValue);
        }

        public static string ToCurrencyString(CultureInfo pRetailerCurrencyCulture, decimal dValue)
        {
            int minimumDecimal = pRetailerCurrencyCulture.NumberFormat.CurrencyDecimalDigits;
            int maximumDecimal = pRetailerCurrencyCulture.NumberFormat.CurrencyDecimalDigits + 3;
            string sValue = dValue.ToString("N" + maximumDecimal);
            string[] sParts = sValue.Split(pRetailerCurrencyCulture.NumberFormat.CurrencyDecimalSeparator[0]);
            string sDecimal = "0";

            if (sParts.Length > 1)
                sDecimal = sParts[1].TrimEnd('0');

            int iDecimalLength = sDecimal.Length;

            if (sDecimal.Length < minimumDecimal)
                iDecimalLength = minimumDecimal;
            else if (sDecimal.Length > maximumDecimal)
                iDecimalLength = maximumDecimal;

            return dValue.ToString("c" + iDecimalLength, pRetailerCurrencyCulture);
            
        }

        public static string ToCurrencyString(decimal dValue, int iMinimumDecimal, int iMaximumDecimal)
        {
            string sValue = dValue.ToString("N" + iMaximumDecimal);
            string[] sParts = sValue.Split(CurrentRetailerCurrencyCulture.NumberFormat.CurrencyDecimalSeparator[0]);
            string sDecimal = "0";

            if (sParts.Length > 1)
                sDecimal = sParts[1].TrimEnd('0');

            int iDecimalLength = sDecimal.Length;

            if (sDecimal.Length < iMinimumDecimal)
                iDecimalLength = iMinimumDecimal;
            else if (sDecimal.Length > iMaximumDecimal)
                iDecimalLength = iMaximumDecimal;

            return dValue.ToString("c" + iDecimalLength, CurrentRetailerCurrencyCulture);
            //return dValue.ToString("c" + iDecimalLength, Thread.CurrentThread.CurrentUICulture).TrimStart('(').TrimEnd(')');
        }

        #endregion

        #region Number Conversion Methods

        public static string ToNumberString(int oValue)
        {
            string sValue = oValue.ToString("N", CurrentRetailerCurrencyCulture);
            if (sValue.Contains(CurrentRetailerCurrencyCulture.NumberFormat.NumberDecimalSeparator))
                sValue = sValue.Substring(0, sValue.IndexOf(CurrentRetailerCurrencyCulture.NumberFormat.NumberDecimalSeparator));
            return sValue;
        }

        public static string ToNumberString(int NumberValue, string Culture)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture(Culture);
            string sValue = NumberValue.ToString("N", culture);
            if (sValue.Contains(culture.NumberFormat.NumberDecimalSeparator))
                sValue = sValue.Substring(0, sValue.IndexOf(culture.NumberFormat.NumberDecimalSeparator));
            return sValue;
        }

        public static string ToDecimalString(decimal DecimalValue, string Culture)
        {
            string sValue = DecimalValue.ToString(CultureInfo.CreateSpecificCulture(Culture));
            //if (sValue.Length > 3)
            //    sValue = sValue.Substring(0, sValue.Length - 3);
            return sValue;
        }

        public static string ToDecimalString(decimal oValue)
        {
            string sValue = oValue.ToString(CurrentRetailerCurrencyCulture);
            //if (sValue.Length > 3)
            //sValue = sValue.Substring(0, sValue.Length - 3);
            return sValue;
        }

        public static string ToDatabaseNumber(string retailerCulture, object oValue)
        {
            cultureString = retailerCulture;
            string sValue = Convert.ToDecimal(oValue, CurrentRetailerCurrencyCulture).ToString();
            return sValue.Replace(CurrentRetailerCurrencyCulture.NumberFormat.NumberDecimalSeparator, ".");
        }

        public static string ToPercentString(string retailerCulture, decimal dValue, int iPrecision)
        {
            cultureString = retailerCulture;
            return dValue.ToString("P" + iPrecision.ToString(), CurrentRetailerCurrencyCulture);
        }

        public static string ToPercentString(string retailerCulture, decimal dValue)
        {
            cultureString = retailerCulture;
            return dValue.ToString("P", CurrentRetailerCurrencyCulture);
        }

        public static decimal GetDecimalFromString(string retailerCulture, string sValue)
        {
            cultureString = retailerCulture;
            sValue = StripCurrencyAndPercentSymbols(sValue);
            sValue = StripGroupSeparators(sValue);
            sValue = sValue.Trim();
            try
            {
                return Decimal.Parse(sValue, NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign | NumberStyles.AllowParentheses | NumberStyles.AllowThousands);
                //return Convert.ToDecimal(sValue);
            }
            catch
            {
                return 0;
            }
        }

        public static string StripCurrencyAndPercentSymbols(string sValue)
        {
            sValue = sValue.Replace(CurrentRetailerCurrencyCulture.NumberFormat.CurrencySymbol, "");
            return sValue.Replace(CurrentRetailerCurrencyCulture.NumberFormat.PercentSymbol, "");
        }

        public static string StripGroupSeparators(string sValue)
        {
            sValue = sValue.Replace(CurrentRetailerCurrencyCulture.NumberFormat.CurrencyGroupSeparator, "");
            sValue = sValue.Replace(CurrentRetailerCurrencyCulture.NumberFormat.NumberGroupSeparator, "");
            return sValue.Replace(CurrentRetailerCurrencyCulture.NumberFormat.PercentGroupSeparator, "");
        }

        #endregion

        #region Date Conversion Methods

        public enum TimePartType
        {
            None,
            MinimumTime,
            MaximumTime
        }

        public static DateTime ToDateTimeFromDatabaseCulture(object oValue)
        {
            return Convert.ToDateTime(oValue, CurrentRetailerCurrencyCulture);
        }
        public static DateTime FromDatabaseToCurrentCulture(object oValue, string culture)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            // DB format mm/dd/yyyy
            string[] sSourceDate = oValue.ToString().Split("/".ToCharArray());
            //string[] sDateFormat = Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern.Split(Thread.CurrentThread.CurrentUICulture.DateTimeFormat.DateSeparator.ToCharArray());
            //int month = 0, day = 0, year = 0;
            //int index = 0;
            //foreach (string position in sDateFormat)
            //{  // get position for current format
            //    if (position.ToUpper().Contains("M"))
            //        month = index;
            //    if (position.ToUpper().Contains("Y"))
            //        year = index;
            //    if (position.ToUpper().Contains("D"))
            //        day = index;
            //    index++;
            //}
            return new DateTime(Convert.ToInt16(sSourceDate[2]), Convert.ToInt16(sSourceDate[0]), Convert.ToInt16(sSourceDate[1]));
        }

        public static SqlDateTime ToDatabaseDateTime(string oValue)
        {
            return SqlDateTime.Parse(oValue);
        }

        public static bool IsAllowableDataRange(string oValue)
        {
            return ((DateTime.Parse(oValue) >= DateTime.Parse("01/01/1996") && DateTime.Parse(oValue) <= DateTime.Parse("12/31/2099")) ? true : false);
        }

        public static string ToDatabaseDateTimeShortString(string sDate)
        {
            try
            {
                //DateTime d;
                //if (DateTime.TryParse(sDate,out d))
                //{
                //    return ToDatabaseDateTimeShortString(sDate);
                //}
                //return sDate;
                //////DateTime date = Convert.ToDateTime(sDate.Trim());
                //////date = date < DatabaseMinimumDate ? DatabaseMinimumDate : date;
                //////return ToDatabaseDateTimeShortString(date);
                return ToDatabaseDateTimeShortString(Convert.ToDateTime(sDate.Trim()));
            }
            catch
            {
                return sDate;
            }
        }
        public static string ToDatabaseDateTimeUTCShortString(string sDate)
        {
            try
            {

                return ToDatabaseDateTimeUtcString(Convert.ToDateTime(sDate.Trim()));
            }
            catch
            {
                return sDate;
            }
        }
        public static string ToDatabaseDateTimeShortString(DateTime oDate)
        {
            return oDate.ToString("MM/dd/yyyy");
        }

        public static string ToDatabaseDateTimeUtcString(DateTime oDate)
        {
            return oDate.ToString("yyyy-MM-dd");
        }


        public static string ToDatabaseDateTimeLongString(string sDate)
        {
            try
            {
                return ToDatabaseDateTimeLongString(sDate.Trim());
            }
            catch
            {
                return sDate;
            }
        }

        public static string ToDatabaseDateTimeLongString(DateTime oDate)
        {
            return oDate.ToString("MM/dd/yyyy hh:mm:ss");
        }

        public static string ToDatabaseDateTimeString(DateTime oDate, TimePartType oType)
        {
            return ToDatabaseDateTimeString(ToDatabaseDateTimeShortString(oDate), oType, false);
        }

        public static string ToDatabaseDateTimeString(DateTime oDate, TimePartType oType, bool bAddQuote)
        {
            return ToDatabaseDateTimeString(ToDatabaseDateTimeShortString(oDate), oType, bAddQuote);
        }

        public static string ToDatabaseDateTimeString(string sDate, TimePartType oType)
        {
            return ToDatabaseDateTimeString(sDate, oType, true);
        }

        public static string ToDatabaseDateTimeString(string sDate, TimePartType oType, bool bAddQuote)
        {
            string sValue = ToDatabaseDateTimeShortString(sDate);
            switch (oType)
            {
                case TimePartType.MinimumTime:
                    sValue += " 00:00:00";
                    break;
                case TimePartType.MaximumTime:
                    sValue += " 23:59:59";
                    break;
            }

            if (bAddQuote)
                return "'" + sValue + "'";
            return sValue;
        }

        public static string ToDateTimeShortString(DateTime dateValue)
        {
            return dateValue.ToString("d", Thread.CurrentThread.CurrentUICulture);
        }

        public static string ToDateTimeLongString(DateTime dateValue)
        {
            return dateValue.ToString("D", Thread.CurrentThread.CurrentUICulture);
        }

        public static string ConvertToDateString(object value)
        {
            string sDate = string.Empty;
            //if (Information.IsDate(value))
            DateTime d;
            if (DateTime.TryParse(value.ToString(), out d))
            {
                //DateTime oDate = DateTime.Parse(value.ToString());
                DateTime oDate = d;
                if (oDate > new DateTime(1900, 1, 1, 0, 0, 0))
                {
                    sDate = oDate.ToString("d");
                }
            }
            return sDate;
        }

        public static string DefaultDateTime()
        {
            return new DateTime(1753, 1, 1, 0, 0, 0, 0, Thread.CurrentThread.CurrentUICulture.Calendar).ToString();
            //return GenerateDateTimeSecondToString(Convert.ToDateTime("1/1/1753 12:00:00 AM"));
            //return GenerateDateTimeSecondToString(new DateTime(1753,1,1,0,0,0,0, Thread.CurrentThread.CurrentUICulture.Calendar));
        }

        public static string LocalTimeZoneFormattedDateTime(DateTime serverDateTime, string TimeZoneFullDesc, string TimeZoneDesc, string Wrap, string culture)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

            DateTime UserDateTime = TimeZoneInfo.ConvertTime(serverDateTime.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById(TimeZoneFullDesc));
            return UserDateTime.Date.ToShortDateString() + (Wrap.Equals("") ? " " : Wrap) + UserDateTime.ToLongTimeString() + " " + TimeZoneDesc;

            //return UserDateTime.ToString("d", Thread.CurrentThread.CurrentUICulture) + " " + TimeZoneDesc;
        }

        public static string LocalTimeZoneFormattedDate(DateTime serverDateTime, string TimeZoneFullDesc, string TimeZoneDesc, string Wrap, string culture)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
            DateTime UserDateTime = TimeZoneInfo.ConvertTime(serverDateTime.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById(TimeZoneFullDesc));
            return UserDateTime.Date.ToShortDateString() + TimeZoneDesc;
            //return UserDateTime.ToString("d", Thread.CurrentThread.CurrentUICulture) + " " + TimeZoneDesc;
        }

        public static string TimeZoneFormattedDateTime(DateTime serverDateTime, string TimeZoneFullDesc, string TimeZoneDesc, string Wrap, string culture)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
            //DateTime UserDateTime = TimeZoneInfo.ConvertTime(serverDateTime.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById(TimeZoneFullDesc));
            return serverDateTime.Date.ToShortDateString() + (Wrap.Equals("") ? " " : Wrap) + serverDateTime.ToLongTimeString() + " " + TimeZoneDesc;
            //return serverDateTime.ToString("d", Thread.CurrentThread.CurrentUICulture) + " " + TimeZoneDesc;

        }

        public static string TimeZoneFormattedDate(DateTime serverDateTime, string TimeZoneFullDesc, string TimeZoneDesc, string Wrap, string culture)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
            //DateTime UserDateTime = TimeZoneInfo.ConvertTime(serverDateTime.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById(TimeZoneFullDesc));
            return serverDateTime.Date.ToShortDateString() + TimeZoneDesc;
            //return serverDateTime.ToString("d", Thread.CurrentThread.CurrentUICulture) + " " + TimeZoneDesc;
        }

        #endregion
    }
}
