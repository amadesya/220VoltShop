using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

namespace courseProd.Services
{
	public class ApiClient
	{
		private readonly HttpClient _http;
		public string Token { get; set; } = string.Empty;
		public string UserRole { get; set; } = string.Empty;
		public string UserName { get; set; } = string.Empty;
        public void SetToken(string token)
        {
            Token = token;

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            UserName = jwt.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? string.Empty;

            UserRole = jwt.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? string.Empty;
        }

        public bool IsAuthenticated => !string.IsNullOrEmpty(Token);

		public ApiClient(string baseAddress)
		{
			_http = new HttpClient { BaseAddress = new System.Uri(baseAddress) };
		}

		private void AddAuth()
		{
			if (!string.IsNullOrEmpty(Token))
			{
				_http.DefaultRequestHeaders.Authorization =
					new AuthenticationHeaderValue("Bearer", Token);
			}
			else
			{
				_http.DefaultRequestHeaders.Remove("Authorization");
			}
		}


		public async Task<(bool ok, JsonDocument? data, string text, int statusCode)> PostJson(string url, object body)
		{
			var json = JsonSerializer.Serialize(body);
			AddAuth();
			var res = await _http.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
			var txt = await res.Content.ReadAsStringAsync();
			JsonDocument? doc = null;
			if (!string.IsNullOrWhiteSpace(txt))
			{
				try { doc = JsonDocument.Parse(txt); } catch { doc = null; }
			}
			return (res.IsSuccessStatusCode, doc, txt ?? string.Empty, (int)res.StatusCode);
		}

        public async Task<T?> GetJson<T>(string url)
        {
            AddAuth();
            var res = await _http.GetAsync(url);
            if (!res.IsSuccessStatusCode) return default;

            var txt = await res.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(txt)) return default;

            try
            {
                return JsonSerializer.Deserialize<T>(txt, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch
            {
                return default;
            }
        }


        public async Task<bool> PutJson(string url, object body)
		{
			AddAuth();
			var json = JsonSerializer.Serialize(body);
			var res = await _http.PutAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
			return res.IsSuccessStatusCode;
		}

		public async Task<bool> DeleteAsync(string url)
		{
			AddAuth();
			var res = await _http.DeleteAsync(url);
			return res.IsSuccessStatusCode;
		}

		public async Task<JsonDocument?> GetJson(string url)
		{
			AddAuth();
			var res = await _http.GetAsync(url);
			if (!res.IsSuccessStatusCode)
			{
				return null;
			}
			var txt = await res.Content.ReadAsStringAsync();
			if (string.IsNullOrWhiteSpace(txt)) return null;
			try { return JsonDocument.Parse(txt); }
			catch { return null; }
		}

	public async Task<byte[]?> GetRawBytes(string url)
	{
		try
		{
			AddAuth();
			var response = await _http.GetAsync(url);
			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadAsByteArrayAsync();
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

    }
}
