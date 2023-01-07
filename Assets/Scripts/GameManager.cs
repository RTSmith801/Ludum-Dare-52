using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public AudioManager am;
    [SerializeField] 
    GameObject[] bartops;
    GameObject patronPrefab;

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

        StartLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartLevel()
    {
        foreach (var bartop in bartops)
        {
            Vector3 pos = new Vector3(-12, 1.01f, bartop.transform.position.z);
            Instantiate(patronPrefab, pos, Quaternion.identity);
        }
    }

}
