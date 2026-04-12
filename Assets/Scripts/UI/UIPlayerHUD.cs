using TMPro;
using UnityEngine;

public class UIPlayerHUD : MonoBehaviour
{
    [SerializeField] private GameDataSO gameDataSO;
    [SerializeField] private TextMeshProUGUI speedMeter;
    [SerializeField] private TextMeshProUGUI pointsTxt;
    [SerializeField] private TextMeshProUGUI enemiesLeftTxt;
    [SerializeField] private GameObject levelFinishedCanvas;
    [SerializeField] private PlayerController playerController;

    int totalPoints;

    private void Awake()
    {
        LevelManager.onLevelFinished += LevelManager_onLevelFinished;
        FsmManager.onCivilianDied += FsmManager_onCivilianDied;
        FsmManager.onEnemyDied += FsmManager_onEnemyDied;
    }

    void Start()
    {
        totalPoints = 0;
        enemiesLeftTxt.text = gameDataSO.EnemiesLeft.ToString();
        pointsTxt.text = totalPoints.ToString();
    }

    void Update()
    {
        speedMeter.text = playerController.CurrentSpeed().ToString("0");
    }

    private void OnDestroy()
    {
        LevelManager.onLevelFinished -= LevelManager_onLevelFinished;
        FsmManager.onCivilianDied -= FsmManager_onCivilianDied;
        FsmManager.onEnemyDied -= FsmManager_onEnemyDied;
    }

    private void LevelManager_onLevelFinished()
    {
        gameDataSO.CurrentScore = totalPoints;
        levelFinishedCanvas.SetActive(true);
    }

    private void FsmManager_onEnemyDied()
    {
        totalPoints += 10;
        enemiesLeftTxt.text = gameDataSO.EnemiesLeft.ToString();
        pointsTxt.text = totalPoints.ToString();
    }

    private void FsmManager_onCivilianDied()
    {
        totalPoints -= 10;
        pointsTxt.text = totalPoints.ToString();
    }
}
