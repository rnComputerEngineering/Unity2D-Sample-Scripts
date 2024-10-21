using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static MainManager instance;
    private int levelInd;
    private int floorInd;
    public GameObject characterPrefab;
    public GameObject playerRef;
    public Vector3 playerPos;
    [SerializeField] private string[] scenesRegularFloor1;
    [SerializeField] private string[] scenesRegularFloor2;
    [SerializeField] private string[] scenesRegularFloor3;
    [SerializeField] private string[] scenesBossFloor1;
    [SerializeField] private string[] scenesBossFloor2;
    [SerializeField] private string sceneBossFloor3;
    public Stats savedStats;
    private void Awake()
    {

        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() // temporary
    {
        Debug.Log("trying to get player ref");
        playerRef = GameObject.FindGameObjectWithTag("Player");
    }

    public void StartRun(GameObject Character)
    {
        levelInd = 0;
        floorInd = 1;
        SceneManager.LoadScene(scenesRegularFloor1[Random.Range(0, scenesRegularFloor1.Length)]);
        playerRef = GameObject.FindGameObjectWithTag("Player");
        savedStats = playerRef.GetComponent<Stats>();


    }

    public void EndLevel()
    {
        savedStats = playerRef.GetComponent<Stats>();
        LevelLoader levelLoader = GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>();
        if (levelInd < 4)
        {
            levelInd++;
        }
        else
        {
            levelInd = 0;
            floorInd++;
        }
        if (levelInd != 4)
        {
            switch (floorInd)
            {
                case 1:
                    StartCoroutine(levelLoader.LoadLevel(scenesRegularFloor1[Random.Range(0, scenesRegularFloor1.Length)]));
                    break;
                case 2:
                    StartCoroutine(levelLoader.LoadLevel(scenesRegularFloor2[Random.Range(0, scenesRegularFloor2.Length)]));
                    break;
                case 3:
                    StartCoroutine(levelLoader.LoadLevel(scenesRegularFloor3[Random.Range(0, scenesRegularFloor3.Length)]));
                    break;
            }
        }
        else
        {
            switch (floorInd)
            {
                case 1:
                    StartCoroutine(levelLoader.LoadLevel(scenesBossFloor1[Random.Range(0, scenesBossFloor1.Length)]));
                    break;
                case 2:
                    StartCoroutine(levelLoader.LoadLevel(scenesBossFloor2[Random.Range(0, scenesBossFloor2.Length)]));
                    break;
                case 3:
                    StartCoroutine(levelLoader.LoadLevel(sceneBossFloor3));
                    break;
            }
        }
        //load level here;
        playerRef = GameObject.FindGameObjectWithTag("Player");
        if (playerRef == null)
        {
            Debug.Log("Player Not Found Error");
        }
        else
        {
            playerRef.GetComponent<Player>().SetStats(savedStats);
        }
        
    }

    private void FixedUpdate()
    {
        if (playerRef != null)
        {
            playerPos = playerRef.transform.position;
        }
        else
        {
            playerRef = GameObject.FindGameObjectWithTag("Player");
        }
        // will add better logic later
        if(GameObject.FindAnyObjectByType<Enemy>() == null)
        {
            EnemySpawner.instance.SpawnWave();
        }
    }

}
