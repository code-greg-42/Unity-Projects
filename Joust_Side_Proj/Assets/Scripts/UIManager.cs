using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Image hitMarker;
    public Image enemyHealthBar;
    public GameObject TargetSpawner;
    public Text gameStatus;
    public Text ammoText;
    public int startingEnemyCount = 10;
    private float hitMarkerTimer = 1.0f;
    private float healthBarTimer = 5.0f;
    private float playerScore = 0.0f;
    private float gameClock = 30.0f;
    private int currentLevel = 1;
    private int numberOfEnemiesForLevel = 10;
    private int enemyIncreasePerLevel = 5;
    private float gameClockIncreasePerLevel = 5.0f;
    private float startGameClock = 30.0f;
    private int currentNumberOfEnemies = 10;
    private int startLevel = 1;
    private float playerAccuracy = 0.0f;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        currentLevel = startLevel;
        numberOfEnemiesForLevel = startingEnemyCount;
        gameClock = startGameClock;
        TargetSpawner.GetComponent<TargetSpawner>().SpawnEnemies(numberOfEnemiesForLevel);
    }

    public void ShowHitMarker()
    {
        hitMarker.fillAmount = 1;
        hitMarkerTimer = 0.25f;
    }
    private void Update()
    {
        if (hitMarker.fillAmount > 0)
        {
            hitMarker.fillAmount -= Time.deltaTime / hitMarkerTimer;
        }
        if (healthBarTimer <= 0) {
            HideEnemyHealthBar();
            healthBarTimer = 5.0f;
        }
        UpdateGameUIComponents();
        gameClock -= Time.deltaTime;
        if (gameClock <= 0.0f) {
            playerScore += 100 * currentLevel;
            currentLevel++;
            numberOfEnemiesForLevel += enemyIncreasePerLevel;
            gameClock = startGameClock + (gameClockIncreasePerLevel * currentLevel);
            TargetSpawner.GetComponent<TargetSpawner>().SpawnEnemies(numberOfEnemiesForLevel);
            UpdateGameUIComponents();
        }
    }
    public void UpdateEnemyHealthBar(float health)
    {
        Debug.Log("Enemy Health: " + health.ToString());
        // enemyHealthBar.fillAmount = health;
    }
    public void HideEnemyHealthBar()
    {
        enemyHealthBar.fillAmount = 0;
    }
    public void UpdateEnemyUIComponents(float health, float maxHealth) {
        UpdateEnemyHealthBar(health / maxHealth);
        healthBarTimer -= Time.deltaTime;
    }
    public void UpdateGameUIComponents() {
    string levelText = "Round: " + currentLevel.ToString();
    string enemyText = "Enemies: " + currentNumberOfEnemies.ToString() + "/" + numberOfEnemiesForLevel.ToString();
    string clockText = "Clock: " + Mathf.RoundToInt(gameClock).ToString() + "/" + Mathf.RoundToInt(startGameClock + (gameClockIncreasePerLevel * currentLevel)).ToString();
    string scoreText = "Score: " + playerScore.ToString();
    string accuracyText = "Accuracy: " + Mathf.RoundToInt(playerAccuracy * 100f).ToString() + "%";
    gameStatus.text = levelText + "\n" + enemyText + "\n" + clockText + "\n" + scoreText + "\n" + accuracyText;
    }
    public void AddScore(float score) {
        playerScore += score;
    }
    public void UpdateGunComponents(float totalHits, float totalShots, int currentAmmo, int maxAmmo) {
        playerAccuracy = totalHits / totalShots;
        ammoText.text = currentAmmo.ToString() + "/" + maxAmmo.ToString();
    }
}
