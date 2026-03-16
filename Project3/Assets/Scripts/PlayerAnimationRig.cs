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
        _rigActive = PlayerAttack.Instance.GetState().CurrentAttack == Attack.Ranged;

        if (_rigActive) 
            RaiseArm();
        else 
            LowerArm();
    }

    private void RaiseArm()
    {
        
    }

    private void LowerArm()
    {
        
    }
}
