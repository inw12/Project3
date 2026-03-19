using UnityEngine;
[CreateAssetMenu(fileName = "TrainingDummyStats", menuName = "EnemyStats/TrainingDummy")]
public class TrainingDummyStats : ScriptableObject
{
    [Header("Game Components")]
    public GameObject projectile;
    public LayerMask targetLayer;
    [Header("Basic Ranged Attack")]
    public float basicRangedDamage = 1f;
    public float basicRangedSpeed = 10f;
    public float basicRangedFireRate = 1f;
    public float basicRangedRange = 25f;
    [Header("Focused Ranged Attack")]
    public float focusedRangedDamage = 5f;
    public float focusedRangedSpeed = 50f;
    public float focusedRangedRange = 100f;
    public float focusedRangedChargeTime = 3f;
    [Header("Melee Attack")]
    public float meleeDamage = 2f;
    [Header("Zone Attack")]
    public float zoneAttackDamage = 5f;
    public float zoneAttackRadius = 15f;
    public float zoneAttackChargeTime = 2.5f;
}
