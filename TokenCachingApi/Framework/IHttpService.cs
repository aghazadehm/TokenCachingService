namespace TokenCachingApi.Framework
{
    public interface IHttpService
    {
        HttpResponseMessage GetFromExternalApi(string requestUrl);
    }
}