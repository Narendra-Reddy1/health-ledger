using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class CustomToggle : Toggle
{
    [SerializeField] private UnityEvent EventToggleOn;
    [SerializeField] private UnityEvent EventToggleOff;



    protected override void OnEnable()
    {
        base.OnEnable();
        base.onValueChanged.AddListener(OnToggle);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        base.onValueChanged.RemoveListener(OnToggle);
    }
    private void OnToggle(bool value)
    {
        if (value)
            EventToggleOn?.Invoke();
        else
            EventToggleOff?.Invoke();

    }
    public void RegisterOnToggleOn(UnityAction action)
    {
        EventToggleOn.AddListener(action);
    }
    public void UnRegisterOnToggleOn(UnityAction action)
    {
        EventToggleOn.RemoveListener(action);

    }
    public void RegisterOnToggleOff(UnityAction action)
    {
        EventToggleOff.AddListener(action);

    }
    public void UnRegisterToggleOff(UnityAction action)
    {
        EventToggleOff.RemoveListener(action);

    }
}
