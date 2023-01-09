using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody rb;
    public float playerMoveSpeed = 1;
    bool inServingArea = false;
    bool facingRight = true;
    Transform servingAreaTransform;
    GameObject beverage;
    GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gm = FindObjectOfType<GameManager>();
        beverage = Resources.Load("Prefabs/Beverage") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Space) && inServingArea && !gm.gamePaused)
		{
			ServeDrink();
		}
	}

	private void FixedUpdate()
	{
        // Since this is called in FixedUpdate(), it does not get read when the game is paused.
		GetInput();
	}

	void GetInput()
    {
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis ("Vertical");

		//if moving left and player is facing right
		if (hor < 0 && facingRight)
			Flip();
		//if moving right and player is facing left
		else if (hor > 0 && !facingRight)
			Flip();

		Vector3 direction = new Vector3(hor, 0, ver) * 25f * playerMoveSpeed;
		rb.velocity = direction;



	}

    void Flip()
    {
        facingRight = !facingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
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
        Vector3 position = servingAreaTransform.position + new Vector3(-1.2f, 1f, 0f);
        Instantiate(beverage, position, Quaternion.identity);
        gm.am.Play("ServeDrink");
    }


}
