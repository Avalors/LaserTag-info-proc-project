using UnityEngine;
using Alteruna;
using TMPro;
using System.Collections;
using System;
using Unity.VisualScripting;

public class TimerManager : AttributesSync
{

    public static TimerManager Instance;

    private PlayerMovement _player;

    [Header("UI elements")]

    public TextMeshProUGUI timerText;

    public GameObject GameOverUi;

    public GameObject HUD;

    public GameObject YouWin;

    public GameObject YouLose;

    public GameObject Tie;

    public TextMeshProUGUI MyKills;

    public TextMeshProUGUI MyDeaths;

    public TextMeshProUGUI ScoreBoardKills;

    public TextMeshProUGUI ScoreBoardEnemyKills;

    [SynchronizableField] private float timer = 180f; //3 minute timer
    private bool isRunning = false;

    [Header("Multiplayer Manager")]
    private Multiplayer _multiplayer; //assign in inspector (multiplayer network manager)

    void Awake()
    {
        if(Instance == null){
            Instance = this;
        }
    }

    void Start()
    {
        TryFindPlayerMove();
    }

    void Update()
    {
        if(_player == null){
            TryFindPlayerMove();
        }
    }

    public void OnPlayerJoined(Multiplayer multiplayer, Room room, User user)
    {
        if(room.Users.Count == room.MaxUsers){
            if(!isRunning){
                StartCoroutine(StartCountdown());
            }
        }
        
    }
    private IEnumerator StartCountdown(){
        if(isRunning) yield break;
        isRunning = true;

        float syncInterval = 0.5f;
        float nextSyncTime = Time.time + syncInterval;

        while(timer > 0){
            timer -= Time.deltaTime;
            UpdateTimerUI(timer);

            if(Time.time >= nextSyncTime){
                BroadcastRemoteMethod("SyncTimer", timer);
                nextSyncTime = Time.time + syncInterval;
            }
            
            yield return null;
        }

        isRunning = false;
        timer = 0;
        BroadcastRemoteMethod("SyncTimer", timer);
        BroadcastRemoteMethod("GameOver");
    }

    [SynchronizableMethod]
    private void GameOver(){
        //fix player position
        Rigidbody rb = _player.GetComponent<Rigidbody>();
        Transform playerTransform = rb.transform;
        PlayerShoot _playerShoot = _player.GetComponentInChildren<PlayerShoot>();

        playerTransform.position = _playerShoot.spawnPoint;

        rb.constraints |= RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;

        //set GameOverUI
        HUD.SetActive(false);
        GameOverUi.SetActive(true);

        MyKills.text = ScoreBoardKills.text;
        MyDeaths.text = ScoreBoardEnemyKills.text;

        int Kills = int.Parse(MyKills.text);
        int Deaths = int.Parse(MyDeaths.text);

        if(Kills < Deaths){
            YouLose.SetActive(true);
        }
        else if(Kills > Deaths){
            YouWin.SetActive(true);
        }
        else{
            Tie.SetActive(true);
        }

    }




    [SynchronizableMethod]
    public void SyncTimer(float newtime){
        timer = newtime;
        UpdateTimerUI(timer);
    }  

    private void UpdateTimerUI(float time){
        int minutes = Mathf.FloorToInt(time/60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0:00}:{1:00}",minutes,seconds);
    }

    private void TryFindPlayerMove(){
        _player = LocalPlayerManager.Instance?.playerMovement; //returns null if not found
    }

}
