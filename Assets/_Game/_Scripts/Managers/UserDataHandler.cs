using BenStudios.EventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataHandler : MonoBehaviour
{
    private long _stepsCount;

    private float _stepsCountPerDay = 1_500f;//this will update based on the user activity

    
    public UserData userData;
    public float StepCountPerDay => _stepsCountPerDay;
    public long TodayStepCount => _stepsCount;
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
        GlobalEventHandler.AddListener(EventID.OnStepCountUpdated, Callback_On_Step_Count);
    }
    private void OnDisable()
    {
        GlobalEventHandler.RemoveListener(EventID.OnStepCountUpdated, Callback_On_Step_Count);
    }

    private void Callback_On_Step_Count(object args)
    {
        var steps = (long)args;
        if (steps > 0)
        {
            _stepsCount += steps;
            GlobalEventHandler.TriggerEvent(EventID.OnStepCountRecorded, _stepsCount);
        }
    }

}
