using System;
using System.Text.RegularExpressions;

namespace WebLMS.Utils
{
    public class Email
    {
        public static bool IsValidEmail(string email)
        {
            const String pattern =
               @"^([0-9a-zA-Z]" + //Start with a digit or alphabetical
               @"([\+\-_\.][0-9a-zA-Z]+)*" + // No continuous or ending +-_. chars in email
               @")+" +
               @"@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,17})$";
            return Regex.IsMatch(email, pattern);
        }
    }
}