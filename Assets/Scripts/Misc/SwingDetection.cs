using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SwingDetection : MonoBehaviour
{
    private IXRSelectInteractor interactor;
    private XRGrabInteractable grab;

    public float swingThreshold;
    public UnityEvent onSwing;

    private Vector3 lastPos;
    private bool isHeld;
    private bool isSwinged;

    private void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        grab.selectEntered.AddListener(OnGrabbed);
        grab.selectExited.AddListener(OnReleased);
    }

    private void OnDisable()
    {
        grab.selectEntered.RemoveListener(OnGrabbed);
        grab.selectExited.RemoveListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        interactor = args.interactorObject;
        lastPos = interactor.transform.parent.transform.localPosition;
        isHeld = true;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        interactor = null;
        isHeld = false;
    }

    private void Update()
    {
        if (!isHeld || interactor == null)
            return;

        Vector3 currentPos = interactor.transform.parent.transform.localPosition;
        float speed = (currentPos - lastPos).magnitude / Time.deltaTime;

        if (speed > swingThreshold && !isSwinged)
        {
            isSwinged = true;
            onSwing.Invoke();
        }

        if (speed < swingThreshold && isSwinged)
        {
            isSwinged = false;
        }

        lastPos = currentPos;
    }
}