using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Patron : MonoBehaviour
{
    public float moveSpeed = 1;
    public float drinkSpeed = 1;
    Beverage bev;

    bool hasHadABeverage = false;
    bool isDrinking = false;
    float knockbackDuration = .2f;
    float knockbackTimer = 0f;
    float knockbackPower = 20f;

    GameManager gm;
    SpriteRenderer sr;


    // Start is called before the first frame update
    void Start()
    {
        SetRandomPatronSprite();
        gm = FindObjectOfType<GameManager>();
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
				transform.position = transform.position - (Vector3.right * Time.deltaTime * moveSpeed * knockbackPower);
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
	}

    IEnumerator HaveADrink()
    {
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
