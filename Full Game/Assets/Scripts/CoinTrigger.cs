using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinTrigger : MonoBehaviour 
{
	[Header("Player Detection")]
	public LayerMask playerMask;
	
	private void OnTriggerEnter2D(Collider2D col) 
	{
		bool hitPlayer = playerMask == (playerMask | (1 << col.gameObject.layer));
		if(hitPlayer)
		{
			Item item = GetComponentInParent<Item>();
			if(item != null)
				item.CollectItem();
		}
    }
}
