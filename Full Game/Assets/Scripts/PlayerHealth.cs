using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour 
{
	[Header("Health")]
	public int startingMaxHealth = 100;
	public int maximumMaxHealth = 200;
	private int m_currentMaxHealth = 100;
	public int currentMaxHealth { get { return m_currentMaxHealth; } set { m_currentMaxHealth = Mathf.Min(Mathf.Max(value, 0), maximumMaxHealth); } }
	public float percentageMaxHealth { get { return (float)currentMaxHealth / (float)maximumMaxHealth; } }
	
	private int m_currentHealth = 100;
	public int currentHealth { get { return m_currentHealth; } set { m_currentHealth = Mathf.Min(Mathf.Max(value, 0), currentMaxHealth); } }
	public float percentageHealth { get { return (float)currentHealth / (float)currentMaxHealth; } }
	
	[Header("Mana")]
	public int startingMana = 0;
	public int startingMaxMana = 100;
	public int maximumMaxMana = 200;
	private int m_currentMaxMana = 100;
	public int currentMaxMana { get { return m_currentMaxMana; } set { m_currentMaxMana = Mathf.Min(Mathf.Max(value, 0), maximumMaxMana); } }
	public float percentageMaxMana { get { return (float)currentMaxMana / (float)maximumMaxMana; } }
	
	private int m_currentMana = 100;
	public int currentMana { get { return m_currentMana; } set { m_currentMana = Mathf.Min(Mathf.Max(value, 0), currentMaxMana); } }
	public float percentageMana { get { return (float)currentMana / (float)currentMaxMana; } }
	
	[Header("Taking Damage")]
	public float invulnerabilityTime = 0.2f;
	public float lastDamageTime { get; set; }
	
	[Header("Recieved health boost!")]
	public float healthPickup = 0.2f;
	
	[Header("Sounds")]
	public AudioClip healClip = null;
	public AudioClip damageClip = null;
	
	private void Awake()
	{
		currentMaxHealth = startingMaxHealth;
		currentHealth = currentMaxHealth;
		
		currentMaxMana = startingMaxMana;
		currentMana = startingMana;
	}
	
	public bool TakeDamage(int damageAmount)
	{
		if(Time.time - lastDamageTime >= invulnerabilityTime)
		{
			currentHealth -= damageAmount;
			
			Manager.Instance.playerAudio.PlayPlayerVoice(damageClip);
			
			if(currentHealth <= 0)
				KillPlayer();
			
			return true;
		}
		
		return false;
	}
	
	public void KillPlayer()
	{
		Manager.Instance.EndGame();
	}
	
	public void Heal(int healAmount)
	{
		currentHealth += healAmount;
		
		Manager.Instance.playerAudio.PlayPlayerVoice(healClip);
	}
	
	public void GainMana(int manaAmount)
	{
		currentMana += manaAmount;
	}
	
	public bool SpendMana(int manaAmount)
	{
		if(currentMana - manaAmount < 0)
			return false;
		
		currentMana -= manaAmount;
		
		return true;
	}
	
	public void IncreaseMaxHealth(int increaseAmount)
	{
		currentMaxHealth += increaseAmount;
	}
	
	public void IncreaseMaxMana(int increaseAmount)
	{
		currentMaxMana += increaseAmount;
	}
}
