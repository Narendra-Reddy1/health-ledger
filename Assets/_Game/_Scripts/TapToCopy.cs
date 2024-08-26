using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TapToCopy : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textToCopy;
    [SerializeField] private Button _button;
    TextEditor textEditor = new TextEditor();
    private void OnEnable()
    {
        _button.onClick.AddListener(CopyCode);
    }
    private void OnDisable()
    {
        _button.onClick.RemoveListener(CopyCode);
    }
    public void CopyCode()
    {
        textEditor.text = _textToCopy.text;
        textEditor.SelectAll();
        textEditor.Copy();
    }
}
