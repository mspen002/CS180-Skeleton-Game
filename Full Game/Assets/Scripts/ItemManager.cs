using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour 
{
	[Header("References")]
	public GameObject potions = null;
	public GameObject coins = null;
	
	private Item[] livingPotions { get { return potions.transform.GetComponentsInChildren<Item>(); } }
	private int numLivingPotions { get { return potions.transform.childCount; } }
	
	private Item[] livingCoins { get { return coins.transform.GetComponentsInChildren<Item>(); } }
	private int numLivingCoins { get { return coins.transform.childCount; } }
	
	private IEnumerator CollectAllCoins()
	{
		while(numLivingCoins > 0)
		{
			foreach(Item item in livingCoins)
			{
				if(item.itemType == Item.ItemType.Coin)
					item.MoveToPlayer();
			}
			yield return new WaitForFixedUpdate();
		}
		
		Manager.Instance.EndRound();
	}
}
