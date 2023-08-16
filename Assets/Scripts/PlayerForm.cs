using UnityEngine;

[CreateAssetMenu(fileName = "PlayerForm", menuName = "ScriptableObjects/PlayerForm")]

public class PlayerForm : ScriptableObject
{
    [SerializeField] public RuntimeAnimatorController animator;
    [SerializeField] public float baseSpeed;
    [SerializeField] public float cooldownAttack;
    [SerializeField] public float cooldownSpecial;

    [SerializeField] public GameObject projectile1;
    [SerializeField] public GameObject projectile2;
    [SerializeField] public GameObject projectile3;
}
