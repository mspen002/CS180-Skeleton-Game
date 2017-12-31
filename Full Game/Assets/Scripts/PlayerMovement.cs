using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour 
{
	[Header("Movement")]
	public GameObject groundChecker;
	public LayerMask groundCheckMask;
	public float speed = 10;
	public float jump = 10;
	
	[Header("Attack: Stats")]
	public int maxAttackPower = 20;
	
	[Header("Attack: Slash")]
	public GameObject slashArea;
	public GameObject sliceObjectRight;
	public GameObject sliceObjectLeft;
	public int sliceBaseDamage;
	public float sliceKnockback;
	public float sliceDestroyDelay = 0.1f;
	public GameObject currentSliceObj { get; set; }
	public bool isSlicing { get; set; }
	
	[Header("Attack: GroundPound")]
	public GameObject groundPoundObject;
	public float fallingKnockback;
	public int fallingDamage = 20;
	public float fallingAttackDamageFrequency = 0.1f;
	public GameObject currentGroundPoundObj { get; set; }
	
	[Header("Attack: SpinAttack")]
	public GameObject SpinAttackObject;
	public float spinKnockback;
	public int spinDamage = 10;
	public float spinAttackDamageFrequency = 0.25f;
	public float spinAttackLifespan = 1f;
	public GameObject currentSpinAttackObj { get; set; }
	public bool isSpinAttacking { get; set; }
	
	[Header("Attack: Ultimate")]
	public GameObject ultimateObjectRight;
	public GameObject ultimateObjectLeft;
	public float ultKnockback;
	public int ultDamage = 10;
	public float ultDamageFrequency = .25f;
	public float ultimateLifespan = 3f;
	public int ultimateManaCost = 25;
	public bool isUsingUltimate { get; set; }
	private GameObject currentUltimateObj { get; set; }
	
	//General attack variables
	public bool isGroundPounding { get; set; }
	private int m_attackPower = 0;
	public int attackPower { get { return m_attackPower; } set { m_attackPower = Mathf.Min(Mathf.Max(value, 0), maxAttackPower); } }
	public float percentageAttackPower { get { return (float)attackPower / (float)maxAttackPower; } }
	
	[Header("Animation")]
    public Animator anim;
    private bool faceRight = true;
	private bool rotating = false;
	SpriteRenderer m_SpriteRenderer;
	
	[Header("Sounds")]
	public AudioClip slashGrunt = null;
	public AudioClip groundPoundGrunt = null;
	public AudioClip spinAttackGrunt = null;
	
	private Vector3 flipper;
	
	private Rigidbody2D m_playerRigidbody = null;
    private Rigidbody2D playerRigidbody { get { if(m_playerRigidbody == null) m_playerRigidbody = this.GetComponent<Rigidbody2D>(); return m_playerRigidbody; } }
	

	private void Awake() 
	{
		playerRigidbody.freezeRotation = true;
        anim = GetComponent<Animator>();
        //flipper = GetComponent<Transform>();
		m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

	private void FixedUpdate() 
	{
		if(Manager.Instance.isPaused)
			return;
		
		HandleMovement();
		HandleAttacks();
		
		if(currentGroundPoundObj == null && isGroundPounding)
			EndGroundPound();
		
		if(currentSliceObj == null && isSlicing)
			DestroySlice();
		
		if(currentSpinAttackObj == null && isSpinAttacking)
			EndSpinAttack();
		
		if(currentUltimateObj == null && isUsingUltimate)
			EndUlti();
    }
	
	private void HandleMovement()
	{
		//playerRigidbody.velocity(movement*speed, 0);
		RaycastHit2D groundCheck = Physics2D.Raycast(groundChecker.transform.position, Vector2.down, 0.1f, groundCheckMask);
		float moveX = Input.GetAxis("Horizontal");
		Vector2 movement = new Vector2(moveX, 0);
		if(Input.GetAxis("Vertical") > 0 || Input.GetAxis("Jump") > 0)
		{
			if(groundCheck.collider != null)
			{
				playerRigidbody.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
			}
		}
		//playerRigidbody.velocity = new Vector2(moveX * speed, playerRigidbody.velocity.y);
		anim.SetFloat("Run", Mathf.Abs(moveX));
		playerRigidbody.velocity = new Vector2(moveX * speed, playerRigidbody.velocity.y);
		if(groundCheck.collider != null)
		{
			//print("check: "+ groundCheck.collider);
			anim.SetBool("Jump", false);
			// isGroundPounding = false;
			rotating = false;
			//playerRigidbody.velocity = new Vector2(moveX * speed, playerRigidbody.velocity.y);
		}
		else if(groundCheck.collider == null && isGroundPounding == true)
		{
			anim.SetBool("Jump", true);
		}
		else
		{
			anim.SetBool("Jump", true);
			//playerRigidbody.velocity = new Vector2(moveX * speed, playerRigidbody.velocity.y);
		}
        if(faceRight == true && moveX < 0)
        {
            faceRight = false;
            flipper = transform.localScale;
			flipper.x *= -1;
			transform.localScale = flipper;
        }
        if (faceRight != true && moveX > 0)
        {
            faceRight = true;
            flipper = transform.localScale;
			flipper.x *= -1;
			transform.localScale = flipper;
        }
		// if(rotating == true)
		// {
			// m_SpriteRenderer.transform.Rotate(Vector3.right * Time.deltaTime);;
		// }
	}
	
	private void HandleAttacks()
	{
		if(isUsingUltimate || isGroundPounding || isSlicing)
			return;
		
		RaycastHit2D groundCheck = Physics2D.Raycast(groundChecker.transform.position, Vector2.down, 0.1f, groundCheckMask);
		if(Input.GetKey(KeyCode.Mouse0) && groundCheck.collider != null && !isSpinAttacking)
		{
			anim.SetBool("Attack", true);
			// Slice();
		}
		
		if(Input.GetKey(KeyCode.Mouse1) && groundCheck.collider != null && !isSpinAttacking)
		{
			Ulti();
		}
		
		//air attacks
		if(Input.GetKey(KeyCode.Mouse0) && groundCheck.collider == null)
		{
			if(isSpinAttacking)
				EndSpinAttack();
			
			playerRigidbody.velocity = new Vector2(0f, 0f);
			playerRigidbody.AddForce(Vector2.down * (jump*10f), ForceMode2D.Impulse);
			GroundPound();
		}
		
		if(Input.GetKey(KeyCode.Mouse1) && groundCheck.collider == null && !isSpinAttacking)
		{
			float moveX = Input.GetAxis("Horizontal");
			Vector2 movement = new Vector2(moveX, 0);
			//playerRigidbody.velocity = new Vector2(moveX, 0);
			//playerRigidbody.AddForce(movement * jump * speed, ForceMode2D.Impulse);
			AirSpin();
		}
	}
	
	public void Slice()
	{
		currentSliceObj = Instantiate(faceRight ? sliceObjectRight : sliceObjectLeft, slashArea.transform.position, transform.rotation);
		currentSliceObj.transform.parent = this.transform;
		
		Slice slice = currentSliceObj.GetComponent<Slice>();
		slice.damage = sliceBaseDamage + attackPower;
		slice.knockback = sliceKnockback;
		
		Manager.Instance.playerAudio.PlayPlayerVoice(slashGrunt);
		isSlicing = true;
		Invoke("attackOver", sliceDestroyDelay);
	}
	
	public void Ulti()
	{
		if(!Manager.Instance.playerHealth.SpendMana(ultimateManaCost))
			return;
		
		currentUltimateObj = Instantiate(faceRight ? ultimateObjectRight : ultimateObjectLeft, slashArea.transform.position, transform.rotation);
		currentUltimateObj.transform.parent = this.transform;
		Invoke("EndUlti", ultimateLifespan);
		
		Ultimate ultimate = currentUltimateObj.GetComponentInChildren<Ultimate>();
		ultimate.damage = ultDamage+ attackPower;
		ultimate.knockback = ultKnockback;
		ultimate.damageFrequency = ultDamageFrequency;
		
		isUsingUltimate = true;
	}
	
	public void EndUlti()
	{
		CancelInvoke("EndUlti");
		Destroy(currentUltimateObj);
		currentUltimateObj = null;
		isUsingUltimate = false;
	}
	
	public void GroundPound()
	{
		currentGroundPoundObj = Instantiate(groundPoundObject, groundChecker.transform.position, transform.rotation);
		currentGroundPoundObj.transform.parent = this.transform;
		
		SliceFalling fall = currentGroundPoundObj.GetComponent<SliceFalling>();
		fall.damage = fallingDamage + attackPower;
		fall.damageFrequency = fallingAttackDamageFrequency;
		fall.knockback = fallingKnockback;
		isGroundPounding = true;
		
		Manager.Instance.playerAudio.PlayPlayerVoice(groundPoundGrunt);
	}
	
	public void EndGroundPound()
	{
		CancelInvoke("EndGroundPound");
		Destroy(currentGroundPoundObj);
		currentGroundPoundObj = null;
		isGroundPounding = false;
	}
	
	public void AirSpin()
	{
		currentSpinAttackObj = Instantiate(SpinAttackObject, this.transform.position, transform.rotation);
		currentSpinAttackObj.transform.parent = this.transform;
		
		Invoke("EndSpinAttack", spinAttackLifespan);
		
		SpinAttack spinAttack = currentSpinAttackObj.GetComponent<SpinAttack>();
		spinAttack.damage = spinDamage + attackPower;
		spinAttack.knockback = spinKnockback;
		spinAttack.damageFrequency = spinAttackDamageFrequency;
		
		isSpinAttacking = true;
		rotating = true;
		
		Manager.Instance.playerAudio.PlayPlayerVoice(spinAttackGrunt);
	}
	
	public void EndSpinAttack()
	{
		CancelInvoke("EndSpinAttack");
		Destroy(currentSpinAttackObj);
		currentSpinAttackObj = null;
		isSpinAttacking = false;
	}
	
	public void attackOver()
	{
		anim.SetBool("Attack", false);
		Invoke("DestroySlice", sliceDestroyDelay);
	}
	
	private void DestroySlice()
	{
		CancelInvoke("attackOver");
		anim.SetBool("Attack", false);
		CancelInvoke("DestroySlice");
		Destroy(currentSliceObj);
		currentSliceObj = null;
		isSlicing = false;
	}
	
	public void IncreaseAttackPower(int increaseAmount)
	{
		attackPower += increaseAmount;
	}
	
	public void EndAttacks()
	{
		if(isUsingUltimate)
			EndUlti();
		
		if(isSlicing)
			attackOver();
		
		if(isSpinAttacking)
			EndSpinAttack();
		
		if(isGroundPounding)
			EndGroundPound();
	}
}


