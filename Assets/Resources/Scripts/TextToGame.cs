namespace TextTyper
{
    using System;
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine.UI;
    using TMPro;

    public class TextToGame : MonoBehaviour
    {
        private int dialogHeadIndex = 0;                                    // The main index of the actual dialog in display    
        private int[] buttonActionArray = { 0, 0, 0 };                      // Stores the dialog index which a button should play
        private DialogBucket dialogBucket = new DialogBucket();             // Stores the many dialogs <type>List
        private ListDialog dialog = new ListDialog();                       // Stores the many strings of text that a diolog is made <type>Queue
                                                                            // Also stores de characters names and button's actions
        public TextAsset textFile;                                          // The text file that is going to be read                                         
        [Header("UI Reference")]
        public UIManager uiManager;                                         // Object that manages the UI

        [Tooltip("The text typer element to type with for the characters")]
        public TextTyper textTyper;                                         // The text typer handler script for characters
        [Tooltip("The text typer element to type with for the narrator")]
        public TextTyper narratorTextTyper;                                 // The text typer handler script for the narrator

        /// <summary>
        /// Start function, executed one time at the start
        /// </summary>
        public void Start()
        {
            textTyper.PrintCompleted.AddListener(HandlePrintCompleted);                             // Listener of the print completed method
            textTyper.CharacterPrinted.AddListener(uiManager.HandleCharacterPrinted);               // Listener of the character printed method

            narratorTextTyper.PrintCompleted.AddListener(HandlePrintCompleted);                     // Listener of the print completed method
            narratorTextTyper.CharacterPrinted.AddListener(uiManager.HandleCharacterPrinted);       // Listener of the character printed method

            SetButtons();                                                                           // Set all the buttons off initially
            EnqueueTextFile();                                                                      // Enqueue the text file provided, filling the DialogBucket
            ShowScript();                                                                           // Starts showing the dialogs
        }

        /// <summary>
        /// Update function, executed several times through runtime
        /// </summary>
        public void Update()
        {
            // Mouse0 skips then goes to the next text to print
            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space))    
                HandlePrintNextClicked();

            // Mouse1 just goes to the next text, without skipping
            if (Input.GetKeyDown(KeyCode.Mouse1))                                       
                HandlePrintNoSkipClicked();
        }

        /// <summary>
        /// Shows the actual line of dialog, name and portrait of the actual character talking
        /// It handles the dialog lines and names
        /// </summary>
        private void ShowScript()
        {
            ListDialog actualDialog = GetCurrentHeadDialog();
            // If dialog is either: OFF, OVER or is not even there at all, don't show anything.  
            if (actualDialog != null && !actualDialog.IsDialogOver() && !actualDialog.IsDialogOff())            
            {
                // Gets the next character name and line from the actual dialog
                string[] nameAndLine = actualDialog.GetNextNameAndLine();   
                if (nameAndLine[0][0] == '/')
                {
                    HandleCommand(nameAndLine);
                }
                else
                {
                    // Updates the portrait and text to the actual character in screen
                    UpdateTextBox(nameAndLine);                            
                }
            } 
        }

        /// <summary>
        /// Handles the command string[] (wait, change background, play sound, etc) and calls 
        /// the appropriate function with given attribute 
        /// </summary>
        /// <param name="command"></param>
        private void HandleCommand(string[] command)
        {
            string commandName = CleanString(command[0]);
            string attribute = CleanString(command[1]);

            // Switch of implemented commnads
            switch (commandName)
            {
                case "wait":
                case "w":
                    StartCoroutine(Wait(int.Parse(attribute)));
                    break;
                case "changeBackground":
                case "changebackground":
                case "bg":
                    uiManager.ChangeBackground(attribute);
                    HandlePrintCompleted();
                    ShowScript();
                    break;
                case "playsound":
                case "playSound":
                case "ps":
                    uiManager.PlaySoundEffect(attribute);
                    HandlePrintCompleted();
                    ShowScript();
                    break;
                case "playMusic":
                case "playmusic":
                case "pm":
                    PlayMusic(attribute);
                    HandlePrintCompleted();
                    ShowScript();
                    break;
                case "fadein":
                case "fadeIn":
                case "fi":
                    uiManager.FadeIn();
                    HandlePrintCompleted();
                    ShowScript();
                    break;
                case "fadeout":
                case "fadeOut":
                case "fo":
                    uiManager.FadeOut();
                    HandlePrintCompleted();
                    ShowScript();
                    break;
                case "blackout":
                case "blackOut":
                case "bo":
                    uiManager.BlackOut();
                    HandlePrintCompleted();
                    ShowScript();
                    break;
                case "clearout":
                case "clearOut":
                case "co":
                    uiManager.ClearOut();
                    HandlePrintCompleted();
                    ShowScript();
                    break;
                case "hideHud":
                case "hidehud":
                case "hh":
                    uiManager.HideHud();
                    HandlePrintCompleted();
                    ShowScript();
                    break;
                case "showHud":
                case "showhud":
                case "sh":
                    uiManager.ShowHud();
                    HandlePrintCompleted();
                    ShowScript();
                    break;
                default:
                    Debug.LogError("Command not recognized");
                    break;
            }
        }

        /// <summary>
        /// Plays a song in the "Musics/" folder called (string)"song"
        /// Or stops any music playing if (string)"song" == ""
        /// </summary>
        /// <param name="song"></param>
        private void PlayMusic(string song)
        {
            AudioSource audioSource = GetComponent<AudioSource>();

            if(audioSource != null)
            {
                if (audioSource.clip.name == song)
                    return;
                else
                    Destroy(gameObject.GetComponent<AudioSource>());
            }

            if(song != "")
            {
                AudioClip clip = Resources.Load<AudioClip>("Musics/" + song);
                if (clip != null)
                {
                    if (audioSource == null)                                            // If there's none, create one
                        audioSource = gameObject.AddComponent<AudioSource>();

                    audioSource.clip = clip;                                            // Change sound effect
                    audioSource.loop = true;
                    audioSource.volume = 0.04f;
                    audioSource.Play();                                                 // Play the sound effect
                }
            } 
        }

        /// <summary>
        /// Updated the text box and visuals with the character name and line provided
        /// </summary>
        /// <param name="nameAndLine">Array of strings. nameAndLine[0] is the name, nameAndLine[1] is the line</param>
        private void UpdateTextBox(string[] nameAndLine)
        {
            if (nameAndLine[0] != uiManager.narratorName)
            {
                uiManager.SetCharaterView(nameAndLine[0]);

                textTyper.gameObject.SetActive(true);
                narratorTextTyper.gameObject.SetActive(false);
                textTyper.TypeText(nameAndLine[1]);                 // Starts feeding the text typer with the dialog lines
            } else
            {
                uiManager.SetNarratorView();

                textTyper.gameObject.SetActive(false);
                narratorTextTyper.gameObject.SetActive(true);
                narratorTextTyper.TypeText(nameAndLine[1]);
            }
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
                            ListDialog tmp = new ListDialog();                                  // If it is, its the end of the actual dialog
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
                        else if(line[0] == '/')                                                 // If it isn't then it's just a dialog line
                        {
                            var item = line.Split('(');
                            dialog.characterNames.Add(item[0]);
                            dialog.dialogueLines.Add(CleanString(item[1]));
                        }
                        else
                        {
                            var item = line.Split('-');                                         // Splits at '-': "Character Name" - "Dialog line" 
                            dialog.characterNames.Add(CleanString(item[0]));                    // Enqueue character name at the dialog dummy object
                            dialog.dialogueLines.Add(CleanString(item[1]));                     // Enqueue dialog line at the dialog dummy object
                        }
                    }
                }   
            }
        }

        /// <summary>
        /// Stop the current dialog from being skipped for a amount of time.
        /// </summary>
        /// <param name="time"> Time to wait </param>
        /// <returns></returns>
        IEnumerator Wait(int time)
        {
            //Debug.Log("Waiting for " + time + "seconds...");
            GetCurrentHeadDialog().SetOff();
            yield return new WaitForSeconds(time);
            GetCurrentHeadDialog().SetOn();
            //Debug.Log("Stopped waiting " + time + "seconds.");
            HandlePrintCompleted();
            ShowScript();
        }

        /// <summary>
        /// Cleans the returns the string clear of any '(',')','\n' or ' ' used for sintax analisys
        /// </summary>
        /// <param name="str">The string to clean</param>
        /// <returns></returns>
        private string CleanString(string str)
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

        /// <summary>
        /// Sets the dialog head to the one provided by the button action array
        /// </summary>
        /// <param name="index"></param>
        public void HandleButtonActions(int index)
        {
            ListDialog currentDialog = GetCurrentHeadDialog();  // Get current dialog
            dialogHeadIndex = buttonActionArray[index];         // Updates the dialog head index to a new dialog
            CheckIfMenu();
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
            foreach (Button child in uiManager.buttonGrid)                        
                child.gameObject.SetActive(false);

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

                ListDialog currentDialog = GetCurrentHeadDialog();

                // Make a test to see if it's the narrator who is talking
                // If it is him, toogle button position to center option buttons box
                uiManager.ToggleButtonPosition(currentDialog.characterNames[currentDialog.characterNames.Count-1] == uiManager.narratorName);

                // Then split on the ';' marker, option divided in the item[]
                var item = options.Split(';');                          
                if (item.Length > 0)                                    // If there's at least one button:
                {
                    var split = item[0].Split('[');
                    uiManager.buttonGrid[0].GetComponentInChildren<Text>().text = split[0];
                    buttonActionArray[0] = int.Parse(CleanString(split[1]));
                    uiManager.buttonGrid[0].gameObject.SetActive(true);
 
                }

                if (item.Length > 1)                                    // If there's at least two buttons:
                {
                    var split = item[1].Split('[');
                    uiManager.buttonGrid[1].GetComponentInChildren<Text>().text = split[0];
                    buttonActionArray[1] = int.Parse(CleanString(split[1]));
                    uiManager.buttonGrid[1].gameObject.SetActive(true);
                }
                
                if (item.Length > 2)                                    // If there's three buttons:
                {
                    var split = item[2].Split('[');
                    uiManager.buttonGrid[2].GetComponentInChildren<Text>().text = split[0];
                    buttonActionArray[2] = int.Parse(CleanString(split[1]));
                    uiManager.buttonGrid[2].gameObject.SetActive(true);
                }
            } 
        }

        /// <summary>
        /// Prints the rest of the dialog, then skips
        /// </summary>
        private void HandlePrintNextClicked()
        {
            if (textTyper.IsSkippable() && textTyper.IsTyping)
                textTyper.Skip();
            else
                ShowScript();
        }

        /// <summary>
        /// Skips the dialog, without printing the rest of the dialog
        /// </summary>
        private void HandlePrintNoSkipClicked()
        {
            ShowScript();
        }

        /// <summary>
        /// Debug.Log with rich tags
        /// </summary>
        /// <param name="tag"></param>
        private void LogTag(RichTextTag tag)
        {
            if (tag != null)
                Debug.Log("Tag: " + tag.ToString());
        }

        /// <summary>
        /// If there's no dialog lines left on the actual dialog, show the buttons (if any)
        /// </summary>
        private void HandlePrintCompleted()
        {
            ListDialog currentDialog = GetCurrentHeadDialog();

            if (currentDialog.currentIndex >= currentDialog.dialogueLines.Count)
                currentDialog.dialogIsOver = true;

            if (!currentDialog.IsDialogOver())
            {
                // If the next line is a certain command, just show script, dont wait for a mouse click
                string testIfCommand = currentDialog.characterNames[currentDialog.currentIndex].Substring(1);
                switch (testIfCommand)
                {
                    case "wait":
                    case "w":
                        ShowScript();
                        break;
                    case "playsound":
                    case "playSound":
                    case "ps":
                        ShowScript();
                        break;
                    case "playMusic":
                    case "playmusic":
                    case "pm":
                        ShowScript();
                        break;
                }
            }
            else
            {
                //Debug.Log("PRINT COMPLETED");
                SetButtons(dialogBucket.dialogs[dialogHeadIndex].action);
            }        
        }

        /// <summary>
        /// Returns the current dialog based on the global variable "dialogHeadIndex"
        /// Returns null if there's none
        /// </summary>
        /// <returns></returns>
        private ListDialog GetCurrentHeadDialog()
        {
            if(dialogHeadIndex >= 0)
            {
                ListDialog currentDialog = dialogBucket.dialogs[dialogHeadIndex];
                return currentDialog;
            }
            return null;
        }

        /// <summary>
        /// If "dialogHeadIndex" is < 0, it's a send to a different scene command.
        /// This checks if that's the case and does the sending
        /// </summary>
        private void CheckIfMenu()
        {
            if (dialogHeadIndex < 0)
            {
                var menuController = GetComponent<MenuController>();                    // Gets the menu handler 
                if (menuController == null)                                             // If there's none:
                    menuController = gameObject.AddComponent<MenuController>();         // Create one
                menuController.GoToScene(0);                                            // Sends to main menu at build index 0
            }
        }
    }
}

