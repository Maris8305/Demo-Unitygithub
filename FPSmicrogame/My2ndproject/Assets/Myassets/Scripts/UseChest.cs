using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseChest : MonoBehaviour
{
    private GameObject OB;
    public GameObject handUI;
    public GameObject objToActivate;
    private bool inReach;
    private bool isOpened = false; // Thêm cái này!

    void Start()
    {
        OB = this.gameObject;
        handUI.SetActive(false);

        if (objToActivate != null)
        {
            objToActivate.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isOpened) return; // Đã mở rồi thì không làm gì

        Debug.Log($"[CHEST] Something touched me! Name: {other.gameObject.name}, Tag: {other.tag}");

        if (other.gameObject.tag == "Reach")
        {
            Debug.Log("[CHEST] ReachTool detected! inReach = TRUE");
            inReach = true;
            handUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (isOpened) return;

        if (other.gameObject.tag == "Reach")
        {
            Debug.Log("[CHEST] ReachTool left! inReach = FALSE");
            inReach = false;
            handUI.SetActive(false);
        }
    }

    void Update()
    {
        if (isOpened) return; // Đã mở rồi thì không làm gì nữa

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log($"[CHEST] Mouse clicked! inReach = {inReach}");
        }

        if (inReach && Input.GetMouseButtonDown(0))
        {
            OpenChest();
        }
    }

    void OpenChest()
    {
        if (isOpened) return;

        isOpened = true;
        Debug.Log("[CHEST] OPENING NOW!");

        // TẮT COLLIDER NGAY
        BoxCollider boxCollider = OB.GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }

        // TẮT UI
        handUI.SetActive(false);
        inReach = false;

        // BẬT OBJECT
        if (objToActivate != null)
        {
            objToActivate.SetActive(true);
        }

        // CHẠY ANIMATION
        Animator animator = OB.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("open", true);
            // animator.SetTrigger("open"); // Uncomment nếu dùng Trigger

            Debug.Log("[CHEST] Animation SetBool('open', true) called!");
        }
        else
        {
            Debug.LogError("[CHEST] NO ANIMATOR FOUND!");
        }

       
        this.enabled = false;
    }
}