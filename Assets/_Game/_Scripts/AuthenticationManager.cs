using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using BenStudios.EventSystem;

namespace BenStudios
{
    public class AuthenticationManager : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _username;
        [SerializeField] private TMP_InputField _password;
        [SerializeField] private CustomToggle _passwordToggle;

        [SerializeField] private Button _login;
        [SerializeField] private Button _signUp;
        [SerializeField] private TextMeshProUGUI _errorTxt;


        private void OnEnable()
        {
            _errorTxt.SetText(string.Empty);
            _password.onFocusSelectAll = false;
            _passwordToggle.onValueChanged.AddListener(_TogglePasswordVisibility);

            _login.onClick.AddListener(_Login);
            _signUp.onClick.AddListener(_SignUp);
        }
        private void OnDisable()
        {
            _passwordToggle.onValueChanged.RemoveListener(_TogglePasswordVisibility);
            _login.onClick.RemoveListener(_Login);
            _signUp.onClick.RemoveListener(_SignUp);

        }

        private void _TogglePasswordVisibility(bool value)
        {
            _password.contentType = value ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
            _password.Select();
        }

        private void Start()
        {
            if (PlayerprefsHandler.GetPlayerPrefsBool(PlayerPrefKeys.isLoggedIn))
            {
                //disable login screen.
                gameObject.SetActive(false);
            }
        }


        private void _Login()
        {
            _errorTxt.SetText(string.Empty);
            if (_username.text.Length < 3)
            {
                _errorTxt.SetText("Username length must be more than 3 characters");
                return;
            }
            else if (!_isPasswordValid(_password.text))
            {
                ///show error text
                _errorTxt.SetText("Please enter the valid password");
                return;
            }
            _ToggleLoadingScreen(true);
            NetworkHandler.Fetch("/login", (userData) =>
             {
                 var loginData = JsonUtility.FromJson<UserData>(userData);
                 PlayerprefsHandler.SetPlayerPrefsAsString(PlayerPrefKeys.username, loginData.user.username);

                 //disable login screen 
                 //show homescreen
                 _ToggleLoadingScreen(false);
                 PlayerprefsHandler.SetPlayerPrefsBool(PlayerPrefKeys.isLoggedIn, true);
                 if (loginData.user.publicKey != null)
                 {
                     PlayerprefsHandler.SetPlayerPrefsAsString(PlayerPrefKeys.publicKey, loginData.user.publicKey);
                     PlayerprefsHandler.SetPlayerPrefsBool(PlayerPrefKeys.hasWallet, true);
                 }
                 gameObject.SetActive(false);

             }, (err) =>
             {
                 var error = JsonUtility.FromJson<BaseErrorResponse>(err);
                 if (error.message.Contains("Invalid credentials"))
                     _errorTxt.SetText("Invalid credentials");
                 else
                     _errorTxt.SetText("Something went wrong. Please try again later");
                 _ToggleLoadingScreen(false);

             }, new NetworkHandler.RequestData
             {
                 method = NetworkHandler.Method.POST,
                 body = "{\"username\":\"" + _username.text + "\",\"password\":\"" + _password.text + " \"}"

             });
        }
        private void _SignUp()
        {
            _errorTxt.SetText(string.Empty);
            if (_username.text.Length < 3)
            {
                _errorTxt.SetText("Username length must be more than 3 characters");
                return;
            }
            else if (!_isPasswordValid(_password.text))
            {
                ///show error text
                _errorTxt.SetText("Password must be 8 characters long and must contain atleast a <b>Number</b> , <b>Capital Letter</b> , <b> Special character </b>");
                return;
            }
            _ToggleLoadingScreen(true);

            NetworkHandler.Fetch("/sign-up", (userData) =>
            {
                //disable login screen show the home page
                var signupData = JsonUtility.FromJson<UserData>(userData);
                PlayerprefsHandler.SetPlayerPrefsAsString(PlayerPrefKeys.username, signupData.user.username);
                PlayerprefsHandler.SetPlayerPrefsBool(PlayerPrefKeys.isLoggedIn, true);
                _ToggleLoadingScreen(false);

                gameObject.SetActive(false);

            }, (err) =>
            {
                var error = JsonUtility.FromJson<BaseErrorResponse>(err);
                if (error.message.Contains("conflicting usernames"))
                    _errorTxt.SetText("Username already taken. Please use different one");
                else
                    _errorTxt.SetText("Something went wrong. Please try again later");
                _ToggleLoadingScreen(false);

                //conflicting usernames
            }, new NetworkHandler.RequestData
            {
                method = NetworkHandler.Method.POST,
                body = "{\"username\":\"" + _username.text + "\",\"password\":\"" + _password.text + " \"}"

            });
        }

        private bool _isPasswordValid(string password)
        {
            string pattern = @"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
            return Regex.IsMatch(password, pattern);
        }


        private void _ToggleLoadingScreen(bool value)
        {
            GlobalEventHandler.TriggerEvent(EventID.OnToggleLoadingPanel, value);
            _login.interactable = !value;
            _signUp.interactable = !value;
        }
    }
}