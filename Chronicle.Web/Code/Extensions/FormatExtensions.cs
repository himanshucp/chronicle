namespace Chronicle.Web.Code.Extensions
{
    public static class FormatExtensions
    {
        // Format date according to Locale

        public static string FormatDate(this DateTime? dt)
        {
            if (dt == null) return "";
            return dt.Value.FormatDate();
        }

        public static string FormatDate(this DateTime dt)
        {
            return dt.ToString("d");
        }

        public static DateTime? FormatToDate(this string s)
        {
            if (string.IsNullOrEmpty(s)) return null;

            if (DateTime.TryParse(s, out DateTime dt))
            {
                return dt;
            }
            return null;
        }

        // Format currency according to Locale / Currency settings

        public static string FormatCurrency(this decimal? amount)
        {
            if (amount == null) return "";
            return amount.Value.FormatCurrency();
        }

        public static string FormatCurrency(this decimal amount)
        {
            var value = amount.ToString("c"); // ;, CurrentUser.CultureInfo);

            //// We have separated currency from culture -- potentially replace currency symbol.
            //if (CurrentUser.CultureInfo.NumberFormat.CurrencySymbol != CurrentUser.Currency.Symbol.Trim())
            //    value.Replace(CurrentUser.CultureInfo.NumberFormat.CurrencySymbol, CurrentUser.Currency.Symbol.Trim());

            return value;
        }
    }
}
