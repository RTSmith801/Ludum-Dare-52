using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public AudioManager am;
    [SerializeField] 
    GameObject[] bartops;
    GameObject patronPrefab;
    List<Patron> patronsInBar;

    public int level = 1;

    private void Awake()
    {
        am = FindObjectOfType<AudioManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        bartops = GameObject.FindGameObjectsWithTag("Bartop");
        patronPrefab = Resources.Load("Prefabs/Patron") as GameObject;
        patronsInBar = new List<Patron>();

        StartLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (patronsInBar.Count <= 0)
        {
            level++;
            StartLevel();
		}
    }

    void StartLevel()
    {
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
				Vector3 pos = new Vector3(-12 - (i), 1.01f, bartop.transform.position.z);
   				GameObject patron = Instantiate(patronPrefab, pos, Quaternion.identity);
                patronsInBar.Add(patron.GetComponent<Patron>());
			}
        }
    }

    public void NotifyGoingHome(Patron patronGoingHome)
    {
        patronsInBar.Remove(patronGoingHome);
    }

}
