using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIPauseMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] private Button resumeBtn;
    [SerializeField] private Button optionsBtn;
    [SerializeField] private Button exitBtn;

    private void Awake()
    {
        resumeBtn.onClick.AddListener(OnResumeBtnClicked);
        optionsBtn.onClick.AddListener(OnOptionsBtnClicked);
        exitBtn.onClick.AddListener(OnExitBtnClicked);
    }

    private void OnDestroy()
    {
        resumeBtn.onClick.RemoveListener(OnResumeBtnClicked);
        optionsBtn.onClick.RemoveListener(OnOptionsBtnClicked);
        exitBtn.onClick.RemoveListener(OnExitBtnClicked);
    }

    private void OnResumeBtnClicked()
    {
        gameObject.SetActive(false);
        gameManager.PauseGame();
    }

    private void OnOptionsBtnClicked()
    {
        gameObject.SetActive(false);
        optionsMenu.gameObject.SetActive(true);
    }

    private void OnExitBtnClicked()
    {
        // Bug: [Alta] - "MainMenuScene" acá vs "MainMenu" en UIGameOver.cs: uno de los dos crashea con "Scene couldn't be loaded" según cómo esté nombrada la escena en Build Settings. Magic strings sin centralizar = error garantizado.
        SceneManager.LoadScene("MainMenuScene");
    }


}
