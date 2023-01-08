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

    // Update is called once per frame
    void Update()
    {
        
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
    }

    void BreakGlass()
    {
        gm.am.Play("GlassBreak");
        gm.GameOver();

        // break glass
        Destroy(gameObject);
    }
}
