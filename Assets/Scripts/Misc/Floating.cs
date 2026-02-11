using UnityEngine;

public class Floating : MonoBehaviour
{
    public float FloatHeight;
    public float FloatSpeed;
    public float RotationSpeed;
    private Vector3 _startPosition;
    public bool ifFollowParent;

    void Start()
    {
        _startPosition = transform.position;
    }

    void Update()
    {
        if (ifFollowParent && transform.parent != null)
        {
            _startPosition = transform.parent.position;
        }
        float newY = Mathf.Sin(Time.time * FloatSpeed) * FloatHeight + _startPosition.y;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);
    }
}
