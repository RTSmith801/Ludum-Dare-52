using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody rb;
    public float playerMoveSpeed = 1;
    bool inServingArea = false;
    Transform servingAreaTransform;
    GameObject beverage;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        beverage = Resources.Load("Prefabs/Beverage") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && inServingArea)
        {
            ServeDrink();
        }
    }

	private void FixedUpdate()
	{
		GetInput();
	}

	void GetInput()
    {
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis ("Vertical");

        Vector3 direction = new Vector3(hor, 0, ver) * 25f * playerMoveSpeed;

        rb.velocity = direction;

       
    }
  
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "ServingArea")
        {
            //print("Player entered serving area");
            inServingArea = true;
            servingAreaTransform = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "ServingArea")
        {
           //print("Player exited serving area");
            inServingArea = false;
            servingAreaTransform = null;
        }
    }

    void ServeDrink()
    {
        //Charge up *Stretch Goal

        //Currently auto-launches drink

        print("Servingfunction called");
        Vector3 position = servingAreaTransform.position + new Vector3(-1.2f, .5f, 0f);
        Instantiate(beverage, position, Quaternion.identity);
    }
}
