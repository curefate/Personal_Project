using UnityEngine;

public class UILookAtCamera : MonoBehaviour
{
    private Transform camTransform;

    void Start()
    {
        camTransform = Camera.main.transform;
    }

    void Update()
    {
        Quaternion target = Quaternion.LookRotation(transform.position - camTransform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 10f);
    }
}
