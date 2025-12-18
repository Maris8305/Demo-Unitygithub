using UnityEngine;
using System.Collections;

public class BookInteraction : InteractibleObject
{
    [Header("References")]
    public Animator bookAnimator;

    [Header("Animation Timing")]
    public float openAnimationDuration = 2.5f;
    public float closeAnimationDuration = 1.5f;

    [Header("Interaction")]
    public KeyCode interactionKey = KeyCode.E;

    private bool isOpen = false;
    private bool isAnimating = false;
    private bool playerInRange = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        Debug.Log("BookInteraction Start - Object: " + gameObject.name);

        if (bookAnimator == null)
        {
            bookAnimator = GetComponent<Animator>();
            if (bookAnimator == null)
            {
                bookAnimator = GetComponentInChildren<Animator>();
            }
        }

        if (bookAnimator != null)
        {
            Debug.Log("Animator found: " + bookAnimator.gameObject.name);
            bookAnimator.SetBool("IsOpening", false);
            bookAnimator.SetBool("IsClosing", false);
        }
        else
        {
            Debug.LogError("NO ANIMATOR!");
        }

        
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Debug.Log("Collider: " + col.GetType() + ", IsTrigger: " + col.isTrigger);
        }
        else
        {
            Debug.LogError("No COLLIDER!");
        }
    }

    void Update()
    {
        
        if (playerInRange && Input.GetKeyDown(interactionKey))
        {
            OnInteracted();
        }
    }

   
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter called! Object: " + other.name + ", Tag: " + other.tag);

        if (other.CompareTag("Player") || other.CompareTag("Reach"))
        {
            playerInRange = true;
            Debug.Log("Player on trigger");
        }
        else
        {
            Debug.Log("Not player, tag: " + other.tag);
        }
    }

   
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Reach"))
        {
            playerInRange = false;
            Debug.Log("Player out of trigger");
        }
    }

    public override void OnInteracted()
    {
        Debug.Log("OnInteracted được gọi! isOpen = " + isOpen);

        if (isAnimating) return;

        if (!isOpen)
        {
            StartCoroutine(OpenBookSequence());
        }
        else
        {
            StartCoroutine(CloseBookSequence());
        }
    }

    IEnumerator OpenBookSequence()
    {
        isAnimating = true;
        Debug.Log("OpenBook");

        if (bookAnimator != null)
        {
            bookAnimator.SetBool("IsOpening", true);
            bookAnimator.SetBool("IsClosing", false);
        }

        yield return new WaitForSeconds(openAnimationDuration);

        isOpen = true;
        isAnimating = false;

        if (bookAnimator != null)
        {
            bookAnimator.SetBool("IsOpening", false);
        }

        Debug.Log("FinishOpening");
    }

    IEnumerator CloseBookSequence()
    {
        isAnimating = true;
        Debug.Log("CloseBook");

        if (bookAnimator != null)
        {
            bookAnimator.SetBool("IsClosing", true);
            bookAnimator.SetBool("IsOpening", false);
        }

        yield return new WaitForSeconds(closeAnimationDuration);

        transform.position = originalPosition;
        transform.rotation = originalRotation;

        isOpen = false;
        isAnimating = false;

        if (bookAnimator != null)
        {
            bookAnimator.SetBool("IsClosing", false);
        }

        Debug.Log("FinishClosing");
    }
}