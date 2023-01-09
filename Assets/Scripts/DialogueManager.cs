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

	bool isVisible;
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
        if (!gm.drunkPatron)
        {
            levelTextArray[0] = "Thanks for working at my bar! Here we value quick service! Just get our patrons a drink before they get to the end of the bar!";
            levelTextArray[1] = "Wow gamer, you really served up those drinks real fast! Be careful you don't get anyone TOO drunk, LOL!";
            levelTextArray[2] = "Wizz bang! You're really good at this, look how much money we're making! God, I love money!";
        }
        else if (gm.drunkPatron)
        {
            levelTextArray[3] = "Oh hey! A customer blacked out! Now we have an opportunity to make some real money! Take this, it's harvest time!";
        }
        //levelTextArray[0] = "Thanks for working at my bar! Here we value quick service! Just get our patrons a drink before they get to the end of the bar!";
        //levelTextArray[1] = "Wow gamer, you really served up those drinks real fast! Be careful you don't get anyone TOO drunk, LOL!";
        //levelTextArray[2] = "Wizz bang! You're really good at this, look how much money we're making! God, I love money!";
        //levelTextArray[3] = "Oh God! I warned you not to get them too drunk! What are we going to do?! Here, take this and HANDLE it!";
        //levelTextArray[4] = "I uh, didn't think you'd get this far...";

        dialogueText.text = levelTextArray[gm.level - 1];
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