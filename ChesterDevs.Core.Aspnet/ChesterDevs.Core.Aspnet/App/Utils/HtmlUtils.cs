using System;
using System.Net;

namespace ChesterDevs.Core.Aspnet.App.Utils
{
    public class HtmlUtils
    {
        public static string ConvertToHtml(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return WebUtility.HtmlEncode(text)
                .Replace(Environment.NewLine, "<br/>");
        }
    }
}