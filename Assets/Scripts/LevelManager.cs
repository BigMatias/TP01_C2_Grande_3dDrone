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
        // Warning: [Media] - CurrentLevel es float en el SO y se compara contra enteros / se usa en switch. Por imprecisión de punto flotante esto puede fallar; debería ser int.
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

        // Suggestion: [Media] - Tres cases idénticos que sólo cambian el campo del SO. Un array EnemySpawnQuantityPerLevel[] indexado por nivel elimina el switch y desbloquea agregar niveles sin tocar código.
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
            // Warning: [Baja] - Sin default case: si CurrentLevel viene en 0 (estado inicial nunca seteado) se rompe.
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
        // Bug: [Alta] - El ScriptableObject persiste su estado entre sesiones en el editor. CurrentLevel se incrementa.
        gameDataSO.CurrentLevel += 1;
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("GameScene");
    }
}
