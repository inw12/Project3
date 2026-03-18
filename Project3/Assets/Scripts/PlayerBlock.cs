using UnityEngine;
public class PlayerBlock : MonoBehaviour
{
    private bool _requestedBlock;

    public void Initialize()
    {
        
    }

    public void UpdateInput(bool input)
    {
        _requestedBlock = input;
    }
}
