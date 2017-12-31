using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour 
{
	public enum ItemType { Coin, HealthPotion} 
	
	[Header("Type")]
	public ItemType itemType;
	
	[Header("Ground Detection")]
	public LayerMask groundMask;
	
	[Header("Player Detection")]
	public LayerMask playerMask;
	
	[Header("Basic Controls")]
	public bool moveTowardPlayer;
	
	[Header("Coin Variables")]
	public int coinValue = 1;
	
	[Header("Health Potion Variables")]
	public int healAmount;
	
	
	[Header("Mana Potion Variables")]
	public int manaAmount;
	
	[Header("Sounds")]
	public AudioClip coinPickupSound = null;
	
	private bool reachGround = false;
	private bool playerNear = false;
	
	private void Update() 
	{
		if(Manager.Instance.isPaused)
			return;
		
		if(reachGround && playerNear && moveTowardPlayer)
			MoveToPlayer();
	}
	
	public void MoveToPlayer()
	{
		Rigidbody2D rigidBody = this.GetComponent<Rigidbody2D>();
		CircleCollider2D circleCollider = this.GetComponent<CircleCollider2D>();
		Destroy(rigidBody);
		Destroy(circleCollider);
		this.transform.position = Vector3.Lerp(Manager.Instance.playerTransform.position, this.transform.position, .95f);
	}
	
	private void OnCollisionEnter2D(Collision2D col) 
	{
		if(groundMask == (groundMask | (1 << col.gameObject.layer)))
			reachGround = true;
	
		bool hitPlayer = playerMask == (playerMask | (1 << col.gameObject.layer));
		switch(itemType)
		{
			case ItemType.HealthPotion:
				if(hitPlayer)
				{
					CollectItem();
					Destroy(this.gameObject);
				}
			break;
			
		//	case ItemType.ManaPotion:
		//		if(hitPlayer)
		//		{
		//			CollectItem();
		//			Destroy(this.gameObject);
		//		}
		//	break;
			
			default:
			break;
		}
    }

	private void OnTriggerEnter2D(Collider2D col) 
	{
		if(playerMask == (playerMask | (1 << col.gameObject.layer)))
			playerNear = true;
    }
	
	public void CollectItem()
	{
		switch(itemType)
		{
			case ItemType.Coin:
				Manager.Instance.playerAudio.PlayPlayerPickupSound(coinPickupSound);
				Manager.Instance.playerInventory.AcquireCoin(coinValue);
			break;
			
			case ItemType.HealthPotion:
				Manager.Instance.playerHealth.Heal(healAmount);
			break;
			
		//	case ItemType.ManaPotion:
		//		Manager.Instance.playerHealth.manaBoost(manaAmount);
		//	break;
			
			default:
			break;
		}
		
		Destroy(this.gameObject);
	}
}
