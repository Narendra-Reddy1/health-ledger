using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField] private GameObject _loginScreen;
    private void Awake()
    {
        if (!PlayerprefsHandler.GetPlayerPrefsBool(PlayerPrefKeys.isLoggedIn))
        {
            _loginScreen.SetActive(true);
            return;
        }
        string username = PlayerprefsHandler.GetPlayerPrefsString(PlayerPrefKeys.username);
        string url = $"/{username}/user-info";
        NetworkHandler.Fetch(url, (data) =>
        {
            var userData = JsonUtility.FromJson<UserData>(data);
            UserDataHandler.instance.userData = userData;
            if (!string.IsNullOrEmpty(userData.user.publicKey))
            {
                PlayerprefsHandler.SetPlayerPrefsBool(PlayerPrefKeys.hasWallet, true);
                UserDataHandler.instance.isParticipatedInTournament = userData.user.tournaments.Any(
                    x => x.tournamentId == PlayerprefsHandler.GetPlayerPrefsInt(PlayerPrefKeys.currentRunningTournament, 0));
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
