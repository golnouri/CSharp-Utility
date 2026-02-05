using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
/// <summary>
/// Utility Class - A collection of helper methods for strings, dates, encryption, and tokens.
///
/// Usage:
///
/// 1. NormalizeString
///    string input = "  Hello  World  ";
///    string normalized = Utility.NormalizeString(input);
///
/// 2. Slugify
///    string title = "Hello World! This is a test.";
///    string slug = Utility.Slugify(title); // Output: "hello-world-this-is-a-test"
///
/// 3. GenerateToken
///    string token = Utility.GenerateToken(16); // Random 16-character token
///
/// 4. StandardizeDateTime
///    DateTime now = DateTime.Now;
///    string dateOnly = Utility.StandardizeDateTime(now, Utility.DateTimeFormat.DateOnly);
///    string dateTime = Utility.StandardizeDateTime(now, Utility.DateTimeFormat.DateTime);
///
/// 5. PersianToGregorian
///    string persianDate = "1404/06/02";
///    DateTime gregorian = Utility.PersianToGregorian(persianDate);
///
/// 6. ToPersianDate
///    DateTime now = DateTime.Now;
///    string simple = Utility.ToPersianDate(now, Utility.FormatFlag.SimpleDate);
///    string longDate = Utility.ToPersianDate(now, Utility.FormatFlag.LongDate);
///    string dateTime = Utility.ToPersianDate(now, Utility.FormatFlag.DateTime);
///
/// 7. Encrypt / Decrypt
///    string text = "Hello World";
///    string encrypted = Utility.Encrypt(text);
///    string decrypted = Utility.Decrypt(encrypted);
///
/// 8. HashPassword
///    string password = "myPassword123";
///    string hash = Utility.HashPassword(password);
///
/// All methods are static and can be called directly from the Utility class.
/// Developed by: Mojtaba Golnouri
/// </summary>
namespace ConsoleApp1
{
    internal class Utility
    {
        /* Remove extra spaces */
        public static string NormalizeString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }            
            string normalized = Regex.Replace(input.Trim(), @"\s+", " ");
            normalized = normalized.Replace('ي', 'ی').Replace('ك', 'ک');
            return normalized;
        }
        /* Convert string to: (URL-friendly) */
        public static string Slugify(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty; 
            }
            string normalized = NormalizeString(input).ToLowerInvariant();
            normalized = Regex.Replace(normalized, @"[^a-z0-9\s]", "");
            normalized = Regex.Replace(normalized, @"\s+", "-");
            return normalized;
        }
        /* Generate Random TOKEN */
        public static string GenerateToken(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            using (var rng = RandomNumberGenerator.Create())
            {
                var token = new StringBuilder();
                byte[] buffer = new byte[1];
                while (token.Length < length)
                {
                    rng.GetBytes(buffer);
                    var rnd = buffer[0] % chars.Length;
                    token.Append(chars[rnd]);
                }
                return token.ToString();
            }
        }
        /* Standard DateTime */
        public enum DateTimeFormat
        {
            DateOnly,  //2026/02/05
            DateTime   //2026/02/05 14:30
        }
        public static string StandardizeDateTime(DateTime date, DateTimeFormat format)
        {
            switch (format)
            {
                case DateTimeFormat.DateOnly:
                    return date.ToString("yyyy/MM/dd");
                case DateTimeFormat.DateTime:
                    return date.ToString("yyyy/MM/dd HH:mm");
                default:
                    return date.ToString("yyyy/MM/dd");
            }
        }
        /* Convert DateTime to Persian Date */
        private static readonly PersianCalendar persianCalendar = new PersianCalendar();
        /* Convert Persian Date to Standard DateTime */
        /// <param name="persianDate">تاریخ شمسی به فرمت "yyyy/MM/dd"</param>
        public static DateTime PersianToGregorian(string persianDate)
        {
            if (string.IsNullOrWhiteSpace(persianDate))
            {
                throw new ArgumentException("Date string cannot be empty.");
            }             
            var parts = persianDate.Split('/');
            if (parts.Length != 3)
            {
                throw new FormatException("Invalid Persian date format. Use yyyy/MM/dd");
            }               
            int year = int.Parse(parts[0]);
            int month = int.Parse(parts[1]);
            int day = int.Parse(parts[2]);
            DateTime gregorianDate = persianCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
            return gregorianDate;
        }
        public enum FormatFlag
        {
            SimpleDate, // 1404/06/05
            LongDate,   // 22 Bahman(*) 1404
            DateTime    // 1404/06/05 06:30
        }
        public static string ToPersianDate(DateTime date, FormatFlag flag)
        {
            int year = persianCalendar.GetYear(date);
            int month = persianCalendar.GetMonth(date);
            int day = persianCalendar.GetDayOfMonth(date);
            int hour = persianCalendar.GetHour(date);
            int minute = persianCalendar.GetMinute(date);
            string[] persianMonths = { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" };
            switch (flag)
            {
                case FormatFlag.SimpleDate:
                    return $"{year:0000}/{month:00}/{day:00}";
                case FormatFlag.LongDate:
                    string monthName = persianMonths[month - 1];
                    return $"{day} {monthName} {year}";
                case FormatFlag.DateTime:
                    return $"{year:0000}/{month:00}/{day:00} {hour:00}:{minute:00}";
                default:
                    return $"{year:0000}/{month:00}/{day:00}";
            }
        }
        /* Encrypt-Decrypt */
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("12345678901234567890123456789012");
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("1234567890123456");
        public static string Encrypt(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (StreamWriter sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                    sw.Close();
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
        public static string Decrypt(string cipherText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (StreamReader sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        /* Hash Password */
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hash)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
