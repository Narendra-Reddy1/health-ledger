using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine.UI;
using BenStudios.EventSystem;

namespace BenStudios
{
    public class FloorButton : MonoBehaviour
    {
        [SerializeField] private Transform _activeIcon;
        [SerializeField] private Transform _inActiveIcon;

        [SerializeField] private Transform _activePose;
        [SerializeField] private Transform _inActivePose;
        [SerializeField] private Transform _selectionHighlight;

        [SerializeField] private Button _button;

        [SerializeField] private ButtonType _buttonType;
        private bool _isSelected = false;

        private void OnEnable()
        {
            _button.onClick.AddListener(_OnClick);
            GlobalEventHandler.AddListener(EventID.OnFloorButtonClicked, Callback_On_Select);

        }
        private void OnDisable()
        {
            _button.onClick.RemoveListener(_OnClick);
            GlobalEventHandler.RemoveListener(EventID.OnFloorButtonClicked, Callback_On_Select);
        }

        [Button]
        public void Activate()
        {
            _ResetEverything();
            _isSelected = true;
            _activeIcon.gameObject.SetActive(true);
            _activeIcon.DOMove(_activePose.position, .25f).From(_inActivePose.position);
            _selectionHighlight.gameObject.SetActive(true);
            _selectionHighlight.DOMove(_activePose.position, .2f).From(_inActivePose.position);
        }

        [Button]
        public void DeActivate()
        {
            _ResetEverything();
            _isSelected = false;
            _inActiveIcon.gameObject.SetActive(true);
            _inActiveIcon.DOMove(_inActivePose.position, .25f).From(_activePose.position);
            _selectionHighlight.DOMove(_inActivePose.position, .1f).From(_activePose.position).onComplete += () =>
              {
                  _selectionHighlight.gameObject.SetActive(false);
              };
        }

        private void _OnClick()
        {
            if (_isSelected) return;
            Activate();
            GlobalEventHandler.TriggerEvent(EventID.OnFloorButtonClicked, _buttonType);
        }
        private void _ResetEverything()
        {
            _activeIcon.gameObject.SetActive(false);
            _inActiveIcon.gameObject.SetActive(false);
            _selectionHighlight.gameObject.SetActive(false);


        }
        private void Callback_On_Select(object args)
        {
            if (_buttonType == (ButtonType)args) return;
            if (_isSelected)
                DeActivate();
        }

    }
    public enum ButtonType
    {
        Wallet,
        Home,
        Leaderboard
    }
}