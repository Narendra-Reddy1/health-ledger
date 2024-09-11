using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TournamentElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _rankTxt;
    [SerializeField] private TextMeshProUGUI _usernameTxt;
    [SerializeField] private TextMeshProUGUI _stepsCountTxt;
    [SerializeField] private TextMeshProUGUI _rewardTxt;
    [SerializeField] private Image _myBg;

    public void Init(Participant participant, int rank, float prizeAmount, bool mine = false)
    {
        _rankTxt.SetText(rank.ToString());
        _usernameTxt.SetText(mine ? "YOU" : participant.username);
        _stepsCountTxt.SetText(participant.steps.ToString());
        _rewardTxt.SetText(prizeAmount.ToString());
        _myBg.gameObject.SetActive(mine);
    }
}
