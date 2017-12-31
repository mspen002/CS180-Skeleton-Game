using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPointerArrow : MonoBehaviour 
{
	private Vector3 originalScale { get; set; }
	private SpriteRenderer m_spriteRenderer = null;
	public SpriteRenderer spriteRenderer { get { if(m_spriteRenderer == null) m_spriteRenderer = this.GetComponent<SpriteRenderer>(); return m_spriteRenderer; } }
	
	private void Awake()
	{
		originalScale = this.transform.localScale;
	}
	
	private void Update() 
	{
		if(Manager.Instance.monsterManager.numLivingEnemies > 0 && Manager.Instance.monsterManager.numLivingEnemies <= 5)
		{
			spriteRenderer.enabled = true;
			
			if(Manager.Instance.playerTransform.position.x > Manager.Instance.monsterManager.firstEnemyTransform.position.x)
				this.transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
			else
				this.transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
			
			Quaternion rotation = Quaternion.LookRotation(Manager.Instance.monsterManager.firstEnemyTransform.position - this.transform.position, this.transform.TransformDirection(this.transform.up));
			this.transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);
		}
		else
		{
			spriteRenderer.enabled = false;
		}
		
		if(Manager.Instance.playerTransform.localScale.x < 0)
			this.transform.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);
		else
			this.transform.localScale = new Vector3(-this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);
	}
}
