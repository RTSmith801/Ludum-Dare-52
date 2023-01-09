using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    GameObject patronPlusPlusPrefab;
    List<Patron> soberPatronsInBar;
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
    public int remainingUnspawnedPatrons;

    //for Harvest Time!
    public bool drunkPatron = false;
    public bool firstDrunkPatron = false;



    public bool gamePaused { get; private set; }
    public bool readyToStartLevel;
    [SerializeField]
    public GameState state { private set; get; }


	private void Awake()
    {
        am = FindObjectOfType<AudioManager>();
        UIText = GameObject.FindGameObjectWithTag("UI").GetComponent<TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetInitialVariables();
        remainingUnspawnedPatrons = startingMaxPatrons;
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
            if (soberPatronsInBar.Count <= 0)
            {
                spawnTimer += randSpawnTime;
            }

			if (spawnTimer > randSpawnTime && remainingUnspawnedPatrons > 0)
			{
				SpawnPatron();
				spawnTimer = 0;
				randSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
			}


            // When there are no more sober patrons in the bar
			if (soberPatronsInBar.Count <= 0 && remainingUnspawnedPatrons <= 0)
			{
                //increase level and increase patrons
                level++;
                remainingUnspawnedPatrons = (startingMaxPatrons + (level + 2));  

                minSpawnTime *= .9f;
				maxSpawnTime *= .9f;

				dialogueManager.SetDialoguePanelVisibility(true);

				if (!drunkPatron)
                {
                    GoToPreLevel();
                }
                else if (drunkPatron)
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
        state = GameState.PostLevel;

        ClearAllDrinks();
        player.EnableKnife(true);
	}

    void PostLevel()
    {


        List<Patron> drunkPatrons = FindObjectsOfType<Patron>().ToList();

        if (drunkPatrons.Count <= 0)
        {
			string text = "These organs you harvested go for big bucks on the black market! Good work, gamer!";
			dialogueManager.SetDialoguePanelVisibility(true, text);
            GoToPreLevel();
		}


		//if (Input.GetKeyDown(KeyCode.Space))
		//{
  //          state = GameState.PreLevel;
		//}
	}

    void GoToPreLevel()
    {
		state = GameState.PreLevel;
		HarvesterBarSign.GetComponent<Animator>().SetBool("HarvestTime", false);
		VirtualCameraOne.SetActive(true);

		am.Stop("BGM2");
		am.Play("BGM1");
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
		patronPlusPlusPrefab = Resources.Load("Prefabs/Patron++") as GameObject;
		soberPatronsInBar = new List<Patron>();
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
        player.EnableKnife(false);
        state = GameState.InLevel;

        ClearAllDrinks();

        // Changing spawning to filter in and have a set number per level. 
        int enemiesPerBar = 1;

        // Spawns all the patrons and adds them to the list of patrons
        foreach (var bartop in bartops)
        {
            for (int i = 0; i < enemiesPerBar; i++)
            {
                SpawnPatron(i, bartop);
			}
        }

        if (level == 4)
        {
			Patron patron = SpawnPatron(-10, bartops[1]);
            patron.SetHealth(1);
		}


        readyToStartLevel = false;
    }

    void ClearAllDrinks()
    {

		foreach (var emptyGlass in FindObjectsOfType<EmptyGlass>())
		{
			Destroy(emptyGlass.gameObject);
		}

		foreach (var Beverage in FindObjectsOfType<Beverage>())
		{
			Destroy(Beverage.gameObject);
		}
	}

    /// <summary>
    /// Spawns a patron at a specified bartop at a given X offset
    /// </summary>
    /// <param name="xOffset"></param>
    /// <param name="zOffset"></param>
    Patron SpawnPatron(float xOffset, GameObject bartop)
    {
		GameObject patronPrefabToSpawn;
		if (level < 4)
			patronPrefabToSpawn = patronPrefab;
		else
			patronPrefabToSpawn = patronPlusPlusPrefab;

		Vector3 pos = new Vector3(-12 - (xOffset), 1.01f, bartop.transform.position.z);
		GameObject patron = Instantiate(patronPrefabToSpawn, pos, Quaternion.identity);
		soberPatronsInBar.Add(patron.GetComponent<Patron>());

        return patron.GetComponent<Patron>();
    }

    /// <summary>
    /// Spawns a patron at a random bartop.
    /// </summary>
    void SpawnPatron()
    {
        GameObject patronPrefabToSpawn;
        if (level < 4)
            patronPrefabToSpawn = patronPrefab;
        else
            patronPrefabToSpawn = patronPlusPlusPrefab;

        //decrease the number of patrons that will spawn in the level
        remainingUnspawnedPatrons--;

        int barNum = Random.Range(0, bartops.Length);
		Vector3 pos = new Vector3(-12, 1.01f, bartops[barNum].transform.position.z);

		GameObject patron = Instantiate(patronPrefabToSpawn, pos, Quaternion.identity);
		soberPatronsInBar.Add(patron.GetComponent<Patron>());
	}

    /// <summary>
    /// used for going home or getting blackout drunk
    /// </summary>
    /// <param name="patronGoingHome"></param>
    public void RemoveFromSoberPatronsList(Patron patronGoingHome)
    {
        soberPatronsInBar.Remove(patronGoingHome);
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

        if (brokenGlasses >= 10)
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
        else if (wastedDrinks >= 10)
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
    public enum GameState { PreLevel, InLevel, PostLevel, GameOver }
}
