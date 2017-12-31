using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour 
{
	private Text m_text = null;
	public Text scoreText { get { if(m_text == null) m_text = this.GetComponentInChildren<Text>(); return m_text; } }
	
	public void UpdateScore(int scoreCount)
	{
		scoreText.text = scoreCount.ToString();
	}
}
