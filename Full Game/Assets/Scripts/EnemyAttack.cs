using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour 
{
	[Header("Collsion Stay")]
	public bool dealDamageOnCollisionStay = false;
	
	[Header("Attack Damage")]
	public int attackDamage = 0;
	
	[Header("Time Between Attacks")]
	public float attackCooldownTime;
	public float lastAttAttackTime { get; set; }
	
	private EnemyAudio m_enemyAudio = null;
	public EnemyAudio enemyAudio { get { if(m_enemyAudio == null) m_enemyAudio = this.GetComponent<EnemyAudio>(); return m_enemyAudio; } }
	
	private void OnCollisionStay2D(Collision2D col) 
	{
		if(Manager.Instance.isPaused)
			return;
		
		if(!dealDamageOnCollisionStay)
			return;
		
		PlayerHealth playerHealth = col.transform.GetComponent<PlayerHealth>();
		if(playerHealth != null)
		{
			if(Time.time - lastAttAttackTime >= attackCooldownTime && playerHealth.TakeDamage(attackDamage))
			{
				lastAttAttackTime = Time.time;
				
				if(enemyAudio != null)	
					enemyAudio.PlayAttackSound();
			}
		}
    }
}
