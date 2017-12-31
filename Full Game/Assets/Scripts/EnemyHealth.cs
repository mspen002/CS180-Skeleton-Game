using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour 
{
	//Identification
	public ulong ID { get; set; }
	
	[Header("Health")]
	public int maxHealth = 100;
	private int m_currentHealth = 100;
	public int currentHealth { get { return m_currentHealth; } set { m_currentHealth = Mathf.Min(Mathf.Max(value, 0), maxHealth); } }
	public float percentageHealth { get { return (float)currentHealth / (float)maxHealth; } }
	
	[Header("Taking Damage")]
	public float invulnerabilityTime = 0.2f;
	public float lastDamageTime { get; set; }
	
	[Header("Drops")]
	public GameObject[] possibleCoinDrops;
	public int numberOfCoinDrops;
	public GameObject[] possibleDrops;
	public int numberOfDrops;
	public float[] coinDropPercentage;
	public float[] dropPercentage;
	public float dropSpawnForce;
	private float randomNumber;
	private float dropComparator = 1;
	private float randomForce;
	private Vector2 forceVector;
	
	[Header("PlayerManaGain")]
	public int playerManaGainOnDeath = 10;
	
	//References
	private HealthBar m_healthBar = null;
	public HealthBar healthBar { get { if(m_healthBar == null) m_healthBar = this.GetComponentInChildren<HealthBar>(); return m_healthBar; } }
	
	private EnemyAudio m_enemyAudio = null;
	public EnemyAudio enemyAudio { get { if(m_enemyAudio == null) m_enemyAudio = this.GetComponent<EnemyAudio>(); return m_enemyAudio; } }
	
	private void Awake()
	{
		currentHealth = maxHealth;
	}
	
	public bool TakeDamage(int damageAmount, string damageSource = "")
	{		
		if(Time.time - lastDamageTime >= invulnerabilityTime)
		{
			currentHealth = Mathf.Max(0, currentHealth - damageAmount);
			
			if(enemyAudio != null)	
				enemyAudio.PlayDamagedSound();
			
			if(currentHealth <= 0)
				Die(damageSource);
			
			if(healthBar != null)
				healthBar.UpdateHealth(percentageHealth);
			
			return true;
		}
		
		return false;
	}
	
	public void Die(string damageSource)
	{
		if(currentHealth >= 1)
			return;
		
		SpawnDrops();
		
		if(damageSource != "Ultimate")
			Manager.Instance.playerHealth.GainMana(playerManaGainOnDeath);
		
		Manager.Instance.enemiesKilled++;
		
		Destroy(this.gameObject);		
	}
	
	private void SpawnDrops()
	{
		/*
		foreach(GameObject obj in possibleDrops)
		{
			GameObject drop = Instantiate(obj, this.transform.position, this.transform.rotation);
			
			Rigidbody2D dropRigidbody = drop.GetComponent<Rigidbody2D>();
			if(dropRigidbody != null)
				dropRigidbody.AddForce(Vector2.up * dropSpawnForce);
			
			drop.transform.parent = Manager.Instance.itemManager.transform;
		}
		*/
		for(int i = 0; i < numberOfCoinDrops; i++)
		{
			randomNumber = Random.value;
			randomForce = Random.Range(-1f, 1f);
			for(int j = possibleCoinDrops.Length - 1; j >= 0; j--)
			{
				dropComparator = dropComparator - coinDropPercentage[j];
				if(dropComparator <= randomNumber)
				{
					GameObject drop = Instantiate(possibleCoinDrops[j], this.transform.position, this.transform.rotation);
			
					Rigidbody2D dropRigidbody = drop.GetComponent<Rigidbody2D>();
					
					forceVector.x = randomForce;
					forceVector.y = 1f;
					
					if(dropRigidbody != null)
					dropRigidbody.AddForce(forceVector * dropSpawnForce);
			
					drop.transform.parent = Manager.Instance.itemManager.coins.transform;
					
					break;
				}
			}
			dropComparator = 1;
		}
		
		for(int x = 0; x < numberOfDrops; x++)
		{
			randomNumber = Random.value;
			randomForce = Random.Range(-1f, 1f);
			for(int y = possibleDrops.Length - 1; y >= 0; y--)
			{
				dropComparator = dropComparator - dropPercentage[y];
				if(dropComparator <= randomNumber)
				{
					GameObject drop = Instantiate(possibleDrops[y], this.transform.position, this.transform.rotation);
			
					Rigidbody2D dropRigidbody = drop.GetComponent<Rigidbody2D>();
					
					forceVector.x = randomForce;
					forceVector.y = 1f;
					
					if(dropRigidbody != null)
					dropRigidbody.AddForce(forceVector * dropSpawnForce);
			
					drop.transform.parent = Manager.Instance.itemManager.potions.transform;
					
					break;
				}
			}
			dropComparator = 1;
		}
	}
}
