using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

// Persistent settings applier that enforces saved settings (audio, fullscreen, resolution)
// across all scenes. Attach to an initial scene or place on a GameObject and it will persist.
public class GlobalSettingsApplier : MonoBehaviour
{
    public static GlobalSettingsApplier Instance { get; private set; }

    [Header("Audio")]
    [Tooltip("Assign the Master AudioMixer used by the game. If left empty the script will try to load 'MasterMixer' from Resources.")]
    public AudioMixer masterMixer;
    public string musicParam = "BGMVolume";
    public string sfxParam = "SFXVolume";

    private const string PREF_WIDTH = "pref_width";
    private const string PREF_HEIGHT = "pref_height";
    private const string PREF_FULLSCREEN = "pref_fullscreen";
    private const string PREF_BGM = "pref_bgm";
    private const string PREF_SFX = "pref_sfx";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (masterMixer == null)
            {
                // try to load from Resources/MasterMixer (optional)
                var loaded = Resources.Load<AudioMixer>("MasterMixer");
                if (loaded != null) masterMixer = loaded;
            }
            ApplySettings();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplySettings();
    }

    public void ApplySettings()
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
            Screen.SetResolution(w, h, Screen.fullScreen);
        }
    }
}
