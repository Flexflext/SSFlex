using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    // Code: Haoke
    // Purpose: - Instance so you can reference it from everywhere and is not destroyed on load.
    //          - Setting up single sounds or random sounds.
    //          - StopAllLoopingSounds Function so the looping clip can be easily stopped.


    public float MasterVolume;
    public Sound[] sounds;

    [SerializeField] private AudioMixer MainMixer;
    [SerializeField] private AudioMixerGroup mixerGroup;

    
    // Setting up the arrays for random sounds.
    [Space]
    [Header("Arrays for Random Sounds")]
    [SerializeField] private AudioClip[] walkOnGravelSounds;
    [SerializeField] private AudioClip[] runOnGravelSounds;
    [SerializeField] private AudioClip[] walkOnStoneSounds;
    [SerializeField] private AudioClip[] runOnStoneSounds;

    public static AudioManager Instance;




    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (Sound s in sounds)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.outputAudioMixerGroup = mixerGroup;

            s.Source.clip = s.Clip;
            s.Source.volume = s.Volume;
            s.Source.pitch = s.Pitch;
            s.Source.loop = s.Loop;
            s.Source.spatialBlend = s.SpatialBlend;
        }
    }




    void Start()
    {
        MainMixer.SetFloat("MasterVolume", Mathf.Log10(MasterVolume) * 20);
    }




    // For playing a Sound normally.
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.Name == name);
        s.Source.Play();
    }




    // For playing random sounds.
    public void PlayRandom(string name, int index)
    {
        Sound s = Array.Find(sounds, sound => sound.Name == name);
        AudioClip audioclip = null;

        switch (index)
        {
            case 1:
                int randomnumberofarray = Random.Range(0, walkOnGravelSounds.Length);
                audioclip = walkOnGravelSounds[randomnumberofarray];

                break;

            case 2:
                randomnumberofarray = Random.Range(0, runOnGravelSounds.Length);
                audioclip = runOnGravelSounds[randomnumberofarray];

                break;

            case 3:
                randomnumberofarray = Random.Range(0, walkOnStoneSounds.Length);
                audioclip = walkOnStoneSounds[randomnumberofarray];

                break;

            case 4:
                randomnumberofarray = Random.Range(0, runOnStoneSounds.Length);
                audioclip = runOnStoneSounds[randomnumberofarray];

                break;


            default:
                break;
        }

        if (!s.Source.isPlaying)
        {
            if (audioclip != null)
            {
                s.Source.clip = audioclip;
            }
            s.Source.Play();
        }
    }





    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.Name == name);
        s.Source.Stop();
    }





    public void SetMasterVolume(float _volume)
    {
        MasterVolume = _volume;
        MainMixer.SetFloat("MasterVolume", Mathf.Log10(MasterVolume) * 20);
    }





    public void StopAllLoopingSounds()
    {
        // Here add looping Sounds or Soundtracks

        Stop("GravelWalk");
        Stop("GravelRun");
        Stop("StoneWalk");
        Stop("StoneRun");
    }
}
