using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField] private GameObject _loginScreen;
    [SerializeField] private GameObject _sessionExpiredMsg;
    [SerializeField] private Credentials _encryptionCredentials;

    private void Awake()
    {

        if (!PlayerprefsHandler.GetPlayerPrefsBool(PlayerPrefKeys.isLoggedIn))
        {
            _loginScreen.SetActive(true);
            return;
        }

        ZPlayerPrefs.Initialize(_encryptionCredentials.EncryptionPassword, _encryptionCredentials.EncryptionSalt);
        NetworkHandler.FetchUserProfile((userData) =>
        {
            UserDataHandler.instance.userData = userData;
            if (!string.IsNullOrEmpty(userData.user.publicKey))
            {
                PlayerprefsHandler.SetPlayerPrefsBool(PlayerPrefKeys.hasWallet, true);


                //fetchLatestTournamentData...
                NetworkHandler.FetchLatestTournamentData((data) =>
                {
                    UserDataHandler.instance.isParticipatedInTournament = userData.user.tournaments.Any(
                        x => x.tournamentId == data.data.tournamentId);

                });
            }
        },
        (err) =>
        {
            Debug.LogError($"Failed to fetch user data <b> Main </b> {err}");
            _loginScreen.SetActive(true);
            _sessionExpiredMsg.SetActive(true);
        });
    }
}
