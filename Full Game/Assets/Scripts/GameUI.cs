using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour 
{
	[Header("References")]
	public GameObject inGamePanel = null;
	public GameObject pauseMenuPanel = null;
	public GameObject shopPanel = null;
	public GameObject endGameMenuPanel = null;
	public HealthBar playerManaBar = null;
	public HealthBar playerHealthBar = null;
	public HealthBar shopPlayerHealthBar = null;
	public HealthBar shopPlayerMaxHealthBar = null;
	public HealthBar shopPlayerAttackPowerBar = null;
	public HealthText shopPlayerHealthText = null;
	public HealthText shopPlayerMaxHealthText = null;
	public HealthText shopPlayerAttackPowerText = null;
	public HealthText manaText = null;
	public ScoreUI shopScoreUI = null;
	public ScoreUI scoreUI = null;
	public ScoreUI healthPotionCostUI = null;
	public ScoreUI increaseMaxHealthCostUI = null;
	public ScoreUI increaseAttackDamageCostUI = null;
	public HealthText healthText = null;
	public RoundText roundText = null;
	
	[Header("Shop: Costs")]
	public int healthPotionCost = 50;
	public int increaseMaxHealthCost = 100;
	public int increaseAttackDamageCost = 100;
	public int costIncreaseOnPurchaseAmount = 25;
	
	[Header("Sounds")]
	public AudioSource buttonAudioSource = null;
	public AudioClip buttonPositiveSound = null;
	public AudioClip buttonNegativeSound = null;
	
	
	private void Update()
	{
		if(Manager.Instance.isPaused)
			return;
		
		UpdateHealth();
		UpdateScore();
		UpdateMana();
	}
	
	public void ToggleUI(bool toggleOn)
	{
		inGamePanel.SetActive(toggleOn);
		shopPanel.SetActive(toggleOn);
		pauseMenuPanel.SetActive(toggleOn);
		endGameMenuPanel.SetActive(toggleOn);
	}
	
	public void PauseGame(bool isPaused)
	{	
		ToggleUI(false);
		inGamePanel.SetActive(!isPaused && !Manager.Instance.isBetweenRounds);
		shopPanel.SetActive(!isPaused && Manager.Instance.isBetweenRounds);
		pauseMenuPanel.SetActive(isPaused);
	}
	
	public void EndRound()
	{
		UpdateShopUI();
		
		ToggleUI(false);
		shopPanel.SetActive(true);
	}
	
	public void BeginNextRound()
	{
		ToggleUI(false);
		inGamePanel.SetActive(true);
		
		Manager.Instance.BeginNextRound();
	}
	
	public void EndGame()
	{
		UpdateHealth();
		UpdateScore();
		
		ToggleUI(false);
		endGameMenuPanel.SetActive(true);
	}
	
	public void ResumeGame()
	{
		Manager.Instance.PauseGame(false);
	}
	
	public void UpdateHealth()
	{
		playerHealthBar.UpdateHealth(Manager.Instance.playerHealth.percentageHealth);
		healthText.UpdateHealthText(Manager.Instance.playerHealth.currentHealth, Manager.Instance.playerHealth.currentMaxHealth);
	}
	
	public void UpdateScore()
	{
		roundText.UpdateRound(Manager.Instance.currentRound);
		scoreUI.UpdateScore(Manager.Instance.playerInventory.coinCount);
	}
	
	public void UpdateMana()
	{
		playerManaBar.UpdateHealth(Manager.Instance.playerHealth.percentageMana);
		manaText.UpdateHealthText(Manager.Instance.playerHealth.currentMana, Manager.Instance.playerHealth.currentMaxMana);
	}
	
	public void LoadScene(string sceneToLoad)
	{
		SceneManager.LoadScene(sceneToLoad);
	}
	
	public void ExitGame()
	{
		Application.Quit();
	}
	
	public void ReloadScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
	
	public void IncreaseMaxHealth(int increaseAmount)
	{
		if(!Manager.Instance.playerInventory.SpendCoins(increaseMaxHealthCost) || Manager.Instance.playerHealth.percentageMaxHealth >= 1f)
		{
			PlayNegativeButtonSound();
			return;
		}
		
		PlayPositiveButtonSound();
		
		Manager.Instance.playerHealth.IncreaseMaxHealth(increaseAmount);
		increaseAttackDamageCost += costIncreaseOnPurchaseAmount;
		UpdateShopUI();
	}
	
	public void IncreaseMaxDamage(int increaseAmount)
	{
		if(!Manager.Instance.playerInventory.SpendCoins(increaseAttackDamageCost) || Manager.Instance.playerMovement.percentageAttackPower >= 1f)
		{
			PlayNegativeButtonSound();
			return;
		}
		
		PlayPositiveButtonSound();
		
		Manager.Instance.playerMovement.IncreaseAttackPower(increaseAmount);
		increaseAttackDamageCost += costIncreaseOnPurchaseAmount;
		UpdateShopUI();
	}
	
	public void Heal(int healAmount)
	{
		if(!Manager.Instance.playerInventory.SpendCoins(healthPotionCost) || Manager.Instance.playerHealth.percentageHealth >= 1f)
		{
			PlayNegativeButtonSound();
			return;
		}
		
		PlayPositiveButtonSound();
		
		Manager.Instance.playerHealth.Heal(healAmount);
		UpdateShopUI();
	}
	
	private void UpdateShopUI()
	{
		shopPlayerHealthBar.UpdateHealth(Manager.Instance.playerHealth.percentageHealth);
		shopPlayerHealthText.UpdateHealthText(Manager.Instance.playerHealth.currentHealth, Manager.Instance.playerHealth.currentMaxHealth);

		shopPlayerMaxHealthBar.UpdateHealth(Manager.Instance.playerHealth.percentageMaxHealth);
		shopPlayerMaxHealthText.UpdateHealthText(Manager.Instance.playerHealth.currentMaxHealth, Manager.Instance.playerHealth.maximumMaxHealth);
		
		shopPlayerAttackPowerBar.UpdateHealth(Manager.Instance.playerMovement.percentageAttackPower);
		shopPlayerAttackPowerText.UpdateHealthText(Manager.Instance.playerMovement.attackPower, Manager.Instance.playerMovement.maxAttackPower);
		
		healthPotionCostUI.UpdateScore(healthPotionCost);
		increaseAttackDamageCostUI.UpdateScore(increaseAttackDamageCost);
		increaseMaxHealthCostUI.UpdateScore(increaseMaxHealthCost);
		
		shopScoreUI.UpdateScore(Manager.Instance.playerInventory.coinCount);
	}
	
	public void PlayPositiveButtonSound()
	{
		buttonAudioSource.Stop();
		buttonAudioSource.PlayOneShot(buttonPositiveSound);
	}
	
	public void PlayNegativeButtonSound()
	{
		buttonAudioSource.Stop();
		buttonAudioSource.PlayOneShot(buttonNegativeSound);
	}
}
