using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : Singleton<Manager>
{
	protected Manager() {}
	
	[Header("References")]
	public GameObject player = null;
	public GameUI gameUI = null;

	private Transform m_playerTransform = null;
	public Transform playerTransform { get { if(m_playerTransform == null) m_playerTransform = player.GetComponent<Transform>(); return m_playerTransform; } }
	
	private PlayerHealth m_playerHealth = null;
	public PlayerHealth playerHealth { get { if(m_playerHealth == null) m_playerHealth = player.GetComponent<PlayerHealth>(); return m_playerHealth; } }
	
	private PlayerMovement m_playerMovement = null;
	public PlayerMovement playerMovement { get { if(m_playerMovement == null) m_playerMovement = player.GetComponent<PlayerMovement>(); return m_playerMovement; } }
	
	private PlayerInventory m_playerInventory = null;
	public PlayerInventory playerInventory { get { if(m_playerInventory == null) m_playerInventory = player.GetComponent<PlayerInventory>(); return m_playerInventory; } }
	
	private PlayerAudio m_playerAudio = null;
	public PlayerAudio playerAudio { get { if(m_playerAudio == null) m_playerAudio = player.GetComponentInChildren<PlayerAudio>(); return m_playerAudio; } }

	private ItemManager m_itemManager = null;
	public ItemManager itemManager { get { if(m_itemManager == null) m_itemManager = this.GetComponentInChildren<ItemManager>(); return m_itemManager; } }
	
	private MonsterManager m_monsterManager = null;
	public MonsterManager monsterManager { get { if(m_monsterManager == null) m_monsterManager = this.GetComponentInChildren<MonsterManager>(); return m_monsterManager; } }
	
	
	//GameState
	public int currentRound { get; set; }
	public int enemiesKilled { get; set; }
	
	public bool isGameOver { get; set; }
	public bool isBetweenRounds { get; set; }
	private bool m_isPaused { get; set; }
	public bool isPaused { get { return isBetweenRounds || m_isPaused || isGameOver; } set { m_isPaused = value; } }
	
	
	private void Awake()
	{
		PauseGame(false);
	}
	
	private void Start()
	{
		currentRound = 1;
		monsterManager.StartRound();
	}
	
	private void Update()
	{
		if(!isGameOver && Input.GetKeyDown(KeyCode.Escape))
			PauseGame(!m_isPaused);
	}
	
	public void PauseGame(bool pause)
	{
		isPaused = pause;
		Time.timeScale = (pause || isBetweenRounds) ? 0f : 1f;
		gameUI.PauseGame(pause);
	}
	
	public void BeginEndRound()
	{
		itemManager.StartCoroutine("CollectAllCoins");
	}
	
	public void EndRound()
	{
		isBetweenRounds = true;
		Time.timeScale = 0f;
		gameUI.EndRound();
		
		playerMovement.EndAttacks();
	}
	
	public void BeginNextRound()
	{
		currentRound++;
		isBetweenRounds = false;
		Time.timeScale = 1f;
		monsterManager.StartRound();
	}
	
	public void EndGame()
	{
		isGameOver = true;
		Time.timeScale = 0f;
		gameUI.EndGame();
		
		if(PlayerPrefs.GetInt("RoundHighScore") < currentRound)
			PlayerPrefs.SetInt("RoundHighScore", currentRound);
		
		if(PlayerPrefs.GetInt("CoinsHighScore") < playerInventory.coinCount)
			PlayerPrefs.SetInt("CoinsHighScore", playerInventory.coinCount);
		
		if(PlayerPrefs.GetInt("EnemiesKilledHighScore") < enemiesKilled)
			PlayerPrefs.SetInt("EnemiesKilledHighScore", enemiesKilled);
	}
}
