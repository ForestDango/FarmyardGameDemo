using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Sounds : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if(audioSource != null)
        {
            audioSource.Play();
        }
    }

    private void OnDisable()
    {
        audioSource.Stop();
    }

    public void SetSound(SoundItem  soundItem)
    {
        audioSource.pitch = Random.Range(soundItem.soundPitchRandomVariationMin, soundItem.soundPitchRandomVariationMax);
        audioSource.volume = soundItem.soundVolume;
        audioSource.clip = soundItem.soundClip;

    }
}
