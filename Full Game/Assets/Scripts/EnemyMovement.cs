using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour 
{
	public enum HoverControl { AlwaysMatchPlayer, OnlyMatchPlayerIfSeen, HoverWithoutMatching, Grounded }
	public enum RotationControl { AlwaysFacePlayer, OnlyFacePlayerIfSeen, IgnorePlayer }
	public enum RotationFiniteControl { RotateAll, OnlyFaceOnX } 
	
	[Header("Main Behavior Controls")]
	public bool wanderLeftRight;
	public LayerMask groundCheckMask;
	public bool turnAroundIfNoFloor;
	public float floorCheckXOffest;
	public float floorCheckDistance;
	public bool clampWanderDistance;
	public float wanderDistance;
	public HoverControl hoverControl;
	public RotationControl rotationControl;
	public RotationFiniteControl rotationFiniteControl;
	public float visionRange;
	public LayerMask playerVisionMask;
	public bool aggroTowardPlayer;
	
	[Header("Horizontal Movement")]
	public float wallCheckDistance;
	
	[Header("Toward Player Movement")]
	public float moveTowardForce;
	public float towardDampeningForce;
	public float towardMaxVelocity;
	
	[Header("Away From Player Movement")]
	public float moveAwayForce;
	public float awayDampeningForce;
	public float awayMaxVelocity;
	
	[Header("Hover")]
	public float hoverForce;
	public float riseDampeningForce;
	public float fallDampeningForce;
	public float riseMaxVelocity;
	public float fallMaxVelocity;
	public float playerHeightModifier;
	public float groundCheckDistance;
	
	//References
	private EnemyAttack m_enemyAttack = null;
	private EnemyAttack enemyAttack { get { if(m_enemyAttack == null) m_enemyAttack = this.GetComponent<EnemyAttack>(); return m_enemyAttack; } }
	private Rigidbody2D m_enemyRigidbody = null;
	private Rigidbody2D enemyRigidbody { get { if(m_enemyRigidbody == null) m_enemyRigidbody = this.GetComponent<Rigidbody2D>(); return m_enemyRigidbody; } }
	private Transform playerTransform { get { return Manager.Instance.playerTransform; } }
	
	//Private controls
	private bool recentlyHitPlayer { get { return ((Time.time < enemyAttack.attackCooldownTime ? enemyAttack.attackCooldownTime : Time.time) - enemyAttack.lastAttAttackTime < enemyAttack.attackCooldownTime); } }
	private Vector3 enemyOriginalScale { get; set; }
	private RaycastHit2D playerVisionRaycast { get; set; }
	private bool canSeePlayer { get { return playerVisionRaycast.transform != null && playerVisionRaycast.transform.GetComponentInChildren<PlayerHealth>() != null; } }
	private bool moveRight { get; set; }
	private float wanderStartXPosition { get; set; }
	private float wanderDistanceRemaining { get { return wanderDistance - Mathf.Abs(this.transform.position.x - wanderStartXPosition); } }
	private bool recentlyChasedPlayer { get; set; }
	
	private void Awake()
	{
		enemyOriginalScale = this.transform.localScale;
		wanderStartXPosition = this.transform.position.x;
	}
	
	private void FixedUpdate()
	{
		if(Manager.Instance.isPaused)
			return;
		
		playerVisionRaycast = Physics2D.Raycast(this.transform.position, playerTransform.position - this.transform.position, visionRange, playerVisionMask);
		
		if(hoverControl != HoverControl.Grounded)
			Hover();
		
		HorizontalMovement();
		
		switch(rotationControl)
		{
			case RotationControl.AlwaysFacePlayer:
				FacePlayer();
			break;
			case RotationControl.OnlyFacePlayerIfSeen:
				if(canSeePlayer)
					FacePlayer();
				else
					FaceLeftRight();
			break;
			case RotationControl.IgnorePlayer:
				FaceLeftRight();
			break;
		}
	}
	
	private void Hover()
	{
		//Make sure these values are never zero to avoid errors
		float enemySafeYPosition = this.transform.position.y == 0f ? 0.01f : this.transform.position.y;
		float playerSafeYPosition = (playerTransform.position.y + playerHeightModifier) == 0f ? 0.01f : (playerTransform.position.y + playerHeightModifier);
		
		RaycastHit2D groundCheck = Physics2D.Raycast(this.transform.position, Vector2.down, groundCheckDistance, groundCheckMask);
		RaycastHit2D roofCheck = Physics2D.Raycast(this.transform.position, Vector2.up, groundCheckDistance, groundCheckMask);
		
		//If you are too close to the ground/roof then move away from it. Otherwise move to player's Y
		if(groundCheck.transform != null)
		{
			float goalYPosition = groundCheck.point.y + groundCheckDistance;
			enemyRigidbody.AddForce(Vector2.up * hoverForce * Mathf.Min(1f, Mathf.Abs(enemySafeYPosition - goalYPosition)));
		}
		else if(roofCheck.transform != null)
		{
			float goalYPosition = roofCheck.point.y - groundCheckDistance;
			enemyRigidbody.AddForce(Vector2.down * hoverForce * Mathf.Min(1f, Mathf.Abs(enemySafeYPosition - goalYPosition)));
		}
		else if((hoverControl == HoverControl.AlwaysMatchPlayer || (hoverControl == HoverControl.OnlyMatchPlayerIfSeen && canSeePlayer)) && enemySafeYPosition - playerSafeYPosition < 0f)
		{
			enemyRigidbody.AddForce(Vector2.up * hoverForce * Mathf.Min(1f, Mathf.Abs(enemySafeYPosition - playerSafeYPosition)));
		}
		
		//If you are rising/falling too fast then slow down
		if(enemyRigidbody.velocity.y > riseMaxVelocity)
			enemyRigidbody.AddForce(Vector2.down * riseDampeningForce * Mathf.Abs(riseMaxVelocity / enemyRigidbody.velocity.y));
		else if(enemyRigidbody.velocity.y < -fallMaxVelocity)
			enemyRigidbody.AddForce(Vector2.up * fallDampeningForce * Mathf.Abs(fallMaxVelocity / enemyRigidbody.velocity.y));
	}
	
	private void HorizontalMovement()
	{
		//If you don't see the player then move left/right otherwise move towards/away from player
		if(recentlyHitPlayer)
		{
			// Debug.Log("AWAY");
			MoveAwayFromPlayer();
		}
		else if(canSeePlayer && aggroTowardPlayer)
		{
			// Debug.Log("TOWARDS");
			MoveTowardPlayer();
		}
		else if(wanderLeftRight)
		{
			// Debug.Log("RIGHT/LEFT");
			MoveLeftRight();
		}
		
		//If you are moving too fast then slow down
		if(enemyRigidbody.velocity.x > towardMaxVelocity)
			enemyRigidbody.AddForce(Vector2.left * towardDampeningForce * Mathf.Abs(towardMaxVelocity / enemyRigidbody.velocity.x));
		else if(enemyRigidbody.velocity.x < -towardMaxVelocity)
			enemyRigidbody.AddForce(Vector2.right * towardDampeningForce * Mathf.Abs(towardMaxVelocity / enemyRigidbody.velocity.x));
	}
	
	private void MoveLeftRight()
	{
		//If you recentlyChasedPlayer then set your wander start position to where you currently are
		if(recentlyChasedPlayer)
		{
			wanderStartXPosition = this.transform.position.x;
			recentlyChasedPlayer = false;
		}
		
		RaycastHit2D leftCheck = Physics2D.Raycast(this.transform.position, Vector2.left, wallCheckDistance, groundCheckMask);
		RaycastHit2D rightCheck = Physics2D.Raycast(this.transform.position, Vector2.right, wallCheckDistance, groundCheckMask);
		// Debug.DrawRay(this.transform.position, Vector2.left, Color.green);
		
		if(turnAroundIfNoFloor)
		{
			Vector3 floorCheckPosition = new Vector3(this.transform.position.x + (moveRight ? floorCheckXOffest : -floorCheckXOffest), this.transform.position.y, this.transform.position.z);
			RaycastHit2D floorCheck = Physics2D.Raycast(floorCheckPosition, Vector2.down, floorCheckDistance, groundCheckMask);
			if(floorCheck.transform == null)
				moveRight = !moveRight;
			Debug.DrawRay(floorCheckPosition, Vector2.down, Color.green);
		}
		else if(clampWanderDistance)
		{
			//Walk until you hit a wall or hit your clamp distance
			if(wanderDistanceRemaining <= 0)
			{
				moveRight = !moveRight;
				wanderStartXPosition = this.transform.position.x;
			}
		}

		//Walk until you hit a wall and then turn around
		if(moveRight && rightCheck.transform != null)
			moveRight = false;
		else if(!moveRight && leftCheck.transform != null)
			moveRight = true;

		
		//Move right/left
		if(moveRight)
		{
			// Debug.Log("RIGHT");
			enemyRigidbody.AddForce(Vector2.right * moveTowardForce);
		}
		else
		{
			// Debug.Log("LEFT");
			enemyRigidbody.AddForce(Vector2.left * moveTowardForce);
		}
	}
	
	private void MoveTowardPlayer()
	{
		recentlyChasedPlayer = true;
		
		//Move toward the player mostly in the x direction
		Vector2 forceDirection = new Vector2((playerTransform.position - this.transform.position).normalized.x, (playerTransform.position - this.transform.position).normalized.y * 0.25f);
		enemyRigidbody.AddForce(forceDirection * moveTowardForce);
	}
	
	private void MoveAwayFromPlayer()
	{
		//Move away from the player mostly in the x direction
		Vector2 forceDirection = new Vector2(-(playerTransform.position - this.transform.position).normalized.x, -(playerTransform.position - this.transform.position).normalized.y * 0.75f);
		enemyRigidbody.AddForce(forceDirection * moveAwayForce);
		
		//If you are moving too fast then slow down
		if(enemyRigidbody.velocity.x > awayMaxVelocity)
			enemyRigidbody.AddForce(Vector2.left * awayDampeningForce * Mathf.Abs(awayMaxVelocity / enemyRigidbody.velocity.x));
		else if(enemyRigidbody.velocity.x < -awayMaxVelocity)
			enemyRigidbody.AddForce(Vector2.right * awayDampeningForce * Mathf.Abs(awayMaxVelocity / enemyRigidbody.velocity.x));
	}
	
	private void FacePlayer()
	{
		//Flip the enemy if he changes what side of the player he is on
		if(playerTransform.position.x > this.transform.position.x)
			this.transform.localScale = new Vector3(-enemyOriginalScale.x, enemyOriginalScale.y, enemyOriginalScale.z);
		else
			this.transform.localScale = new Vector3(enemyOriginalScale.x, enemyOriginalScale.y, enemyOriginalScale.z);
		
		//Look Directly at the player or don't change rotation at all
		if(rotationFiniteControl == RotationFiniteControl.RotateAll)
		{
			Quaternion rotation = Quaternion.LookRotation(playerTransform.position - this.transform.position, this.transform.TransformDirection(this.transform.up));
			this.transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);
		}
		else
			this.transform.rotation = new Quaternion(0, 0, 0, 0);
	}
	
	private void FaceLeftRight()
	{
		//Flip the enemy if he changes what side of the player he is on
		if(moveRight)
			this.transform.localScale = new Vector3(-enemyOriginalScale.x, enemyOriginalScale.y, enemyOriginalScale.z);
		else
			this.transform.localScale = new Vector3(enemyOriginalScale.x, enemyOriginalScale.y, enemyOriginalScale.z);
		
		//Always make the enemy look at the player
        this.transform.rotation = new Quaternion(0, 0, 0, 0);
	}
}
