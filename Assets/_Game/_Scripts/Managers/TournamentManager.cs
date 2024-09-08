using BenStudios.EventSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
public class TournamentManager : MonoBehaviour
{
    [SerializeField] private TournamentElement _tournamentElement;
    [SerializeField] private Transform _content;
    [SerializeField] private TextMeshProUGUI _startDate;
    [SerializeField] private TextMeshProUGUI _endDate;
    [SerializeField] private TextMeshProUGUI _tournamentIdTxt;
    [SerializeField] private TextMeshProUGUI _prizepoolTxt;
    [SerializeField] private TextMeshProUGUI _tournamentTxHash;
    [SerializeField] private CustomButton _txHashBtn;
    [SerializeField] private CustomButton _joinTournamentBtn;
    [SerializeField] private GameObject _fadingGroup;

    private long _endTimeStamp;
    private long _startTimeStamp;

    private int _prizeDistributionID;
    private int _tournamentId;
    private string _txHash;
    private ObjectPool<TournamentElement> _pool;


    private const string EXPLORER_BASE_URL = "https://www.oklink.com/amoy/address";//later refactor this to use based on the selected blockchain

    private void Awake()
    {
        _pool = new ObjectPool<TournamentElement>(() =>
          {
              TournamentElement element = Instantiate(_tournamentElement, _content);
              element.gameObject.SetActive(false);
              return element;
          },
          (element) =>
          {
              element.gameObject.SetActive(true);
          },
          (element) =>
          {
              element.gameObject.SetActive(false);
          }, defaultCapacity: 50, maxSize: 100);
    }
    private void OnEnable()
    {
        _CreateLeaderboard();
    }

    private void OnDisable()
    {
        _DestroyLeaderboard();
    }

    private void _CreateLeaderboard()
    {
        GlobalEventHandler.TriggerEvent(EventID.OnToggleLoadingPanel, true);
        int tournamentId = 0;
        NetworkHandler.Fetch($"/tournament/get/{tournamentId}", (data) =>
        {
            var tournamentData = JsonUtility.FromJson<TournamentData>(data);
            _prizeDistributionID = tournamentData.prizeDistributionId;
            _txHash = tournamentData.txHash;
            _SetTxHash(_txHash);
            _endTimeStamp = tournamentData.endTime;
            _startTimeStamp = tournamentData.startTime;
            string username = UserDataHandler.instance.userData.user.username;
            bool isParticipated = tournamentData.participants.Any(x => x.username == username);
            if (!isParticipated)
            {
                _fadingGroup.SetActive(true);
                _joinTournamentBtn.onClick.RemoveAllListeners();
                _joinTournamentBtn.onClick.AddListener(_JoinTournament);
            }
            else
            {
                _fadingGroup.SetActive(false);
            }
            var count = tournamentData.participants.Count;
            for (int i = 0; (i < count) && (i < 100); i++)
            {
                var element = _pool.Get();
                element.Init(tournamentData.participants[i], i + 1, tournamentData.participants[i].username == username);
            }
            _StartCountdown();
            _txHashBtn.onClick.AddListener(_OpenTournamentTxHash);
            GlobalEventHandler.TriggerEvent(EventID.OnToggleLoadingPanel, false);
        }, (err) =>
        {
            Debug.LogError("Tournament Manager...CreateLeaderboard");
            GlobalEventHandler.TriggerEvent(EventID.OnToggleLoadingPanel, false);
        }, new NetworkHandler.RequestData
        {
            method = NetworkHandler.Method.GET,
        });
    }
    private void _DestroyLeaderboard()
    {
        _StopCountdown();
        for (int i = 0; i < _content.childCount; i++)
        {
            _content.GetChild(i).gameObject.SetActive(false);
        }
        _txHashBtn.onClick.RemoveListener(_OpenTournamentTxHash);
    }


    private void _JoinTournament()
    {
        GlobalEventHandler.TriggerEvent(EventID.OnToggleLoadingPanel, true);
        if (!PlayerprefsHandler.GetPlayerPrefsBool(PlayerPrefKeys.hasWallet))
        {
            Debug.LogError("Please create wallet to join tournaments");
            GlobalEventHandler.TriggerEvent(EventID.OnToggleLoadingPanel, false);
            //Show a popup here
            return;
        }
        NetworkHandler.Fetch("/tournament/join", (data) =>
        {
            var resultData = JsonUtility.FromJson<JoinTournamentResult>(data);
            _fadingGroup.SetActive(false);
            _joinTournamentBtn.onClick.RemoveAllListeners();
            //Show some popup wiht success info.
            GlobalEventHandler.TriggerEvent(EventID.OnToggleLoadingPanel, false);
            UserDataHandler.instance.isParticipatedInTournament = true;
            UserDataHandler.instance.participatedInTournamentId = _tournamentId;

        }, (err) =>
        {
            Debug.LogError("_JOin tournament");
            GlobalEventHandler.TriggerEvent(EventID.OnToggleLoadingPanel, false);
        }, new NetworkHandler.RequestData
        {
            method = NetworkHandler.Method.POST,
            body = "{\"username\":\"" + UserDataHandler.instance.userData.user.username + "\",\"tournamentId\":" + _tournamentId + "}"
        });

    }

    private void _StartCountdown()
    {
        InvokeRepeating(nameof(_EndTimeTick), 0, 1);
    }
    private void _StopCountdown()
    {
        CancelInvoke(nameof(_EndTimeTick));
    }
    private void _EndTimeTick()
    {
        long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long remainingTime = _endTimeStamp - currentTimestamp;
        if (remainingTime <= 0)
        {
            _StopCountdown();
        }
        TimeSpan timeSpan = TimeSpan.FromSeconds(remainingTime);

        int days = timeSpan.Days;
        int hours = timeSpan.Hours;
        int minutes = timeSpan.Minutes;
        int seconds = timeSpan.Seconds;
        _endDate.text = string.Format("{0}D : {1}H : {2}M : {3:D2}S", days, hours, minutes, seconds);
    }
    private void _SetTxHash(string txHash)
    {
        var start = txHash.Substring(0, 4);
        var end = txHash.Substring(txHash.Length - 5, 5);
        _tournamentTxHash.SetText(start + "...." + end);
    }
    private void _OpenTournamentTxHash()
    {
        Application.OpenURL(EXPLORER_BASE_URL + "/" + _txHash);

    }
}
