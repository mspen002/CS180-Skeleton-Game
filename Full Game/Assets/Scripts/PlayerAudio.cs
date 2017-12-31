using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour 
{
	public AudioSource playerVoiceSource = null;
	public AudioSource playerAttackSource = null;
	public AudioSource playerPickupSource = null;
	
	public void PlayPlayerVoice(AudioClip audioClip)
	{
		playerVoiceSource.Stop();
		playerVoiceSource.PlayOneShot(audioClip);
	}
	
	public void PlayPlayerAttackSound(AudioClip audioClip)
	{
		playerAttackSource.Stop();
		playerAttackSource.PlayOneShot(audioClip);
	}
	
	public void PlayPlayerPickupSound(AudioClip audioClip)
	{
		playerPickupSource.Stop();
		playerPickupSource.PlayOneShot(audioClip);
	}
}
