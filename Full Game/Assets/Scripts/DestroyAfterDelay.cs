using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterDelay : MonoBehaviour {

	public float delay { get; set; }
	
	public void DestroyThis()
	{
		Destroy(this);
		Manager.Instance.playerMovement.isUsingUltimate = false;
	}
}
