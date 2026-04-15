using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("AudioClips")]
    [SerializeField] private AudioClip[] music;
    [SerializeField] private AudioClip enemyKillledSfx;
    [SerializeField] private AudioClip civilianKillledSfx;
    [SerializeField] private AudioClip playerHurtSfx;
    [SerializeField] private AudioClip playerDiedSfx;
    [SerializeField] private AudioClip playerShootM1Sfx;
    [SerializeField] private AudioClip playerShootM2Sfx;
    [SerializeField] private AudioClip levelCompletedSfx;
    [SerializeField] private AudioClip victorySfx;
    [SerializeField] private AudioClip gameOverSfx;
    [SerializeField] private AudioClip enemyShootSfx;

    [SerializeField] private AudioClip buttonClickedAudio;

    [Header("AudioSources")]
    [SerializeField] private AudioSource audioSourceMusic;
    [SerializeField] private AudioSource audioSourceSfx;

    private int lastMusicIndex = -1;

    private void Awake()
    {
        FsmManager.onEnemyShoot += FsmManager_onEnemyShoot; ;
        LevelManager.onLevelFinished += LevelManager_onLevelFinished;
        LevelManager.onGameOver += LevelManager_onGameOver;
        PlayerController.onPlayerDied += PlayerMovement_playerDied;
        PlayerController.onPlayerHurt += PlayerController_onPlayerHurt;
        PlayerController.onPlayerShootM1 += PlayerController_onPlayerShootM1;
        PlayerController.onPlayerShootM2 += PlayerController_onPlayerShootM2;
        UIButton.onButtonClicked += UIButton_onButtonClicked;
        FsmManager.onEnemyDied += EnemyController_onEnemyDie;
        FsmManager.onCivilianDied += FsmManager_onCivilianDied; 
    }

    private void LevelManager_onGameOver(bool won)
    {
        audioSourceMusic.Stop();
        if (won)
        {
            audioSourceSfx.PlayOneShot(victorySfx);
        }
        else
        {
            audioSourceSfx.PlayOneShot(gameOverSfx);
        }
    }

    private void Start()
    {
        PlayRandomMusic();
    }

    private void Update()
    {
        if (!audioSourceMusic.isPlaying)
        {
            PlayRandomMusic();
        }
    }
    private void OnDestroy()
    {
        FsmManager.onEnemyShoot -= FsmManager_onEnemyShoot;
        PlayerController.onPlayerDied -= PlayerMovement_playerDied;
        PlayerController.onPlayerHurt -= PlayerController_onPlayerHurt;
        PlayerController.onPlayerShootM1 -= PlayerController_onPlayerShootM1;
        PlayerController.onPlayerShootM2 -= PlayerController_onPlayerShootM2;
        UIButton.onButtonClicked -= UIButton_onButtonClicked;
        FsmManager.onEnemyDied -= EnemyController_onEnemyDie;
        FsmManager.onCivilianDied -= FsmManager_onCivilianDied;
    }

    private void LevelManager_onLevelFinished()
    {
        audioSourceSfx.PlayOneShot(levelCompletedSfx);
    }

    private void FsmManager_onEnemyShoot()
    {
        audioSourceSfx.PlayOneShot(enemyShootSfx);
    }

    private void PlayerController_onPlayerHurt()
    {
        audioSourceSfx.PlayOneShot(playerHurtSfx);
    }

    private void PlayerController_onPlayerShootM2()
    {
        audioSourceSfx.PlayOneShot(playerShootM2Sfx);
    }

    private void PlayerController_onPlayerShootM1()
    {
        audioSourceSfx.PlayOneShot(playerShootM1Sfx);
    }

    private void EnemyController_onEnemyDie()
    {
        audioSourceSfx.PlayOneShot(enemyKillledSfx);
    }

    private void UIButton_onButtonClicked()
    {
        audioSourceSfx.PlayOneShot(buttonClickedAudio);
    }

    private void PlayerMovement_playerDied()
    {
        audioSourceSfx.PlayOneShot(playerDiedSfx);
    }

    private void FsmManager_onCivilianDied()
    {
        audioSourceSfx.PlayOneShot(civilianKillledSfx);
    }

    public void ReproduceClip(AudioClip audioClip)
    {
        audioSourceSfx.PlayOneShot(audioClip);
    }
    private void PlayRandomMusic()
    {
        if (music.Length == 0) return;

        int randomIndex;

        do
        {
            randomIndex = Random.Range(0, music.Length);
        }
        while (randomIndex == lastMusicIndex && music.Length > 1);

        lastMusicIndex = randomIndex;

        audioSourceMusic.clip = music[randomIndex];
        audioSourceMusic.Play();
    }
}