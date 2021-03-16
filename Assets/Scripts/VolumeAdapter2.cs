using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeAdapter2 : MonoBehaviour
{
    private static readonly string BackgroundPref = "BackgroundPref";
    private float backgroundFloat;
    public AudioSource backgroundAudio;

    void Awake(){
        ContinueSettings();
    }

    private void ContinueSettings(){
        backgroundFloat = PlayerPrefs.GetFloat(BackgroundPref);
        backgroundAudio.volume = backgroundFloat;
    }
}
