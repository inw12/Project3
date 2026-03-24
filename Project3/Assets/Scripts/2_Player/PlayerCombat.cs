using UnityEngine;
public struct CombatInput
{
    public bool Ranged;
    public bool Melee;
    public Vector2 MousePosition;
}
public struct CombatState
{
    public AttackType CurrentAttack;
    public Vector3 Target;
}
public enum AttackType
{
    None, Ranged, Melee
}
public class PlayerCombat : MonoBehaviour
{
    public static PlayerCombat Instance { get; private set; }

    private bool _combatInputEnabled;

    // Requested Inputs
    private bool _requestedRanged;
    private bool _requestedMelee;
    private Vector2 _requestedMousePos;

    // Melee Variables
    private bool _comboActive;

    void Awake()
    {
        // Singleton Initialization
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Initialize()
    {
        _combatInputEnabled = true;
    }

    public void UpdateInput(CombatInput input)
    {
        if (_combatInputEnabled)
        {
            _requestedMousePos = input.MousePosition;

            // Ranged attack should only be available if the button is pressed
            // AND we're not in the middle of a melee attack string
            _requestedRanged = input.Ranged && !_comboActive;

            // Melee attack should only be available if the button is pressed
            // AND we're not firing ranged attacks
            _requestedMelee = input.Melee && !_requestedRanged;
        }       
    }
}
