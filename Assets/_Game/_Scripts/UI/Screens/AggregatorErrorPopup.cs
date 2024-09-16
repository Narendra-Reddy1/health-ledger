using BenStudios.EventSystem;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggregatorErrorPopup : MonoBehaviour
{
    [SerializeField] private Transform _errorPopup;
    private void OnEnable()
    {
        GlobalEventHandler.AddListener(EventID.OnAggregatorErrorEncountered, Callback_On_Error);
    }

    private void OnDisable()
    {
        GlobalEventHandler.RemoveListener(EventID.OnAggregatorErrorEncountered, Callback_On_Error);
    }

    public void OnCloseBtnClicked()
    {

        _errorPopup.DOScale(0, .25f).From(1f).onComplete += () =>
          {
              _errorPopup.gameObject.SetActive(false);
          };
    }

    private void Callback_On_Error(object args)
    {
        _errorPopup.gameObject.SetActive(true);
        _errorPopup.DOScale(1, .25f).From(.7f);
    }


}
