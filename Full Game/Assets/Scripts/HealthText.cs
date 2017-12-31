using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthText : MonoBehaviour 
{
	private Text m_text = null;
	public Text healthText { get { if(m_text == null) m_text = this.GetComponent<Text>(); return m_text; } }
	
	public void UpdateHealthText(int currentHealth, int maxHealth)
	{
		healthText.text = currentHealth.ToString() + " " + "/" + " " + maxHealth.ToString();
	}
}
