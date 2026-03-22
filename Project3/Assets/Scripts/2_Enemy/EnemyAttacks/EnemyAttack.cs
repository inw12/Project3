using UnityEngine;
public abstract class EnemyAttack : MonoBehaviour
{
    // Attack ID (identification for state machine and animator control)
    [SerializeField] protected int attackID;

    protected int playerHurtbox;
    protected int playerParrybox;
    protected LayerMask playerLayerMask;

    protected readonly Collider[] _hitBuffer = new Collider[10];

    protected virtual void Awake()
    {
        playerHurtbox = LayerMask.NameToLayer("PlayerHurtbox");
        playerParrybox = LayerMask.NameToLayer("PlayerParrybox");
        playerLayerMask = (1 << playerHurtbox) | (1 << playerParrybox);
    }

    public void HandleHit(Collider hit, float damage)
    {
        var layer = hit.gameObject.layer;
        if (layer == playerParrybox)
            OnParryboxHit();
        else if (layer == playerHurtbox)
            OnHurtboxHit(hit.gameObject, damage);
    }

    // "What happens when I hit the player's hurtbox?"
    protected virtual void OnHurtboxHit(GameObject other, float damage) {}    

    // "What happens when I hit the player's parrybox?"
    protected virtual void OnParryboxHit() {}   

    // Called in 'Update()' in 'Enemy.cs'
    public abstract void Attack(Transform target);  

    public virtual void Cancel() {}

    public int GetAttackID() => attackID;
}
