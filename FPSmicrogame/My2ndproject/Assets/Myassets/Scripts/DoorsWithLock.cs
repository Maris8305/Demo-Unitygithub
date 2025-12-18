using System.Collections;
using UnityEngine;

public class DoorsWithLock : MonoBehaviour
{
    [Header("Door Settings")]
    public Transform door;
    public float openAngle = 90f;
    public float openSpeed = 2f;

    [Header("Key Settings")]
    public GameObject KeyINV;

    [Header("UI Settings")]
    public GameObject openText;
    public GameObject lockedText;

    [Header("Audio Settings")]
    public AudioSource doorSound;
    public AudioSource lockedSound;

    private bool isOpen = false;
    private bool inReach = false;
    private bool hasKey = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        if (door == null)
        {
            Debug.LogError("DOOR chua duoc gan!");
            return;
        }

        closedRotation = door.rotation;
        openRotation = Quaternion.Euler(door.eulerAngles + Vector3.up * openAngle);

        if (openText != null) openText.SetActive(false);
        if (lockedText != null) lockedText.SetActive(false);

        inReach = false;
        hasKey = false;
    }

    void Update()
    {
        if (KeyINV != null && KeyINV.activeInHierarchy)
        {
            hasKey = true;
        }
        else
        {
            hasKey = false;
        }

        if (isOpen)
        {
            door.rotation = Quaternion.Slerp(door.rotation, openRotation, Time.deltaTime * openSpeed);
        }
        else
        {
            door.rotation = Quaternion.Slerp(door.rotation, closedRotation, Time.deltaTime * openSpeed);
        }

        if (inReach && Input.GetKeyDown(KeyCode.E))
        {
            if (hasKey)
            {
                isOpen = !isOpen;

                if (doorSound != null && !doorSound.isPlaying)
                {
                    doorSound.Play();
                }
            }
            else
            {
                if (lockedSound != null)
                {
                    lockedSound.Play();
                }

                if (lockedText != null)
                {
                    StartCoroutine(BlinkLockedText());
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = true;

            if (hasKey)
            {
                if (openText != null) openText.SetActive(true);
                if (lockedText != null) lockedText.SetActive(false);
            }
            else
            {
                if (openText != null) openText.SetActive(false);
                if (lockedText != null) lockedText.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;

            if (openText != null) openText.SetActive(false);
            if (lockedText != null) lockedText.SetActive(false);
        }
    }

    IEnumerator BlinkLockedText()
    {
        if (lockedText == null) yield break;

        for (int i = 0; i < 3; i++)
        {
            lockedText.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            lockedText.SetActive(true);
            yield return new WaitForSeconds(0.1f);
        }
    }
}