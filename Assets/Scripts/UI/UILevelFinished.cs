using UnityEngine;

public class UILevelFinished : MonoBehaviour
{
    private void Awake()
    {
        LevelManager.onLevelFinished += LevelManager_onLevelFinished;
    }

    private void OnDestroy()
    {
        LevelManager.onLevelFinished -= LevelManager_onLevelFinished;
    }

    private void LevelManager_onLevelFinished()
    {
        gameObject.SetActive(true);
    }
}
