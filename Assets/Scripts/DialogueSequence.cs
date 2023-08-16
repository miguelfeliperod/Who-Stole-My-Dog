using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSequence", menuName = "ScriptableObjects/DialogueSequence")]
public class DialogueSequence : ScriptableObject 
{ 
    public List<Dialogue> dialogueList;
}

[Serializable]
public struct Dialogue
{
    public Sprite characterImage;
    public Sprite characterImageTalking;
    public string text;
} 