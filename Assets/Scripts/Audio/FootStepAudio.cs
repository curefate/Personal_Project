using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FootStepAudio : MonoBehaviour
{
    public AudioClip footStepAudio;
    public AudioClip teleportAudio;
    public float stepLength;

    private AudioSource _audioSource;
    private CharacterController _characterController;
    private Vector3 _lastPosition;
    private float _movedDistance;

    void Start()
    {
        _audioSource = Camera.main.TryGetComponent<AudioSource>(out var source) ? source : Camera.main.AddComponent<AudioSource>();
        _characterController = GetComponent<CharacterController>();
        _lastPosition = transform.position;
    }

    void Update()
    {
        if (_characterController.isGrounded)
        {
            _movedDistance += Vector3.Distance(_lastPosition, transform.position);
            _lastPosition = transform.position;

            if (_movedDistance >= 3 * stepLength)
            {
                _audioSource.PlayOneShot(teleportAudio);
                _movedDistance = 0f;
            }

            if (_movedDistance >= stepLength)
            {
                _audioSource.PlayOneShot(footStepAudio);
                _movedDistance = 0f;
            }
        }
    }
}
