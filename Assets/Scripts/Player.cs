using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody rb;
    public float playerMoveSpeed = 1;
    bool inServingArea = false;
    bool facingRight;
    Transform servingAreaTransform;
    GameObject beverage;
    GameManager gm;

    public bool beverageLock = false;

    public SpriteRenderer knifeSprite;

    List<Patron> harvestTargets;

    // Start is called before the first frame update
    void Start()
    {
        facingRight = transform.position.x == 1;
        rb = GetComponent<Rigidbody>();
        gm = FindObjectOfType<GameManager>();
        beverage = Resources.Load("Prefabs/Beverage") as GameObject;
        harvestTargets = new List<Patron>();
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Space))
		{
            if (inServingArea && !beverageLock && gm.state == GameManager.GameState.InLevel)
			    ServeDrink();
            else if (harvestTargets.Count > 0 && gm.state == GameManager.GameState.PostLevel)
            {
				Sprite[] _knife = Resources.LoadAll<Sprite>("Sprites/Knife");
				knifeSprite.sprite = _knife[1];

				gm.am.Play("Blood1");
				gm.am.Play("Money");

                // animate blood

                Patron patronToDestroy = harvestTargets[0];
                harvestTargets.Remove(patronToDestroy);
                Destroy(patronToDestroy.gameObject);

				gm.UpdateScoreUI(10000);
			}

		}
	}

    public void SetPlayerMoveSpeed()
    {
        
        if (gm.state == GameManager.GameState.PostLevel)
        {
			playerMoveSpeed = .5f;
		}
        else
        {
            playerMoveSpeed = 1f;
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
			UpdateXScaleForFacing();
		//if moving right and player is facing left
		else if (hor > 0 && !facingRight)
			UpdateXScaleForFacing();

		Vector3 direction = new Vector3(hor, 0, ver) * 25f * playerMoveSpeed;
		rb.velocity = direction;



	}

    public void EnableKnife(bool enable)
    {
        knifeSprite.gameObject.SetActive(enable);
    }

    void UpdateXScaleForFacing()
    {
        facingRight = !facingRight;

        float xScale = facingRight ? 1 : -1;
        transform.localScale = new Vector3(xScale, 1, 1);

        //Vector3 theScale = transform.localScale;
        //theScale.x *= -1;
        //transform.localScale = theScale;
    }
  
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ServingArea")
        {
            //print("Player entered serving area");
            inServingArea = true;
            servingAreaTransform = other.transform;
        }
        else if (other.gameObject.tag == "Patron")
            harvestTargets.Add(other.GetComponent<Patron>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "ServingArea")
        {
           //print("Player exited serving area");
            inServingArea = false;
            servingAreaTransform = null;
        }
		else if (other.gameObject.tag == "Patron")
			harvestTargets.Remove(other.GetComponent<Patron>());
	}


	void ServeDrink()
    {
        //Charge up *Stretch Goal

        //Currently auto-launches drink
        Vector3 position = servingAreaTransform.position + new Vector3(-1.2f, 1f, 0f);
        Instantiate(beverage, position, Quaternion.identity);
        gm.am.Play("ServeDrink");
    }

    public void BeverageLock(bool _beverageLock)
    {
        if (_beverageLock)
            beverageLock = true;
        else
            StartCoroutine(UnlockBeverage());
    }

    IEnumerator UnlockBeverage()
    {
		yield return new WaitForSecondsRealtime(.1f);
        beverageLock = false;
	}


}
