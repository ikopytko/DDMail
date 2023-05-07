using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DDMail.Properties;

namespace DDMail.Services
{
    public class TokenService
    {
        private string? _token;

        public TokenService()
        {
            _token = Settings.Default.Token;
        }

        public string? GetToken() => _token;

        public void SaveToken(string? token)
        {
            _token = token;
            Settings.Default.Token = _token;
            Settings.Default.Save();
        }
    }
}
