using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour 
{
	public enum PlatformType { ExpandingPlatform, DisappearPlatform, MovePlatform, AppearPlatform };

	[Header("Type")]
	public PlatformType platformType;
	
	[Header("Expansion Multiplier")]
	public float expandMultiplier = 2;
	
	[Header("Require Touch to Expand")]
	public bool expandTouch = true;
	
	[Header("Time between Expanding")]
	public float expandTime = 2f;
	
	[Header("Player Detection")]
	public LayerMask playerMask;
	
	[Header("Want Reappear")]
	public bool wantReappear = false;
	
	[Header("Require Player to disappear")]
	public bool wantPlayer = false;
	
	[Header("Disappear Time")]
	public float disappearTime = 1;
	
	[Header("Reappear Time")]
	public float reappearTime = 1;
	
	[Header("Move Destination")]
	public float xCoordinate = 0;
	public float yCoordinate = 0;
	
	[Header("Movement Speed")]
	public float movementSpeed = 1;
	
	[Header("Move Only When On")]
	public bool needOn = false;
	
	[Header("Time Until Appear")]
	public float appearOn = 5f;
	
	private bool playerTouch = false;
	private float startTime = 0f;
	private float timePass = 0f;
	private bool longEnough = false;
	private float initialX;
	private float initialY;
	private Vector3 moveDestination;
	private bool isReturning = false;
	private bool isOff = false;
	private BoxCollider2D m_Collider;
	private bool recordTouch = false;
	private bool startNow = false;
	private float appearTime = 0f;
	private float expandDelay = 0f;
	private float delayComparison = 0f;
	private float expandTimeComparison = 0f;
	private bool isExpanded = false;
	private float expandStartTime = 0f;
	private bool disappearCallOnce = false;
	private bool appearCallOnce = false;
	
	private void OnTriggerEnter2D(Collider2D col) 
	{	
		if(playerMask == (playerMask | (1 << col.gameObject.layer)))
		{
			playerTouch = true;
		}
		
		if(startTime == 0f && wantPlayer)
		{
			startTime = Time.time;
		}
		recordTouch = true;
    }
	
	private void OnTriggerExit2D(Collider2D col) 
	{
		if(playerMask == (playerMask | (1 << col.gameObject.layer)))
		{
			playerTouch = false;
		}
		expandDelay = Time.time;
		//startTime = 0f;
    }
	
	private void Start()
	{
		m_Collider = GetComponent<BoxCollider2D>();
		initialX = this.transform.position.x;
		initialY = this.transform.position.y;
		if(!wantPlayer)
		{
			startTime = Time.time;
		}
		appearTime = Time.time;
		expandStartTime = Time.time;
	}
	
	private void Update()
	{
		if(Manager.Instance.isPaused)
			return;
		
		switch(platformType)
		{
			case PlatformType.ExpandingPlatform:
				if(expandTouch)
				{
					if(playerTouch)
					{
						this.transform.localScale = new Vector3((float)(1 * expandMultiplier), (float)1, (float)1);
					}
					else
					{
						delayComparison = Time.time;
						if(delayComparison - expandDelay > .1f)
						{
							this.transform.localScale = new Vector3((float)1, (float)1, (float)1);
						}
					}	
				}
				else
				{
					if(!isExpanded)
					{
						expandTimeComparison = Time.time;
						if(expandTimeComparison - expandStartTime > expandTime)
						{
							this.transform.localScale = new Vector3((float)(1 * expandMultiplier), (float)1, (float)1);
							isExpanded = true;
							expandStartTime = Time.time;
						}
					}
					else
					{
						expandTimeComparison = Time.time;
						if(expandTimeComparison - expandStartTime > expandTime)
						{
							delayComparison = Time.time;
							if(delayComparison - expandDelay > .1f)
							{
								this.transform.localScale = new Vector3((float)1, (float)1, (float)1);
								isExpanded = false;
								expandStartTime = Time.time;
							}
						}				
					}
				}
			break;
			case PlatformType.DisappearPlatform:
				if((recordTouch || !wantPlayer) && isOff == false)
				{
					if(!startNow && wantPlayer)
					{
						startTime = Time.time;
						startNow = true;
					}
					timePass = Time.time - startTime;
					if(timePass >= disappearTime)
					{
						longEnough = true;
					}
				}
				
				if(longEnough && isOff == false)
				{
					//gameObject.SetActive(false);
					foreach(Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
						renderer.enabled = false;
						
					foreach(BoxCollider2D boxCollider2D in gameObject.GetComponentsInChildren<BoxCollider2D>())
					{
						if(!boxCollider2D.isTrigger)
							boxCollider2D.enabled = false;
					}
					
					isOff = true;
					startTime = Time.time;
				}
				if(isOff && wantReappear)
				{
					timePass = Time.time - startTime;
					if(timePass >= reappearTime)
					{
						//gameObject.SetActive(true);
						foreach(Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
						renderer.enabled = true;
						
						foreach(BoxCollider2D boxCollider2D in gameObject.GetComponentsInChildren<BoxCollider2D>())
						{
							if(!boxCollider2D.isTrigger)
								boxCollider2D.enabled = true;
						}
						
						isOff = false;
						longEnough = false;
						recordTouch = false;
						startNow = false;
						startTime = Time.time;
					}
				}
			break;
			case PlatformType.MovePlatform:
				if(!needOn)
				{
					if(!isReturning)
					{
						moveDestination = transform.position;
						moveDestination.x = xCoordinate;
						moveDestination.y = yCoordinate;
						this.transform.position = Vector3.MoveTowards(this.transform.position, moveDestination, movementSpeed * .05f);	
						if(moveDestination == this.transform.position)
						{	
							isReturning = true;
						}
					}
					if(isReturning)
					{
						moveDestination = transform.position;
						moveDestination.x = initialX;
						moveDestination.y = initialY;
						this.transform.position = Vector3.MoveTowards(this.transform.position, moveDestination, movementSpeed * .05f);	
						if(moveDestination == this.transform.position)
						{
							isReturning = false;
						}
					}
				}
				else
				{
					if(playerTouch)
					{
						if(!isReturning)
						{
							moveDestination = transform.position;
							moveDestination.x = xCoordinate;
							moveDestination.y = yCoordinate;
							this.transform.position = Vector3.MoveTowards(this.transform.position, moveDestination, movementSpeed * .05f);	
							if(moveDestination == this.transform.position)
							{	
								isReturning = true;
							}
						}
						if(isReturning)
						{
							moveDestination = transform.position;
							moveDestination.x = initialX;
							moveDestination.y = initialY;
							this.transform.position = Vector3.MoveTowards(this.transform.position, moveDestination, movementSpeed * .05f);	
							if(moveDestination == this.transform.position)
							{
								isReturning = false;
							}
						}
					}
					else
					{
						moveDestination = transform.position;
						moveDestination.x = initialX;
						moveDestination.y = initialY;
						this.transform.position = Vector3.MoveTowards(this.transform.position, moveDestination, movementSpeed * .05f);
					}
				}
			break;
			case PlatformType.AppearPlatform:
				appearTime = Time.time;
				if(appearTime < appearOn)
				{
					gameObject.GetComponent<Renderer>().enabled = false;
					if(!disappearCallOnce)
					{
						m_Collider.enabled = !m_Collider.enabled;
						disappearCallOnce = true;
					}
				}
				else
				{
					gameObject.GetComponent<Renderer>().enabled = true;
					if(!appearCallOnce)
					{
						m_Collider.enabled = !m_Collider.enabled;
						appearCallOnce = true;
					}
				}
			break;
			default:
			break;
		}
	}
}
