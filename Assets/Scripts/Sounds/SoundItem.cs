using UnityEngine;

[System.Serializable]
public class SoundItem
{
    public SoundName soundName;
    public AudioClip soundClip;
    public string soundDescription;
    [Range(0.1f, 1.5f)] public float soundPitchRandomVariationMin = 0.8f;
    [Range(0.1f, 1.5f)] public float soundPitchRandomVariationMax = 1.2f;
    [Range(0f, 1f)] public float soundVolume = 1f;
}
