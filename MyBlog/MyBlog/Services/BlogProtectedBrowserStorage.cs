using Components.Interfaces;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace MyBlog.Services;

public class BlogProtectedBrowserStorage : IBrowserStorage
{
    public BlogProtectedBrowserStorage(ProtectedSessionStorage storage)
    {
        Storage = storage;
    }

    public ProtectedSessionStorage Storage { get; }

    public async Task DeleteAsync(string key)
    {
        await Storage.DeleteAsync(key);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await Storage.GetAsync<T>(key);
        if (value.Success)
        {
            return value.Value;
        }
        return default(T);
    }

    public async Task SetAsync(string key, object value)
    {
        await Storage.SetAsync(key, value);
    }
}
