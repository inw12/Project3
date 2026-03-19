using UnityEngine;

public class AttackIndicator : MonoBehaviour
{
    [SerializeField] private Renderer baseRenderer;
    [SerializeField] private Renderer fillRenderer;
    [SerializeField] private Color baseColor;
    [SerializeField] private Color fillColor;

    private Transform fillTransform;

    void Awake()
    {
        baseRenderer.material.color = baseColor;
        fillRenderer.material.color = fillColor;

        fillTransform = fillRenderer.transform;

        Hide();
    }

    public void Initialize(TrainingDummyStats stats, Vector3 position)
    {
        var diameter = stats.zoneAttackRadius * 2f;
        transform.localScale = new Vector3(diameter, 1f, diameter);

        transform.position = position;
    }

    public void UpdateIndicator(float p)
    {
        // Scale from 0 to 1 relative to the parent's scale
        // localScale stays within (0,0,0) to (1,1,1) so it
        // never exceeds the base indicator's size
        float scale = Mathf.Clamp01(p);
        fillTransform.localScale = new Vector3(scale, scale, scale);
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