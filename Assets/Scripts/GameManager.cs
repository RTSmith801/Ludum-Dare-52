using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
//using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;
//using static UnityEditor.PlayerSettings;

public class GameManager : MonoBehaviour
{
    public AudioManager am;
    [SerializeField] 
    GameObject[] bartops;
    GameObject patronPrefab;
    List<Patron> patronsInBar;
    public DialogueManager dialogueManager;

    //Scene Switch Objects
    public GameObject VirtualCameraOne;
    public GameObject HarvesterBarSign;

    //UI
    TextMeshProUGUI UIText;
    int score = 0;

    //Custom Text Variables
    int brokenGlasses = 0;
    int wastedDrinks = 0;

    public bool gameOver = false;

    Player player;
    [SerializeField]
    Vector3 playerSpawnPosition;

    public int level = 1;

    //spawn time variables
    float baseMinSpawnTime = .5f;
    float baseMaxSpawnTime = 3f;
    float minSpawnTime;
    float maxSpawnTime;
    float randSpawnTime;
    float spawnTimer;

    //determines how many patrons will spawn in the level
    public int startingMaxPatrons = 4;
    public int levelPatrons;

    //for Harvest Time!
    public bool drunkPatron = false; 



    public bool gamePaused { get; private set; }
    public bool readyToStartLevel;
	GameState state;


