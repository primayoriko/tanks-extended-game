using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeAdapterSetup : MonoBehaviour
{
    private static readonly string FirstPlay = "FirstPlay";
    private static readonly string BackgroundPref = "BackgroundPref";
    private int firstPlayInt;
    public Slider backgroundSlider;
    private float backgroundFloat;
    public AudioSource backgroundAudio;

    void Start(){
        firstPlayInt = PlayerPrefs.GetInt(FirstPlay);
        if(firstPlayInt ==0){
            backgroundFloat = .125f;
            backgroundSlider.value = backgroundFloat;
            PlayerPrefs.SetFloat(BackgroundPref, backgroundFloat);
            PlayerPrefs.SetInt(FirstPlay, -1);
        }else{
            backgroundFloat = PlayerPrefs.GetFloat(BackgroundPref);
            backgroundSlider.value = backgroundFloat;
        }
    }

    public void SaveSoundSettings(){
        PlayerPrefs.SetFloat(BackgroundPref, backgroundSlider.value);
    }

    void OnApplicationFocus(bool infocus){
        if(!infocus){
            SaveSoundSettings();
        }
    }

    public void UpdateSound(){
        backgroundAudio.volume = backgroundSlider.value;
    }

}
