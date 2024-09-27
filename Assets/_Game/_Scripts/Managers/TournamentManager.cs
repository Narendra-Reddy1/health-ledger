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

    private Tournament _tournament;
    private int _prizePoolAmount;
    private int _prizeDistributionID;
    private int _tournamentId;
    private string _txHash;
    private ObjectPool<TournamentElement> _pool;
    private List<TournamentElement> _activeElements = new List<TournamentElement>();
#if TRON
    private const string EXPLORER_BASE_URL = "https://nile.tronscan.org/#/transaction/";//later refactor this to use based on the selected blockchain
#else
    private const string EXPLORER_BASE_URL = "https://www.oklink.com/amoy/address";//later refactor this to use based on the selected blockchain
#endif
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
        //int tournamentId = 0;
        NetworkHandler.Fetch($"/tournament/get-latest-tournament", (data) =>
        {
            _tournament = JsonUtility.FromJson<Tournament>(data);
            _prizeDistributionID = _tournament.data.prizeDistributionId;
            _tournamentId = _tournament.data.tournamentId;
            _tournamentIdTxt.SetText(_tournamentId.ToString());
            _txHash = _tournament.data.txHash;
            _SetTxHash(_txHash);
            _endTimeStamp = _tournament.data.endTime;
            _startTimeStamp = _tournament.data.startTime;
            string username = UserDataHandler.instance.userData.user.username;
            _prizePoolAmount = _tournament.data.prizePool;
            _prizepoolTxt.SetText(_prizePoolAmount.ToString());
            _StartCountdown();
            _txHashBtn.onClick.AddListener(_OpenTournamentTxHash);
            bool isParticipated = _tournament.data.participants.Any(x => x.username == username);

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
            var count = _tournament.data.participants.Count;
            for (int i = 0; (i < count) && (i < 100); i++)
            {
                var element = _pool.Get();
                element.Init(_tournament.data.participants[i], i + 1, _GetPrizePoolShare(i + 1), _tournament.data.participants[i].username == username);
                _activeElements.Add(element);
            }
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
        _activeElements.Clear();
        _txHashBtn.onClick.RemoveListener(_OpenTournamentTxHash);
    }

    private void _JoinTournament()
    {
        GlobalEventHandler.TriggerEvent(EventID.OnToggleLoadingPanel, true);
        if (!PlayerprefsHandler.GetPlayerPrefsBool(PlayerPrefKeys.hasWallet))
        {
            Debug.LogError("Please create wallet to join tournaments");
            GlobalEventHandler.TriggerEvent(EventID.OnJoinTournamentWithoutWallet);
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
            body = "{\"tournamentId\":" + _tournamentId + "}"
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
            _endDate.text = string.Format("--D : --H : --M : --S");
            _OnTournamentEnd();
            return;
        }
        TimeSpan timeSpan = TimeSpan.FromSeconds(remainingTime);

        int days = timeSpan.Days;
        int hours = timeSpan.Hours;
        int minutes = timeSpan.Minutes;
        int seconds = timeSpan.Seconds;
        _endDate.text = string.Format("{0}D : {1}H : {2}M : {3:D2}S", days, hours, minutes, seconds);
    }
    //1728611215

    private void _OnTournamentEnd()
    {
        _fadingGroup.SetActive(true);
        _joinTournamentBtn.interactable = false;
        _joinTournamentBtn.GetComponentInChildren<TextMeshProUGUI>().SetText("Tournament Ended");

    }
    /*
      first;
      second;
      third;
      fourToTen;
      elevenToTwentyFive;
      twentySixToFifty;
      fiftyOneToHundred;
     */
    private float _GetPrizePoolShare(int rank)
    {

        if (rank >= 51 && rank <= 100) return GetShare(5f / (100f - 50f));
        if (rank >= 26 && rank <= 50) return GetShare(8f / (50f - 25f));
        if (rank >= 11 && rank <= 25) return GetShare(16f / (25f - 10f));
        if (rank >= 4 && rank <= 10) return GetShare(20f / (10f - 3f));
        if (rank == 3) return GetShare(20f);
        if (rank == 2) return GetShare(15f);
        if (rank == 1) return GetShare(25f);

        return 0; //not eligible for Prizepool
        float GetShare(float percent)
        {
            return (_prizePoolAmount * percent) / 100f;
        }
    }
    private void _SetTxHash(string txHash)
    {
        if (string.IsNullOrEmpty(txHash) || string.IsNullOrWhiteSpace(txHash)) return;
        var start = txHash.Substring(0, 4);
        var end = txHash.Substring(txHash.Length - 5, 5);
        _tournamentTxHash.SetText(start + "...." + end);
    }
    private void _OpenTournamentTxHash()
    {
        Application.OpenURL(EXPLORER_BASE_URL + "/" + _txHash);

    }
}
