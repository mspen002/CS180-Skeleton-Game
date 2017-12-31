using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour 
{
	[Header("Spawning Enemies")]
	public GameObject[] spawnableEnemies = null;
	public float[] enemySpawnChance = null;
	public int[] enemyNumSpawnRolls = null;
	public Transform[] spawnPoints = null;
	
	//Tracking spawned enemies
	public Transform firstEnemyTransform { get { return numLivingEnemies > 0 ? this.transform.GetChild(0) : null; } }
	public int numLivingEnemies { get { return this.transform.childCount; } }
	private ulong availableEnemyID { get; set; }
	private bool hasSpawnedEnemiesThisRound { get; set; }
	
	public void StartRound()
	{
		if(spawnPoints.Length <= 0)
		{
			Debug.LogError("MonsterManager: No Spawn Points Set");
			return;
		}
		
		if(spawnableEnemies.Length != enemySpawnChance.Length || spawnableEnemies.Length != enemyNumSpawnRolls.Length)
		{
			Debug.LogError("MonsterManager: Mismatched spawnableEnemies, enemySpawnChance, and enemyNumSpawnRolls");
			return;
		}
		
		hasSpawnedEnemiesThisRound = false;
		for(int i = Manager.Instance.currentRound; i > 0; i--)
		{
			for(int j = 0; j < spawnableEnemies.Length; j++)
			{
				for(int k = 0; k < enemyNumSpawnRolls[j]; k++)
				{
					if(!hasSpawnedEnemiesThisRound || Random.Range(0f, 1f) <= enemySpawnChance[j])
					{
						hasSpawnedEnemiesThisRound = true;
						GameObject enemyObj = Instantiate(spawnableEnemies[j], spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity,  this.transform);
						EnemyHealth enemyHealth = enemyObj.GetComponentInChildren<EnemyHealth>();
						enemyHealth.ID = availableEnemyID++;
					}
				}
			}
		}
		
		StartCoroutine("RoundCheck");
	}
	
	private IEnumerator RoundCheck()
	{
		while(numLivingEnemies > 0)
			yield return new WaitForFixedUpdate();
		
		Manager.Instance.BeginEndRound();
	}
}
