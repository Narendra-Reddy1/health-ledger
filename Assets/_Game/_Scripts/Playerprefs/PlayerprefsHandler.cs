using UnityEngine;

public static class PlayerprefsHandler
{
    public static void SavePlayerPrefs()
    {
        PlayerPrefs.Save();
    }

    public static void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    public static void DeletePlayerPrefsKey(PlayerPrefKeys key)
    {
        PlayerPrefs.DeleteKey(key.ToString());
    }
    public static void DeletePlayerPrefsKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }

    public static bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }
    public static bool HasKey(PlayerPrefKeys key)
    {
        return PlayerPrefs.HasKey(key.ToString());
    }
    public static void SetPlayerPrefsAsString(PlayerPrefKeys key, string value = "")
    {
        PlayerPrefs.SetString(key.ToString(), value);
        PlayerPrefs.Save();
    }
    public static void SetSecurePlayerPrefsAsString(PlayerPrefKeys key, string value = "")
    {
        ZPlayerPrefs.SetString(key.ToString(), value);
        ZPlayerPrefs.Save();
    }

    public static void SetPlayerPrefsInt(PlayerPrefKeys key, int value)
    {
        PlayerPrefs.SetInt(key.ToString(), value);
        PlayerPrefs.Save();
    }
    public static void SetSecurePlayerPrefsInt(PlayerPrefKeys key, int value)
    {
        ZPlayerPrefs.SetInt(key.ToString(), value);
        ZPlayerPrefs.Save();
    }

    public static void SetPlayerPrefsInt(string key, int value)
    {
        PlayerPrefs.SetInt(key.ToString(), value);
        PlayerPrefs.Save();
    }
    public static void SetSecurePlayerPrefsInt(string key, int value)
    {
        ZPlayerPrefs.SetInt(key.ToString(), value);
        ZPlayerPrefs.Save();
    }
    public static void SetPlayerPrefsFloat(PlayerPrefKeys key, float value)
    {
        PlayerPrefs.SetFloat(key.ToString(), value);
        PlayerPrefs.Save();
    }
    public static void SetSecurePlayerPrefsFloat(PlayerPrefKeys key, float value)
    {
        ZPlayerPrefs.SetFloat(key.ToString(), value);
        ZPlayerPrefs.Save();
    }

    public static void SetPlayerPrefsBool(PlayerPrefKeys key, bool value)
    {
        SetPlayerPrefsInt(key, value ? 1 : 0);
    }

    public static void SetPlayerPrefsBool(string key, bool value)
    {
        SetPlayerPrefsInt(key, value ? 1 : 0);
    }
    public static void SetSecurePlayerPrefsBool(PlayerPrefKeys key, bool value)
    {
        SetSecurePlayerPrefsInt(key, value ? 1 : 0);
    }

    public static void SetSecurePlayerPrefsBool(string key, bool value)
    {
        SetSecurePlayerPrefsInt(key, value ? 1 : 0);
    }

    public static void AddPlayerPrefsStringCollection(PlayerPrefKeys key, string value)
    {
        string stringKey = key.ToString();
        string currentValue = PlayerPrefs.GetString(stringKey, string.Empty);
        if (string.IsNullOrEmpty(currentValue))
        {
            currentValue = value;
        }
        else
        {
            currentValue += "," + value;
        }

        PlayerPrefs.SetString(stringKey, currentValue);
        PlayerPrefs.Save();
    }






    public static string[] GetPlayerPrefsStringCollectionAsArray(PlayerPrefKeys key)
    {
        string[] collection = PlayerPrefs.GetString(key.ToString(), string.Empty).Split(',');
        return collection;
    }
    public static int GetPlayerPrefsInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key.ToString(), defaultValue);
    }
    public static int GetPlayerPrefsInt(PlayerPrefKeys key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key.ToString(), defaultValue);
    }
    public static string GetPlayerPrefsString(PlayerPrefKeys key, string defaultValue = "")
    {
        return PlayerPrefs.GetString(key.ToString(), defaultValue);
    }
    public static float GetPlayerPrefsFloat(PlayerPrefKeys key, float defaultValue = 0)
    {
        return PlayerPrefs.GetFloat(key.ToString(), defaultValue);
    }

    public static bool GetPlayerPrefsBool(PlayerPrefKeys key, bool defaultValue = false)
    {
        return GetPlayerPrefsInt(key, defaultValue ? 1 : 0) == 1;
    }

    public static bool GetPlayerPrefsBool(string key, bool defaultValue = false)
    {
        return (GetPlayerPrefsInt(key, defaultValue ? 1 : 0) == 1);
    }





    public static int GetSecurePlayerPrefsInt(string key, int defaultValue = 0)
    {
        return ZPlayerPrefs.GetInt(key.ToString(), defaultValue);
    }
    public static int GetSecurePlayerPrefsInt(PlayerPrefKeys key, int defaultValue = 0)
    {
        return ZPlayerPrefs.GetInt(key.ToString(), defaultValue);
    }
    public static string GetSecurePlayerPrefsString(PlayerPrefKeys key, string defaultValue = "")
    {
        return ZPlayerPrefs.GetString(key.ToString(), defaultValue);
    }
    public static float GetSecurePlayerPrefsFloat(PlayerPrefKeys key, float defaultValue = 0)
    {
        return ZPlayerPrefs.GetFloat(key.ToString(), defaultValue);
    }

    public static bool GetSecurePlayerPrefsBool(PlayerPrefKeys key, bool defaultValue = false)
    {
        return GetSecurePlayerPrefsInt(key, defaultValue ? 1 : 0) == 1;
    }

    public static bool GetSecurePlayerPrefsBool(string key, bool defaultValue = false)
    {
        return GetSecurePlayerPrefsInt(key, defaultValue ? 1 : 0) == 1;
    }
}


