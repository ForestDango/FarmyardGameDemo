using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private GameObject soundPrefab = null;
    [Header("Audio Sources")]
    [SerializeField] private AudioSource ambientSoundAudioSource;
    [SerializeField] private AudioSource gameMusicAudioSource;

    [Header("Audio Mixers")]
    [SerializeField] private AudioMixer gameAudioMixer;

    [Header("Audio Snapshots")]
    [SerializeField] private AudioMixerSnapshot gameMusicSnapShot;
    [SerializeField] private AudioMixerSnapshot gameAmbientSnapShot;

    [Header("other")]
    [SerializeField] private SO_SoundList so_SoundList;

    [SerializeField] private SO_SceneSoundsList so_SceneSoundList;
    [SerializeField] private float defaultSceneMusicPlayTimeSeconds = 120f;
    [SerializeField] private float sceneMusicStartMinSecs = 20f;
    [SerializeField] private float sceneMusicStartMaxSecs = 40f;
    [SerializeField] private float musicTransitionSecs = 8f;


    private Dictionary<SoundName, SoundItem> soundDictionary;
    private Dictionary<SceneName, SceneSoundItem> sceneSoundDictionary;

    private Coroutine playSceneSoundCoroutine;

    protected override void Awake()
    {
        base.Awake();
        soundDictionary = new Dictionary<SoundName, SoundItem>();

        foreach (SoundItem soundItem in so_SoundList.soundDetails)
        {
            soundDictionary.Add(soundItem.soundName,soundItem);
        }

        sceneSoundDictionary = new Dictionary<SceneName, SceneSoundItem>();

        foreach (SceneSoundItem sceneSoundItem in so_SceneSoundList.sceneSoundDetails)
        {
            sceneSoundDictionary.Add(sceneSoundItem.sceneName, sceneSoundItem);
        }
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += PlayerSceneSounds;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= PlayerSceneSounds;
    }

    public void PlayerSceneSounds()
    {
        SoundItem musicSoundItem;
        SoundItem ambientSoundItem;

        float musicForPlayTime = defaultSceneMusicPlayTimeSeconds;

        if(Enum.TryParse<SceneName> (SceneManager.GetActiveScene().name,out SceneName currentSceneName))
        {
            if(sceneSoundDictionary.TryGetValue(currentSceneName,out SceneSoundItem sceneSoundItem))
            {
                soundDictionary.TryGetValue(sceneSoundItem.musicForScene, out musicSoundItem);
                soundDictionary.TryGetValue(sceneSoundItem.ambientSceneForScene, out ambientSoundItem);
            }
            else
            {
                return;
            }

            if(playSceneSoundCoroutine != null)
            {
                StopCoroutine(playSceneSoundCoroutine);
            }

            playSceneSoundCoroutine = StartCoroutine(PlaySceneSoundCoroutine(musicForPlayTime, musicSoundItem, ambientSoundItem));
        }
    }

    private IEnumerator PlaySceneSoundCoroutine(float musicPlaySecs, SoundItem musicSoundItem, SoundItem ambientSoundItem)
    {
        if(musicSoundItem != null && ambientSoundItem != null)
        {
            PlayAmbientSoundClip(ambientSoundItem, 0f);

            yield return new WaitForSeconds(UnityEngine.Random.Range(sceneMusicStartMinSecs, sceneMusicStartMaxSecs));

            PlayMusicSoundClip(musicSoundItem, musicTransitionSecs);

            yield return new WaitForSeconds(musicPlaySecs);

            PlayAmbientSoundClip(ambientSoundItem, musicTransitionSecs);
        }
    }

    private void PlayAmbientSoundClip(SoundItem ambientSoundItem, float musicTransitionSecs)
    {
        gameAudioMixer.SetFloat("AmbientVolume", ConvertSoundVolumeDecimalFractionToDecibels(ambientSoundItem.soundVolume));

        ambientSoundAudioSource.clip = ambientSoundItem.soundClip;
        ambientSoundAudioSource.Play();

        gameAmbientSnapShot.TransitionTo(musicTransitionSecs);
    }

    private void PlayMusicSoundClip(SoundItem musicSoundItem, float musicTransitionSecs)
    {
        gameAudioMixer.SetFloat("MusicVolume", ConvertSoundVolumeDecimalFractionToDecibels(musicSoundItem.soundVolume));

        ambientSoundAudioSource.clip = musicSoundItem.soundClip;
        ambientSoundAudioSource.Play();

        gameAmbientSnapShot.TransitionTo(musicTransitionSecs);
    }

    private float ConvertSoundVolumeDecimalFractionToDecibels(float volumeDecimalFraction)
    {
        return (volumeDecimalFraction * 100f - 80f) ;
    }

    public void PlaySound(SoundName soundName)
    {
        if(soundDictionary.TryGetValue(soundName,out SoundItem soundItem) && soundPrefab != null)
        {
            GameObject soundGmaeObject = PoolManager.Instance.ReuseObject(soundPrefab, Vector3.zero, Quaternion.identity);

            Sounds sound = soundGmaeObject.GetComponent<Sounds>();

            sound.SetSound(soundItem);
            soundGmaeObject.SetActive(true);
            StartCoroutine(DisableSoundCoroutine(soundGmaeObject, soundItem.soundClip.length));
        }
    }

    private IEnumerator DisableSoundCoroutine(GameObject soundGmaeObject, float soundDuration)
    {
        yield return new WaitForSeconds(soundDuration);
        soundGmaeObject.SetActive(false);
    }
}
