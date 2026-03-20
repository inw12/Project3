using UnityEngine;
public abstract class EnemyAttack : MonoBehaviour
{
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

    protected void HandleHit(Collider hit, float damage)
    {
        var layer = hit.gameObject.layer;
        if (layer == playerHurtbox)
            OnHurtboxHit();
        else if (layer == playerParrybox)
            OnParryboxHit();
    }

    protected virtual void OnHurtboxHit() {}    // "What happens when I hit the player's hurtbox?"
    protected virtual void OnParryboxHit() {}   // "What happens when I hit the player's parrybox?"

    public abstract void Attack();      // child classes MUST implement this method
    public virtual void Cancel() {}     // child classes MAY implement this method
}
