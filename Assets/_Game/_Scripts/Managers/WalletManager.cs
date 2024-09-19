using BenStudios.EventSystem;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BenStudios
{
    public class WalletManager : MonoBehaviour
    {

        [Header("Wallet")]
        [SerializeField] private Transform _walletMainPage;
        [SerializeField] private TextMeshProUGUI _balanceTxt;
        [SerializeField] private TextMeshProUGUI _publicKeyTxt;
        [SerializeField] private Button _refreshBtn;

        [Space]
        [Header("Withdraw")]
        [SerializeField] private Transform _withdrawPanel;
        [SerializeField] private TextMeshProUGUI _dailyLimitTxt;
        [SerializeField] private TextMeshProUGUI _minWithdrawLimitTxt;
        [SerializeField] private TextMeshProUGUI _withdraFeeTxt;
        [SerializeField] private TextMeshProUGUI _withdrawErrorTxt;
        [SerializeField] private TMP_InputField _toAddressField;
        [SerializeField] private TMP_InputField _withdrawPinField;
        [SerializeField] private TMP_InputField _withdrawAmountField;

        [SerializeField] private Button _withdrawBtn;
        [SerializeField] private Button _withdrawPanelBtn;
        [SerializeField] private Button _withdrawPanelCloseBtn;

        [Space]
        [Header("History")]
        [SerializeField] private HistoryElement _historyElement;
        [SerializeField] private Transform _walletHistoryParent;


        [Space]
        [Header("New Wallet")]
        [SerializeField] private Transform _generateWalletPanel;
        [SerializeField] private TMP_InputField _secretPin;
        [SerializeField] private TMP_InputField _confirmSecretPin;
        [SerializeField] private Button _generateWallet;

        [SerializeField] private TextMeshProUGUI _errorTxt;


        private void OnEnable()
        {
            _Init();
            _ResetWithdrawPanel();
            _withdrawPanel.gameObject.SetActive(false);
            _withdrawPanelBtn.onClick.AddListener(_OpenWithdrawPanel);
            _withdrawPanelCloseBtn.onClick.AddListener(_CloseWithdrawPanel);
            _refreshBtn.onClick.AddListener(_RefreshBalance);
            _generateWallet.onClick.AddListener(_GenerateNewWallet);
            _secretPin.onEndEdit.AddListener(_OnSecretPinEntered);
            _confirmSecretPin.onEndEdit.AddListener(_OnSecretPinEntered);
            _withdrawBtn.onClick.AddListener(_WithdrawBalance);

            _withdrawPinField.onEndEdit.AddListener(_OnWithdrawPinEntered);
            _toAddressField.onEndEdit.AddListener(_OnToAddressEntered);
            _withdrawAmountField.onEndEdit.AddListener(_OnWithdrawAmountEntered);
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

            _withdrawPinField.onEndEdit.RemoveListener(_OnWithdrawPinEntered);
            _toAddressField.onEndEdit.RemoveListener(_OnToAddressEntered);
            _withdrawAmountField.onEndEdit.RemoveListener(_OnWithdrawAmountEntered);
        }
        private void _Init()
        {
            bool hasWallet = PlayerprefsHandler.GetPlayerPrefsBool(PlayerPrefKeys.hasWallet);
            _generateWalletPanel.gameObject.SetActive(!hasWallet);
            if (hasWallet)
            {
                _RefreshBalance();
            }
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
                _balanceTxt.SetText($"{walletBalance.balances.tokens.ToString()} HLT");
                _publicKeyTxt.SetText(walletBalance.publicKey);
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


        #region Create Wallet

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
                _errorTxt.SetText("Pin is short. Secure pin must be 6 digits");
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
                _balanceTxt.SetText($"{wallet.user.balance.ToString()} HLT");
                _generateWalletPanel.DOScale(0, 0.3f).onComplete += () =>
                {
                    _generateWalletPanel.gameObject.SetActive(false);
                };
                PlayerprefsHandler.SetPlayerPrefsBool(PlayerPrefKeys.hasWallet, true);
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

        #endregion Create Wallet

        #region Withdraw

        private void _ResetWithdrawPanel()
        {
            _withdrawAmountField.text = string.Empty;
            _toAddressField.text = string.Empty;
            _withdrawPinField.text = string.Empty;
            _withdrawErrorTxt.SetText(string.Empty);
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
                _ResetWithdrawPanel();
                _withdrawPanel.gameObject.SetActive(false);
            };

        }
        bool _isAmountValid = false;
        private void _OnWithdrawAmountEntered(string amount)
        {
            if (amount.Length <= 0) return;
            var value = float.Parse(amount);
            if (value <= 0)
            {
                _withdrawErrorTxt.SetText("Amount should be a positive number");
                _isAmountValid = false;
            }
            else if (value > float.Parse(UserDataHandler.instance.userData.user.balance))
            {
                Debug.Log("Insufficent balance");
                _withdrawErrorTxt.SetText("Insufficient balance");
                _isAmountValid = false;
            }
            else
            {
                _withdrawErrorTxt.SetText(string.Empty);
                _isAmountValid = true;
            }
            _withdrawBtn.interactable = _IsAddressValid(_toAddressField.text) && _isAmountValid;
        }
        private void _OnWithdrawPinEntered(string pin)
        {
            if (pin.Length < 6)
                _withdrawErrorTxt.SetText("Incorrect PIN");
        }
        private void _OnToAddressEntered(string address)
        {

            bool isValidAddress = _IsAddressValid(_toAddressField.text);
            if (!isValidAddress)
            {
                _withdrawErrorTxt.SetText("Wallet addres is invalid");
            }
            else if (address == UserDataHandler.instance.userData.user.publicKey)
            {
                _withdrawErrorTxt.SetText("Really(-_-)? Transfering to your wallet also incur gas cost!!");
            }
            _withdrawBtn.interactable = isValidAddress && _isAmountValid;
        }
        private void _WithdrawBalance()
        {
            string username = PlayerprefsHandler.GetPlayerPrefsString(PlayerPrefKeys.username);
            string url = $"/{username}/withdraw";
            string passkey = _withdrawPinField.text;
            string toAddress = _toAddressField.text;
            float amount = float.Parse(_withdrawAmountField.text);
            //if (!_IsAddressValid(toAddress))
            //{
            //    Debug.LogError("Invalid wallet address");
            //    return;
            //}
            //if (_IsPinCorrect())
            //{
            //    Debug.Log("Incorrect PIN");
            //    return;
            //}

            NetworkHandler.Fetch(url, (data) =>
            {

            }, (err) =>
            {

            }, new NetworkHandler.RequestData
            {
                method = NetworkHandler.Method.POST,
                body = "{\"passkey\":\"" + passkey + "\",\"amount\":" + amount + ",\"toAddress\":\"" + toAddress + "\"}"
            });
        }

        private bool _IsAddressValid(string address)
        {
            // Check if the address starts with "0x" and is 42 characters long
            if (string.IsNullOrEmpty(address) || address.Length != 42 || !address.StartsWith("0x"))
                return false;
            string hexPart = address.Substring(2); // Remove the "0x" prefix
            return Regex.IsMatch(hexPart, @"^[0-9a-fA-F]{40}$");
        }


        #endregion Withdraw

    }
}