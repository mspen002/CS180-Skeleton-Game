using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slice : MonoBehaviour 
{	
	public int damage { get; set; }
	public float knockback { get; set; }
	
	private void OnTriggerEnter2D(Collider2D col)
	{
		EnemyHealth enemyHealth = col.transform.GetComponent<EnemyHealth>();
		if(enemyHealth != null)
		{
			enemyHealth.TakeDamage(damage);
					
			Vector3 awayFromPlayerDirection = col.transform.position - Manager.Instance.playerTransform.position;
			Vector3 forceDirection = (Vector3.up + awayFromPlayerDirection).normalized;
			
			Rigidbody2D enemyBody = col.transform.GetComponent<Rigidbody2D>();
			enemyBody.AddForce(forceDirection * (knockback), ForceMode2D.Impulse);
		}
	}
}
