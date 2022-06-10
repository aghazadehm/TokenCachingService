using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace TokenCachingApi.Framework
{
    public class TokenService : ITokenService
    {
        private readonly IMemoryCache _cache;
        private readonly IOptions<EndpointSetting> _options;

        public TokenService(IMemoryCache cache, IOptions<EndpointSetting> options)
        {
            _cache = cache;
            _options = options;
        }

        public string FetchToken()
        {
            string token = string.Empty;

            // if cache doesn't contain 
            // an entry called TOKEN
            // error handling mechanism is mandatory
            if (!_cache.TryGetValue("TOKEN", out token))
            {
                var tokenmodel = this.GetTokenFromApi();
                if(tokenmodel==null)
                {
                    throw new InvalidOperationException("Token couldn't generate");
                }
                // keep the value within cache for 
                // given amount of time
                // if value is not accessed within the expiry time
                // delete the entry from the cache
                var options = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(
                              TimeSpan.FromTicks(tokenmodel.Expiration.Ticks));

                _cache.Set("TOKEN", tokenmodel.Token, options);

                token = tokenmodel.Token;
            }

            return token;
        }

        private AccessToken? GetTokenFromApi()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _options.Value.BaseUri + "/Authenticate/Token")
            {
                Content = JsonContent.Create(new
                {
                    UserName = _options.Value.UserName,
                    Password = _options.Value.Password
                })
            };

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = client.Send(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(response.ReasonPhrase, null, response.StatusCode);
            }
            var content = response.Content.ReadAsStringAsync().Result;
            var token = JsonConvert.DeserializeObject<AccessToken>(content);
            return token;
        }
    }
}
