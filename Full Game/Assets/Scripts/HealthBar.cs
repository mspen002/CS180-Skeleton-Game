using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour 
{
	private Slider m_slider = null;
	public Slider slider { get { if(m_slider == null) m_slider = this.GetComponent<Slider>(); return m_slider; } }
	
	public void UpdateHealth(float healthPercentage) 
	{
		if(slider == null)
			return;
		
		slider.value = healthPercentage;
	}
}
