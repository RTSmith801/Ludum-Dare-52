using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Patron : MonoBehaviour
{
    public float moveSpeed = 1;
    public float drinkSpeed = 1;
    Beverage bev;

    bool isDrinking = false;
    float knockBackDuration = .2f;
    float knockBackTimer = 0f;

    SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        SetRandomPatronSprite();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDrinking)
            transform.position = transform.position + (Vector3.right * Time.deltaTime * moveSpeed);
        else if (knockBackTimer < knockBackDuration)
        {
            knockBackTimer += Time.deltaTime;
			transform.position = transform.position - (Vector3.right * Time.deltaTime * moveSpeed * 10);
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
        isDrinking = true;
        knockBackTimer = 0f;

        yield return new WaitForSeconds(drinkSpeed);

        isDrinking = false;
        bev.ReturnThatGlass();
    }


    void SetRandomPatronSprite()
    {
        sr = GetComponentInChildren<SpriteRenderer>();

        int spriteNum = Random.Range(0, 64);
        //sr.sprite = Resources.Load<Sprite>("Sprites/out");
    }

}
