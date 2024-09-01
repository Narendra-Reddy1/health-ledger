using System.Net.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class NetworkHandler
{

    public enum Method
    {
        GET,
        POST,
        PUT,
        PATCH,
        DELETE
    }
    public class RequestData
    {
        public Dictionary<string, string> headers;
        public string body;
        public string contentType = "application/json";
        public Method method;
    }
    static readonly HttpClient client = new HttpClient();
    public static void Init()
    {

    }

    public static async void Fetch(string url, Action onSuccess, Action onFail, RequestData requestData = null)
    {
        HttpRequestMessage request = _GetHttpRequest(url, requestData);
        HttpResponseMessage response = await client.SendAsync(request);
        try
        {
            response.EnsureSuccessStatusCode();
            onSuccess?.Invoke();
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log($"Status code: {response.StatusCode} phrase: {response.ReasonPhrase}");
            onFail?.Invoke();
        }
    }

    public static async void Fetch<T>(string url, Action<T> onSuccess, Action onFail, RequestData requestData = null)
    {
        HttpRequestMessage request = _GetHttpRequest(url, requestData);
        HttpResponseMessage response = await client.SendAsync(request);
        try
        {
            response.EnsureSuccessStatusCode();
            string data = await response.Content.ReadAsStringAsync();
            onSuccess?.Invoke(JsonUtility.FromJson<T>(data));
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log($"Status code: {response.StatusCode} phrase: {response.ReasonPhrase}");
            onFail?.Invoke();
        }
    }
    public static async void Fetch<T>(string url, Action<T> onSuccess, Action<string> onFail, RequestData requestData = null)
    {
        HttpRequestMessage request = _GetHttpRequest(url, requestData);
        HttpResponseMessage response = await client.SendAsync(request);
        try
        {
            response.EnsureSuccessStatusCode();
            string data = await response.Content.ReadAsStringAsync();
            onSuccess?.Invoke(JsonUtility.FromJson<T>(data));

        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log($"Status code: {response.StatusCode} phrase: {response.ReasonPhrase}");
            onFail?.Invoke(e.ToString());
        }
    }

    private static HttpRequestMessage _GetHttpRequest(string url, RequestData requestData)
    {
        HttpRequestMessage request = new HttpRequestMessage();
        switch (requestData.method)
        {
            case Method.GET:
                request.Method = HttpMethod.Get;
                break;
            case Method.POST:
                request.Method = HttpMethod.Post;
                break;
            case Method.PUT:
                request.Method = HttpMethod.Put;
                break;
            case Method.PATCH:
                request.Method = HttpMethod.Patch;
                break;
            case Method.DELETE:
                request.Method = HttpMethod.Delete;
                break;
        }
        if (requestData.body != null)
            request.Content = new StringContent(requestData.body, System.Text.Encoding.UTF8, requestData.contentType);
        if (requestData.headers != null)
            foreach (var kvp in requestData.headers)
            {
                request.Headers.Add(kvp.Key, kvp.Value);
            }
        request.RequestUri = new Uri(url);
        return request;

    }
}
