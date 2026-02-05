# CSharp-Utility

A collection of reusable helper methods for C# projects. This library includes methods for:

- String manipulation
- Date and time conversion (Persian and Gregorian)
- Encryption and decryption (AES)
- Password hashing (SHA-256)
- Random token generation

---

## Features

### String Utilities
- `NormalizeString(string input)`  
  Remove extra spaces and normalize characters.

- `Slugify(string input)`  
  Convert a string to URL-friendly format.

- `GenerateToken(int length)`  
  Generate a secure random token.

### Date Utilities
- `StandardizeDateTime(DateTime date, DateTimeFormat format)`  
  Convert a DateTime to a standard string (date only or date + time).

- `ToPersianDate(DateTime date, FormatFlag flag)`  
  Convert Gregorian DateTime to Persian date in multiple formats.

- `PersianToGregorian(string persianDate)`  
  Convert Persian date string to a standard Gregorian DateTime.

### Security Utilities
- `Encrypt(string plainText)` / `Decrypt(string cipherText)`  
  AES encryption and decryption using a fixed key and IV.

- `HashPassword(string password)`  
  Hash a password using SHA-256.

---

## Installation

Copy the `Utility.cs` file into your C# project or add it as a library reference.

---

## Usage Examples

```csharp
// String Utilities
string input = "  Hello World!  ";
string normalized = Utility.NormalizeString(input);
string slug = Utility.Slugify(input);
string token = Utility.GenerateToken(16);

// Date Utilities
DateTime now = DateTime.Now;
string dateOnly = Utility.StandardizeDateTime(now, Utility.DateTimeFormat.DateOnly);
string dateTime = Utility.StandardizeDateTime(now, Utility.DateTimeFormat.DateTime);

string persian = Utility.ToPersianDate(now, Utility.FormatFlag.LongDate);
DateTime gregorian = Utility.PersianToGregorian("1404/06/02");

// Security Utilities
string encrypted = Utility.Encrypt("Hello World");
string decrypted = Utility.Decrypt(encrypted);
string hash = Utility.HashPassword("myPassword123");

© Mojtaba Golnouri  
GitHub: https://github.com/golnouri
