using BenStudios.EventSystem;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BenStudios
{
    public class WalletManager : MonoBehaviour
    {

        [SerializeField] private Transform _walletMainPage;
        [SerializeField] private TextMeshProUGUI _balanceTxt;
        [SerializeField] private TextMeshProUGUI _publicKeyTxt;
        [SerializeField] private Button _refreshBtn;


        [SerializeField] private Transform _withdrawPanel;
        [SerializeField] private TextMeshProUGUI _dailyLimitTxt;
        [SerializeField] private TextMeshProUGUI _minWithdrawLimitTxt;
        [SerializeField] private TextMeshProUGUI _withdraFeeTxt;
        [SerializeField] private Button _withdrawBtn;
        [SerializeField] private Button _withdrawPanelBtn;
        [SerializeField] private Button _withdrawPanelCloseBtn;

        [SerializeField] private HistoryElement _historyElement;
        [SerializeField] private Transform _walletHistoryParent;


        [SerializeField] private Transform _generateWalletPanel;
        [SerializeField] private TMP_InputField _secretPin;
        [SerializeField] private TMP_InputField _confirmSecretPin;
        [SerializeField] private Button _generateWallet;

        [SerializeField] private TextMeshProUGUI _errorTxt;


        private void OnEnable()
        {

            _withdrawPanelBtn.onClick.AddListener(_OpenWithdrawPanel);
            _withdrawPanelCloseBtn.onClick.AddListener(_CloseWithdrawPanel);
            _refreshBtn.onClick.AddListener(_RefreshBalance);
            _generateWallet.onClick.AddListener(_GenerateNewWallet);
            _secretPin.onEndEdit.AddListener(_OnSecretPinEntered);
            _confirmSecretPin.onEndEdit.AddListener(_OnSecretPinEntered);
            _withdrawBtn.onClick.AddListener(_WithdrawBalance);
        }

        private void OnDisable()
        {
            _withdrawPanelBtn.onClick.RemoveListener(_OpenWithdrawPanel);
            _withdrawPanelCloseBtn.onClick.RemoveListener(_CloseWithdrawPanel);
            _withdrawBtn.onClick.RemoveListener(_WithdrawBalance);
            _refreshBtn.onClick.RemoveListener(_RefreshBalance);
            _generateWallet.onClick.RemoveListener(_GenerateNewWallet);
            _secretPin.onEndEdit.RemoveListener(_OnSecretPinEntered);
            _confirmSecretPin.onEndEdit.RemoveListener(_OnSecretPinEntered);
        }
        private void Start()
        {
            _Init();
        }
        private void _Init()
        {
            bool hasWallet = PlayerprefsHandler.GetPlayerPrefsBool(PlayerPrefKeys.hasWallet);
            _generateWalletPanel.gameObject.SetActive(!hasWallet);
        }

        //Move these methods a generic script to use for all screens.
        private void _OpenWithdrawPanel()
        {
            _withdrawPanel.gameObject.SetActive(true);
            _withdrawPanel.DOScale(1, .3f).From(0.4f).onComplete += () =>
            {
                _ConfigureWallet();
            };

        }
        private void _CloseWithdrawPanel()
        {
            _withdrawPanel.DOScale(0, .3f).From(1).onComplete += () =>
              {
                  _withdrawPanel.gameObject.SetActive(false);
              };

        }

        private void _RefreshBalance()
        {
            GlobalEventHandler.TriggerEvent(EventID.OnToggleLoadingPanel, true);
            string username = PlayerprefsHandler.GetPlayerPrefsString(PlayerPrefKeys.username);
            if (username.Length <= 0) return;
            string url = $"/{username}/get-balance";
            NetworkHandler.Fetch(url, (data) =>
            {
                WalletBalance walletBalance = JsonUtility.FromJson<WalletBalance>(data);
                _balanceTxt.SetText(walletBalance.balances.tokens.ToString());
                GlobalEventHandler.TriggerEvent(EventID.OnToggleLoadingPanel, false);
            }, (err) =>
            {
                Debug.LogError($"error with balance refresh {err}");
                GlobalEventHandler.TriggerEvent(EventID.OnToggleLoadingPanel, false);
            }, new NetworkHandler.RequestData
            {
                method = NetworkHandler.Method.GET
            });
        }

        private void _WithdrawBalance()
        {

        }

        private void _OnSecretPinEntered(string value)
        {
            _errorTxt.SetText(string.Empty);
            if (_secretPin.text.Length > 0 && _confirmSecretPin.text.Length > 0)
            {
                bool isMatched = (_secretPin.text == _confirmSecretPin.text);
                _generateWallet.interactable = isMatched;
                if (!isMatched)
                    _errorTxt.SetText("Secure Pin doesn't match");
            }
        }

        private void _GenerateNewWallet()
        {
            if (_secretPin.text.Length < 6)
            {
                _errorTxt.SetText("Secure Pin is shorter. It must be 6 characters in length");
                return;
            }
            if (_secretPin.text != _confirmSecretPin.text)
            {
                _errorTxt.SetText("Secure Pin doesn't match");
                return;
            }
            GlobalEventHandler.TriggerEvent(EventID.OnToggleLoadingPanel, true);
            _generateWallet.interactable = false;
            string username = PlayerprefsHandler.GetPlayerPrefsString(PlayerPrefKeys.username);
            if (username.Length <= 0) return;
            string url = $"/{username}/create-wallet";
            //RIGHT NOW Anyone can create a wallet to any user Try to add some authentication/Authorization to secure
            NetworkHandler.Fetch(url, (walletData) =>
            {
                UserData wallet = JsonUtility.FromJson<UserData>(walletData);
                _walletMainPage.gameObject.SetActive(true);
                _publicKeyTxt.SetText(wallet.user.publicKey);
                _balanceTxt.SetText(wallet.user.balance.ToString());
                _generateWalletPanel.DOScale(0, 0.3f).onComplete += () =>
                {
                    _generateWalletPanel.gameObject.SetActive(false);
                };
                GlobalEventHandler.TriggerEvent(EventID.OnToggleLoadingPanel, false);

            }, (err) =>
            {
                BaseErrorResponse error = JsonUtility.FromJson<BaseErrorResponse>(err);
                if (error != null)
                {
                    if (error.message.Contains("Wallet already exists") || error.message.Contains("Secuirity Pin is short"))
                        _errorTxt.SetText(error.message);
                }
                else
                    _errorTxt.SetText("Something went wrong. Please try again later");

                _generateWallet.interactable = true;
                GlobalEventHandler.TriggerEvent(EventID.OnToggleLoadingPanel, false);
            }, new NetworkHandler.RequestData
            {
                method = NetworkHandler.Method.POST,
                body = "{\"passkey\":\"" + _confirmSecretPin.text + "\"}"

            });
        }

        private void _ConfigureWallet()
        {
            GlobalEventHandler.TriggerEvent(EventID.OnToggleLoadingPanel, true);
            NetworkHandler.Fetch("/config/wallet", (data) =>
            {
                Wallet walletData = JsonUtility.FromJson<ConfigData>(data).wallet;
                _withdraFeeTxt.SetText(walletData.withdraw.withdrawFee.ToString() + "%");
                _dailyLimitTxt.SetText(walletData.withdraw.transactionLimits.perDay.max.ToString());
                _minWithdrawLimitTxt.SetText(walletData.withdraw.transactionLimits.perTransaction.min.ToString());
                GlobalEventHandler.TriggerEvent(EventID.OnToggleLoadingPanel, false);
            }, (err) =>
            {
                Debug.LogError("COnfigWallet......");
                Debug.LogError(err);
                GlobalEventHandler.TriggerEvent(EventID.OnToggleLoadingPanel, false);
            }, new NetworkHandler.RequestData
            {
                method = NetworkHandler.Method.GET
            });
        }

    }
}