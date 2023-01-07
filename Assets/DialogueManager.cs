using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

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

	// Start is called before the first frame update
	void Start()
    {
		isVisible = false;
        gm = FindObjectOfType<GameManager>();  
        scrollingTextTimer = 0;
		dialoguePanel = GameObject.Find("DialoguePanel");
		dialogueAnimator = dialoguePanel.GetComponent<Animator>();
		dialogueText = dialoguePanel.GetComponentInChildren<TextMeshProUGUI>();

	}

	// Update is called once per frame
	void Update()
    {
        if (startTextScroll && scrollingTextTimer < scrollingTextDuration)
		{
			scrollingTextTimer += Time.unscaledDeltaTime;

			dialogueText.maxVisibleCharacters = (int)Mathf.Lerp(0, dialogueText.text.Length, scrollingTextTimer / scrollingTextDuration);
		}
    }

	public void SetDialoguePanelVisibility(bool visible)
	{
		isVisible = visible;
		gm.PauseGame(visible);

		if (visible)
		{
			scrollingTextTimer = 0;
			dialogueText.maxVisibleCharacters = 0;
			dialogueAnimator.SetTrigger("DialogueOpen");
			StartCoroutine(StartTextScroll());
		}
		else
			dialogueAnimator.SetTrigger("DialogueClose");

	}

	IEnumerator StartTextScroll()
	{
		startTextScroll = false;

		yield return new WaitForSecondsRealtime(.3f);

		startTextScroll = true;

	}

}
