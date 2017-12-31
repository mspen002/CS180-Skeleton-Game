using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : Singleton<Manager>
{
	protected MainMenuManager() {}
	
	public ScoreUI coinHighScore = null;
	public ScoreUI enemiesKilledHighScore  = null;
	public ScoreUI roundNumHighScore = null;
	
	[Header("Sounds")]
	public AudioSource buttonAudioSource = null;
	public AudioClip buttonPositiveSound = null;
	public AudioClip buttonNegativeSound = null;
	
	private void Start()
	{
		roundNumHighScore.UpdateScore(PlayerPrefs.GetInt("RoundHighScore"));
		enemiesKilledHighScore.UpdateScore(PlayerPrefs.GetInt("EnemiesKilledHighScore"));
		coinHighScore.UpdateScore(PlayerPrefs.GetInt("CoinsHighScore"));
	}
	
	public void LoadScene(string sceneToLoad)
	{
		SceneManager.LoadScene(sceneToLoad);
	}
	
	public void ExitGame()
	{
		Application.Quit();
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