using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using DDMail.Properties;

namespace DDMail.Services
{
    public class AddressService
    {
        private static readonly HttpClient _sharedClient =
            new() {BaseAddress = new Uri("https://quack.duckduckgo.com/api/email/") };
        private readonly TokenService _tokenService;

        private string? _nextAlias;

        public AddressService(TokenService tokenService)
        {
            _tokenService = tokenService;
            _nextAlias = Settings.Default.NextAlias;
        }

        public string? GetNextAlias() => _nextAlias;

        public async Task<string?> GenerateNewAlias()
        {
            _sharedClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var response = await _sharedClient.PostAsync("addresses", null);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStreamAsync();

            using var document = await JsonDocument.ParseAsync(content);

            var address = document.RootElement.GetProperty("address").GetString();
            
            _nextAlias = address;

            Settings.Default.NextAlias = _nextAlias;
            Settings.Default.Save();

            return _nextAlias;
        }
    }
}
