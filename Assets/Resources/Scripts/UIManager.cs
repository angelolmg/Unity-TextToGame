namespace TextTyper
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIManager : MonoBehaviour
    {
        public AudioClip buttonSound;

        // Start is called before the first frame update
        void Start()
        {

        }

        public void playButtonSound()
        {
            var audioSource = GetComponent<AudioSource>();                      // Gets the audio source 
            if (audioSource == null)                                            // If there's none, create one
                audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.clip = buttonSound;                                     // Change print sound effect
            audioSource.volume = 0.3f;
            audioSource.Play();                                                 // Play the sound effect
        }


    }
}


