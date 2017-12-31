using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceFalling : MonoBehaviour 
{
	public AudioClip hitGroundSound = null;
	
	public int damage { get; set; }
	public float knockback { get; set; }
	public float damageFrequency { get; set; }
	
	private List<EnemyHealth> enemyHealths = new List<EnemyHealth>();
	private bool isRunningDamageEnemiesCoroutine { get; set; }
	
	private void OnDestroy()
	{
		StopCoroutine("DamageEnemies");
	}
	
	private void OnTriggerEnter2D(Collider2D col)
	{
		EnemyHealth enemyHealth = col.transform.GetComponent<EnemyHealth>();
		if(enemyHealth != null)
		{
			enemyHealths.Add(enemyHealth);
			
			if(!isRunningDamageEnemiesCoroutine)
				StartCoroutine("DamageEnemies");
		}
		
		if(col.gameObject.tag == "Ground")
		{
			Manager.Instance.playerMovement.isGroundPounding = false;
			Destroy(gameObject);
			
			Manager.Instance.playerAudio.PlayPlayerVoice(hitGroundSound);
		}
	}
	
	private void OnTriggerExit2D(Collider2D col)
	{
		EnemyHealth enemyHealth = col.transform.GetComponent<EnemyHealth>();
		if(enemyHealth != null)
		{
			for(int i = 0; i < enemyHealths.Count; ++i)
			{
				if(enemyHealths[i].ID == enemyHealth.ID)
				{
					enemyHealths.RemoveAt(i);
					break;
				}
			}
		}
	}
	
	private IEnumerator DamageEnemies()
	{
		isRunningDamageEnemiesCoroutine = true;
		
		while(enemyHealths.Count > 0)
		{
			for(int i = 0; i < enemyHealths.Count;)
			{
				EnemyHealth enemyHealth = enemyHealths[i];
				
				if(enemyHealth == null)
				{
					enemyHealths.RemoveAt(i);
					continue;
				}
				
				enemyHealth.TakeDamage(damage);
				
				Transform enemyTransform = enemyHealth.transform;
					
				Vector3 awayFromPlayerDirection = enemyTransform.position - Manager.Instance.playerTransform.position;
				Vector3 forceDirection = (Vector3.up + awayFromPlayerDirection).normalized;
				
				Rigidbody2D enemyBody = enemyTransform.GetComponent<Rigidbody2D>();
				enemyBody.AddForce(forceDirection * (knockback), ForceMode2D.Impulse);
				
				++i;
			}
			
			yield return new WaitForSeconds(damageFrequency);
		}
		
		isRunningDamageEnemiesCoroutine = false;
	}
}
