using BenStudios.EventSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
namespace BenStudios
{
    public class Pedometer : MonoBehaviour
    {

        int _stepsOffset;
        bool _isPermissionGranted = false;
        private int _previousSteps = 0;
        private int _totalRecordedSteps;

        void Start()
        {
            if (Application.isEditor)
            {
                Debug.Log("Playing in editor");
                return;
            }

            RequestPermission();
        }

        void Update()
        {
            if (Application.isEditor || !_isPermissionGranted)
            {
                Debug.Log("No permission given");
                return;
            }

            if (StepCounter.current == null)
            {
                Debug.Log("StepCounter not available");
                return;
            }

            if (_stepsOffset == 0)
            {
                _stepsOffset = StepCounter.current.stepCounter.ReadValue();
            }
            else
            {
                int currentSteps = StepCounter.current.stepCounter.ReadValue();
                int stepsTaken = currentSteps - _stepsOffset;
                int stepsDiff = stepsTaken - _previousSteps;
                if (stepsDiff > 0)
                {
                    _totalRecordedSteps += stepsDiff;
                    GlobalEventHandler.TriggerEvent(EventID.OnStepCountUpdated, stepsDiff);
                }
                _previousSteps = stepsTaken;
            }
        }

        async void RequestPermission()
        {
#if UNITY_EDITOR
            Debug.Log("Editor Platform");
#endif
#if UNITY_ANDROID
            AndroidRuntimePermissions.Permission result = await AndroidRuntimePermissions.RequestPermissionAsync("android.permission.ACTIVITY_RECOGNITION");
            if (result == AndroidRuntimePermissions.Permission.Granted)
            {
                _isPermissionGranted = true;
                InitializeStepCounter();
            }
            else if (result == AndroidRuntimePermissions.Permission.Denied)
            {
                Debug.Log("Activity Premission declined");
                GlobalEventHandler.TriggerEvent(EventID.OnActivityPermissionDeclined);
            }
#endif
        }

        void InitializeStepCounter(bool fromPause = false)
        {
            InputSystem.EnableDevice(StepCounter.current);
            if (!fromPause)
                _stepsOffset = StepCounter.current.stepCounter.ReadValue();
            else
            {
                _stepsOffset = PlayerprefsHandler.GetSecurePlayerPrefsInt(PlayerPrefKeys.totalCountedSteps);
                //int cachedOffset = PlayerprefsHandler.GetSecurePlayerPrefsInt(PlayerPrefKeys.totalCountedSteps);
                //_totalRecordedSteps = PlayerprefsHandler.GetSecurePlayerPrefsInt(PlayerPrefKeys.totalRecordedSteps);
                //_stepsOffset = StepCounter.current.stepCounter.ReadValue();
                //_stepsOffset -= _totalRecordedSteps;

                //if (_stepsOffset > (cachedOffset - _totalRecordedSteps))
                //{
                //    _totalRecordedSteps += (_stepsOffset - (cachedOffset - _totalRecordedSteps));
                //    // _previousSteps = _stepsOffset - previousOffset;
                //}
                //PlayerprefsHandler.SetSecurePlayerPrefsInt(PlayerPrefKeys.totalRecordedSteps, 0);
                PlayerprefsHandler.SetSecurePlayerPrefsInt(PlayerPrefKeys.totalCountedSteps, 0);
            }
        }

        void OnApplicationPause(bool pause)
        {
            /*
             * //need to test this thing
             * is it mean this step counter resets when app is paused???
             * then that will be an extra headache to handle.....
             */
            PlayerprefsHandler.SetSecurePlayerPrefsInt(PlayerPrefKeys.totalRecordedSteps, _totalRecordedSteps);
            PlayerprefsHandler.SetSecurePlayerPrefsInt(PlayerPrefKeys.totalCountedSteps, StepCounter.current.stepCounter.ReadValue());
            if (!pause && _isPermissionGranted)
            {
                // Reinitialize the step counter when the app is resumed
                InitializeStepCounter(true);
                //Trigger re-initialize Event;
                //reset playerprefs.
            }
        }
    }

}