using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour {

	private Slider m_slider = null;
	public Slider slider { get { if(m_slider == null) m_slider = this.GetComponent<Slider>(); return m_slider; } }
	
	// Update is called once per frame
	public void UpdateMana (float manaPercentage) 
	{
		if(slider == null)
		{
			return;
		}
		slider.value = manaPercentage;
		
	}
}
