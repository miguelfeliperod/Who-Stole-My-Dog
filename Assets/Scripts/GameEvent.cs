using System;
using UnityEngine;

[Serializable]
public class GameEvent
{
    public GameEventType gameEventType;

    public float TimeToWait;

    // DIALOGUE
    public  string text;
    public  Sprite characterImage;
    public  Sprite characterImageTalking;

    // AUDIO
    public AudioClip audioSound;
    public bool isSFX;
    public bool stopMusic;

    // FADE
    public bool isFadeIn;
    public Color fadeColor;

    // GameObject
    public Vector3 destinyPlace;
    public GameObject eventObject;
    public bool isMovement;
    public bool isEnable;
}

public enum GameEventType
{
    Dialogue, Audio, GameObject, WaitEvent, Fade
}