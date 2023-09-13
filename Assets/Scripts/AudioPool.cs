using UnityEngine;

[CreateAssetMenu(fileName = "AudioPool", menuName = "ScriptableObjects/AudioPool")]
public class AudioPool : ScriptableObject
{
    [Space(25.0f)]
    [Header("MUSICS")]
    [Space(25.0f)]
    public AudioClip Menu;
    public AudioClip Level1;
    public AudioClip Level2;
    public AudioClip Level3;
    public AudioClip Boss;
    public AudioClip Win;
    public AudioClip Lose;
    public AudioClip Credits;
    public AudioClip Proposal;
}
