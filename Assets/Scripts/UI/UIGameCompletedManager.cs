using UnityEngine;

public class UIGameCompletedManager : MonoBehaviour
{
    [SerializeField] private GameDataSO gameDataSO;
    [SerializeField] private GameObject gameFinishedCanvas;
    [SerializeField] private GameObject gameOverCanvas;

    private void Awake()
    {
        LevelManager.onGameOver += LevelManager_onGameOver;
    }

    private void OnDestroy()
    {
        LevelManager.onGameOver -= LevelManager_onGameOver;
    }

    private void LevelManager_onGameOver(bool won)
    {
        if (won)
        {
            Debug.Log(won);
            gameFinishedCanvas.SetActive(true);
        }

        if (!won)
        {
            gameOverCanvas.SetActive(true);
        }
    }
}
