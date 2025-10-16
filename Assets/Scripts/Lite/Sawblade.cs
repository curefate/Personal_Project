using UnityEngine;

public class Sawblade : MonoBehaviour
{
    public float Speed;
    public float RotateSpeed;
    public Transform Lpoint;
    public Transform Rpoint;
    public int Damage;

    private Transform targetPoint;

    void Start()
    {
        targetPoint = Rpoint;
    }

    void Update()
    {
        float step = Speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, step);

        transform.Rotate(transform.forward, RotateSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.001f)
        {
            targetPoint = targetPoint == Rpoint ? Lpoint : Rpoint;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // TODO
    }
}
