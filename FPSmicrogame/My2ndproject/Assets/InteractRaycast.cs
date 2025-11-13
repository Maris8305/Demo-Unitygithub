using UnityEngine;
using TMPro; 

public class InteractRaycast : MonoBehaviour
{
    public float interactRange = 3f;
    public Camera playerCam;
    public TextMeshProUGUI interactionText;

    private DoorController currentDoor;

    void Update()
    {
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            DoorController door = hit.transform.GetComponent<DoorController>();

            if (door != null)
            {
                currentDoor = door;
                interactionText.gameObject.SetActive(true);
                interactionText.text = "Press E to open";

                if (Input.GetKeyDown(KeyCode.E))
                {
                    door.ToggleDoor();
                }
            }
            else
            {
                HideText();
            }
        }
        else
        {
            HideText();
        }
    }

    void HideText()
    {
        if (interactionText.gameObject.activeSelf)
            interactionText.gameObject.SetActive(false);
        currentDoor = null;
    }
}