	private void Awake()
    {
        am = FindObjectOfType<AudioManager>();
        UIText = GameObject.FindGameObjectWithTag("UI").GetComponent<TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetInitialVariables();
        levelPatrons = startingMaxPatrons;
        state = GameState.PreLevel;

		dialogueManager.SetDialoguePanelVisibility(true); // this pauses the game
        score = 0;
        UpdateScoreUI(0);
        gameOver = false;
        drunkPatron = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case GameState.PreLevel:
                PreLevel();
                break;
            case GameState.InLevel:
                InLevel();
                break;
            case GameState.PostLevel:
                PostLevel();
                break;
            case GameState.GameOver:
                GameOver();
                break;
            default:
                break;
        }



    }

    void PreLevel()
    {
        if (readyToStartLevel)
            StartLevel();

    }

    void InLevel()
    {
		if (!gamePaused)
		{
			spawnTimer += Time.deltaTime;

            // Jump timer to spawan another patron if the bar is empty
            if (patronsInBar.Count <= 0)
            {
                spawnTimer += randSpawnTime;
            }

			if (spawnTimer > randSpawnTime && levelPatrons > 0)
			{
				SpawnPatron();
				spawnTimer = 0;
				randSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
			}

			if (patronsInBar.Count <= 0 && levelPatrons <= 0)
			{
                //increase level and increase patrons
                level++;
                levelPatrons = (startingMaxPatrons + (level * 2));  

                minSpawnTime *= .9f;
				maxSpawnTime *= .9f;

				dialogueManager.SetDialoguePanelVisibility(true);

                //Currently hardcoded to stop at level 4. Revisit this to continue gameplay. 
				if (level < 4 && drunkPatron)
                {
                    state = GameState.PreLevel;
                    HarvesterBarSign.GetComponent<Animator>().SetBool("HarvestTime", false);
                    VirtualCameraOne.SetActive(true);
                }
                else
                {
                    GoToPostLevel();
                    HarvesterBarSign.GetComponent<Animator>().SetBool("HarvestTime", true);

                    //Turning off VirtualCameraOne zooms in to CamTwo. Enable CamOne again to zoom back out. 
                    VirtualCameraOne.SetActive(false);
                }
                    

			}
		}
	}

    void GoToPostLevel()
    {
		am.Stop("BGM1");
		am.Play("BGM2");

	}

    void PostLevel()
    {

		if (Input.GetKeyDown(KeyCode.Space))
		{
            state = GameState.PreLevel;
		}
	}

    public void GameOver()
    {   
        gameOver = true;
        StartCoroutine(GameOverSequence());
    }

    IEnumerator GameOverSequence()
    {
        yield return new WaitForSecondsRealtime(5f);
        //reloading current screne. Ditching main menu? 
        //SceneManager.LoadScene(0);
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex);
    }

    /// <summary>
    /// This function exists to make the Start() function easier to read
    /// </summary>
    void SetInitialVariables()
	{
		bartops = GameObject.FindGameObjectsWithTag("Bartop");
		patronPrefab = Resources.Load("Prefabs/Patron") as GameObject;
		patronsInBar = new List<Patron>();
		player = FindObjectOfType<Player>();
        dialogueManager = FindObjectOfType<DialogueManager>();
        readyToStartLevel = false;


		minSpawnTime = baseMinSpawnTime;
		maxSpawnTime = baseMaxSpawnTime;
		spawnTimer = 0;
		randSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
	}

	public void PauseGame(bool pause)
    {
        if (pause)
        {
			gamePaused = true;
			Time.timeScale = 0f;
		}
        else
        {
            gamePaused = false;
			Time.timeScale = 1f;
		}

    }

    void StartLevel()
    {
        state = GameState.InLevel;
        //Now being changed in Dialogue Manager
		//dialogueManager.SetDialoguePanelVisibility(false);

        //Stopping player from moving in between levels.
        //player.transform.position = playerSpawnPosition;

        // Changing spawning to filter in and have a set number per level. 
        int enemiesPerBar = 1;
        //if (level < 4)
        //    enemiesPerBar = level;
        //else
        //    enemiesPerBar = 4;

        // Spawns all the patrons and adds them to the list of patrons
        foreach (var bartop in bartops)
        {
            for (int i = 0; i < enemiesPerBar; i++)
            {
                SpawnPatron(i, bartop);
			}
        }

        readyToStartLevel = false;
    }

    /// <summary>
    /// Spawns a patron at a specified bartop at a given X offset
    /// </summary>
    /// <param name="xOffset"></param>
    /// <param name="zOffset"></param>
    void SpawnPatron(float xOffset, GameObject bartop)
    {
		Vector3 pos = new Vector3(-12 - (xOffset), 1.01f, bartop.transform.position.z);
		GameObject patron = Instantiate(patronPrefab, pos, Quaternion.identity);
		patronsInBar.Add(patron.GetComponent<Patron>());
    }

    /// <summary>
    /// Spawns a patron at a random bartop.
    /// </summary>
    void SpawnPatron()
    {
        //decrease the number of patrons that will spawn in the level
        levelPatrons--;

        int barNum = Random.Range(0, bartops.Length);
		Vector3 pos = new Vector3(-12, 1.01f, bartops[barNum].transform.position.z);

		GameObject patron = Instantiate(patronPrefab, pos, Quaternion.identity);
		patronsInBar.Add(patron.GetComponent<Patron>());
	}

    public void NotifyGoingHome(Patron patronGoingHome)
    {
        patronsInBar.Remove(patronGoingHome);
    }

    public void UpdateScoreUI(int update)
    {
        score += update;
        UIText.text = "$" + score;
    }

    // End game conditions below

    public void BrokenGlass()
    {
        brokenGlasses += 1;
        am.Play("GlassBreak");
        if (brokenGlasses == 1)
        {
            //Temp solution. decrease profit by $3 for every glass that breaks
            string text = "Hey! Those mugs are expensive! You're paying for that!";
            dialogueManager.SetDialoguePanelVisibility(true, text);
        }

        else if (brokenGlasses == 3)
        {
            //Temp solution. decrease profit by $3 for every glass that breaks
            string text = "I need you to actually catch those mugs.";
            dialogueManager.SetDialoguePanelVisibility(true, text);
        }

        else if (brokenGlasses == 5)
        {
            //Temp solution. decrease profit by $3 for every glass that breaks
            string text = "Catch the mugs, dammit!";
            dialogueManager.SetDialoguePanelVisibility(true, text);
        }

        if (brokenGlasses == 6)
        {
            //Temp solution. decrease profit by $3 for every glass that breaks
            string text = "That's the last straw! I'm going to have to let you go.";
            dialogueManager.SetDialoguePanelVisibility(true, text);
            GameOver();
        }

        UpdateScoreUI(-3);

    }

    public void WastedDrink()
    {
        wastedDrinks += 1;
        am.Play("GlassBreak");

        //Doing this instead of game over?
        if (wastedDrinks == 1)
        {
            //Temp solution. decrease profit by $7 for every full beer that goes to waste
            string text = "Who was that drink supposed to go to? That's coming out of your pay!";
            dialogueManager.SetDialoguePanelVisibility(true, text);
        }
        else if (wastedDrinks == 3)
        {
            //Temp solution. decrease profit by $3 for every glass that breaks
            string text = "Be careful with those drinks!";
            dialogueManager.SetDialoguePanelVisibility(true, text);
        }
        else if (wastedDrinks == 5)
        {
            //Temp solution. decrease profit by $3 for every glass that breaks
            string text = "I feel like you're not taking this seriously.";
            dialogueManager.SetDialoguePanelVisibility(true, text);
        }
        else if (wastedDrinks == 6)
        {
            //Temp solution. decrease profit by $3 for every glass that breaks
            string text = "Okay, this isn't working out. Come back when you actually want to work.";
            dialogueManager.SetDialoguePanelVisibility(true, text);
            GameOver();
        }

        UpdateScoreUI(-7);
    }

    public void PatronCrossedFinishLine()
    {
        string text = "You let a customer get to the front of the bar!? Unacceptable! You're fired!";
        dialogueManager.SetDialoguePanelVisibility(true, text);
        GameOver();
    }
    enum GameState { PreLevel, InLevel, PostLevel, GameOver }
}
