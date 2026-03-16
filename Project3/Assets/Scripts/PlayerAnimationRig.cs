///
/// *** This script controls the animation rig
///     that controls the character's LEFT ARM
///     to shoot projectiles
/// 
using UnityEngine;
using UnityEngine.Animations.Rigging;
public class PlayerAnimationRig : MonoBehaviour
{
    [SerializeField] private MultiAimConstraint shoulderAim;
    [SerializeField] private TwoBoneIKConstraint armAim;
    [Space]
    [SerializeField] private Transform target;
    [SerializeField] private Transform hint;
    [Space]
    [SerializeField] [Range(0f, 100f)] private float targetWeight = 100f;
    [SerializeField] private float animationSpeed = 10f;
    [SerializeField] private float elbowOffset = 0.5f;

    private AttackState _state;
    private bool _rigActive;

    public void Initialize()
    {
        shoulderAim.weight = 0f;
        armAim.weight = 0f;

        target.position = Vector3.zero;
        hint.position = Vector3.zero;

        _rigActive = false;
    }

    // This rig will toggle between ON and OFF depending
    //  on if the player is currently performing a ranged attack
    public void UpdateRig()
    {
        _state = PlayerAttack.Instance.GetState();

        _rigActive = _state.CurrentAttack == Attack.Ranged;
        if (_rigActive) 
            RaiseArm();
        else 
            LowerArm();
    }

    private void RaiseArm()
    {
        // Update TARGET position
        target.position = _state.AttackPosition;

        // Update HINT position
        var shoulder = shoulderAim.data.constrainedObject.transform;
        var direction = (target.position - shoulder.position).normalized;
        var targetPosition = shoulder.position
                            + direction * 0.25f             // slightly forward
                            + Vector3.down * -elbowOffset;  // slightly backward
        hint.position = targetPosition;

        // Lerp WEIGHT values to 100
        shoulderAim.weight = armAim.weight = Mathf.Lerp
        (
            shoulderAim.weight,
            targetWeight,
            1f - Mathf.Exp(-animationSpeed * Time.deltaTime)
        );
    }

    private void LowerArm()
    {
        // * Lerp WEIGHT values to 0
        shoulderAim.weight = armAim.weight = Mathf.Lerp
        (
            shoulderAim.weight,
            0f,
            1f - Mathf.Exp(-animationSpeed * Time.deltaTime)
        );
    }
}
