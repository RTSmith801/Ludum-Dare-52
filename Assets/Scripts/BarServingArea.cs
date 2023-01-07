using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class BarServingArea : MonoBehaviour
{
    Animator anim;
    bool playerInServingArea = false;
    
    void Start()
    {
        anim = transform.gameObject.GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInServingArea = true;
            anim.SetTrigger("PlayerEnter");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInServingArea = false;
            anim.SetTrigger("PlayerExit");
        }
    }
}
