using System;
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
    [SerializeField] private float comboBuffer;
    private int _comboCounter;
    private float _comboTimer;

    [Header("Unity Components")]
    [SerializeField] private PlayerAnimationController animationController;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform melee1Hitbox;
    [SerializeField] private Transform melee2Hitbox;
    [SerializeField] private Transform melee3Hitbox;

    // For enemy tracking/targeting
    private readonly Collider[] _outerHits = new Collider[5];
    private Vector3 _target;

    private bool _meleeInputEnabled;

    public void Initialize()
    {
        ResetMeleeCombo();
    }

    // Called every frame in "PlayerCombat" when in the "Melee" state
    public void UpdateMeleeAttack(ref CombatState state, ref bool meleeStarted, ref bool meleeInputEnabled, float deltaTime)
    {
        _meleeInputEnabled = meleeInputEnabled;

        if (_meleeInputEnabled)
        {
            // Increment Timers
            _dashTimer += deltaTime;
            _comboTimer += deltaTime;
        }

        // Scan for enemies
        var outerHits = Physics.OverlapSphereNonAlloc
        (
            transform.position,
            meleeOuterRange,
            _outerHits,
            enemyLayer,
            QueryTriggerInteraction.Ignore
        );
        _target = outerHits > 0 ? _outerHits[0].transform.position : Vector3.zero;

        // Exit Melee State once timer exceeds combo input buffer
        if (_comboTimer > comboBuffer) 
        {
            ResetMeleeCombo();
            state.CurrentAction = CombatAction.None;
            meleeStarted = false;
            PlayerMovement.Instance.EnableMovementInput();
        }
    }

    // Called whenever "melee" button is pressed
    public void TriggerAttack()
    {
        if (_meleeInputEnabled)
        {
            _comboTimer = 0f;
            _dashTimer = 0f;

            if (_comboCounter == 3) _comboCounter = 0;
            _comboCounter++;
            _comboCounter = Math.Clamp(_comboCounter, 0, 3);

            animationController.UpdateMeleeAnimation(_comboCounter);
        }
    }

    private void ResetMeleeCombo()
    {
        _comboCounter = 0;
        _comboTimer = 0f;
        _dashTimer = 0f;
    }
}
