using BenStudios.EventSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserDataHandler : MonoBehaviour
{
    private int _stepsCounter;
    private int _prevUpdatedUserSteps;
    private int _totalStepsInTheSession;
    private int _tournamentSteps;
    private int _prevUpdatedTournamentSteps;

    private float _stepsCountPerDay = 1_500f;//this will update based on the user activity

    public UserData userData;
    public float StepCountPerDay => _stepsCountPerDay;
    public int TodayStepCount => _stepsCounter;

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
        InvokeRepeating(nameof(_UploadToBlockchain), 45, 45);
        InvokeRepeating(nameof(_UploadToServer), 60, 60);
        GlobalEventHandler.AddListener(EventID.OnStepCountUpdated, Callback_On_Step_Count);
    }
    private void OnDisable()
    {
        CancelInvoke(nameof(_UploadToBlockchain));
        CancelInvoke(nameof(_UploadToServer));
        GlobalEventHandler.RemoveListener(EventID.OnStepCountUpdated, Callback_On_Step_Count);
    }


    private void _UploadToBlockchain()
    {
        int currentRunningTournamentId = PlayerprefsHandler.GetPlayerPrefsInt(PlayerPrefKeys.currentRunningTournament);
        if (!isParticipatedInTournament) return;
        NetworkHandler.RecordTournamentSteps(_tournamentSteps - _prevUpdatedTournamentSteps, (result) =>
          {
              _prevUpdatedTournamentSteps += result.addedStepsCount;
              //trigger event to update the leaderboard element.

          }, (err) =>
          {
              Debug.LogError("Failed to upload Steps to server!!");
          });
    }


    private void _UploadToServer()
    {
        NetworkHandler.RecordUserSteps(_stepsCounter - _prevUpdatedUserSteps, (result) =>
        {
            _prevUpdatedUserSteps += result.addedStepsCount;
            //update ui event
        }, (err) =>
        {
            Debug.LogError("Failed to upload Steps to server!!");
        });
    }

    private void Callback_On_Step_Count(object args)
    {
        var steps = (int)args;
        if (steps > 0)
        {
            _stepsCounter += steps;
            _totalStepsInTheSession += steps;
            if (isParticipatedInTournament) _tournamentSteps += steps;
            GlobalEventHandler.TriggerEvent(EventID.OnStepCountRecorded, _totalStepsInTheSession);
        }
    }

}
