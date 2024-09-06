using BenStudios.EventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{


    [SerializeField] private GameObject _loadingPanel;

    private void OnEnable()
    {
        GlobalEventHandler.AddListener(EventID.OnToggleLoadingPanel, _ToggleLoadingScreen);
    }
    private void OnDisable()
    {
        GlobalEventHandler.RemoveListener(EventID.OnToggleLoadingPanel, _ToggleLoadingScreen);
    }

    private void _ToggleLoadingScreen(object args)
    {
        if (!_loadingPanel) return;
        var value = (bool)args;
        _loadingPanel.SetActive(value);

    }
}
