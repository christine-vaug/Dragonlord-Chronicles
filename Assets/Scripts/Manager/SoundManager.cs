using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public static SoundManager Instance {
        get {
            if (_instance == null) {
                _instance = Object.FindObjectOfType<SoundManager>();
            }
            if (_instance == null) {
                Debug.LogError("An SoundManager object must exist for music to play");
            }
            return _instance;
        }
    }
    private static SoundManager _instance;

    Dictionary<string, AudioClip> soundEffects;
    Dictionary<string, AudioClip> music;
    private AudioClip currentBackgroundMusic;
    private AudioSource musicPlayer, soundEffectPlayer;

    void Awake () {
        
        DontDestroyOnLoad(this);
        FindAudioSources();
        LoadAudioClipsFromResources();

        //PlayMusic("Kings And Dragons");
    }

    void Update () {
        
    }

    void FindAudioSources () {

        AudioSource[] sources = GetComponentsInChildren<AudioSource>();
        musicPlayer = sources[0];
        soundEffectPlayer = sources[1];

    }

    void LoadAudioClipsFromResources () {

        soundEffects = new Dictionary<string, AudioClip>();
        music = new Dictionary<string, AudioClip>();

        AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio\\Sound Effects");
        foreach (AudioClip clip in clips) {
            soundEffects.Add(clip.name, clip);
        }

        clips = Resources.LoadAll<AudioClip>("Audio\\Music");
        foreach (AudioClip clip in clips) {
            music.Add(clip.name, clip);
        }
    }
    
    public void StopMusic () {

        musicPlayer.Stop();
    }

    public void PlayMusic(string name, float volume = 1f) {


        if (music.TryGetValue(name, out currentBackgroundMusic)) {
            musicPlayer.clip = currentBackgroundMusic;
            musicPlayer.Play();
            musicPlayer.volume = volume;
            musicPlayer.loop = true;
        }
    }

    public void PlaySoundEffect (string name, float volume = 1f) {

        AudioClip soundFX;
        if (soundEffects.TryGetValue(name, out soundFX)) {
            soundEffectPlayer.clip = soundFX;
            soundEffectPlayer.PlayOneShot(soundFX, volume);

        }
    }

    
}
