using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundText : MonoBehaviour 
{
	private Text m_roundText = null;
	public Text roundText { get { if(m_roundText == null) m_roundText = this.GetComponentInChildren<Text>(); return m_roundText; } }
	
	public void UpdateRound(int currentRound)
	{
		roundText.text = currentRound.ToString();
	}
}
