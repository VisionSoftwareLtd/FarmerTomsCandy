using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SoundVariety
{
    public string name;
    public int varietyCount;
}

/**
    * Manages playing sound effects and music in the game. A normal minimal
    * set of music would be: MainMenu, Intro, Game, Win, Lose.
*/
public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private AudioClip[] musicClips;
    private Dictionary<string, AudioClip> audioClipDictionary;
    private Dictionary<string, AudioClip> musicClipDictionary;
    [SerializeField] private int numAudioSources = 5;
    private AudioSource[] audioSources;
    private AudioSource musicAudioSource;
    private int currentAudioSourceIndex = 0;
    private string lastMusicPlayed;

    public static SoundManager instance { get; private set; }
    [SerializeField] private SoundVariety[] soundVarieties;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitialiseClipDictionaries();
        InitialiseAudioSources();
    }

    private void InitialiseClipDictionaries()
    {
        audioClipDictionary = new Dictionary<string, AudioClip>();
        for (int i = 0; i < audioClips.Length; i++)
        {
            audioClipDictionary[audioClips[i].name] = audioClips[i];
        }

        musicClipDictionary = new Dictionary<string, AudioClip>();
        for (int i = 0; i < musicClips.Length; i++)
        {
            musicClipDictionary[musicClips[i].name] = musicClips[i];
        }
    }

    private void InitialiseAudioSources()
    {
        audioSources = new AudioSource[numAudioSources];
        for (int i = 0; i < numAudioSources; i++)
        {
            GameObject audioObject = new GameObject("AudioSource_" + i);
            audioObject.transform.SetParent(transform);
            audioSources[i] = audioObject.AddComponent<AudioSource>();
            audioSources[i].playOnAwake = false;
            audioSources[i].loop = false;
        }

        musicAudioSource = new AudioSource();
        GameObject musicObject = new GameObject("MusicAudioSource");
        musicObject.transform.SetParent(transform);
        musicAudioSource = musicObject.AddComponent<AudioSource>();
        musicAudioSource.playOnAwake = false;
        musicAudioSource.loop = true;
    }

    public AudioSource PlaySoundRandomPitch(string soundName, float volume = 1f, float minPitch = 0.9f, float maxPitch = 1.1f)
    {
        float randomPitch = UnityEngine.Random.Range(minPitch, maxPitch);
        return PlaySound(soundName, volume, randomPitch);
    }

    public AudioSource PlaySound(string soundName, float volume = 1f, float pitch = 1f)
    {
        soundName = GetVarietySoundName(soundName);
        if (!audioClipDictionary.ContainsKey(soundName))
        {
            Debug.LogError("SoundManager: Sound not found - " + soundName);
            return null;
        }
        AudioSource source;
        int initialIndex = currentAudioSourceIndex;
        do
        {
            source = audioSources[currentAudioSourceIndex];
            currentAudioSourceIndex = (currentAudioSourceIndex + 1) % audioSources.Length;
            if (currentAudioSourceIndex == initialIndex)
            {
                Debug.LogWarning("All audio sources are busy. Overwriting a sound.");
                break;
            }
        } while (source.isPlaying);
        source.clip = audioClipDictionary[soundName];
        source.volume = volume;
        source.pitch = pitch;
        source.loop = false;
        source.Play();
        return source;
    }

    private string GetVarietySoundName(string baseName)
    {
        foreach (var variety in soundVarieties)
        {
            if (variety.name == baseName && variety.varietyCount > 1)
            {
                int randomIndex = UnityEngine.Random.Range(1, variety.varietyCount + 1);
                return baseName + randomIndex;
            }
        }
        return baseName;
    }

    public void PlayMusic(string musicName, bool loop = true, float volume = 1f, float pitch = 1f)
    {
        if (!musicClipDictionary.ContainsKey(musicName))
        {
            Debug.LogError("SoundManager: Music not found - " + musicName);
            return;
        }
        if (musicName == lastMusicPlayed && musicAudioSource.isPlaying)
        {
            return;
        }
        lastMusicPlayed = musicName;
        musicAudioSource.clip = musicClipDictionary[musicName];
        musicAudioSource.volume = volume;
        musicAudioSource.pitch = pitch;
        musicAudioSource.loop = loop;
        musicAudioSource.Play();
    }

    internal void StopAllSounds()
    {
        foreach (var source in audioSources)
        {
            if (source.isPlaying)
            {
                source.Stop();
            }
        }
    }
}
