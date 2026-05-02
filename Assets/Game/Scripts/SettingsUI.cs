using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Hook two UI Sliders (whole-number min/max in the inspector) to mouse look sensitivity and master volume.
/// Assign the sliders in the inspector or add this next to your pause/start menu panel.
/// </summary>
public class SettingsUI : MonoBehaviour
{
    [Header("Sliders (0 = left, 1 = right)")]
    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private Slider volumeSlider;

    [Header("Mouse sensitivity range")]
    [SerializeField] private float minSensitivity = 0.5f;
    [SerializeField] private float maxSensitivity = 8f;

    [Header("Optional references (auto-found if empty)")]
    [SerializeField] private PlayerController player;
    [SerializeField] private AudioManager audioManager;

    private const string VolPrefKey = "MasterVolume";

    void Awake()
    {
        if (player == null) player = FindObjectOfType<PlayerController>();
        if (audioManager == null) audioManager = AudioManager.Instance != null ? AudioManager.Instance : FindObjectOfType<AudioManager>();
    }

    void Start()
    {
        if (mouseSensitivitySlider != null)
        {
            mouseSensitivitySlider.minValue = 0f;
            mouseSensitivitySlider.maxValue = 1f;
            mouseSensitivitySlider.wholeNumbers = false;

            float sens = player != null ? player.MouseSensitivity : 2f;
            float t = Mathf.InverseLerp(minSensitivity, maxSensitivity, sens);
            mouseSensitivitySlider.SetValueWithoutNotify(Mathf.Clamp01(t));
            mouseSensitivitySlider.onValueChanged.AddListener(OnMouseSensitivitySliderChanged);
        }

        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.wholeNumbers = false;

            float vol = PlayerPrefs.HasKey(VolPrefKey) ? PlayerPrefs.GetFloat(VolPrefKey) : 1f;
            if (audioManager != null) vol = audioManager.MasterVolume;
            volumeSlider.SetValueWithoutNotify(Mathf.Clamp01(vol));
            volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);
        }
    }

    void OnDestroy()
    {
        if (mouseSensitivitySlider != null)
            mouseSensitivitySlider.onValueChanged.RemoveListener(OnMouseSensitivitySliderChanged);
        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveListener(OnVolumeSliderChanged);
    }

    private void OnMouseSensitivitySliderChanged(float slider01)
    {
        float sens = Mathf.Lerp(minSensitivity, maxSensitivity, slider01);
        if (player != null)
            player.SetMouseSensitivity(sens);
    }

    private void OnVolumeSliderChanged(float normalized)
    {
        if (audioManager != null)
            audioManager.SetMasterVolume(normalized);
    }
}
