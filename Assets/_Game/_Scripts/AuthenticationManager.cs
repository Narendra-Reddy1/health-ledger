using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;

namespace BenStudios
{
    public class AuthenticationManager : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _username;
        [SerializeField] private TMP_InputField _password;
        [SerializeField] private CustomToggle _passwordToggle;

        [SerializeField] private Button _login;
        [SerializeField] private Button _signUp;

        private void OnEnable()
        {
            _passwordToggle.onValueChanged.AddListener(_TogglePasswordVisibility);
        }
        private void OnDisable()
        {
            _passwordToggle.onValueChanged.RemoveListener(_TogglePasswordVisibility);

        }

        private void _TogglePasswordVisibility(bool value)
        {
            _password.contentType = value ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        }

        private void Start()
        {
            if (PlayerprefsHandler.GetPlayerPrefsBool(PlayerPrefKeys.isLoggedIn))
            {

            }
            else
            {
                NetworkHandler.Fetch<LoginData, BaseErrorResponse>("login", (LoginData) => { 
                }, (err) =>
                {

                }, new NetworkHandler.RequestData
                {
                    method=NetworkHandler.Method.POST,
                    body= "{\"username\":\"reddy\",\"password\":\"reddy123\"}"
                });

            }
        }

    }
}