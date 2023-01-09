using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Patron : MonoBehaviour
{
    // Made private so I could update in script without unity being a fuck-head. 
    float moveSpeedMin = 1;
    float moveSpeedMax = 2.5f;
    float moveSpeed;
    float drinkSpeed = 1;
    Beverage bev;

    bool hasHadABeverage = false;
    bool isDrinking = false;
    float knockbackDuration = .2f;
    float knockbackTimer = 0f;
    float knockbackPower = 35f;

    GameManager gm;
    SpriteRenderer sr;

    [SerializeField]
    bool canTakeDamage;
    [SerializeField]
    int health = 3;
    public bool isDrunk = false;


    // Start is called before the first frame update
    void Start()
    {
        // Makes sure it doesn't overwrite the special case on level 4
        if (health != 1)    
            SetHealth(Random.Range(1, 4));
        SetRandomPatronSprite();


        //Randomize Move Speed;
        SetRandomMoveSpeed();

        gm = FindObjectOfType<GameManager>();
    }

    void SetRandomMoveSpeed()
    {
        moveSpeed = Random.Range(moveSpeedMin, moveSpeedMax);
    }

    // Update is called once per frame
    void Update()
    {
        if (!gm.gamePaused)
        {
            if (!isDrunk)
            {
				if (!isDrinking)
					transform.position = transform.position + (Vector3.right * Time.deltaTime * moveSpeed);
				else if (knockbackTimer < knockbackDuration)
				{
					knockbackTimer += Time.deltaTime;
					//knockback changed to moveSpeedMin
					transform.position = transform.position - (Vector3.right * Time.deltaTime * moveSpeedMin * knockbackPower);
				}

				if (transform.position.x < -10 && hasHadABeverage)
					GoHome();
			}

            else
            {

            }

		}
	}

    /// <summary>
    /// The Game Manager can set the moveSpeed and drinkSpeed here after instantiating.
    /// </summary>
    public void Initialize(float _moveSpeed, float _drinkSpeed)
    {
        moveSpeed = _moveSpeed;
        drinkSpeed= _drinkSpeed;
    }

    /// <summary>
    /// This is for the GM to manually set the health in special situations.
    /// </summary>
    public void SetHealth(int _health)
    {
        health = _health;
    }

    void GoHome()
    {
        gm.am.Play("Money");
        gm.RemoveFromSoberPatronsList(this);
        Destroy(gameObject);
    }

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Beverage" && !isDrinking && !isDrunk)
        {
            bev = other.GetComponent<Beverage>();
            bev.StopThatDrink(transform);

            StartCoroutine(HaveADrink());
        }

        if(other.tag == "FinishLine")
        {   
            gm.PatronCrossedFinishLine();
        }

	}

    IEnumerator HaveADrink()
    {
        //Update Score by $7 per beer
        gm.UpdateScoreUI(7);

        hasHadABeverage = true;
        isDrinking = true;
        knockbackTimer = 0f;
		gm.am.Play("gulp");

        if (canTakeDamage)
            health--;

		yield return new WaitForSeconds(drinkSpeed);

		isDrinking = false;
        bev.ReturnThatGlass();

        if (health <= 0)
        {
            Blackout();
        }
    }

    void Blackout()
    {
        gm.am.Play("thud");
        isDrunk = true;
        gm.drunkPatron = true;
        GetComponent<SphereCollider>().radius = 4f;
        transform.position = transform.position + new Vector3(1, 0, 1);
        sr.transform.rotation = sr.transform.rotation * Quaternion.Euler(0, 0, -90);
		gm.RemoveFromSoberPatronsList(this);
	}


    void SetRandomPatronSprite()
    {
        sr = GetComponentInChildren<SpriteRenderer>();

        int spriteNum = Random.Range(0, 64);
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/out");
        sr.sprite = sprites[spriteNum];
    }

   

}
