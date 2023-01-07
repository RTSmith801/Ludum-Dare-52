using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyGlass : MonoBehaviour
{
    Rigidbody rb;
    public float glassMoveSpeed = 1;
    // Start is called before the first frame update
    void Start()
    {
		rb = GetComponent<Rigidbody>();
		rb.velocity = new Vector3(1, 0, 0) * glassMoveSpeed;
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnCollisionEnter(Collision collision)
	{
        if (collision.transform.tag == "Player")
            CatchGlass();
	}

    void CatchGlass()
    {
        Destroy(gameObject);
    }
}
