using UnityEngine;
using TMPro;
public class ScoreboardManager : MonoBehaviour
{
    public TextMeshProUGUI localKillsTxt;
    public TextMeshProUGUI otherKillsTxt;

    private PlayerShoot _localPlayerShoot;
    private PlayerShoot _otherPlayerShoot;

    private void Start()
    {
        TryFindPlayersShoot();
    }

    private void Update()
    {

        if((_localPlayerShoot == null) || (_otherPlayerShoot == null)){
            TryFindPlayersShoot();
        }
        else{
            SetScoreBoard();
        }
    }

    private void TryFindPlayersShoot(){
        _localPlayerShoot = LocalPlayerManager.Instance?.playerShoot; //returns null if not found
        _otherPlayerShoot = LocalPlayerManager.Instance?.otherPlayerShoot; //returns null if not found

        if (_localPlayerShoot != null && _otherPlayerShoot != null)
        {
            Debug.Log("local and other PlayerShoot found and assigned to scoreboard");
            SetScoreBoard();
        }

    }

    public void SetScoreBoard(){
        localKillsTxt.text = _otherPlayerShoot.deaths.ToString();
        otherKillsTxt.text = _localPlayerShoot.deaths.ToString();
    }

}
