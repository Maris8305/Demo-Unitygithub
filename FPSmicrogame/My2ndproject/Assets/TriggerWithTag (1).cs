using UnityEngine;
using UnityEngine.Events;

[AddComponentMenu("Custom/Trigger With Tag")]
public class TriggerWithTag : MonoBehaviour
{
    [Header("Tag Settings")]
    [Tooltip("Objects with this tag will trigger the events.")]
    public string tagToCheck = "Player";

    [Header("Trigger Events")]
    public UnityEvent onTriggerEnterEvent;
    public UnityEvent onTriggerExitEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(tagToCheck))
        {
            Debug.Log($"{tagToCheck} entered trigger of {gameObject.name}");
            onTriggerEnterEvent?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(tagToCheck))
        {
            Debug.Log($"{tagToCheck} exited trigger of {gameObject.name}");
            onTriggerExitEvent?.Invoke();
        }
    }
}
