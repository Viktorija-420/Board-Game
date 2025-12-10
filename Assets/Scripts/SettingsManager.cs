using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Dropdown resolutionDropdown;
    // public Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Button resetButton;
    public Button backButton;

    [Header("Audio")]
    public AudioMixer masterMixer; // assign MasterMixer
    // parameter names exposed in AudioMixer
    public string musicParam = "BGMVolume";
    public string sfxParam = "SFXVolume";

    private Resolution[] availableResolutions;

    // PlayerPrefs keys
    private const string PREF_WIDTH = "pref_width";
    private const string PREF_HEIGHT = "pref_height";
    private const string PREF_FULLSCREEN = "pref_fullscreen";
    private const string PREF_BGM = "pref_bgm";
    private const string PREF_SFX = "pref_sfx";

    void Start()
    {
        // populate resolution dropdown
        availableResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        int currentResolutionIndex = 0;
        var options = availableResolutions
            .Select(r => $"{r.width} x {r.height} @{r.refreshRate}Hz")
            .ToList();

        resolutionDropdown.AddOptions(options.ToList());

        // Load saved settings or set defaults
        LoadSettings();

        // UI listeners
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);
        bgmSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        resetButton.onClick.AddListener(ResetToDefaults);
        backButton.onClick.AddListener(OnBackPressed);
    }

    void LoadSettings()
    {
        // Resolution
        if (PlayerPrefs.HasKey(PREF_WIDTH) && PlayerPrefs.HasKey(PREF_HEIGHT))
        {
            int w = PlayerPrefs.GetInt(PREF_WIDTH);
            int h = PlayerPrefs.GetInt(PREF_HEIGHT);
            bool matched = false;
            for (int i = 0; i < availableResolutions.Length; i++)
            {
                if (availableResolutions[i].width == w && availableResolutions[i].height == h)
                {
                    resolutionDropdown.value = i;
                    Screen.SetResolution(w, h, Screen.fullScreen, availableResolutions[i].refreshRate);
                    matched = true;
                    break;
                }
            }
            if (!matched) resolutionDropdown.value = availableResolutions.Length - 1; // fallback
        }
        else
        {
            // default: current screen resolution
            int index = availableResolutions.Length - 1;
            for (int i = 0; i < availableResolutions.Length; i++)
            {
                if (availableResolutions[i].width == Screen.currentResolution.width &&
                    availableResolutions[i].height == Screen.currentResolution.height)
                {
                    index = i;
                    break;
                }
            }
            resolutionDropdown.value = index;
        }

        // Fullscreen
        if (PlayerPrefs.HasKey(PREF_FULLSCREEN))
        {
            bool fs = PlayerPrefs.GetInt(PREF_FULLSCREEN) == 1;
            fullscreenToggle.isOn = fs;
            Screen.fullScreen = fs;
        }
        else
        {
            fullscreenToggle.isOn = Screen.fullScreen;
        }

        // BGM / SFX
        float bgmVal = PlayerPrefs.HasKey(PREF_BGM) ? PlayerPrefs.GetFloat(PREF_BGM) : 1f;
        float sfxVal = PlayerPrefs.HasKey(PREF_SFX) ? PlayerPrefs.GetFloat(PREF_SFX) : 1f;
        bgmSlider.value = bgmVal;
        sfxSlider.value = sfxVal;
        // apply to mixer
        AudioSettings.ApplyVolume(masterMixer, musicParam, bgmVal);
        AudioSettings.ApplyVolume(masterMixer, sfxParam, sfxVal);
    }

    public void OnResolutionChanged(int index)
    {
        if (index < 0 || index >= availableResolutions.Length) return;
        var res = availableResolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen, res.refreshRate);
        PlayerPrefs.SetInt(PREF_WIDTH, res.width);
        PlayerPrefs.SetInt(PREF_HEIGHT, res.height);
        PlayerPrefs.Save();
    }

    public void OnFullscreenToggled(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt(PREF_FULLSCREEN, isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void OnBgmVolumeChanged(float value)
    {
        AudioSettings.ApplyVolume(masterMixer, musicParam, value);
        PlayerPrefs.SetFloat(PREF_BGM, value);
        PlayerPrefs.Save();
    }

    public void OnSfxVolumeChanged(float value)
    {
        AudioSettings.ApplyVolume(masterMixer, sfxParam, value);
        PlayerPrefs.SetFloat(PREF_SFX, value);
        PlayerPrefs.Save();
    }

    public void ResetToDefaults()
    {
        // defaults: fullscreen true, max volumes, native resolution last entry
        bool defaultFullscreen = true;
        float defaultBgm = 1f;
        float defaultSfx = 1f;

        fullscreenToggle.isOn = defaultFullscreen;
        Screen.fullScreen = defaultFullscreen;
        PlayerPrefs.SetInt(PREF_FULLSCREEN, defaultFullscreen ? 1 : 0);

        bgmSlider.value = defaultBgm;
        sfxSlider.value = defaultSfx;

        // set resolution to current system resolution (best match)
        var current = Screen.currentResolution;
        int matchIndex = 0;
        for (int i = 0; i < availableResolutions.Length; i++)
        {
            if (availableResolutions[i].width == current.width &&
                availableResolutions[i].height == current.height)
            {
                matchIndex = i;
                break;
            }
        }
        resolutionDropdown.value = matchIndex;
        OnResolutionChanged(matchIndex);

        OnBgmVolumeChanged(defaultBgm);
        OnSfxVolumeChanged(defaultSfx);

        PlayerPrefs.Save();
    }

    public void OnBackPressed()
    {
        // example: go back to MainScene
        SceneManager.LoadScene("MainMenu");
    }
}
