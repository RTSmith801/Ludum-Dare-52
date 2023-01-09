using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    GameManager gm;
    public float scrollingTextDuration = 1;
    float scrollingTextTimer;

    Animator dialogueAnimator;
    GameObject dialoguePanel;
    TextMeshProUGUI dialogueText;

    public bool isVisible { get; private set; }
	bool startTextScroll;
	string[] levelTextArray;

	// Start is called before the first frame update
	void Start()
    {
		isVisible = false;
        gm = FindObjectOfType<GameManager>();  
        scrollingTextTimer = 0;
		dialoguePanel = GameObject.Find("DialoguePanel");
		dialogueAnimator = dialoguePanel.GetComponent<Animator>();
		dialogueText = dialoguePanel.GetComponentInChildren<TextMeshProUGUI>();
		levelTextArray = new string[10];

	}

	// Update is called once per frame
	void Update()
    {
        if (startTextScroll)
			DoTheTextScroll();
		else if (Input.GetKeyDown(KeyCode.Space) && isVisible && !gm.gameOver)
        {
            if (gm.state == GameManager.GameState.PreLevel)
                gm.readyToStartLevel = true;

            SetDialoguePanelVisibility(false);
        }

	}

	void DoTheTextScroll()
	{
		if (scrollingTextTimer < scrollingTextDuration)
		{
			scrollingTextTimer += Time.unscaledDeltaTime;

			dialogueText.maxVisibleCharacters = (int)Mathf.Lerp(0, dialogueText.text.Length, scrollingTextTimer / scrollingTextDuration);
		}
		else
		{
			startTextScroll = false;
			gm.am.Stop("boop");
		}
	}

	public void SetDialoguePanelVisibility(bool visible)
	{
		isVisible = visible;
		gm.PauseGame(visible);

		if (visible)
		{
			SetText();
			scrollingTextTimer = 0;
			dialogueText.maxVisibleCharacters = 0;
			dialogueAnimator.SetTrigger("DialogueOpen");
			StartCoroutine(StartTextScroll());
		}
		else
			dialogueAnimator.SetTrigger("DialogueClose");

	}


    void SetText()
    {
        string textToSay = "";

        if (!gm.drunkPatron)
        {
            if(gm.level == 1)
            {
                textToSay = "Thanks for working at my bar! Here we value quick service, and money! Just get our patrons a drink before they get to the end of the bar!";
            }
            else if (gm.level == 2)
            {
                textToSay = "Wizz bang! You're really good at this, look how much money we're making! God, I love money!";
            }
            else if (gm.level == 3)
            {
                textToSay = "Wow gamer, you really served up those drinks real fast! Be careful you don't get anyone TOO drunk, LOL!";
            }
            else
            {
                int rand = Random.Range(0, 4);
                switch (rand)
                {
                    case 0:
                        textToSay = "Here comes more money! I mean customers...";
                        break;
                    case 1:
                        textToSay = "Good work, partner! Keep it going!";
                        break;
                    case 2:
                        textToSay = "We're making a \"killing\"!";
                        break;
                    case 3:
                        textToSay = "Don't you just love working here?";
                        break;
                    default:
                        textToSay = "We're making a \"killing\"!";
                        break;
                }
            }   
            
        }
        else if (gm.drunkPatron  && gm.firstDrunkPatron == false)
        {
            textToSay = "Oh hey! A customer blacked out! Now we have an opportunity to make some real money! Take this, it's harvest time!";
            gm.firstDrunkPatron = true;
        }
        else if (gm.drunkPatron)
        {
            int rand = Random.Range(0, 4);
            switch (rand)
            {
                case 0:
                    textToSay = "Oh goodie! It's harvest time!";
                    break;
                case 1:
                    textToSay = "Time to make some real money!";
                    break;
                case 2:
                    textToSay = "Don't worry, they won't be missed!";
                    break;
                case 3:
                    textToSay = "This is my favorite part of this job!";
                    break;
                default:
                    textToSay = "We're making a \"killing\"!";
                    break;
            }
        }


        dialogueText.text = textToSay;
    }

    // Overloaded Spaghetti Code
    public void SetDialoguePanelVisibility(bool visible, string customText)
    {
        isVisible = visible;
        gm.PauseGame(visible);

        if (visible)
        {
            SetText(customText);
            scrollingTextTimer = 0;
            dialogueText.maxVisibleCharacters = 0;
            dialogueAnimator.SetTrigger("DialogueOpen");
            StartCoroutine(StartTextScroll());
        }
        else
            dialogueAnimator.SetTrigger("DialogueClose");

    }

    void SetText(string customText)
    {
        dialogueText.text = customText;
    }


    IEnumerator StartTextScroll()
	{
		yield return new WaitForSecondsRealtime(.3f);

		startTextScroll = true;
		gm.am.Play("boop");

	}

}
