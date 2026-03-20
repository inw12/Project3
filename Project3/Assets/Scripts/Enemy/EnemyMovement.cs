/// -------------------------------------------
/// Enemy
///   L *EnemyMovement (this script)
/// -------------------------------------------
/// 
/// ** This script will be used to control the enemy's movement via. CharacterController from other scripts
/// 

using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class EnemyMovement : MonoBehaviour
{
    private CharacterController _controller;

    private float _moveSpeed;
    private Vector3 _currentVelocity;

    public void Initialize(float speed)
    {
        _controller = GetComponent<CharacterController>();
        _moveSpeed = speed;
    }

    // Will be called in 'FixedUpdate()' in 'Enemy.cs'
    public void UpdateMovement()
    {
        
    }

    public void UpdateVelocity(Vector3 targetVelocity)
    {
        _currentVelocity = targetVelocity;
    }

    public void StopMovement()
    {
        _currentVelocity = Vector3.zero;
    }
}
