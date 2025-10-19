using UnityEngine;

public class GameOverCamera : MonoBehaviour
{
    private Camera _camera;

    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    void Update()
    {
        _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, 30f, Time.deltaTime * 0.5f);
    }
}
