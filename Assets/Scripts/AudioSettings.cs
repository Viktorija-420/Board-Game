using UnityEngine;
using UnityEngine.Audio;

public static class AudioSettings
{
    // sliderValue is 0..1. We convert to dB for AudioMixer.
    // common mapping: if slider == 0 => -80 dB (silent), else 20 * log10(value)
    private const float MIN_DB = -80f;

    public static void ApplyVolume(AudioMixer mixer, string exposedParam, float sliderValue)
    {
        float db;
        if (sliderValue <= 0.0001f)
            db = MIN_DB;
        else
            db = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
        mixer.SetFloat(exposedParam, db);
    }
}
