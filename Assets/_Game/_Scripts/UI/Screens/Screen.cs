using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenStudios
{
    public class Screen : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private CanvasGroup _canvasGroup;



        public void ToggleScreen(bool value)
        {
            gameObject.SetActive(value);
            _canvas.enabled = value;
        }
    }
}