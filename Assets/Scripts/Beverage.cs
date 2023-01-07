using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Beverage : MonoBehaviour
{
    Rigidbody rb;
    public float beverageMoveSpeed = 1;
	SphereCollider beverageCollider;
    GameObject emptyGlass;

	// Start is called before the first frame update
	void Start()
    {
        emptyGlass = Resources.Load("Prefabs/EmptyGlass") as GameObject;
        rb = GetComponent<Rigidbody>();
        beverageCollider = GetComponent<SphereCollider>();
        Vector3 direction = new Vector3(-1, 0, 0) * beverageMoveSpeed;
        rb.velocity = direction;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    public void StopThatDrink(Transform newParent)
    {
        rb.velocity = Vector3.zero;
        beverageCollider.enabled = false;
        rb.constraints = RigidbodyConstraints.FreezePosition;
        transform.parent = newParent;
    }

    public void ReturnThatGlass()
    {
		Instantiate(emptyGlass, transform.position, Quaternion.identity);

		Destroy(gameObject);
    }
}
