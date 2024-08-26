using AYellowpaper.SerializedCollections;
using BenStudios.EventSystem;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BenStudios
{

    public class UIManager : MonoBehaviour
    {
        [Header("Steps")]
        [SerializeField] private TextMeshProUGUI _stepsCountTxt;
        [SerializeField] private Image _stepsCountRadialFillbar;
        [SerializeField] private SerializedDictionary<ButtonType, Screen> _pages;
        [SerializeField] private SerializedDictionary<ButtonType, FloorButton> _floorBtns;

        //[SerializeField] private TextMeshProUGUI _stepsTargetCountTxt;
        private void OnEnable()
        {
            ResetState();
            GlobalEventHandler.AddListener(EventID.OnStepCountRecorded, Callback_On_Step_Count_Recorded);
            GlobalEventHandler.AddListener(EventID.OnFloorButtonClicked, Callback_On_Floor_Btn_Click);
        }
        private void OnDisable()
        {
            GlobalEventHandler.RemoveListener(EventID.OnStepCountRecorded, Callback_On_Step_Count_Recorded);
            GlobalEventHandler.RemoveListener(EventID.OnFloorButtonClicked, Callback_On_Floor_Btn_Click);
        }
        private void Start()
        {
            _floorBtns[ButtonType.Home].Activate(); //Default Home
        }

        private void Callback_On_Step_Count_Recorded(object args)
        {
            var steps = (long)args;
            _stepsCountTxt.SetText(steps.ToString());
            float fillAmount = ((float)steps / UserDataHandler.instance.StepCountPerDay);
            _stepsCountRadialFillbar.fillAmount = fillAmount;
        }
        private void ResetState()
        {
            _stepsCountRadialFillbar.fillAmount = 0;
        }


        private void Callback_On_Floor_Btn_Click(object args)
        {
            var btnType = (ButtonType)args;
            _ResetAllPages();
            _pages[btnType].ToggleCanvas(true);
        }
        void _ResetAllPages()
        {
            _pages.Values.ToList().ForEach(x => x.ToggleCanvas(false));
        }


#if UNITY_EDITOR

        long step = 1;
        [Button]
        public void AddStep()
        {
             
            GlobalEventHandler.TriggerEvent(EventID.OnStepCountUpdated, step);
        }
#endif
    }
}