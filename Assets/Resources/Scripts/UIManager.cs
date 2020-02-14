﻿namespace TextTyper
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    public class UIManager : MonoBehaviour
    {
        private static int narratorTalkingLeftPadding = -26;
        private static int showPortraitNameMargin = 205;
        private static int showPortraitButtonPadding = 475;

        [Header("General Setup")]
        public bool showPortrait = true;                                
        public bool showNames = true;
        public string narratorName = "Narrador";                        // Name of the character who is the narrator

        [Header("Objects Reference")]
        public Button[] buttonGrid;                                     // Stores the three interaction buttons
        public Image[] imageGrid;                                       // Image array to show in the center of the screen

        public Image imagePanel;                                        // Character's portrait panel
        public Image bgPanel;                                           // Background panel

        public TextMeshProUGUI characterName;                           // The text box for the character's name

        public Sprite defaultSprite;                                    // The default character sprite. 
                                                                        // It's displayed when non-other is available
        public AudioClip buttonClickSound;
        public AudioClip buttonHoverSound;
        public AudioClip printSoundEffect;                              // Print character sound effect

        public Animator fadeAnimator;                                       

        [Header("Volumes Reference")]
        [Range(0f,0.5f)]
        public float musicVolume = 0.04f;
        [Range(0f, 0.5f)]
        public float buttonPressVolume = 0.05f;
        [Range(0f, 0.5f)]
        public float buttonHoverVolume = 0.05f;
        [Range(0f, 0.5f)]
        public float characterPrintVolume = 0.1f;
        [Range(0f, 0.5f)]
        public float soundEffectVolume = 0.05f;

        // Start is called before the first frame update
        void Start()
        {
            characterName.gameObject.SetActive(showNames);
            if(defaultSprite == null)
                defaultSprite = Resources.Load<Sprite>("Sprites/Default");                  // Loads the default sprite

            portraitMode(showPortrait);
        }

        private void portraitMode(bool show)
        {
            GameObject windowParent = characterName.transform.parent.gameObject;
            TextMeshProUGUI[] childArray = windowParent.GetComponentsInChildren<TextMeshProUGUI>();

            int buttonLeftPadding = showPortraitButtonPadding;
            int nameAndContexLeftMargin = showPortraitNameMargin;

            if (!show)
            {
                buttonLeftPadding = 0;
                nameAndContexLeftMargin = 25;
                imagePanel.gameObject.SetActive(false);
            }

            childArray[0].margin = new Vector4(nameAndContexLeftMargin, 0, 25, 0);
            childArray[1].margin = new Vector4(nameAndContexLeftMargin, 0, 25, 0);
            buttonGrid[0].GetComponentInParent<GridLayoutGroup>().padding.left = buttonLeftPadding;

            //Debug.Log("Setting button padding to " + buttonLeftPadding);
            //Debug.Log(buttonGrid[0].GetComponentInParent<GridLayoutGroup>().padding.left);
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

            if(showPortrait)
                portrait.SetActive(true);
        }

        public void HideHud()
        {
            GameObject buttonParent = buttonGrid[0].transform.parent.gameObject;
            GameObject window = characterName.transform.parent.gameObject;
            GameObject portrait = imagePanel.transform.gameObject;

            buttonParent.SetActive(false);
            window.SetActive(false);

            if(showPortrait)
                portrait.SetActive(false);
        }

        public void playButtonClick(GameObject btn)
        {
            AudioSource audioSource = btn.GetComponent<AudioSource>();          // Gets the audio source 
            if (audioSource == null)                                            // If there's none, create one
                audioSource = btn.AddComponent<AudioSource>();

            audioSource.clip = buttonClickSound;                                // Change print sound effect
            audioSource.volume = buttonPressVolume;
            audioSource.Play();                                                 // Play the sound effect
        }

        public void playButtonHover(GameObject btn)
        {
            AudioSource audioSource = btn.GetComponent<AudioSource>();          // Gets the audio source 
            if (audioSource == null)                                            // If there's none, create one
                audioSource = btn.AddComponent<AudioSource>();

            audioSource.clip = buttonHoverSound;                                // Change print sound effect
            audioSource.volume = buttonHoverVolume;
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
            characterName.gameObject.SetActive(showNames);

            if (showPortrait)
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
        public void ToggleButtonPosition(bool narratorIsTalking)
        {
            if (narratorIsTalking)
            {
                buttonGrid[0].GetComponentInParent<GridLayoutGroup>().padding.left = narratorTalkingLeftPadding;
            } else
            {
                portraitMode(showPortrait);
            }   
        }
        
        public void SetImageView(string images)
        {
            // Clear all image views first
            foreach(Image child in imageGrid)
            {
                child.sprite = null;
                child.gameObject.SetActive(false);
            }
            
            // Split "images" string given into smaller string itens to analyse and display
            var itens = images.Split(';');

            // Gets name and custom scale (if there's one) of the Image and then applies it. Ex.: "cat*1.5"
            // In this case, it will search in the directory for a sprite called "cat" and apply a 1.5 local scale(x,y) to its rect transform. 
            if(itens.Length > 0)
            {
                var nameAndScale = itens[0].Split('*');                                                     // Split name and scale on the '*' mark
                Sprite firstImage = Resources.Load<Sprite>("Sprites/" + CleanString(nameAndScale[0]));      // Loads the sprite
                imageGrid[0].gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);       // Resets the scale to 1

                if (firstImage != null)     // If there's a sprite, push it 
                {
                    imageGrid[0].gameObject.SetActive(true);
                    imageGrid[0].sprite = firstImage;
                }

                if (nameAndScale.Length > 1)    // If a scale was given, apply it
                {
                    float scale = float.Parse(nameAndScale[1]);
                    imageGrid[0].gameObject.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, 1);
                }

                if (itens.Length > 1)
                {
                    nameAndScale = itens[1].Split('*');                                                     
                    Sprite secondImage = Resources.Load<Sprite>("Sprites/" + CleanString(nameAndScale[0]));
                    imageGrid[1].gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);       
                    
                    if (secondImage != null)
                    {
                        imageGrid[1].gameObject.SetActive(true);
                        imageGrid[1].sprite = secondImage;
                    }
                    if (nameAndScale.Length > 1)   
                    {
                        float scale = float.Parse(nameAndScale[1]);
                        imageGrid[1].gameObject.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, 1);
                    }

                    if (itens.Length > 2)
                    {
                        nameAndScale = itens[2].Split('*');
                        Sprite thirdImage = Resources.Load<Sprite>("Sprites/" + CleanString(nameAndScale[0]));
                        imageGrid[2].gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);  
                        
                        if (thirdImage != null)
                        {
                            imageGrid[2].gameObject.SetActive(true);
                            imageGrid[2].sprite = thirdImage;
                        }
                        if (nameAndScale.Length > 1)    
                        {
                            float scale = float.Parse(nameAndScale[1]);
                            imageGrid[2].gameObject.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, 1);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Cleans the returns the string clear of any '(',')','\n' or ' ' used for sintax analisys
        /// </summary>
        /// <param name="str">The string to clean</param>
        /// <returns></returns>
        public string CleanString(string str)
        {
            if (str.Length == 0)
                return "";

            while (str[0] == ' ')
                str = str.Substring(1);

            if (str[0] == '(' || str[0] == '[' || str[0] == '/')
                str = str.Substring(1);

            while (str[str.Length - 1] == ' ' || str[str.Length - 1] == '\r')
                str = str.Substring(0, str.Length - 1);

            if (str[str.Length - 1] == ')' || str[str.Length - 1] == ']')
                str = str.Substring(0, str.Length - 1);

            return str;
        }
    }
}


