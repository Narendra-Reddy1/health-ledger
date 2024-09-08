using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BenStudios
{


    public class HistoryElement : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI _timestampTxt;
        [SerializeField] private TextMeshProUGUI _amountTxt;
        [SerializeField] private GameObject _inIcon;
        [SerializeField] private GameObject _outIcon;



        public void Init()
        {

        }
    }
}