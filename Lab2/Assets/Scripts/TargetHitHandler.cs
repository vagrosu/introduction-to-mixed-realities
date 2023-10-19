using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class TargetHitHandler : MonoBehaviour
{
    public UnityEvent onTargetHit;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<XRGrabInteractable>())
        {
            onTargetHit?.Invoke();
        }
    }
}
