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
        // Warning: [Alta] - ToString("0") asigna un nuevo string cada frame → presión sostenida sobre el GC. Cachear el último valor entero y actualizar el texto sólo si cambió respecto al frame anterior.
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
        // Error: [Media] - totalPoints duplica la responsabilidad de gameDataSO.CurrentScore (LevelManager ya lleva esa cuenta). Dos fuentes de verdad → divergen fácilmente entre la UI y el SO.
        // Suggestion: [Baja] - "+10" hardcodeado. Existe gameDataSO.PointsOnKill exactamente para esto.
        totalPoints += 10;
        enemiesLeftTxt.text = gameDataSO.EnemiesLeft.ToString();
        pointsTxt.text = totalPoints.ToString();
    }

    private void FsmManager_onCivilianDied()
    {
        // Suggestion: [Baja] - Mismo problema: "-10" hardcodeado en lugar de gameDataSO.PointsReducedOnKill.
        totalPoints -= 10;
        pointsTxt.text = totalPoints.ToString();
    }
}
