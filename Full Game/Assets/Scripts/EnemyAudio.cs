using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudio : MonoBehaviour 
{
	public AudioSource enemyVoiceSource = null;
	public AudioClip attackClip = null;
	public AudioClip damagedClip = null;
	
	public void PlayAttackSound()
	{
		enemyVoiceSource.Stop();
		enemyVoiceSource.PlayOneShot(attackClip);
	}
	
	public void PlayDamagedSound()
	{
		enemyVoiceSource.Stop();
		enemyVoiceSource.PlayOneShot(damagedClip);
	}
}
