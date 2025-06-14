using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using FluentAssertions;
using Medicine_DP;
using Medicine_DP.Config;
using Medicine_DP.Elements;
using Medicine_DP.Models;
using Medicine_DP.Pages;
using Medicine_DP.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Xunit;
using static Medicine_DP.Pages.Login;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;





namespace UnitTest
{
    public class PasswordHashTests
    {
        // Тестируемый класс с методом HashPassword
        private class PasswordService
        {
            public string HashPassword(string password)
            {
                using (var sha256 = System.Security.Cryptography.SHA256.Create())
                {
                    var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                    return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                }
            }
        }

        [Fact]
        public void HashPassword_Returns_64_Characters()
        {
            // Arrange
            var service = new PasswordService();

            // Act
            string hash = service.HashPassword("testpassword");

            // Assert
            Assert.AreEqual(64, hash.Length);
        }

        [Fact]
        public void HashPassword_Returns_Hexadecimal_String()
        {
            // Arrange
            var service = new PasswordService();
            var validChars = "0123456789abcdef";

            // Act
            string hash = service.HashPassword("anotherpassword");

            // Assert
            foreach (char c in hash)
            {
                Assert.Contains(c.ToString(), validChars);
            }
        }

        [Fact]
        public void HashPassword_Same_Password_Always_Returns_Same_Hash()
        {
            // Arrange
            var service = new PasswordService();

            // Act
            string hash1 = service.HashPassword("password123");
            string hash2 = service.HashPassword("password123");

            // Assert
            Assert.AreEqual(hash1, hash2);
        }

        [Fact]
        public void HashPassword_Different_Passwords_Return_Different_Hashes()
        {
            // Arrange
            var service = new PasswordService();

            // Act
            string hash1 = service.HashPassword("password123");
            string hash2 = service.HashPassword("password456");

            // Assert
            Assert.AreNotEqual(hash1, hash2);
        }

    }
    [TestClass]
    public class GenerateRandomPasswordTests
    {
        private const string ValidChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

        [TestMethod]
        public void GenerateRandomPassword_ContainsAllCharacterTypes()
        {
            // Arrange
            var service = new Login();
            int length = 100; // Большая длина для статистики
            int testIterations = 50;
            bool hasLower = false, hasUpper = false, hasDigit = false;

            for (int i = 0; i < testIterations; i++)
            {
                // Act
                string result = service.GenerateRandomPassword(length);

                // Assert
                hasLower |= result.Any(char.IsLower);
                hasUpper |= result.Any(char.IsUpper);
                hasDigit |= result.Any(char.IsDigit);
            }

            Assert.IsTrue(hasLower, "No lowercase letters found");
            Assert.IsTrue(hasUpper, "No uppercase letters found");
            Assert.IsTrue(hasDigit, "No digits found");
        }
    }
}
