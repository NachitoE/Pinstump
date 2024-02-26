using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public PlayerInput pInput;
    
    [Header("Gameplay")]
    public int lives;
    private int defaultLives;
    public GameObject ball;
    private BallController ballC;
    private Vector2 initialBallPos;
    public GameObject freeBallTMPGO;
    public int chanceToGetFreeBall;
    private AudioManager audioM;

    [Header("Flipper Actions")]
    private InputAction leftFlipperAction;
    private InputAction rightFlipperAction;
    [Header("Flippers")]
    public Action onLeftFlipperPerformed;
    public Action onRightFlipperPerformed;
    public Action onLeftFlipperReleased;
    public Action onRightFlipperReleased;

    [Header("Plunger Actions")]
    private InputAction plungerAction;
    [Header("Plunger")]
    public Action onPlungerPerformed;
    public Action onPlungerReleased;

    [Header("Points")]
    public int points;
    private TextMeshProUGUI pointsTMP;
    public Action onPointsChanged;

    [Header("Enemy Spawners")]
    public Action onEnemySpawnSignal;
    public Action onStopEnemySpawnSignal;
    private float enemyZoneTimeForFullRotation;
    public float EnemyZoneTimeForFullRotation
    {
        get
        {
            return enemyZoneTimeForFullRotation;
        }
        set
        {
            enemyZoneTimeForFullRotation = value;
            onEnemyZoneTimeForFullRotationChanged?.Invoke();

        }
    }
    public Action onEnemyZoneTimeForFullRotationChanged;
    public int pointsForEnemySpawn;

    [Header("Ball camera")]
    public GameObject ballCameraContainer;
    private Vector2 ballPosInCamera;

    [Header("UI")]
    public GameObject pauseMenu;
    public InputAction pauseAction;
    public GameObject gameOverMenu;
    public Action onGameOver;
    public GameObject[] restartButtonsGO;
    public List<Button> restartButtonsB = new();
    public TextMeshProUGUI gameOverPointsTMP;
    private Scrollbar SFXScrollbar;
    private Scrollbar musicScrollbar;

    private void Awake()
    {

        instance = this;
        
        //assign initial lives value
        defaultLives = lives;
        //assign InputActions to Actions
        leftFlipperAction = pInput.actions["LeftFlipper"];
        rightFlipperAction = pInput.actions["RightFlipper"];
        plungerAction = pInput.actions["Plunger"];
        pauseAction = pInput.actions["Pause"];

        //flippers
        leftFlipperAction.performed += (x) => onLeftFlipperPerformed.Invoke();
        rightFlipperAction.performed += (x) => onRightFlipperPerformed.Invoke();
        leftFlipperAction.canceled += (x) => onLeftFlipperReleased.Invoke();
        rightFlipperAction.canceled += (x) => onRightFlipperReleased.Invoke();

        //plunger
        plungerAction.performed += (x) => onPlungerPerformed.Invoke();
        plungerAction.canceled += (x) => onPlungerReleased.Invoke();

        //ball
        ballCameraContainer = GameObject.FindGameObjectWithTag("BallCameraContainer");
        ball = GameObject.FindGameObjectWithTag("Ball");
        initialBallPos = ball.transform.position;
        ballC = ball.GetComponent<BallController>();

        //points
        pointsTMP = GameObject.FindGameObjectWithTag("PointsTMP").GetComponent<TextMeshProUGUI>();
        freeBallTMPGO = GameObject.FindGameObjectWithTag("FreeBallTMP");
        freeBallTMPGO.SetActive(false);

        //enemy
        pointsForEnemySpawn = UnityEngine.Random.Range(10, 20);
        onPointsChanged += CheckEnemySpawning;

        //UI
        restartButtonsGO = GameObject.FindGameObjectsWithTag("RestartButton");
        foreach (GameObject GO in restartButtonsGO) restartButtonsB.Add(GO.GetComponent<Button>());
        foreach (Button B in restartButtonsB) B.onClick.AddListener(GameRestart);

        SFXScrollbar = GameObject.FindGameObjectWithTag("SFXScrollbar").GetComponent<Scrollbar>();
        musicScrollbar = GameObject.FindGameObjectWithTag("MusicScrollbar").GetComponent<Scrollbar>();
        SFXScrollbar.onValueChanged.AddListener(ChangeSFXVol);
        musicScrollbar.onValueChanged.AddListener(ChangeMusicVol);

        gameOverPointsTMP = GameObject.FindGameObjectWithTag("GameOverPointsTMP").GetComponent<TextMeshProUGUI>();
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        pauseMenu.SetActive(false);
        pauseAction.performed += TogglePause;
        gameOverMenu = GameObject.FindGameObjectWithTag("GameOverMenu");
        gameOverMenu.SetActive(false);
        onGameOver += ToggleGameOver;
        


    }
    private void Start()
    {
        audioM = AudioManager.instance;
    }

    private void FixedUpdate()
    {
        UpdateBallCameraContainer();

        if(ballPosInCamera.y < 0) //if ball falls off the map
        {
            ReseatBall();
        }


    }
    private void UpdateBallCameraContainer()
    {
        ballPosInCamera = Camera.main.WorldToViewportPoint(ball.gameObject.transform.position);
        if (ballPosInCamera.x > 1 || ballPosInCamera.x < 0 || ballPosInCamera.y > 1.5 || ballPosInCamera.y < 0)
        {
            ballCameraContainer.SetActive(true);
        }
        else ballCameraContainer.SetActive(false);
    }

    private void ReseatBall()
    {
        if(lives > 0)
        {
            lives--;
            ballC.Restart();
            //AUDIO
            int num = UnityEngine.Random.Range(0, 100);
            if(num <= chanceToGetFreeBall)
            {
                lives++;
                StartCoroutine(ShowFreeBallText());
                Debug.Log("FREE BALL!");
            }

            Debug.Log(initialBallPos);
            ball.transform.position = initialBallPos;
        }else onGameOver?.Invoke();
        
    }

    private void GameRestart()
    {

        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        freeBallTMPGO.SetActive(false);
        Time.timeScale = 1;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies) Destroy(enemy.gameObject);
        onStopEnemySpawnSignal?.Invoke();
        lives = defaultLives;
        points = 0;
        pointsTMP.text = points.ToString();
        ballC.Restart();
        ball.transform.position = initialBallPos;

    }
    private void TogglePause(InputAction.CallbackContext ctx)
    {
        if (!pauseMenu.activeSelf)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }
    }
    private void ToggleGameOver()
    {
        if (!gameOverMenu.activeSelf)
        {
            gameOverPointsTMP.text = "Points: " + points.ToString();
            gameOverMenu.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            gameOverMenu.SetActive(false);
            Time.timeScale = 1;
        }
    }
    public void SumPoints(int p)
    {
        points += p;
        Debug.Log(points);
        pointsTMP.text = points.ToString();
        onPointsChanged?.Invoke();
    }

    private void CheckEnemySpawning() //checks if it should spawn enemies
    {
        if(points == pointsForEnemySpawn)
        {
            onEnemySpawnSignal?.Invoke();
            pointsForEnemySpawn += UnityEngine.Random.Range(15, 25);
        }
        
    }

    private void ChangeSFXVol(float val)
    {
        audioM.SFXSource.volume = val;
        Debug.Log(val);
    }

    private void ChangeMusicVol(float val)
    {
        audioM.MusicSource.volume = val;
    }
    IEnumerator ShowFreeBallText()
    {
        freeBallTMPGO.SetActive(true);
        //AUDIO
        yield return new WaitForSeconds(2);
        freeBallTMPGO.SetActive(false);
    }

}

 

