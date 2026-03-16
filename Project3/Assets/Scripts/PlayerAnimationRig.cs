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

    public void Initialize()
    {
        shoulderAim.weight = 0f;
        armAim.weight = 0f;
    }

    public void UpdateRig()
    {
        
    }
}
