﻿using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

public static class HttpClientExtensions
{
    public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
    {
        var dataAsString = System.Text.Json.JsonSerializer.Serialize(data);
        var content = new StringContent(dataAsString);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return httpClient.PostAsync(url, content);
    }

    public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
    {
        var dataAsString = System.Text.Json.JsonSerializer.Serialize(data);
        var content = new StringContent(dataAsString);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return httpClient.PutAsync(url, content);
    }

    public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
    {
        var dataAsString = await content.ReadAsStringAsync();
        return System.Text.Json.JsonSerializer.Deserialize<T>(dataAsString)!;
    }
}