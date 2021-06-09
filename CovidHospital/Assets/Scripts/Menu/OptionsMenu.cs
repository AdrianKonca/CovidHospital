using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Image SoundImg;
    public Sprite MaxSound;
    public Sprite MediumSound;
    public Sprite MinSound;
    public Sprite NoSound;
    public AudioMixer Mixer;
    Resolution[] resolutions;
    public Dropdown resolutionDropdown;

    private void Start()
    {
        resolutions = Screen.resolutions;
        int currentResolution = 0;
        resolutionDropdown.ClearOptions();

        List<string> reso = new List<string>();
        string tmp;

        for (int i=0; i < resolutions.Length; i++)
        {
            tmp = resolutions[i].width + " x " + resolutions[i].height;
            reso.Add(tmp);

            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolution = i;
            }
        }

        resolutionDropdown.AddOptions(reso);
        resolutionDropdown.value = currentResolution;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetVolume(float volume)
    {
        Mixer.SetFloat("GlobalVolume", volume);
        if (volume > -20.0)
            SetImage(MaxSound);
        else if (volume <= -20.0 && volume >= -60.0)
            SetImage(MediumSound);
        else if (volume == -80.0)
            SetImage(NoSound);
        else
            SetImage(MinSound);
    }

    private void SetImage(Sprite newImg)
    {
        SoundImg.sprite = newImg;
    }

    public void SetGraphics(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public void SetFullscreen(bool isfullscreen)
    {
        Screen.fullScreen = isfullscreen;
    }

    public void SetResolution(int index)
    {
        Screen.SetResolution(resolutions[index].width,resolutions[index].height, Screen.fullScreen);
    }

}
