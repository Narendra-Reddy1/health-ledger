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
    public const string BASE_URL = "http://localhost:3000";
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
    public static async void Fetch<T1, T2>(string url, Action<T1> onSuccess, Action<T2> onFail, RequestData requestData = null)
    {
        HttpRequestMessage request = _GetHttpRequest(url, requestData);
        HttpResponseMessage response = await client.SendAsync(request);
        string data = string.Empty;
        try
        {
            data = await response.Content.ReadAsStringAsync();
            Debug.Log(data);
            response.EnsureSuccessStatusCode();
            onSuccess?.Invoke(JsonUtility.FromJson<T1>(data));

        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log($"Status code: {response.StatusCode} phrase: {response.ReasonPhrase}");
            onFail?.Invoke(JsonUtility.FromJson<T2>(data));
        }
    }
    public static async void Fetch(string url, Action<string> onSuccess, Action<string> onFail, RequestData requestData = null)
    {
        HttpRequestMessage request = _GetHttpRequest(url, requestData);
        Debug.Log("body \n " + requestData.body);
        HttpResponseMessage response = await client.SendAsync(request);
        string data = string.Empty;
        try
        {

            data = await response.Content.ReadAsStringAsync();
            Debug.Log(data);
            response.EnsureSuccessStatusCode();
            onSuccess?.Invoke(data);

        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log($"Status code: {response.StatusCode} phrase: {response.ReasonPhrase}");
            onFail?.Invoke(data);
        }
    }
    public static async void Fetch2(string url, Action<HttpResponseMessage> onSuccess, Action<HttpResponseMessage> onFail, RequestData requestData = null)
    {
        HttpRequestMessage request = _GetHttpRequest(url, requestData);
        HttpResponseMessage response = await client.SendAsync(request);
        string data = string.Empty;
        try
        {

            data = await response.Content.ReadAsStringAsync();
            Debug.Log(data);
            response.EnsureSuccessStatusCode();
            onSuccess?.Invoke(response);

        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log($"Status code: {response.StatusCode} phrase: {response.ReasonPhrase}");
            onFail?.Invoke(response);
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
        request.RequestUri = new Uri(BASE_URL + url);
        return request;

    }
}


#region Data


[Serializable]
public class UserData
{
    public User user;
}

[Serializable]
public class User
{
    public string username;
    public string stepsCount;
    public string publicKey;
    public float balance;
    public List<TournamentEntry> tournaments;

}


#region Wallet

[Serializable]
public class ConfigData
{
    public Wallet wallet;
}


[Serializable]
public class WalletBalance
{
    public string publicKey;
    public Balances balances;
}

[Serializable]
public class Balances
{
    public string tokens;
}

[Serializable]
public class Wallet
{
    public Withdraw withdraw;
}

[Serializable]
public class Withdraw
{
    public int withdrawFee;
    public Transactionlimits transactionLimits;
}

[Serializable]
public class Transactionlimits
{
    public Limit perTransaction;
    public Limit perDay;
}

[Serializable]
public class Limit
{
    public int min;
    public int max;
}

#endregion Wallet

#region Tournament


//get 
[Serializable]
public class TournamentData
{
    public int tournamentId;
    public int startTime;
    public int endTime;
    public int prizePool;
    public int prizeDistributionId;
    public List<Participant> participants;
    public string txHash;
}

[Serializable]
public class Participant
{
    public string username;
    public int steps;
    public string publicKey;
}


[Serializable]
public class PrizePoolDistribution
{
    public int first;
    public int second;
    public int third;
    public int fourToTen;
    public int elevenToTwentyFive;
    public int twentySixToFifty;
    public int fiftyOneToHundred;
}

[Serializable]
public class TournamentEntry
{
    public bool isParticipated;
    public int tournamentId;

}


[Serializable]
public class JoinTournamentResult
{
    public string txHash;
    public int tournamentId;
    public int stepsCount;
}


#endregion Tournament


[Serializable]
public class BaseErrorResponse
{
    public string message;
}

#endregion Data


