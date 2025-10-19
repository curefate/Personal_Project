using UnityEngine;

public class Floating : MonoBehaviour
{
    public float FloatHeight;
    public float FloatSpeed;
    public float RotationSpeed;
    private Vector3 _startPosition;

    void Start()
    {
        _startPosition = transform.position;
    }

    void Update()
    {
        float newY = Mathf.Sin(Time.time * FloatSpeed) * FloatHeight  + _startPosition.y;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);
    }
}
