using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemySpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public static EnemySpawner instance;
    [SerializeField] private int wavePointTotal;
    private int wavePointCurrent;
    [SerializeField] private List<GameObject> spawnableEnemies;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void SpawnWave()
    {
        Debug.Log("SpawningEnemy");
        if (spawnableEnemies.Count == 0 || wavePointTotal < 0)
        {
            return;
        }
        wavePointCurrent = wavePointTotal;
        Dictionary<GameObject,int> currentWaveEnemies = new Dictionary<GameObject,int>();
        for (int i = 0; i < spawnableEnemies.Count; i++)
        {
            currentWaveEnemies.Add(spawnableEnemies[i], spawnableEnemies[i].GetComponent<Enemy>().WavePoint());
        }
        int currentIteration = 0;
        int maxIteration = 100;
        while (maxIteration > currentIteration)
        {
            int num = Random.Range(0,currentWaveEnemies.Count);
            Debug.Log(wavePointCurrent);
            SpawnEnemyAtRandomPos(currentWaveEnemies.Keys.ElementAt(num));
            wavePointCurrent = wavePointCurrent - currentWaveEnemies.Values.ElementAt(num);
            List<GameObject> toRemove = new List<GameObject>();
            foreach(var e in currentWaveEnemies)
            {
                if(e.Value > wavePointCurrent)
                {
                    toRemove.Add(e.Key);
                }
            }
            foreach (var e in toRemove)
            {
                currentWaveEnemies.Remove(e);
            }
            if(wavePointCurrent <= 0  || currentWaveEnemies.Count <= 0)
            {
                break;
            }
            currentIteration++;
        }
    }

    private void SpawnEnemyAtRandomPos(GameObject prefab)
    {
        Debug.Log("hello");
        Vector2 spawnPos = AStarManager.instance.GetRandomNode().transform.position;
        Quaternion spawnRot = AStarManager.instance.GetRandomNode().transform.rotation;
        // will add spawn animation before spawning here
        Instantiate(prefab,spawnPos,spawnRot);
    }
}
