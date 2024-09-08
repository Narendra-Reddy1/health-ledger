using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private void Awake()
    {
        if (!PlayerprefsHandler.GetPlayerPrefsBool(PlayerPrefKeys.isLoggedIn)) return;

        string username = PlayerprefsHandler.GetPlayerPrefsString(PlayerPrefKeys.username);
        string url = $"/{username}/user-info";
        NetworkHandler.Fetch(url, (data) =>
        {
            var userData = JsonUtility.FromJson<UserData>(data);
            UserDataHandler.instance.userData = userData;
            if (userData.user.publicKey != null)
            {
                PlayerprefsHandler.SetPlayerPrefsBool(PlayerPrefKeys.hasWallet, true);
            }

        }, (err) =>
        {
            Debug.LogError($"Failed to fetch user data <b> Main </b> {err}");
        }, new NetworkHandler.RequestData
        {
            method = NetworkHandler.Method.GET
        });
    }
}
