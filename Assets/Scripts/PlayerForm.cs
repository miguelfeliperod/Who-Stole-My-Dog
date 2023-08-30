using UnityEngine;

[CreateAssetMenu(fileName = "PlayerForm", menuName = "ScriptableObjects/PlayerForm")]

public class PlayerForm : ScriptableObject
{
    [SerializeField] public RuntimeAnimatorController animator;
    [SerializeField] public float baseAcceleration;
    [SerializeField] public float maxSpeed;
    [SerializeField] public float jumpForce;
    [SerializeField] public float cooldownAttack;
    [SerializeField] public float cooldownSpecial;
    [SerializeField] public float manaRegenRate;
    [SerializeField] public float hungryDepletionRate;
    [SerializeField] public float attackMPCost;
    [SerializeField] public float specialMPCost;

    [SerializeField] public GameObject projectile1;
    [SerializeField] public GameObject projectile2;
    [SerializeField] public GameObject projectile3;
}
