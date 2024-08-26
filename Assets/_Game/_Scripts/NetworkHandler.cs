using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
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
        public Method method;
    }

    public static void FetchData(string url, Action onSuccess, Action onFail, RequestData requestData = null)
    {
        using (HttpClient client = new HttpClient())
        {
            if (requestData != null)
                if (requestData.headers.Count > 0)
                    foreach (var kvp in requestData.headers)
                        client.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);


        }

    }

    public static void FetchData<T>(string url, Action<T> onSuccess, Action onFail, RequestData requestData = null)
    {

    }
    public static void FetchData<T>(string url, Action<T> onSuccess, Action<string> onFail, RequestData requestData = null)
    {

    }
}
