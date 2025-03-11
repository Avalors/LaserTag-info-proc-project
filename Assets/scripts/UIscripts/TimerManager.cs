using UnityEngine;
using Alteruna;
using TMPro;
using System.Collections;
using System;

public class TimerManager : AttributesSync
{

    public static TimerManager Instance;

    public TextMeshProUGUI timerText;
    [SynchronizableField] private float timer = 180f; //3 minute timer
    private bool isRunning = false;

    public Multiplayer _multiplayer; //assign in inspector (multiplayer network manager)

    void Awake()
    {
        if(Instance == null){
            Instance = this;
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
        GameOver();
    }

    [SynchronizableMethod]
    private void GameOver(){
        Debug.Log("GAME OVER"); //IMPLEMENT FUNCTIONALITY
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

}
