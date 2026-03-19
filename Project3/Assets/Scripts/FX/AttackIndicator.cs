using UnityEngine;

public class AttackIndicator : MonoBehaviour
{
    [SerializeField] private Renderer baseRenderer;
    [SerializeField] private Renderer fillRenderer;
    private Material baseMaterial;
    private Material fillMaterial;

    private Transform fillTransform;
    private static readonly int BaseColor = Shader.PropertyToID("_Color");

    void Awake()
    {
        baseMaterial = baseRenderer.material;
        fillMaterial = fillRenderer.material;

        fillTransform = fillRenderer.transform;

        Hide();
    }

    public void Initialize(TrainingDummyStats stats)
    {
        var diameter = stats.zoneAttackRadius * 2f;
        transform.localScale = new Vector3(diameter, 1f, diameter);

        baseMaterial.SetColor(BaseColor, stats.zoneBaseColor);
        fillMaterial.SetColor(BaseColor, stats.zoneFillColor);
    }

    public void UpdateIndicator(float p)
    {
        // Scale from 0 to 1 relative to the parent's scale
        // localScale stays within (0,0,0) to (1,1,1) so it
        // never exceeds the base indicator's size
        float scale = Mathf.Clamp01(p);
        fillTransform.localScale = new Vector3(scale, 1f, scale);
    }

    public void Show()
    {
        baseRenderer.enabled = true;
        fillRenderer.enabled = true;

        fillTransform.localScale = Vector3.zero;
    }

    public void Hide()
    {
        baseRenderer.enabled = false;
        fillRenderer.enabled = false;
    }
}