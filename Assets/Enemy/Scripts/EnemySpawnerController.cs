using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerController : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    public int spawnCount;
    public int maxSpawnCount;
    public float timeSpawnInterval;
    private Vector3 enemyZonePos;
    public float longitude;
    private float enemySpeed;

    private void Start()
    {
        spawnCount = 0;
        enemyZonePos = GameObject.FindGameObjectWithTag("EnemyZone").transform.position;
        longitude = (transform.position - enemyZonePos).magnitude;
        Debug.Log("l " + longitude);
        GameManager.instance.onEnemySpawnSignal += () => StartCoroutine(SpawnEnemies());
        GameManager.instance.onStopEnemySpawnSignal += () =>
        {
            StopAllCoroutines();
            spawnCount = 0;
        };
        GameManager.instance.onEnemyZoneTimeForFullRotationChanged += () => timeSpawnInterval = GameManager.instance.EnemyZoneTimeForFullRotation / maxSpawnCount;

    }
    private void SpawnEnemy()
    {
        Debug.Log(longitude);
        enemySpeed = longitude / timeSpawnInterval;
        EnemyController enemyC = Instantiate(enemyPrefab, transform.position, Quaternion.identity).GetComponent<EnemyController>();
        enemyC.speed = enemySpeed;
        AudioManager.instance.PlaySFX(AudioManager.instance.enemySpawning);
    }

    IEnumerator SpawnEnemies()
    {
        SpawnEnemy();
        spawnCount++;
        yield return new WaitForSeconds(timeSpawnInterval);
        if (spawnCount < maxSpawnCount) StartCoroutine(SpawnEnemies());
        else spawnCount = 0;

    }
}
