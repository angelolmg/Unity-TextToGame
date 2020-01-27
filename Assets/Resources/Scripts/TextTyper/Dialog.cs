namespace TextTyper
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public class DialogBucket
    {
        public List<ListDialog> dialogs = new List<ListDialog>();
    }

    public class ListDialog
    {
        public List<string> dialogueLines = new List<string>();
        public List<string> characterNames = new List<string>();
        public string action = "";

        public bool dialogIsOver = false;
        public bool dialogIsOff = false;
        public int currentIndex = 0;

        public void CleanDialog()
        {
            dialogueLines.Clear();
            characterNames.Clear();
            action = "";
        }

        public string[] GetNextNameAndLine()
        {
            if (currentIndex < characterNames.Count)
            {
                string[] nameAndLine = new string[2];

                nameAndLine[0] = characterNames[currentIndex];
                nameAndLine[1] = dialogueLines[currentIndex];

                if (++currentIndex >= dialogueLines.Count)
                    dialogIsOff = true;

                return nameAndLine;
            }
            return null;
        }

        public bool IsDialogOver()
        {
            return dialogIsOver;
        }

        public bool IsDialogOff()
        {
            return dialogIsOff;
        }

        public void SetDialogActive()
        {
            currentIndex = 0;
            dialogIsOver = false;
            dialogIsOff = false;
        }

        public void SetOn()
        {
            dialogIsOff = false;
        }

        public void SetOff()
        {
            dialogIsOff = true;
        }
    }
}
