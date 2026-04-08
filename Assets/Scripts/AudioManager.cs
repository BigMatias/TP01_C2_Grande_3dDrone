using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("References")]
    [Header("AudioClips")]
    [SerializeField] private AudioClip[] music;
    [SerializeField] private AudioClip buttonClickedAudio;

    [Header("AudioSources")]
    [SerializeField] private AudioSource audioSourceMusic;
    [SerializeField] private AudioSource audioSourceSfx;

    private int lastMusicIndex = -1;

    private void Awake()
    {
        UIButton.onButtonClicked += UIButton_onButtonClicked;
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
        UIButton.onButtonClicked -= UIButton_onButtonClicked;
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

    private void UIButton_onButtonClicked()
    {
        audioSourceSfx.PlayOneShot(buttonClickedAudio);
    }

    public void ReproduceClip(AudioClip audioClip)
    {
        audioSourceSfx.PlayOneShot(audioClip);
    }
}