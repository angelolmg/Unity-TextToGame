namespace TextTyper
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
        //private static int showPortraitButtonPadding = 475;

        [Header("General Setup")]
        public bool showPortrait = true;
        public bool showNames = true;
        public string narratorName = "Narrador";                        // Name of the character who is the narrator

        [Header("Objects References")]
        public Button[] buttonGrid;                                     // Stores the three interaction buttons
        public Image[] imageGrid;                                       // Image array to show in the center of the screen

        public Image imagePanel;                                        // Character's portrait panel
        public Image bgPanel;                                           // Background panel
        public Image dialogBox;                                         // Dialog box view
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI contentText;
        public TextMeshProUGUI narratorText;

        public TextMeshProUGUI characterName;                           // The text box for the character's name

        [Header("Default References")]
        public Sprite defaultSpritePortrait;                                    // The default character sprite. It's displayed when non-other is available
        public Sprite defaultDialogBox;
        public Sprite defaultButtonSprite;
        public AudioClip defaultButtonClickSound;
        public AudioClip defaultButtonHoverSound;
        public AudioClip defaultPrintSoundEffect;                              // Print character sound effect
        public TMP_FontAsset DefaultFontAsset;
        public TMP_FontAsset DefaultButtonFontAsset;

        public Animator fadeAnimator;

        [Header("Volume References")]
        [Range(0f, 0.5f)]
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
            if (defaultSpritePortrait == null)
                defaultSpritePortrait = Resources.Load<Sprite>("Sprites/Portraits/Default");                  // Loads the default sprite

            portraitMode(showPortrait);
            SetDefaultAssets();    
        }

        

        private void portraitMode(bool show)
        {
            GameObject windowParent = characterName.transform.parent.gameObject;
            TextMeshProUGUI[] childArray = windowParent.GetComponentsInChildren<TextMeshProUGUI>();

            int nameAndContexLeftMargin = showPortraitNameMargin;

            if (!show)
            {
                nameAndContexLeftMargin = 25;
                imagePanel.gameObject.SetActive(false);
            }

            childArray[0].margin = new Vector4(nameAndContexLeftMargin, 0, 25, 0);
            childArray[1].margin = new Vector4(nameAndContexLeftMargin, 0, 25, 0);
        }

        private void SetDefaultAssets()
        {
            SetDefaultButtonAssets();
            SetDefaultDialogBoxAssets();
        }

        private void SetDefaultButtonAssets()
        {
            Sprite setButtonSprite = Resources.Load<Sprite>("Sprites/UI/DefaultButton");
            TMP_FontAsset buttonFont = Resources.Load<TMP_FontAsset>("Fonts/ButtonFont");

            if (setButtonSprite)
            {
                foreach (Button btn in buttonGrid)
                    btn.GetComponent<Image>().sprite = setButtonSprite;
            }
            else Debug.Log("No button skin set. Using the default.");

            if (buttonFont)
            {
                foreach (Button btn in buttonGrid)
                    btn.GetComponentInChildren<TextMeshProUGUI>().font = buttonFont;
            } else Debug.Log("No button font loaded. Using the default.");
        }

        private void SetDefaultDialogBoxAssets()
        {
            Sprite setDialogSprite = Resources.Load<Sprite>("Sprites/UI/DefaultDialog");

            if (setDialogSprite)
            {
                foreach (Button btn in buttonGrid)
                    defaultDialogBox = setDialogSprite;
            }
            else Debug.Log("No dialog box set. Using the default.");
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

            if (showPortrait)
                portrait.SetActive(true);
        }

        public void HideHud()
        {
            GameObject buttonParent = buttonGrid[0].transform.parent.gameObject;
            GameObject window = characterName.transform.parent.gameObject;
            GameObject portrait = imagePanel.transform.gameObject;

            buttonParent.SetActive(false);
            window.SetActive(false);

            if (showPortrait)
                portrait.SetActive(false);
        }

        public void playButtonClick(GameObject btnTree)
        {
            AudioSource audioSource = btnTree.GetComponent<AudioSource>();      // Gets the audio source 
            if (audioSource == null)                                            // If there's none, create one
                audioSource = btnTree.AddComponent<AudioSource>();

            audioSource.clip = defaultButtonClickSound;                                // Change print sound effect
            audioSource.volume = buttonPressVolume;
            audioSource.Play();                                                 // Play the sound effect
        }

        public void playButtonHover(GameObject btnTree)
        {
            AudioSource audioSource = btnTree.GetComponent<AudioSource>();      // Gets the audio source 
            if (audioSource == null)                                            // If there's none, create one
                audioSource = btnTree.AddComponent<AudioSource>();

            audioSource.clip = defaultButtonHoverSound;                                // Change print sound effect
            audioSource.volume = buttonHoverVolume;
            audioSource.Play();                                                 // Play the sound effect
        }

        public void PlaySoundEffect(string sound)
        {
            AudioClip clip = Resources.Load<AudioClip>("Sounds/SFX/" + sound);
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

        public void StopSoundEffects()
        {
            AudioSource audioSource = GetComponent<AudioSource>();              // Gets the audio source 
            if (audioSource != null)                                            
                audioSource.Stop();
        }
        /// <summary>
        /// Changes current background image by the one in "Sprites/" + the name sent, but only if not null.
        /// </summary>
        /// <param name="bg"></param>
        public void ChangeBackground(string bg)
        {
            Sprite bgSprite = Resources.Load<Sprite>("Sprites/Backgrounds/" + bg);
            if (bgSprite != null)
                bgPanel.sprite = bgSprite;
        }

        public void SetCharaterView(string name)
        {
            characterName.gameObject.SetActive(showNames);

            if (showPortrait)
                imagePanel.gameObject.SetActive(true);

            characterName.text = name + ":";                                                        // Sets the character's name 
            Sprite actualCharacterSprite = Resources.Load<Sprite>("Sprites/Portraits/" + name);     // Load the sprite with the name provided                                                                              
            imagePanel.sprite = defaultSpritePortrait;                                                      // Sets the default sprite to the portrait
            if (actualCharacterSprite)                                                              // But what if there's a actual character sprite?
                imagePanel.sprite = actualCharacterSprite;                                          // Set it instead

            SetCharacterPrintSound(name);
            SetCharacterFont(name);
            SetDialogBoxView(name);
        }

        public void SetNarratorView()
        {
            characterName.gameObject.SetActive(false);
            imagePanel.gameObject.SetActive(false);

            SetCharacterPrintSound(narratorName);
            SetCharacterFont(narratorName);
            SetDialogBoxView(narratorName);
        }

        private void SetCharacterPrintSound(string name)
        {
            AudioClip defaultPrintSound = Resources.Load<AudioClip>("Sounds/Interface/DefaultPrintSound");
            defaultPrintSoundEffect = defaultPrintSound;

            if(name != "")
            {
                AudioClip characterPrintSound = Resources.Load<AudioClip>("Sounds/Interface/" + name + "_snd");
                if (characterPrintSound)
                    defaultPrintSoundEffect = characterPrintSound;
                else
                    Debug.Log("NO PRINT EFFECT FOUND");
            }
        }

        private void SetCharacterFont(string name)
        {
            TMP_FontAsset defaultFont = Resources.Load<TMP_FontAsset>("Fonts/DefaultFont");
            contentText.font = defaultFont;
            nameText.font = defaultFont;

            if (name != "")
            {
                TMP_FontAsset characterFont = Resources.Load<TMP_FontAsset>("Fonts/" + name + "_font");
                if (characterFont)
                {
                    contentText.font = characterFont;
                    nameText.font = characterFont;
                } else Debug.Log("NO FONT FOUND");
            }
        }

        private void SetDialogBoxView(string name = "")
        {
            dialogBox.sprite = defaultDialogBox;                                                                            // Sets the default sprite to the dialog box

            // If a name is sent, then search for it
            if(name != ""){
                Sprite actualCharacterDialogBox = Resources.Load<Sprite>("Sprites/Boxes/" + name + "_box");    // Load the sprite with the name provided + "_box"
                if (actualCharacterDialogBox)                                                                  // But what if there's a actual character dialog box sprite?
                    dialogBox.sprite = actualCharacterDialogBox;                                               // Set it instead
            }  
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

            audioSource.clip = defaultPrintSoundEffect;                                // Change print sound effect
            audioSource.volume = characterPrintVolume;                          // Change audio volume
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
            
            // TO DO: put this into recursive loop

            if(itens.Length > 0)
            {
                var nameAndScale = itens[0].Split('*');                                                     // Split name and scale on the '*' mark
                Sprite firstImage = Resources.Load<Sprite>("Sprites/Images/" + CleanString(nameAndScale[0]));      // Loads the sprite
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
                    Sprite secondImage = Resources.Load<Sprite>("Sprites/Images/" + CleanString(nameAndScale[0]));
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
                        Sprite thirdImage = Resources.Load<Sprite>("Sprites/Images/" + CleanString(nameAndScale[0]));
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


