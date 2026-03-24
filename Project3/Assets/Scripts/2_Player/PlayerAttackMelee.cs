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
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform melee1Hitbox;
    [SerializeField] private Transform melee2Hitbox;
    [SerializeField] private Transform melee3Hitbox;

    // For enemy tracking/targeting
    private readonly Collider[] _outerHits = new Collider[5];
    private Vector3 _target;

    public void Initialize()
    {
        ResetMeleeCombo();
    }

    // Called every frame in "PlayerCombat" when in the "Melee" state
    public void UpdateMeleeAttack(ref CombatState state, float deltaTime)
    {
        // Increment Timers
        _dashTimer += deltaTime;
        _comboTimer += deltaTime;

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
        }
    }

    // Called whenever "melee" button is pressed
    public void TriggerAttack()
    {
        _comboTimer = 0f;
        _dashTimer = 0f;

        _comboCounter++;
    }

    private void ResetMeleeCombo()
    {
        _comboCounter = 0;
        _comboTimer = 0f;
        _dashTimer = 0f;
    }
}
