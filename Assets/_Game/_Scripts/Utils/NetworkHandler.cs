using System.Net.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using BenStudios.EventSystem;

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
    //public const string BASE_URL = "http://localhost:3000";
#if TRON
    public const string BASE_URL = "https://health-ledger-backend.vercel.app";
#else  //ETH
    public const string BASE_URL = "https://health-ledger-backend.vercel.app";
#endif
    static readonly HttpClient client = new HttpClient();


    public static void Init()
    {

    }

    public static async void Fetch(string url, Action onSuccess, Action onFail, RequestData requestData = null, bool isCritical = false)
    {
        HttpRequestMessage request = _GetHttpRequest(url, requestData, isCritical);
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
            if (e.Message.Contains("Error: AggregateError"))
            {
                GlobalEventHandler.TriggerEvent(EventID.OnAggregatorErrorEncountered);
            }
            onFail?.Invoke();
        }
    }

    public static async void Fetch<T>(string url, Action<T> onSuccess, Action onFail, RequestData requestData = null, bool isCritical = false)
    {
        HttpRequestMessage request = _GetHttpRequest(url, requestData, isCritical);
        HttpResponseMessage response = await client.SendAsync(request);
        string data = string.Empty;
        try
        {
            response.EnsureSuccessStatusCode();
            data = await response.Content.ReadAsStringAsync();
            onSuccess?.Invoke(JsonUtility.FromJson<T>(data));
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log($"Status code: {response.StatusCode} phrase: {response.ReasonPhrase}");
            var err = JsonUtility.FromJson<BaseErrorResponse>(data);
            if (err.message.Contains("Error: AggregateError"))
            {
                GlobalEventHandler.TriggerEvent(EventID.OnAggregatorErrorEncountered);
            }
            onFail?.Invoke();
        }
    }
    public static async void Fetch<T>(string url, Action<T> onSuccess, Action<string> onFail, RequestData requestData = null, bool isCritical = false)
    {
        HttpRequestMessage request = _GetHttpRequest(url, requestData, isCritical);
        HttpResponseMessage response = await client.SendAsync(request);
        string data = string.Empty;
        try
        {
            response.EnsureSuccessStatusCode();
            data = await response.Content.ReadAsStringAsync();
            onSuccess?.Invoke(JsonUtility.FromJson<T>(data));

        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log($"Status code: {response.StatusCode} phrase: {response.ReasonPhrase}");
            var err = JsonUtility.FromJson<BaseErrorResponse>(data);
            if (err.message.Contains("Error: AggregateError"))
            {
                GlobalEventHandler.TriggerEvent(EventID.OnAggregatorErrorEncountered);
            }
            onFail?.Invoke(e.ToString());
        }
    }
    public static async void Fetch<T1, T2>(string url, Action<T1> onSuccess, Action<T2> onFail, RequestData requestData = null, bool isCritical = false)
    {
        HttpRequestMessage request = _GetHttpRequest(url, requestData, isCritical);
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
            var err = JsonUtility.FromJson<BaseErrorResponse>(data);
            if (err != null)
                if (err.message.Contains("Error: AggregateError"))
                {
                    GlobalEventHandler.TriggerEvent(EventID.OnAggregatorErrorEncountered);
                }
            onFail?.Invoke(JsonUtility.FromJson<T2>(data));
        }
    }
    public static async void Fetch(string url, Action<string> onSuccess, Action<string> onFail, RequestData requestData = null, bool isCritical = false)
    {
        HttpRequestMessage request = _GetHttpRequest(url, requestData, isCritical);
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
            var err = JsonUtility.FromJson<BaseErrorResponse>(data);
            if (err != null)
                if (err.message.Contains("Error: AggregateError"))
                {
                    GlobalEventHandler.TriggerEvent(EventID.OnAggregatorErrorEncountered);
                }
            onFail?.Invoke(data);
        }
    }
    public static async void Fetch2(string url, Action<HttpResponseMessage> onSuccess, Action<HttpResponseMessage> onFail, RequestData requestData = null, bool isCritical = false)
    {
        HttpRequestMessage request = _GetHttpRequest(url, requestData, isCritical);
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
            var err = JsonUtility.FromJson<BaseErrorResponse>(data);
            if (err.message.Contains("Error: AggregateError"))
            {
                GlobalEventHandler.TriggerEvent(EventID.OnAggregatorErrorEncountered);
            }
            onFail?.Invoke(response);
        }
    }


    private static HttpRequestMessage _GetHttpRequest(string url, RequestData requestData, bool isCritical = false)
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
        if (requestData.headers == null)
            requestData.headers = new Dictionary<string, string>();

        string token = PlayerprefsHandler.GetPlayerPrefsString(PlayerPrefKeys.authToken);
        if (IsTokenExpired(token) && isCritical)
            requestData.headers.Add("authorization", PlayerprefsHandler.GetPlayerPrefsString(PlayerPrefKeys.fallbackToken));
        foreach (var kvp in requestData.headers)
        {
            request.Headers.Add(kvp.Key, kvp.Value);
        }
        request.RequestUri = new Uri(BASE_URL + url);
        return request;

    }


    public static bool IsTokenExpired(string token)
    {
        //just to be sure the token won't expire within a minute or two
        long graceValue = 120;

        //string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6InIyZWR3dzF5IiwicHVibGljS2V5IjoiMHgiLCJpYXQiOjE3MjczNjEyNDIsImV4cCI6MTcyNzk2NjA0Mn0.YZAPMRhkSe3U81uFVjEkwuZHyWAij2o96HvWzERSut4";
        // Split the token into its parts (Header, Payload, Signature)
        string[] tokenParts = token.Split('.');
        if (tokenParts.Length < 2)
        {
            throw new System.ArgumentException("Invalid JWT token");
        }

        // Decode the payload (which contains the 'exp' field)
        string payload = Base64UrlDecode(tokenParts[1]);
        Newtonsoft.Json.Linq.JObject payloadData = Newtonsoft.Json.Linq.JObject.Parse(payload);

        // Extract the 'exp' field, which is a Unix timestamp
        long exp = payloadData["exp"].ToObject<long>();

        // Convert the expiration time to DateTime
        DateTime expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp + graceValue).UtcDateTime;

        // Compare the expiration time with the current time
        return expirationTime < DateTime.UtcNow;
    }

    // Helper method to decode the Base64Url-encoded JWT part
    private static string Base64UrlDecode(string input)
    {
        // Replace URL-safe characters and pad with '=' characters
        string output = input.Replace('-', '+').Replace('_', '/');
        switch (output.Length % 4)
        {
            case 2: output += "=="; break;
            case 3: output += "="; break;
        }

        // Convert the Base64 string to a byte array
        var byteArray = Convert.FromBase64String(output);

        // Convert byte array to string
        return System.Text.Encoding.UTF8.GetString(byteArray);
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
    public string token;
    public string fallbackToken;
    public string username;
    public string stepsCount;
    public string publicKey;
    public string balance;
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
public class Tournament
{
    public TournamentData data;
    public PrizePoolDistribution distribution;
}

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
[Serializable]
public class BlockchainRecordStepsResult
{

    public string txHash;
    public int tournamentId;
    public int updatedSteps;
    public int addedStepsCount;
}
[Serializable]
public class RecordStepsResult
{
    public int updatedSteps;
    public int addedStepsCount;
}

#endregion Tournament


[Serializable]
public class BaseErrorResponse
{
    public string message;
}

#endregion Data