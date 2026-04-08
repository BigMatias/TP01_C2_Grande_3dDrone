using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIGameOver : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Button yesBtn;
    [SerializeField] private Button noBtn;

    private void Awake()
    {
        yesBtn.onClick.AddListener(OnYesBtnClicked);
        noBtn.onClick.AddListener(OnNoBtnClicked);
        PlayerController.onPlayerDied += PlayerMovement_playerDied;
    }

    private void OnDestroy()
    {
        yesBtn.onClick.RemoveListener(OnYesBtnClicked);
        noBtn.onClick.RemoveListener(OnNoBtnClicked);
        PlayerController.onPlayerDied -= PlayerMovement_playerDied;
    }

    private void OnYesBtnClicked()
    {
        SceneManager.LoadScene("Game");
    }

    private void OnNoBtnClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void PlayerMovement_playerDied()
    {
        gameManager.PauseGame();
        gameOverPanel.gameObject.SetActive(true);
    }

}
