using UnityEngine;
public class EnemyDummyHitFeedback : MonoBehaviour
{
    [SerializeField] private Transform enemyModel;

    [Header("Emission")]
    [SerializeField] private Color hitColor;
    [SerializeField] private float hitBrightness;
    [SerializeField] private float effectSpeed;
    private Color _defaultColor;
    private Material _material;

    [Header("Scale Pulse")]
    [SerializeField] private float pulseAmount = 0.15f;
    [SerializeField] private float shrinkSpeed = 25f;
    [SerializeField] private float growSpeed = 25f;
    private Vector3 _defaultScale;
    private Vector3 _scaleOffset;

    [Header("Particles")]
    [SerializeField] private GameObject hitParticles;
    [SerializeField] private GameObject deathParticles;
    [SerializeField] private float deathSpeed;
    private bool _deathEffectTriggered = false;  

    public void Initialize()
    {
        // Emission
        if (enemyModel.TryGetComponent(out MeshRenderer mesh)) {
            _material = mesh.material;
        }
        _material.EnableKeyword("_EMISSION");
        _defaultColor = new(0, 0, 0);

        // Scale
        _defaultScale = transform.localScale;
    }

    public void UpdateEnemyModel(bool isAlive)
    {
        // Emission Lerp
        Color current = _material.GetColor("_EmissionColor");
        Color next = Color.Lerp(current, _defaultColor, Time.deltaTime * effectSpeed);
        _material.SetColor("_EmissionColor", next);

        // Scale Lerp
        if (isAlive)
        {
            // Return to normal scale
            enemyModel.localScale = _defaultScale + _scaleOffset;
            _scaleOffset = Vector3.Lerp(_scaleOffset, Vector3.zero, Time.deltaTime * growSpeed);
        }
        else
        {
            // Shrink to nothingness
            enemyModel.localScale = _defaultScale - _scaleOffset;
            _scaleOffset = Vector3.Lerp(_scaleOffset, Vector3.one, Time.deltaTime * deathSpeed);
            if (enemyModel.localScale.y < 0.25f)
            {
                if (!_deathEffectTriggered)
                {
                    _deathEffectTriggered = true;

                    // Death Particle Effect
                    _ = Instantiate(deathParticles, transform.position, Quaternion.identity);

                    // Destroy Object
                    Destroy(enemyModel.gameObject);
                }
            }
        }
    }

    // Without Particles
    public void TriggerHitFeedback()
    {
        _scaleOffset = Vector3.one * -pulseAmount;
        _material.SetColor("_EmissionColor", hitColor * hitBrightness);
    }
    // With Particles
    public void TriggerHitFeedback(Vector3 point, Vector3 normal)
    {
        _scaleOffset = Vector3.one * -pulseAmount;
        _material.SetColor("_EmissionColor", hitColor * hitBrightness);
        _ = Instantiate(hitParticles, point, Quaternion.LookRotation(normal));
    }
}
