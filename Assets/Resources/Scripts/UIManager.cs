namespace TextTyper
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    public class UIManager : MonoBehaviour
    {
        public string narratorName = "Narrador";                        // Name of the character who is the narrator

        public Button[] buttonGrid;                                     // Stores the three interaction buttons

        public Image imagePanel;                                        // Character's portrait panel
        public Image bgPanel;                                           // Background panel

        public TextMeshProUGUI characterName;                           // The text box for the character's name

        public Sprite defaultSprite;                                    // The default character sprite. 
                                                                        // It's displayed when non-other is available
        public AudioClip buttonSound;
        public AudioClip printSoundEffect;                              // Print character sound effect

        public Animator fadeAnimator;                                       

        [Header("Volumes Reference")]
        [Range(0f,0.5f)]
        public float musicVolume = 0.04f;
        [Range(0f, 0.5f)]
        public float buttonPressVolume = 0.05f;
        [Range(0f, 0.5f)]
        public float characterPrintVolume = 0.1f;
        [Range(0f, 0.5f)]
        public float soundEffectVolume = 0.05f;

        // Start is called before the first frame update
        void Start()
        {
            if(defaultSprite == null)
                defaultSprite = Resources.Load<Sprite>("Sprites/Default");                  // Loads the default sprite
        }

        public void FadeIn()
        {
            fadeAnimator.SetTrigger("fadeIn");
        }

        public void FadeOut()
        {
            fadeAnimator.SetTrigger("fadeOut");
        }

        public void BlackOut()
        {
            fadeAnimator.SetTrigger("blackOut");
        }

        public void ClearOut()
        {
            fadeAnimator.SetTrigger("clearOut");
        }

        public void ShowHud()
        {
            GameObject buttonParent = buttonGrid[0].transform.parent.gameObject;
            GameObject window = characterName.transform.parent.gameObject;
            GameObject portrait = imagePanel.transform.gameObject;

            buttonParent.SetActive(true);
            window.SetActive(true);
            portrait.SetActive(true);
        }

        public void HideHud()
        {
            GameObject buttonParent = buttonGrid[0].transform.parent.gameObject;
            GameObject window = characterName.transform.parent.gameObject;
            GameObject portrait = imagePanel.transform.gameObject;

            buttonParent.SetActive(false);
            window.SetActive(false);
            portrait.SetActive(false);
        }

        public void playButtonSound(GameObject btn)
        {
            AudioSource audioSource = btn.GetComponent<AudioSource>();                      // Gets the audio source 
            if (audioSource == null)                                            // If there's none, create one
                audioSource = btn.AddComponent<AudioSource>();

            audioSource.clip = buttonSound;                                     // Change print sound effect
            audioSource.volume = buttonPressVolume;
            audioSource.Play();                                                 // Play the sound effect
        }

        public void PlaySoundEffect(string sound)
        {
            AudioClip clip = Resources.Load<AudioClip>("Sounds/" + sound);
            if (clip != null)
            {
                AudioSource audioSource = GetComponent<AudioSource>();              // Gets the audio source 
                if (audioSource == null)                                            // If there's none, create one
                    audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = clip;                                            // Change sound effect
                audioSource.volume = soundEffectVolume;
                audioSource.Play();                                                 // Play the sound effect
            }
        }

        /// <summary>
        /// Changes current background image by the one in "Sprites/" + the name sent, but only if not null.
        /// </summary>
        /// <param name="bg"></param>
        public void ChangeBackground(string bg)
        {
            Sprite bgSprite = Resources.Load<Sprite>("Sprites/" + bg);
            if (bgSprite != null)
                bgPanel.sprite = bgSprite;
        }

        public void SetCharaterView(string name)
        {
            characterName.gameObject.SetActive(true);
            imagePanel.gameObject.SetActive(true);
            characterName.text = name + ":";                                              // Sets the character's name 
            Sprite actualCharacterSprite = Resources.Load<Sprite>("Sprites/" + name);     // Load the sprite with the name provided                                                                              
            imagePanel.sprite = defaultSprite;                                            // Sets the default sprite to the portrait
            if (actualCharacterSprite)                                                    // But what if there's a actual character sprite?
                imagePanel.sprite = actualCharacterSprite;                                // Set it instead
        }

        public void SetNarratorView()
        {
            characterName.gameObject.SetActive(false);
            imagePanel.gameObject.SetActive(false);
        }

        /// <summary>
        /// Handles the sound of each printed character
        /// </summary>
        /// <param name="printedCharacter"></param>
        public void HandleCharacterPrinted(string printedCharacter)
        {
            if (printedCharacter == " " || printedCharacter == "\n")            // Do not play a sound for whitespace
                return;

            var audioSource = GetComponent<AudioSource>();                      // Gets the audio source 
            if (audioSource == null)                                            // If there's none, create one
                audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.clip = printSoundEffect;                                // Change print sound effect
            audioSource.volume = characterPrintVolume;                                            // Change audio volume
            audioSource.Play();                                                 // Play the sound effect
        }

        /// <summary>
        /// Toogles the button padding in case the narrator is talking, to center the buttons correctly
        /// </summary>
        /// <param name="way"></param>
        public void ToggleButtonPosition(bool move)
        {
            if (move)
                buttonGrid[0].GetComponentInParent<GridLayoutGroup>().padding.left = 5;
            else
                buttonGrid[0].GetComponentInParent<GridLayoutGroup>().padding.left = 660;
        }
    }
}


