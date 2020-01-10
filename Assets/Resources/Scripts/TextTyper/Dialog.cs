namespace TextTyper
{
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
        private int currentIndex = 0;

        public void CleanDialog()
        {
            dialogueLines.Clear();
            characterNames.Clear();
            action = "";
        }

        public string[] GetNextNameAndLine()
        {
            string[] nameAndLine = new string[2];
            nameAndLine[0] = characterNames[currentIndex];
            nameAndLine[1] = dialogueLines[currentIndex];

            if (++currentIndex == dialogueLines.Count)
                dialogIsOver = true;

            return nameAndLine;
        }

        public bool IsDialogOver()
        {
            return dialogIsOver;
        }

        public void SetDialogActive()
        {
            currentIndex = 0;
            dialogIsOver = false;
        }    
    }
}
