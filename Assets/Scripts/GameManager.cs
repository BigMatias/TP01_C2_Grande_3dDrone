using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject OptionsMenu;

    private bool gamePaused = false;

    private void Awake()
    {
        Time.timeScale = 1f;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!gamePaused)
            {
                PauseGame();
                PauseMenu.gameObject.SetActive(true);
                OptionsMenu.gameObject.SetActive(false);
            }
            else
            {
                PauseGame();
                PauseMenu.gameObject.SetActive(false);
                OptionsMenu.gameObject.SetActive(false);
            }
        }
    }

    public void PauseGame()
    {
        if (!gamePaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
            gamePaused = true;

        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
            gamePaused = false;
        }
    }

}
