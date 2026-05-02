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

    private const string MasterVolumePrefKey = "MasterVolume";

    private AudioSource musicSource;
    private AudioSource sfxSource;
    private float masterVolume = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (PlayerPrefs.HasKey(MasterVolumePrefKey))
            masterVolume = PlayerPrefs.GetFloat(MasterVolumePrefKey);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        ApplyMusicVolume();

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = 1f;
    }

    /// <summary>Overall volume multiplier (0–1). Persisted under MasterVolume.</summary>
    public void SetMasterVolume(float normalized)
    {
        masterVolume = Mathf.Clamp01(normalized);
        PlayerPrefs.SetFloat(MasterVolumePrefKey, masterVolume);
        PlayerPrefs.Save();
        ApplyMusicVolume();
    }

    public float MasterVolume => masterVolume;

    private void ApplyMusicVolume()
    {
        if (musicSource != null)
            musicSource.volume = musicVolume * masterVolume;
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
        ApplyMusicVolume();
        musicSource.Play();
    }

    private void PlaySfx(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
    }
}
