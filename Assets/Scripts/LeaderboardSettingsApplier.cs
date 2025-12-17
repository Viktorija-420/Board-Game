using UnityEngine;
using UnityEngine.Audio;

// Applies settings saved in SettingsManager (PlayerPrefs) when the Leaderboard scene starts.
public class LeaderboardSettingsApplier : MonoBehaviour
{
    [Header("Audio")]
    public AudioMixer masterMixer;
    public string musicParam = "BGMVolume";
    public string sfxParam = "SFXVolume";

    // PlayerPrefs keys (must match SettingsManager)
    private const string PREF_WIDTH = "pref_width";
    private const string PREF_HEIGHT = "pref_height";
    private const string PREF_FULLSCREEN = "pref_fullscreen";
    private const string PREF_BGM = "pref_bgm";
    private const string PREF_SFX = "pref_sfx";

    void Start()
    {
        ApplyAudioSettings();
        ApplyDisplaySettings();
    }

    void ApplyAudioSettings()
    {
        if (masterMixer == null) return;
        float bgmVal = PlayerPrefs.HasKey(PREF_BGM) ? PlayerPrefs.GetFloat(PREF_BGM) : 1f;
        float sfxVal = PlayerPrefs.HasKey(PREF_SFX) ? PlayerPrefs.GetFloat(PREF_SFX) : 1f;
        AudioSettings.ApplyVolume(masterMixer, musicParam, bgmVal);
        AudioSettings.ApplyVolume(masterMixer, sfxParam, sfxVal);
    }

    void ApplyDisplaySettings()
    {
        if (PlayerPrefs.HasKey(PREF_FULLSCREEN))
        {
            bool fs = PlayerPrefs.GetInt(PREF_FULLSCREEN) == 1;
            Screen.fullScreen = fs;
        }

        if (PlayerPrefs.HasKey(PREF_WIDTH) && PlayerPrefs.HasKey(PREF_HEIGHT))
        {
            int w = PlayerPrefs.GetInt(PREF_WIDTH);
            int h = PlayerPrefs.GetInt(PREF_HEIGHT);
            // Keep current refresh rate
            Screen.SetResolution(w, h, Screen.fullScreen);
        }
    }
}
