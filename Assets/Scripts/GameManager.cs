using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.PlayerSettings;

public class GameManager : MonoBehaviour
{
    public AudioManager am;
    [SerializeField] 
    GameObject[] bartops;
    GameObject patronPrefab;
    List<Patron> patronsInBar;
    DialogueManager dialogueManager;

    Player player;
    [SerializeField]
    Vector3 playerSpawnPosition;

    public int level = 1;

    //spawn time variables
    float baseMinSpawnTime = 3f;
    float baseMaxSpawnTime = 8f;
    float minSpawnTime;
    float maxSpawnTime;
    float randSpawnTime;
    float spawnTimer;



    public bool gamePaused { get; private set; }
	GameState state;


	private void Awake()
    {
        am = FindObjectOfType<AudioManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetInitialVariables();

		state = GameState.PreLevel;

		dialogueManager.SetDialoguePanelVisibility(true); // this pauses the game
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
            default:
                break;
        }



    }

    void PreLevel()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartLevel();
        }
    }

    void InLevel()
    {
		if (!gamePaused)
		{
			spawnTimer += Time.deltaTime;
			if (spawnTimer > randSpawnTime)
			{
				SpawnPatron();
				spawnTimer = 0;
				randSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
			}

			if (patronsInBar.Count <= 0)
			{
				level++;

				minSpawnTime *= .9f;
				maxSpawnTime *= .9f;

                if (level < 4)
                    state = GameState.PreLevel;
                else
                    GoToPostLevel();


				dialogueManager.SetDialoguePanelVisibility(true);
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
		dialogueManager.SetDialoguePanelVisibility(false);

        player.transform.position = playerSpawnPosition;

        int enemiesPerBar;
        if (level < 4)
            enemiesPerBar = level;
        else
            enemiesPerBar = 4;

        // Spawns all the patrons and adds them to the list of patrons
        foreach (var bartop in bartops)
        {
            for (int i = 0; i < enemiesPerBar; i++)
            {
                SpawnPatron(i, bartop);
			}
        }
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
        int barNum = Random.Range(0, bartops.Length);
		Vector3 pos = new Vector3(-12, 1.01f, bartops[barNum].transform.position.z);

		GameObject patron = Instantiate(patronPrefab, pos, Quaternion.identity);
		patronsInBar.Add(patron.GetComponent<Patron>());
	}

    public void NotifyGoingHome(Patron patronGoingHome)
    {
        patronsInBar.Remove(patronGoingHome);
    }



	enum GameState { PreLevel, InLevel, PostLevel }
}
