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
    //int health = 3;


    // Start is called before the first frame update
    void Start()
    {
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
	}

    /// <summary>
    /// The Game Manager can set the moveSpeed and drinkSpeed here after instantiating.
    /// </summary>
    public void Initialize(float _moveSpeed, float _drinkSpeed)
    {
        moveSpeed= _moveSpeed;
        drinkSpeed= _drinkSpeed;
    }

    void GoHome()
    {
        //Audio for getting them out of the bar? 
        gm.NotifyGoingHome(this);
        Destroy(gameObject);
    }

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Beverage" && !isDrinking)
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

        yield return new WaitForSeconds(drinkSpeed);

        isDrinking = false;
        bev.ReturnThatGlass();
    }


    void SetRandomPatronSprite()
    {
        sr = GetComponentInChildren<SpriteRenderer>();

        int spriteNum = Random.Range(0, 64);
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/out");
        sr.sprite = sprites[spriteNum];
    }

   

}
