﻿using Blazored.SessionStorage;
using Components.Interfaces;

namespace BlazorWebAssembly.Client.Services;

public class BlogBrowserStorage : IBrowserStorage
{
    public BlogBrowserStorage(ISessionStorageService storage)
    {
        Storage = storage;
    }

    public ISessionStorageService Storage { get; }

    public async Task DeleteAsync(string key)
    {
        await Storage.RemoveItemAsync(key);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
       return await Storage.GetItemAsync<T> (key);
    }

    public async Task SetAsync(string key, object value)
    {
        await Storage.SetItemAsync(key, value);
    }
}