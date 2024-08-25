using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class StepLogger : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI  DEBUGTEXT;
    [SerializeField] private TextMeshProUGUI _footStepCountTxt;
    [SerializeField] private TextMeshProUGUI _distanceTxt;

    long stepOffset;
    bool permissionGranted = false;

    void Start()
    {
        if (Application.isEditor)
        {
            DEBUGTEXT.text = "Running in Editor";
            return;
        }

        RequestPermission();
    }

    void Update()
    {
        if (Application.isEditor || !permissionGranted)
        {
            DEBUGTEXT.text = "Please provide required permissions";
            return;
        }

        if (StepCounter.current == null)
        {
           
            DEBUGTEXT.text = "StepCounter not available";
            return;
        }

        if (stepOffset == 0)
        {
            stepOffset = StepCounter.current.stepCounter.ReadValue();
            DEBUGTEXT.text = "Step offset " + stepOffset;
        }
        else
        {
            long currentSteps = StepCounter.current.stepCounter.ReadValue();
            long stepsTaken = currentSteps - stepOffset;
            _footStepCountTxt.text = "Steps: " + stepsTaken;

            //try
            //{
            //    Challenge.staticData.player.steps = (int)stepsTaken;
            //}
            //catch (System.OverflowException)
            //{
            //    DEBUGTEXT.text = "Failure to convert long steps";
            //}
        }
    }

    async void RequestPermission()
    {
#if UNITY_EDITOR
        DEBUGTEXT.text = "Editor Platform";
#endif
#if UNITY_ANDROID
        AndroidRuntimePermissions.Permission result = await AndroidRuntimePermissions.RequestPermissionAsync("android.permission.ACTIVITY_RECOGNITION");
        if (result == AndroidRuntimePermissions.Permission.Granted)
        {
            permissionGranted = true;
            DEBUGTEXT.text = "Permission granted";
            InitializeStepCounter();
        }
        else
        {
            DEBUGTEXT.text = "Permission state: " + result;
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
