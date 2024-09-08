using BenStudios.EventSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserDataHandler : MonoBehaviour
{
    private int _stepsCount;
    private int _tournamentSteps;

    private float _stepsCountPerDay = 1_500f;//this will update based on the user activity


    public UserData userData;
    public float StepCountPerDay => _stepsCountPerDay;
    public int TodayStepCount => _stepsCount;

    public bool isParticipatedInTournament;
    public int participatedInTournamentId;


    public static UserDataHandler instance { get; private set; }
    private void Awake()
    {
        if (instance != this && instance != null)
            Destroy(gameObject);
        else
            instance = this;
    }

    private void OnEnable()
    {
       // InvokeRepeating(nameof(_UploadToBlockchain), 120, 120);
        //InvokeRepeating(nameof(_UploadToServer), 30, 30);
        GlobalEventHandler.AddListener(EventID.OnStepCountUpdated, Callback_On_Step_Count);
    }
    private void OnDisable()
    {
        //CancelInvoke(nameof(_UploadToBlockchain));
        //CancelInvoke(nameof(_UploadToServer));
        GlobalEventHandler.RemoveListener(EventID.OnStepCountUpdated, Callback_On_Step_Count);
    }


    private void _UploadToBlockchain()
    {
        int currentRunningTournamentId = PlayerprefsHandler.GetPlayerPrefsInt(PlayerPrefKeys.currentRunningTournament);
        bool isParticipated = userData.user.tournaments.Any(x => (x.tournamentId == currentRunningTournamentId && isParticipatedInTournament));
        if (!isParticipated) return;
        NetworkHandler.Fetch("/tournament/record-steps", (data) =>
        {
            _tournamentSteps = 0;
            //trigger event to update the leaderboard element.

        }, (err) =>
        {
            Debug.LogError("Failed to upload Steps to server!!");
        }, new NetworkHandler.RequestData
        {
            method = NetworkHandler.Method.PATCH,
            body = "{\"steps\":" + _tournamentSteps + ",\"tournamentId\":" + 0 + ",\"username\":\"" + userData.user.username + "\"}"
        });
    }
    private void _UploadToServer()
    {
        int currentRunningTournamentId = PlayerprefsHandler.GetPlayerPrefsInt(PlayerPrefKeys.currentRunningTournament);
        NetworkHandler.Fetch("/record-steps", (data) =>
        {
            _stepsCount = 0;
        }, (err) =>
        {
            Debug.LogError("Failed to upload Steps to server!!");
        }, new NetworkHandler.RequestData
        {
            method = NetworkHandler.Method.PATCH,
            body = "{\"steps\":" + _stepsCount + ",\"username\":\"" + userData.user.username + "\"}"
        });
    }

    private void Callback_On_Step_Count(object args)
    {
        var steps = (int)args;
        if (steps > 0)
        {
            _stepsCount += steps;
            GlobalEventHandler.TriggerEvent(EventID.OnStepCountRecorded, _stepsCount);
        }
    }

}
