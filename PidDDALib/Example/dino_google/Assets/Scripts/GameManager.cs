using System;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float initialGameSpeed = 5f;
    public float gameSpeedIncrease = 0.1f;
    public float startTime = 60f;
    public float MaxSpeed = 20f;
    public int Hits = 0;
    public int Jumps = 0; 
    public int ScoreControl = 0;
    public float Level = 1f;
    public float MaxLevel = 10f;
    public float RoundLevelUp = 3f;

    public Guid userId = Guid.Empty;

    public float gameSpeed { get; private set; }

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI hiscoreText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI hitsText;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button retryButton;

    private Player player;
    private Spawner spawner;

    private float score;
    public float hiScore;
    public float Score => score;
    private float timeRemaining;
    private int ddaType = -1;
    private int counter;
    private bool isDebugging = false;

    private int acertos = 0;
    private int erros = 0;

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) {
            Instance = null;
        }
    }

    private void Start()
    {
        startTime = 90;

        player = FindFirstObjectByType<Player>();
        spawner = FindFirstObjectByType<Spawner>();

        userId = Guid.NewGuid();

        NewGame();
    }

    public async void NewGame()
    {
        // Initialize the timer with 60 seconds at the start
        timeRemaining = startTime;

        //isDebugging = true;
        //ddaType = 1;

        if (!isDebugging)
        {
            await WebRequestController.Instance.GetBalanceBegin();
            await WebRequestController.Instance.SetBalance(userId);

            ddaType = WebRequestController.Balance;
        }

        score = 0f;
        hiScore = 0f;
        acertos = 0;
        erros = 0;
        gameSpeed = initialGameSpeed;
        enabled = true;
        Hits = 0;
        Jumps = 0;
        ScoreControl = 0;
        Level = 1;
        counter = 0;

        Obstacle[] obstacles = FindObjectsByType<Obstacle>(FindObjectsSortMode.None);
            

        foreach (var obstacle in obstacles)
        {
            Destroy(obstacle.gameObject);
        }

        player.gameObject.SetActive(true);
        spawner.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);

        if (!isDebugging)
        {
            await WebRequestController.Instance.SetGameplay(userId, Jumps, gameSpeed, (startTime - timeRemaining),
                                                "new_game", gameSpeed, counter, Hits,
                                                Score, Level, hiScore, 0f);
        }

        UpdateHiscore();
    }

    public async void GameOver()
    {     
        gameSpeed = 0f;
        enabled = false;
        /*

        player.gameObject.SetActive(false);
        spawner.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);
     
        UpdateHiscore();
        */

        if (!isDebugging)
        {
            await WebRequestController.Instance.SetGameplay(userId, Jumps, gameSpeed, (startTime - timeRemaining),
                                                "end_game", gameSpeed, counter, Hits,
                                                Score, Level, hiScore, SpeedPIDController.PidController.Setpoint);
        }

        UpdateHiscore();
        SceneManager.LoadScene("GameOver");
     }

    public async void Hint()
    {
        Hits += 1;
        ScoreControl += 1;
        erros += 1;
        counter++;

        hitsText.text = Mathf.FloorToInt(Hits).ToString("D5");

        CalcSpeed(ScoreControl, false);

        if (!isDebugging)
        {
            await WebRequestController.Instance.SetGameplay(userId, Jumps, gameSpeed, (startTime - timeRemaining),
                                                "gameplay_hit", gameSpeed, counter, Hits,
                                                Score, Level, hiScore, SpeedPIDController.PidController.Setpoint);
        }

        UpdateHiscore();
        UptadeSpawner();
        score = 0f;
    }

    public void UptadeSpawner()
    {
        spawner.spawnRate = (spawner.maxSpawnRate / gameSpeed) * initialGameSpeed;        
    }

    public async void Point()
    {
        Jumps += 1;
        ScoreControl -= 1;
        acertos += 1;
        counter++;

        pointsText.text = Mathf.FloorToInt(Jumps).ToString("D5");

        CalcSpeed(ScoreControl, true);

        if (!isDebugging)
        {
            await WebRequestController.Instance.SetGameplay(userId, Jumps, gameSpeed, (startTime - timeRemaining),
                                                "gameplay_point", gameSpeed, counter, Hits,
                                                Score, Level, hiScore, SpeedPIDController.PidController.Setpoint);
        }

        UptadeSpawner();
    }

    void CalcSpeed(float value, bool score)
    {   
        if (Jumps > Hits) {
            Level = (Convert.ToInt32(Math.Abs(Jumps - Hits) / 3)) + 1;

            if (Level > MaxLevel)
            {
                Level = MaxLevel;
            }
        }
        else
        {
            Level = 1;
        }

        var diff = Math.Abs(acertos - erros);        
        if (diff >= RoundLevelUp)
        {   
            if (ddaType == 1)
            {
                var newSetPointVariation = Level / MaxLevel * Convert.ToInt32(Math.Abs(Jumps - Hits)) + initialGameSpeed;
                SpeedPIDController.PidController.Setpoint = newSetPointVariation;                
            }            

            acertos = 0; 
            erros = 0;
        }

        if (ddaType == 0)
        {   
            if(Jumps > Hits)
            {
                gameSpeed = Level / MaxLevel * (Math.Abs(value)) + initialGameSpeed;
            }
            else{
                gameSpeed = initialGameSpeed;
            }
        }
        else if (ddaType == 1)
        {
            var newVariation = Level / MaxLevel * (Math.Abs(value));
            //SpeedPIDController.inputVariable = (score ? (-1* (acertos + 1)) : (erros + 1));
            SpeedPIDController.inputVariable = (score ? (newVariation * -1) : newVariation);
            var newSpeed = SpeedPIDController.outputVariable;
            gameSpeed = (newSpeed < initialGameSpeed ? initialGameSpeed : newSpeed);
            print("-----------------New Speed:  " + Mathf.FloorToInt(gameSpeed).ToString("F5"));
        }
    }
                         
    private void Update()
    {        
        if (ddaType > -1) { 
            score += (gameSpeed * Time.deltaTime)*1.5f;
        
            scoreText.text = Mathf.FloorToInt(score).ToString("D5");

            // Subtract time passed since last frame (deltaTime) from the timer
            if (timeRemaining > 0f)
            {
                timeRemaining -= Time.deltaTime;
                timerText.text = timeRemaining.ToString("F0");
            }
            else
            {
                // Ensure the timer stops at 0 and handle the end of the countdown
                timeRemaining = 0f;
                timerText.text = "End!";
                GameOver();
            }

            if (SpeedPIDController.PidController.Setpoint < 5)
            {
                Level = 1;            
            }
            levelText.text = Mathf.FloorToInt(Level).ToString("D5");
        }
        else
        {
            timerText.text = "Wait!";
        }
    }

    private void UpdateHiscore()
    {
        if (score > hiScore)
        {
            hiScore = score;
            score = 0f;
        }

        hiscoreText.text = Mathf.FloorToInt(hiScore).ToString("D5");
    }

}
