namespace TextTyper
{
    using System;
    using System.Collections.Generic;

    public class DialogBucket
    {
        public List<Dialog> dialogs = new List<Dialog>();
    }

    public class Dialog
    {
        public Queue<string> dialogueLines;
        public Queue<string> characterNames;
        public string action;

        public Dialog()
        {
            dialogueLines = new Queue<string>();
            characterNames = new Queue<string>();
            action = "";
        }

        public void CleanDialog()
        {
            dialogueLines.Clear();
            characterNames.Clear();
            action = "";
        }
    }
}
