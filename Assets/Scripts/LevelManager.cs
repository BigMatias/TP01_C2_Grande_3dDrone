using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameDataSO gameDataSO;
    [SerializeField] private CitizenDataSO citizenDataSO;
    [SerializeField] private GameManager gameManager;

    private Coroutine waitAndNextLevelCoroutine;

    public static event Action onLevelFinished;
    public static event Action<bool> onGameOver;

    private void Awake()
    {
        FsmManager.onEnemyDied += FsmManager_onEnemyDied;
        FsmManager.onCivilianDied += FsmManager_onCivilianDied;
        PlayerController.onPlayerDied += PlayerController_onPlayerDied;
    }

    void Start()
    {
        if (gameDataSO.CurrentLevel <= 1)
        {
            gameDataSO.TotalScore = 0;
        }

        gameDataSO.CurrentScore = 0;

        if (gameDataSO.CurrentLevel >= 4)
        {
            gameManager.PauseGame();
            onGameOver?.Invoke(true);
        }

        switch (gameDataSO.CurrentLevel)
        {
            case 1:
                {
                    gameDataSO.EnemiesNeededToKill = citizenDataSO.EnemySpawnQuantityLevel1;
                    break;
                }
            case 2:
                {
                    gameDataSO.EnemiesNeededToKill = citizenDataSO.EnemySpawnQuantityLevel2;
                    break;
                }
            case 3:
                {
                    gameDataSO.EnemiesNeededToKill = citizenDataSO.EnemySpawnQuantityLevel3;
                    break;
                }
        }
        gameDataSO.EnemiesLeft = gameDataSO.EnemiesNeededToKill;
    }

    private void OnDestroy()
    {
        FsmManager.onEnemyDied -= FsmManager_onEnemyDied;
        FsmManager.onCivilianDied -= FsmManager_onCivilianDied;
        PlayerController.onPlayerDied -= PlayerController_onPlayerDied;

        if (waitAndNextLevelCoroutine != null)
            StopCoroutine(waitAndNextLevelCoroutine);
    }

    private void FsmManager_onEnemyDied()
    {
        gameDataSO.EnemiesLeft -= 1;
        gameDataSO.CurrentScore += gameDataSO.PointsOnKill;
        if (gameDataSO.EnemiesLeft <= 0)
        {
            waitAndNextLevelCoroutine = StartCoroutine(WaitAndNextLevel());
        }
    }

    private void FsmManager_onCivilianDied()
    {
        gameDataSO.CurrentScore -= gameDataSO.PointsReducedOnKill;
    }

    private void PlayerController_onPlayerDied()
    {
        onGameOver?.Invoke(false);
    }

    private IEnumerator WaitAndNextLevel()
    {
        onLevelFinished?.Invoke();
        gameDataSO.TotalScore += gameDataSO.CurrentScore;
        gameDataSO.CurrentLevel += 1;
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("GameScene");
    }
}
