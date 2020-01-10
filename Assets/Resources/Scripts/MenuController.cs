using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Sprite backGroundImage;
    public AudioClip backGroundMusic;
    public AudioClip buttonSound;
    public Sprite buttonSkin;

    void Start()
    {
        SetVisuals();
    }

    public void GoToScene(int sceneIndex)
    {
        Debug.Log("Going to scene " + sceneIndex + "...");
        SceneManager.LoadScene(sceneIndex);
    }

    public void ExitApplication()
    {
        Debug.Log("Exiting Application...");
        Application.Quit();
    }

    public void playButtonSound()
    {
        var audioSource = GetComponent<AudioSource>();                      // Gets the audio source 
        if (audioSource == null)                                            // If there's none, create one
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = buttonSound;                                     // Change print sound effect
        audioSource.volume = 0.5f;
        audioSource.Play();                                                 // Play the sound effect
    }

    public void SetVisuals()
    {
        Image backGround = GameObject.Find("Background").GetComponent<Image>();
        AudioSource audioSource = GameObject.Find("Main Audio").GetComponent<AudioSource>();

        if(backGround != null)
            backGround.sprite = backGroundImage;
        audioSource.clip = backGroundMusic;
        audioSource.Play();

        Transform buttonArray = GameObject.Find("Buttons").transform;
        foreach (Transform child in buttonArray)
        {
            child.GetComponent<Image>().sprite = buttonSkin;
        }
    }
}
