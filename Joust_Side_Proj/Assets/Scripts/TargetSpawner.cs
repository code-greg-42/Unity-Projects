using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public void SpawnEnemies(int enemyTotal) {
        for (int i = 0; i < enemyTotal; i++) {
            Vector3 randomPosition = new Vector3(Random.Range(-20f, 20f), 0.0f, Random.Range(-20f, 20f));
            Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
        }
    }
}
