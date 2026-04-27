using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music Clips")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameplayMusic;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip ballSpawn;
    [SerializeField] private AudioClip backboardHit;
    [SerializeField] private AudioClip rimHit;
    [SerializeField] private AudioClip winSting;
    [SerializeField] private AudioClip loseSting;

    [Header("Behavior")]
    [Tooltip("After win/lose sting finishes, resume menu music.")]
    [SerializeField] private bool resumeMenuMusicAfterSting = true;
    [Tooltip("Extra pause (seconds) after the sting before menu music resumes.")]
    [SerializeField] private float postStingDelay = 0.25f;

    [Header("Volumes")]
    [Range(0f, 1f)] [SerializeField] private float musicVolume = 0.5f;
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume;
    }

    public void PlayMenuMusic() => PlayMusic(menuMusic);
    public void PlayGameMusic() => PlayMusic(gameplayMusic);

    public void PlayBallSpawn() => PlaySfx(ballSpawn);
    public void PlayBackboardHit() => PlaySfx(backboardHit);
    public void PlayRimHit() => PlaySfx(rimHit);

    public void PlayWin()
    {
        StopMusic();
        PlaySfx(winSting);
        ScheduleMenuMusicAfter(winSting);
    }

    public void PlayLose()
    {
        StopMusic();
        PlaySfx(loseSting);
        ScheduleMenuMusicAfter(loseSting);
    }

    private void ScheduleMenuMusicAfter(AudioClip stingClip)
    {
        if (!resumeMenuMusicAfterSting) return;
        if (menuMusic == null) return;

        StopAllCoroutines();
        float delay = (stingClip != null ? stingClip.length : 0f) + postStingDelay;
        StartCoroutine(PlayMusicAfterDelay(menuMusic, delay));
    }

    private IEnumerator PlayMusicAfterDelay(AudioClip clip, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        PlayMusic(clip);
    }

    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying) musicSource.Stop();
    }

    private void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    private void PlaySfx(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }
}
