using UnityEngine;

public class ToolInitialTransform : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool initialized = false;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialized = true;
    }

    public void ResetToInitial()
    {
        if (!initialized) return;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
    }
}