using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BenStudios {
    public class WalletManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _balanceTxt;
        [SerializeField] private TextMeshProUGUI _publicKeyTxt;
        [SerializeField] private Button _refreshBtn;

        [SerializeField] private HistoryElement _historyElement;
        [SerializeField] private Transform _walletHistoryParent;

    }
}