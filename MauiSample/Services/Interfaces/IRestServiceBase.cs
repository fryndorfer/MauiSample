namespace MauiSample.Services.Interfaces
{
    public interface IRestServiceBase
    {
        string M42ApiPrefix { get; set; }
        Task<T?> GetAsync<T>(string url);
        Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data);
        Task<string> PutAsync<TRequest>(string url, TRequest data);
        Task<bool> DeleteAsync(string url);
    }
}
