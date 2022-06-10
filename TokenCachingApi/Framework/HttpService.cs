namespace TokenCachingApi.Framework
{
    public class HttpService : IHttpService
    {
        private ITokenService _tokenService;

        public HttpService(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public HttpResponseMessage GetFromExternalApi(string requestUrl)
        {
            using HttpClient client = new();
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("Authorization", $"Bearer {_tokenService.FetchToken()}");
            var res = client.Send(request);
            return res;
        }
    }
}
