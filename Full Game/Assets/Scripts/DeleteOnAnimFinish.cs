using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteOnAnimFinish : MonoBehaviour {

	public float delay = 0f;

	private void Start() 
	{
		Destroy(gameObject, this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + delay); 
		Invoke("NoLongerSlicing", this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + delay);
	}
	
	private void NoLongerSlicing()
	{
		Manager.Instance.playerMovement.isSlicing = false;
	}
}
