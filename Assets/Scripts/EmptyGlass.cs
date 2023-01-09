using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyGlass : MonoBehaviour
{
    GameManager gm;
    Rigidbody rb;
    public float glassMoveSpeed = 1;
    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
		rb = GetComponent<Rigidbody>();
		rb.velocity = new Vector3(1, 0, 0) * glassMoveSpeed;
	}

	private void OnCollisionEnter(Collision collision)
	{
        string tag = collision.transform.tag;

        if (tag == "Player")
            CatchGlass();
        else if (tag == "Floor")
            BreakGlass();
	}

    void CatchGlass()
    {
        Destroy(gameObject);
        gm.am.Play("catch");
        gm.UpdateScoreUI(1);
    }

    void BreakGlass()
    {   
        gm.BrokenGlass();
        // Instantiate broken glass
        Destroy(gameObject);
    }
}
