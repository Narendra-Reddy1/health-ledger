using BenStudios.EventSystem;
using DG.Tweening;
using UnityEngine;

namespace BenStudios
{
    public class CreateWalletPopup : MonoBehaviour
    {
        [SerializeField] private Transform _popup;
        [SerializeField] private FloorButton _walletBtn;
        [SerializeField] private CustomButton _createWalletBtn;

        private void OnEnable()
        {
            _createWalletBtn.onClick.AddListener(_OnCreateBtnClicked);
            GlobalEventHandler.AddListener(EventID.OnJoinTournamentWithoutWallet, Callback_On_Wallet_Popup_Requested);
        }
        private void OnDisable()
        {
            _createWalletBtn.onClick.RemoveListener(_OnCreateBtnClicked);
            GlobalEventHandler.RemoveListener(EventID.OnJoinTournamentWithoutWallet, Callback_On_Wallet_Popup_Requested);
        }


        private void _OnCreateBtnClicked()
        {
            _walletBtn._OnClick();
            _popup.DOScale(0.3f, .1f).From(1).onComplete += () =>
            {
                _popup.gameObject.SetActive(false);
            };
        }

        private void Callback_On_Wallet_Popup_Requested(object args)
        {
            _popup.gameObject.SetActive(true);
            _popup.DOScale(1, .2f).From(0.45f);
        }
    }
}