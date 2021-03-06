﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Rank
{
    NORANK,
    A,
    S,
    SS
};

public class RoadManager : MonoBehaviour
{
    [SerializeField] int curCheckPoint, lastCheckPoint;
    [SerializeField] int checkPointCounter;
    [SerializeField] int lap = 1;
    [SerializeField] int totalLapsToFinish = 3;


    public GameObject VRCamera, NormalCamera;


    public CameraFollow360 cam360;

    public GameObject[] checkPoints;
    public GameObject racer;
    public PlayerController playerController;
    public GameObject semaphore;

    public Image sempaphoreLed1, sempaphoreLed2, sempaphoreLed3;

    private float timer = 0.0f; 
    [SerializeField] private int seconds;
    [SerializeField] private int trackFinishTimeByPlayer;

    [Header("Total Time to get rank A")]
    [SerializeField] private int rankAValue = 100;

    [Header("Total Time to get rank S")]
    [SerializeField] private int rankSValue = 80;

    [Header("Total Time to get rank SS")]
    [SerializeField] private int rankSSValue = 60;

    [SerializeField] GameObject raceEndUI;
    [SerializeField] Text rankFinalDisplay;

    public Text lapUI, rankUI, timerUI, bestTimeUI;

    public Rank rank;

    void OnEnable()
    {
        EventManager.onStartRace += UpdateGameStatusToRunningForEnablePauseMenu;//BeginLevel
        EventManager.onRaceFinished += CalculateRank; //EndLevel;
        
        EventManager.onRaceFinished += UpdateGameStatusToRunningForDisanablePauseMenu; //EndLevel;
    }

    void OnDisable()
    {
        EventManager.onStartRace -= UpdateGameStatusToRunningForEnablePauseMenu;//Begin level
        EventManager.onRaceFinished -= CalculateRank;//EndLevel;
        
        EventManager.onRaceFinished -= UpdateGameStatusToRunningForDisanablePauseMenu;//EndLevel;
    }

    //public GAMESTATUS isPaused { get; private set; } // Terrible I know 

    void UpdateGameStatusToRunningForEnablePauseMenu()
    {
        GameManager.Instance.gameLayerStatus = GAMESTATUS.RUNNING;
    }
    void UpdateGameStatusToRunningForDisanablePauseMenu()
    {
        GameManager.Instance.gameLayerStatus = GAMESTATUS.INIT;
    }

    void CalculateRank()
    {
        raceEndUI.SetActive(true);
        if (trackFinishTimeByPlayer < rankAValue)
        {
            print("1 star");
            rank = Rank.A;
            rankFinalDisplay.text = "A";
            rankUI.text = "A";
            if (trackFinishTimeByPlayer < rankSValue)
            {
                print("2 star");
                rankFinalDisplay.text = "S";
                rankUI.text = "S";
                rank = Rank.S;
                if (trackFinishTimeByPlayer < rankSSValue)
                {
                    print("3 star");
                    rankFinalDisplay.text = "SS";
                    rankUI.text = "SS";
                    rank = Rank.SS;
                }
            }
        }
        else
        {
            rank = Rank.NORANK;
        } 

        StartCoroutine(ShowRank());
    }

    IEnumerator ShowRank()
    {
        //Show on UI the players rank and persists it
        print("Congratulations your rank is: " + rank);
        yield return new WaitForSeconds(5f);
        SelectScene();
    }

    void SelectScene()
    {
        if (GameManager.Instance.isUsingVR)
            GameManager.Instance.ChangeState(State.SelectTrackVR);
        else
            GameManager.Instance.ChangeState(State.SelectTrack);
    }

    bool toogleClock = false;
    void TimerCounter()
    {
        timer += Time.deltaTime;
        seconds = (int)(timer % 60);
    }

    void DisplayTimerUI()
    {
        timerUI.text = timer.ToString("0.00");
    }

    [Header("Enable intro cutscene before start")]
    [SerializeField] bool enableIntro = false;
    private void Start()
    {

        curCheckPoint = 0;
        lastCheckPoint = checkPoints.Length - 1;
        raceEndUI.SetActive(false);
        EventManager.StartRace();
        cam360.GetComponent<CameraFollow360>();
        //scoreManager.GetComponent<SumScoreExample>();
        
        if (GameManager.Instance.isUsingVR)
        {
            VRCamera.SetActive(true);
            NormalCamera.SetActive(false);
        }

        if (enableIntro && !GameManager.Instance.isUsingVR)
        {
            cam360.distance = -50;
            playerController.GetComponent<PlayerController>().enabled = false;
        } else
            toogleClock = true;

    }

    private void Update()
    {
        if (enableIntro)
        {
            if (cam360.distance < 35)
            {
                cam360.distance += 8.5f * Time.deltaTime;
            }

            if (cam360.distance > 20 && !semaphoreStartCount)
            {
                semaphoreStartCount = true;
                StartCoroutine(Semaphore());
            }
        }

        if (toogleClock)
        {
            TimerCounter();
            DisplayTimerUI();
        }
    }

    bool semaphoreStartCount;
    IEnumerator Semaphore()
    {
        //scoreManager.ToggleTimed();
        //semaphoreStartCount = true;
        print("Red");
        sempaphoreLed1.color = Color.red;
        yield return new WaitForSeconds(1f);
        print("3");
        yield return new WaitForSeconds(1f);
        print("2");
        print("Yellow");
        sempaphoreLed2.color = Color.yellow;
        yield return new WaitForSeconds(1f);
        print("1");
        yield return new WaitForSeconds(1f);
        print("Green");
        sempaphoreLed3.color = Color.green;
        playerController.enabled = true;
        print("Go!!!!");
        toogleClock = true;
        yield return new WaitForSeconds(1f);
        semaphore.SetActive(false);
    }

    public void CheckPointReached()
    {
        print("Congrats you passed through the: " + (curCheckPoint + 1) + "th CheckPoint.");
        checkPointCounter += 1;
        curCheckPoint = checkPointCounter;
        if(curCheckPoint == checkPoints.Length)
        {
            lap += 1;   //Add lap
            curCheckPoint = 0;//Reset checkPoint back to 0
            checkPointCounter = 0;
            print("Lap: " + lap);
            lapUI.text = lap.ToString();
            if (lap == totalLapsToFinish)
            {
                lapUI.text = "Final Lap";
                print("Final Lap");
            }
        }

        if (lap > totalLapsToFinish)
        {
            trackFinishTimeByPlayer = seconds;
            EventManager.RaceFinished();
        }
    }



}
