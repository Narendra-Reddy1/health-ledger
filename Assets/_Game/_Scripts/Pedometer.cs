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

        long stepOffset;
        bool permissionGranted = false;
        private long previousSteps = 0;
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
            if (Application.isEditor || !permissionGranted)
            {
                Debug.Log("No permission given");
                return;
            }

            if (StepCounter.current == null)
            {
                Debug.Log("StepCounter not available");
                return;
            }

            if (stepOffset == 0)
            {
                stepOffset = StepCounter.current.stepCounter.ReadValue();
            }
            else
            {
                long currentSteps = StepCounter.current.stepCounter.ReadValue();
                long stepsTaken = currentSteps - stepOffset;
                long stepsDiff = stepsTaken - previousSteps;
                if (stepsDiff > 0)
                    GlobalEventHandler.TriggerEvent(EventID.OnStepCountUpdated, stepsDiff);
                previousSteps = stepsTaken;
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
                permissionGranted = true;
                InitializeStepCounter();
            }
            else if (result == AndroidRuntimePermissions.Permission.Denied)
            {
                Debug.Log("Activity Premission declined");
                GlobalEventHandler.TriggerEvent(EventID.OnActivityPermissionDeclined);
            }
#endif
        }

        void InitializeStepCounter()
        {
            InputSystem.EnableDevice(StepCounter.current);
            stepOffset = StepCounter.current.stepCounter.ReadValue();
        }

        void OnApplicationPause(bool pause)
        {
            /*
             * //need to test this thing
             * is it mean this step counter resets when app is paused???
             * then that will be an extra headache to handle.....
             */
            if (!pause && permissionGranted)
            {
                // Reinitialize the step counter when the app is resumed
                InitializeStepCounter();
            }
        }
    }

}