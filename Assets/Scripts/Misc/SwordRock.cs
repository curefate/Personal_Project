using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(AudioSource))]
public class SwordRock : MonoBehaviour
{
    public GameObject excalibur;
    public AudioClip swordRockAudio;
    public AudioClip stoneCrushAudio;
    public AudioClip cureAudio;
    public TextPusher textPusher;

    private AudioSource _audioSource;
    private EnergyManager _energyManager;

    private float _lastYpos;
    private float _timer;
    private float _pauseTimer;
    private float _checkInterval = .1f;
    private float _threshold = 0.05f;
    private bool _checkSword = true;
    private float _pauseSoundFadeTime = 0.1f;

    void Start()
    {
        _lastYpos = excalibur.transform.position.y;
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = swordRockAudio;
        _audioSource.Play();
        _audioSource.Pause();
        _audioSource.pitch = 1.3f;
        _energyManager = FindFirstObjectByType<EnergyManager>();
    }

    void Update()
    {
        if (!_checkSword) return;
        _timer += Time.deltaTime;
        if (_timer < _checkInterval) return;
        _timer = 0f;

        if (Mathf.Abs(excalibur.transform.position.y - _lastYpos) > _threshold)
        {
            _audioSource.UnPause();
            _pauseTimer = 0f;
        }
        else
        {
            _pauseTimer += Time.deltaTime;
            if (_pauseTimer >= _pauseSoundFadeTime)
            {
                _audioSource.Pause();
            }
        }
        _lastYpos = excalibur.transform.position.y;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.gameObject == excalibur)
        {
            StartCoroutine(ShakeAndSink());
            _audioSource.pitch = 1f;
            _checkSword = false;
            _audioSource.Stop();
            _audioSource.PlayOneShot(stoneCrushAudio);
            _audioSource.PlayOneShot(cureAudio);
            textPusher.PushText("Your energy has been restored...");
            _energyManager.Energy += 100;

            excalibur.transform.parent = null;
            var exrb = excalibur.GetComponent<Rigidbody>();
            exrb.linearDamping = 0;
            exrb.angularDamping = 0.1f;
            exrb.useGravity = true;
            exrb.constraints = RigidbodyConstraints.None;
            excalibur.GetComponent<XRGrabInteractable>().movementType = XRGrabInteractable.MovementType.Instantaneous;

            Destroy(gameObject, 3f);
            Destroy(transform.parent.gameObject, 3f);
        }
    }

    IEnumerator ShakeAndSink()
    {
        while (true)
        {
            var target = transform.position + new Vector3(Random.Range(-0.05f, 0.05f), -0.2f, Random.Range(-0.05f, 0.05f));
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * 5f);
            yield return new WaitForSeconds(0.07f);
        }
    }

    void OnDestroy()
    {
        textPusher.PushText("Release the real power of the sword.\nDispel the darkness!");
    }
}
