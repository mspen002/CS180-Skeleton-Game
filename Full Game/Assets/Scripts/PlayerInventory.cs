using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour 
{
	public int coinCount { get; set; }

	public void AcquireCoin(int coinAmount)
	{
		coinCount += coinAmount;
	}
	
	public bool SpendCoins(int numCoins)
	{
		if(coinCount - numCoins < 0)
			return false;
		
		coinCount -= numCoins;
		
		return true;
	}
}
