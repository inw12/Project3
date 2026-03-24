using UnityEngine;
public class PlayerAttackMelee : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float damage;
    [SerializeField] private float meleeOuterRange;
    [SerializeField] private float meleeInnerRange;

    [Header("Dash Movement")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashAcceleration;
    [SerializeField] private float dashDuration;
    private float _dashTimer;

    [Header("Combo")]
    [SerializeField] private float comboExtensionWindow;
    private int _comboCounter;
    private float _comboTimer;
}
