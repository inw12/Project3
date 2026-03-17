using UnityEngine;
public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float timeToDestroy;
    private void Start() => Destroy(gameObject, timeToDestroy);
}
