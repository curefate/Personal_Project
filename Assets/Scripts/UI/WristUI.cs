using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

public class WristUI : MonoBehaviour
{
    public Transform hand;
    public Transform anchor;
    public GameObject uiCanvas;
    public float threshold;
    public Vector3 offset;

    private HapticImpulsePlayer _hapticImpulsePlayer;
    private bool _playedHaptic;

    void Start()
    {
        _hapticImpulsePlayer = hand.GetComponent<HapticImpulsePlayer>();
    }

    void Update()
    {
        float dot = Vector3.Dot(hand.right, Vector3.up);
        uiCanvas.transform.position = anchor.position + offset;

        if (1 - dot > threshold)
        {
            uiCanvas.SetActive(false);
            _playedHaptic = false;
        }
        else
        {
            uiCanvas.SetActive(true);
            if (!_playedHaptic)
            {
                _hapticImpulsePlayer?.SendHapticImpulse(0.5f, 0.2f);
                _playedHaptic = true;
            }
        }
    }
}
