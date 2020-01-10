namespace TextTyper
{
    using UnityEngine;
    using System.Collections.Generic;
    using UnityEngine.UI;
    using TMPro;

    public class TextToGame : MonoBehaviour
    {
        private int dialogHeadIndex = 0;                                // The main index of the actual dialog in display    
        private int[] buttonActionArray = { 0, 0, 0 };                  // Stores the dialog index which a button should play

        private DialogBucket dialogBucket = new DialogBucket();         // Stores the many dialogs <type>List
        //private Dialog dialog = new Dialog();                           // Stores the many strings of text that a diolog is made <type>Queue
                                                                        // Also stores de characters names and button's actions
        private ListDialog dialog = new ListDialog();

        public TextAsset textFile;                                      // The text file that is going to be read                                         
        public AudioClip printSoundEffect;                              // Print character sound effect

        [Header("UI References")]
        public Button firstButton;                                      // The three UI buttons
        public Button secondButton;
        public Button thirdButton;
        public Image imagePanel;                                        // The image panel. Character portrait
        public TextMeshProUGUI characterName;                           // The text box for the character's name

        private Sprite defaultSprite;                                   // The default character sprite. 
                                                                        // It's displayed when non-other is available

        [Tooltip("The text typer element to type with")]
        public TextTyper textTyper;                                     // The text typer handler script
            
        /// <summary>
        /// Start function, executed one time at the start
        /// </summary>
        public void Start()
        {
            textTyper.PrintCompleted.AddListener(HandlePrintCompleted);             // Listener of the print completed method
            textTyper.CharacterPrinted.AddListener(HandleCharacterPrinted);         // Listener of the character printed method

            //firstButton.onClick.AddListener(() => HandleButtonActions(0));          // Listener of the buttons
            //secondButton.onClick.AddListener(() => HandleButtonActions(1));         // It handles the click by calling the handling method
            //thirdButton.onClick.AddListener(() => HandleButtonActions(2));          // With the argument of its index

            defaultSprite = Resources.Load<Sprite>("Sprites/Default");              // Loads the default sprite

            SetButtons();                                                           // Set all the buttons off initially
            EnqueueTextFile();                                                      // Enqueue the text file provided, filling the DialogBucket
            ShowScript();                                                           // Starts showing the dialogs
        }

        /// <summary>
        /// Update function, executed several times through runtime
        /// </summary>
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))                                   // Mouse0 skips then goes to the next text to print
                HandlePrintNextClicked();
            if (Input.GetKeyDown(KeyCode.Mouse1))                                   // Mouse1 just goes to the next text, without skipping
                HandlePrintNoSkipClicked();
        }

        /// <summary>
        /// Shows the actual line of dialog, name and portrait of the actual character talking
        /// It Dequeues the dialog lines and names
        /// </summary>
        private void ShowScript()
        {
            ListDialog actualDialog = GetCurrentHeadDialog();
            if (!actualDialog.IsDialogOver())                                          // If there's no more dialog lines, stop
            {
                string[] nameAndLine = actualDialog.GetNextNameAndLine();

                UpdateCharacterVisual(nameAndLine[0]);                        // Updates the visual aspect (name, portrait) to the actual character in screen
                textTyper.TypeText(nameAndLine[1]);                           // Starts feeding the text typer with the dialog lines
            } 
        }

        /// <summary>
        /// Updates the visual aspect (name, portrait) to the actual character in screen
        /// </summary>
        /// <param name="actualDialog"></param>
        private void UpdateCharacterVisual(string character)
        {
            characterName.text = character + ":";                                               // Sets the character's name 

            Sprite actualCharacterSprite = Resources.Load<Sprite>("Sprites/" + character);      // Load the sprite with the name provided                                                                              
            imagePanel.sprite = defaultSprite;                                                  // Sets the default sprite to the portrait
            if (actualCharacterSprite)                                                          // But what if there's a actual character sprite?
                imagePanel.sprite = actualCharacterSprite;                                      // Set it instead
        }

        /// <summary>
        /// Equeues the .txt file with the dialog, sintax sensetive.
        /// Lines starting with '#' are comments, lines starting with '(' are button toggle commands
        /// </summary>
        private void EnqueueTextFile()
        {
            string theWholeFileAsOneLongString = textFile.text;                                 // Gets text file as a one long string
            List<string> eachLine = new List<string>();                                         // Creates a list of string to store the lines of text
            eachLine.AddRange(theWholeFileAsOneLongString.Split('\n'));                         // Splits (at line break) then adds the lines to the list

            foreach (string line in eachLine)                                                   // Loops through each line of text
            {
                if(line.Length > 1)                                                             // Checks if theres any characters
                {
                    if (line[0] != '#')                                                         // Checks if the line begins with '#' (a comment, should not do anything)
                    {
                        if (line[0] == '(')                                                     // Check if it is a action/toggle button command
                        {
                            ListDialog tmp = new ListDialog();                                          // If it is, its the end of the actual dialog
                            tmp.action = line;                                                  // Creates a new tmp dialog, update the action as the last line
                            int index = dialog.dialogueLines.Count;
                            for (int i = 0; i < index; i++)                                     // Dequeue everything inside this new tmp dialog
                            {
                                tmp.characterNames.Add(dialog.characterNames[i]);
                                tmp.dialogueLines.Add(dialog.dialogueLines[i]);
                            }
                            dialog.CleanDialog();
                            dialogBucket.dialogs.Add(tmp);                                      // Add it to the dialog bucket for future use
                        }
                        else                                                                    // If it isn't then it's just a dialog line
                        {
                            var item = line.Split('-');                                         // Splits at '-': "Character Name" - "Dialog line" 

                            dialog.characterNames.Add(CleanString(item[0]));                // Enqueue character name at the dialog dummy object
                            dialog.dialogueLines.Add(CleanString(item[1]));                 // Enqueue dialog line at the dialog dummy object
                        }
                    }
                }   
            }
        }

        /// <summary>
        /// Cleans the returns the string clear of any '(',')','\n' or ' ' used for sintax analisys
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string CleanString(string str)
        {
            if (str[0] == ' ' || str[0] == '(')
                str = str.Substring(1);
            if (str[str.Length - 1] == ' ' || str[str.Length - 1] == '\n')
                str = str.Substring(0, str.Length - 1);
            if (str[str.Length - 2] == ')')
                str = str.Substring(0, str.Length - 2);

            return str;
        }


        /// <summary>
        /// Sets the dialog head to the one provided by the button action array
        /// </summary>
        /// <param name="index"></param>
        public void HandleButtonActions(int index)
        {
            ListDialog currentDialog = GetCurrentHeadDialog();  // Get current dialog
            dialogHeadIndex = buttonActionArray[index];         // Updates the dialog head index to a new dialog
            currentDialog.SetDialogActive();                    // Reset the last dialog, could be useful later
            SetButtons();                                       // Set the buttons off
            ShowScript();                                       // Shows next line of dialog, pointed by the new index
        }

        /// <summary>
        /// Show the buttons using a action string [sintax: "(option 1, option 2, option 3)"]
        /// Or hides if no string is sent
        /// </summary>
        /// <param name="rawOptions"></param>
        private void SetButtons(string rawOptions = "")
        {
            firstButton.gameObject.SetActive(false);                    // Hides the buttons
            secondButton.gameObject.SetActive(false);
            thirdButton.gameObject.SetActive(false);

            if (rawOptions != "")                                       // If no string is sent, just leave everything off              
            {
                string options = CleanString(rawOptions);               // Clean raw string

                if(options[0] == '[')
                {
                    int num = int.Parse(options.Substring(1, options.Length - 2));
                    GetCurrentHeadDialog().SetDialogActive();
                    dialogHeadIndex = num;       
                    return;
                }

            
                var item = options.Split(';');                          // Then split on the ';' marker, option divided in the item[]
                if (item.Length > 0)                                    // If there's at least one button:
                {
                    firstButton.GetComponentInChildren<Text>().text = item[0].Substring(0,item[0].Length-3);        // Change button text
                    buttonActionArray[0] = int.Parse(item[0].Substring(item[0].Length - 2, 1));                     // Change button action index
                    firstButton.gameObject.SetActive(true);                                                         // Enable/show the button
                }

                if (item.Length > 1)                                    // If there's at least two buttons:
                {
                    secondButton.GetComponentInChildren<Text>().text = item[1].Substring(0, item[1].Length - 3);    // Change button text
                    buttonActionArray[1] = int.Parse(item[1].Substring(item[1].Length - 2, 1));                     // Change button action index
                    secondButton.gameObject.SetActive(true);                                                        // Enable/show the button
                }
                
                if (item.Length > 2)                                    // If there's three buttons:
                {
                    thirdButton.GetComponentInChildren<Text>().text = item[2].Substring(0, item[2].Length - 3);     // Change button text
                    buttonActionArray[2] = int.Parse(item[2].Substring(item[2].Length - 2, 1));                     // Change button action index
                    thirdButton.gameObject.SetActive(true);                                                         // Enable/show the button
                }
            } 
        }

        /// <summary>*
        /// Prints the rest of the dialog, then skips
        /// </summary>
        private void HandlePrintNextClicked()
        {
            if (textTyper.IsSkippable() && textTyper.IsTyping)
            {
                textTyper.Skip();
            }
            else
            {
                ShowScript();
            }
        }

        /// <summary>*
        /// Skips the dialog, without printing the rest of the dialog
        /// </summary>
        private void HandlePrintNoSkipClicked()
        {
            ShowScript();
        }

        /// <summary>*
        /// Debug.Log with rich tags
        /// </summary>
        /// <param name="tag"></param>
        private void LogTag(RichTextTag tag)
        {
            if (tag != null)
                Debug.Log("Tag: " + tag.ToString());
        }

        /// <summary>
        /// Handles the sound of each printed character
        /// </summary>
        /// <param name="printedCharacter"></param>
        private void HandleCharacterPrinted(string printedCharacter)
        {
            if (printedCharacter == " " || printedCharacter == "\n")            // Do not play a sound for whitespace
                return;
            
            var audioSource = GetComponent<AudioSource>();                      // Gets the audio source 
            if (audioSource == null)                                            // If there's none, create one
                audioSource = gameObject.AddComponent<AudioSource>();
            
            audioSource.clip = printSoundEffect;                                // Change print sound effect
            audioSource.Play();                                                 // Play the sound effect
        }

        /// <summary>
        /// If there's no dialog lines left on the actual dialog, show the buttons (if any)
        /// </summary>
        private void HandlePrintCompleted()
        {
            ListDialog currentDialog = GetCurrentHeadDialog();
            if (currentDialog.IsDialogOver())
                SetButtons(dialogBucket.dialogs[dialogHeadIndex].action);
        }

        private ListDialog GetCurrentHeadDialog()
        {
            ListDialog currentDialog = dialogBucket.dialogs[dialogHeadIndex];
            return currentDialog;
        }
    }
}

