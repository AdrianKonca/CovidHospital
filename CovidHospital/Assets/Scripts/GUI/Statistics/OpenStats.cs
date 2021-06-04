using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OpenStats : MonoBehaviour
{
    Text txt;
    public GameObject stats;
    private void Start()
    {
        if (stats.activeSelf)
            stats.SetActive(false);
    }
    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit;
            if (hit = Physics2D.Raycast(mousePos2D, Vector2.zero))
            {
                if (hit.rigidbody)
                {
                    openStatistics(hit);
                }
            }
        }
    }

    private void openStatistics(RaycastHit2D hit)
    {
        if (stats.activeSelf)
            stats.SetActive(false);
        else
            stats.SetActive(true);
        txt = stats.transform.GetChild(1).GetComponent<Text>();
        //txt.text = hit.transform.gameObject.GetComponent<>();
        
    }
    public void CloseWindow()
    {
        stats.SetActive(false);
    }
}

