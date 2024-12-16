using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDialogueData", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public class DialogueEntry
    {
        public string lines;
        public AudioClip voiceLine;
        public HighlightInfo[] highlights;
    }

    [Header("Dialogue Content")]
    public List<DialogueEntry> dialogueEntries = new List<DialogueEntry>();

    [System.Serializable]
    public class HighlightInfo
    {
        public string phrase;
        public Color highlightColor;
    }
}