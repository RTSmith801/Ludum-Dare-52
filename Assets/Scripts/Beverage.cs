using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Beverage : MonoBehaviour
{
    GameManager gm;
    Rigidbody rb;
    public float beverageMoveSpeed = 1;
	SphereCollider beverageCollider;
    GameObject emptyGlass;
    Animator animator;
    int brokenGlasses = 0;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        emptyGlass = Resources.Load("Prefabs/EmptyGlass") as GameObject;
        rb = GetComponent<Rigidbody>();
        beverageCollider = GetComponent<SphereCollider>();
        Vector3 direction = new Vector3(-1, 0, 0) * beverageMoveSpeed;
        rb.velocity = direction;
        animator = gameObject.GetComponent<Animator>();
    }

    public void StopThatDrink(Transform newParent)
    {
        rb.velocity = Vector3.zero;
        beverageCollider.enabled = false;
        rb.constraints = RigidbodyConstraints.FreezePosition;
        transform.parent = newParent;
        animator.SetTrigger("IsDrinking");
    }

    public void ReturnThatGlass()
    {
		Instantiate(emptyGlass, transform.position, Quaternion.identity);

		Destroy(gameObject);
    }

	private void OnCollisionEnter(Collision collision)
	{
        if (collision.transform.name == "Wall-Left")
            BreakGlass();
	}

    void BreakGlass()
    {
        gm.WastedDrink();
        // breaking glass goes here
        Destroy(gameObject);
	}
}
