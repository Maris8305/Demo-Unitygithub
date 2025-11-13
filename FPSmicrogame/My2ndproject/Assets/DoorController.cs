using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Transform door;
    public float openAngle = 90f;
    public float openSpeed = 2f;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = door.rotation;
        openRotation = Quaternion.Euler(door.eulerAngles + Vector3.up * openAngle);
    }

    void Update()
    {
       
        if (isOpen)
            door.rotation = Quaternion.Slerp(door.rotation, openRotation, Time.deltaTime * openSpeed);
        else
            door.rotation = Quaternion.Slerp(door.rotation, closedRotation, Time.deltaTime * openSpeed);
    }

 
    public void ToggleDoor()
    {
        isOpen = !isOpen;
    }
}
