using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


/// <summary>
/// This class manages the game Options
/// </summary>
public class OptionsManager : MonoBehaviour
{
    public float Fov => mFov;
    public float MouseSensitivity => mMouseSensitivity;

    //[SerializeField]
    //private GameObject mPauseMenu;
    //[SerializeField]
    //private GameObject mGameOverScreen;

    [Header("Audio Components")]
    [SerializeField]
    private AudioMixer mMusicMixer;
    [SerializeField]
    private AudioMixer mSFXMixer;
    //[SerializeField]
    //private AudioMixer mVoiceMixer;
    //[SerializeField]
    //private AudioVolumen mVolumen;

    [Header("Resolution Components")]
    [SerializeField]
    private TMP_Dropdown mResolutionDropDown;
    [SerializeField]
    private Resolution[] mResolutions;
    private List<string> mResolutionOptions;

    [Header("Graphic Components")]
    [SerializeField]
    private TMP_Dropdown mGraphicDropDown;

    [Header("Audio Slider")]
    [SerializeField]
    private Slider mMusicSlider;
    [SerializeField]
    private Slider mSFXSlider;
    //[SerializeField]
    //private Slider mVoiceSlider;

    private float mFov;
    private float mMouseSensitivity;


    //private void Awake()
    //{
    //    mVolumen = FindObjectOfType<AudioVolumen>();
    //}

    /// <summary>
    /// Reads out all the available resolutions to display them properly
    /// </summary>
    private void Start()
    {
        mResolutionOptions = new List<string>();

        mResolutions = Screen.resolutions;

        if (mResolutionDropDown != null)
        {
            mResolutionDropDown.ClearOptions();
            int currentResIdx = 0;
            for (int i = 0; i < mResolutions.Length; i++)
            {
                string option = mResolutions[i].width + " x " + mResolutions[i].height + " x " + mResolutions[i].refreshRate + "Hz";
                mResolutionOptions.Add(option);

                if (mResolutions[i].width == Screen.currentResolution.width && mResolutions[i].height == Screen.currentResolution.height)
                {
                    currentResIdx = i;
                }
            }

            mResolutionDropDown.AddOptions(mResolutionOptions);
            mResolutionDropDown.value = currentResIdx;
            mResolutionDropDown.RefreshShownValue();

            mGraphicDropDown.value = (int)QualitySettings.currentLevel;
        }
    }

    ///// <summary>
    ///// Adjusts the slider so they portray the right values after scene loading
    ///// </summary>
    //private void Update()
    //{
    //    mMusicMixer.SetFloat("Volume", Mathf.Log10(mVolumen.MusicVolumen) * 20);
    //    mMusicSlider.value = mVolumen.MusicVolumen;

    //    mVoiceMixer.SetFloat("Voice", Mathf.Log10(mVolumen.VoiceVolumen) * 20);
    //    mVoiceSlider.value = mVolumen.VoiceVolumen;

    //    mSFXMixer.SetFloat("SFX", Mathf.Log10(mVolumen.SFXVolumen) * 20);
    //    mSFXSlider.value = mVolumen.SFXVolumen;
    //}

    /// <summary>
    /// Method to adjust the Audio
    /// </summary>
    public void SetMusicVolumen(float _volume)
    {
        //mMusicMixer.SetFloat("Volume", Mathf.Log10(_volume) * 20);
        //mVolumen.MusicVolumen = _volume;
    }
    public void SetVoiceVolumen(float _volume)
    {
        //mVoiceMixer.SetFloat("Voice", Mathf.Log10(_volume) * 20);
        //mVolumen.VoiceVolumen = _volume;
    }
    public void SetSFXVolumen(float _volume)
    {
        //mSFXMixer.SetFloat("SFX", Mathf.Log10(_volume) * 20);
        //mVolumen.SFXVolumen = _volume;
    }

    /// <summary>
    /// Method to toggle between fullscreen or windowed
    /// </summary>
    public void SetFullscreen(bool _isFullscreen)
    {
        Screen.fullScreen = _isFullscreen;

        if (_isFullscreen)
            Debug.Log("Is Fullscreen");

        else
            Debug.Log("Is Windowed");
    }

    /// <summary>
    /// Sets the Resolution to the Desired value
    /// </summary>
    public void SetResolutuon(int _resIdx)
    {
        Resolution res = mResolutions[_resIdx];

        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }

    public void SetFov(float _value)
    {
        mFov = _value;
    }

    public void SetMouseSensitivity(float _value)
    {
        mMouseSensitivity = _value;
    }

    public void SetGraphicQuality(int _graphicQualityIdx)
    {
        QualitySettings.SetQualityLevel(_graphicQualityIdx);
    }
}
