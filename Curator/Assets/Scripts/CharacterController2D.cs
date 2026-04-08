using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class CharacterController2D : MonoBehaviour
{
	public float m_JumpForce = 800f;							// Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
	/*
	[SerializeField] private Transform m_WallCheckL;
    [SerializeField] private Transform m_WallCheckR;
    */
	[SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

	public StatTracker sTracker;
    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
    private float direction;
    /*
	private float k_WallRadius = .2f; //Radius of the overlap circle to determine if wallsliding
	private bool m_Sliding;             // Whether or not the player is wallsliding
	
	
	private Vector2 wallJumpPower = new Vector2(8f, 16f);
	*/
    private bool hasFirearm;
	public GameObject firearmHurtboxSide;
	public GameObject firearmHurtboxUp;
	public GameObject firearmHurtboxDown;
	public float gunDirection;

	public GameObject attackHurtboxSide;
	public GameObject attackHurtboxUp;
	public GameObject attackHurtboxDown;
	public float attackDirection;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

	

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		m_MovementSmoothing = 0.05f;

		gunDirection = 30f;
		attackDirection = 15.5f;

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;
		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (wasGrounded)
					OnLandEvent.Invoke();
			}
		}

        /*
        bool wasSliding = m_Sliding;
		m_Sliding = false;
		*/
        
    }

    public void Update()
    {
		direction = -transform.localScale.x;


        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!hasFirearm)
            {
                return;
            }
            else
                GunAttack();
        }
		if (Input.GetButtonDown("Fire1"))
		{
		Attack();
		}
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("down");
            Physics2D.IgnoreLayerCollision(9, 7, true);
			Invoke("UndoIgnore", 0.3f);
        }

		/*
        if (Physics2D.OverlapCircle(m_WallCheckL.position, k_WallRadius, m_WhatIsGround))
        {
            if (Input.GetButtonDown("Jump") && !m_Grounded)
            {
                Debug.Log("Wall");
                m_Rigidbody2D.velocity = new Vector2(direction * wallJumpPower.x, wallJumpPower.y);
				if (transform.localScale.x != direction)
				{
					Flip();
				}
            }

        }
        if (Physics2D.OverlapCircle(m_WallCheckR.position, k_WallRadius, m_WhatIsGround))
        {
            if (Input.GetButtonDown("Jump") && !m_Grounded)
            {
                Debug.Log("Wall");

                m_Rigidbody2D.velocity = new Vector2(direction * wallJumpPower.x, wallJumpPower.y);
            }
        }
		*/

    }
	private void UndoIgnore()
	{
        Physics2D.IgnoreLayerCollision(9, 7, false);
    }

    public void Move(float move, bool crouch, bool jump)
	{
		// If crouching, check to see if the character can stand up
		if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround) && m_Grounded)
			{
				crouch = true;
			}
		}


		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{
			
			// If crouching
			if (crouch)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			} else
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}
			
			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);

			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if (m_Grounded && jump)
		{
			// Add a vertical force to the player.
			m_Grounded = false;
			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 11.5f);
		}
		/*
		else if (!m_Grounded && jump && !m_DoubleJumped)
		{
			m_DoubleJumped = true;
			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 15.5f);
        }*/
	}

	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
		gunDirection *= -1;
		attackDirection *= -1;
	}
	private void Attack()
	{
		m_MovementSmoothing = 0.4f;
		if (Input.GetKey(KeyCode.W))
		{
			attackHurtboxUp.gameObject.SetActive(true);
			//m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, -11.5f);
            Invoke("DisableHurtBox", 0.5f);
            Invoke("ResetMovementSmoothing", 0.1f);
			return;
        }
		if (Input.GetKey(KeyCode.S))
		{
            attackHurtboxDown.gameObject.SetActive(true);
           // m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 11.5f);
            Invoke("DisableHurtBox", 0.5f);
            Invoke("ResetMovementSmoothing", 0.1f);
        }
		else
		{
            attackHurtboxSide.gameObject.SetActive(true);
           // m_Rigidbody2D.velocity = new Vector2(attackDirection, m_Rigidbody2D.velocity.y);
            Invoke("DisableHurtBox", 0.5f);
            Invoke("ResetMovementSmoothing", 0.5f);
            return;
		}
	}
    private void GunAttack()
	{
		
		if (sTracker.AmmoCount == 0)
		{
			return;
		}
		else
            m_MovementSmoothing = 0.4f;
        if (Input.GetKey(KeyCode.W))
		{
			firearmHurtboxUp.gameObject.SetActive(true);
			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, -15.5f);
			Invoke("DisableHurtBox", 0.5f);
			Invoke("ResetMovementSmoothing", 0.1f);
			sTracker.UpdateAmmoCount(-1);
			return;
		}
		if (Input.GetKey(KeyCode.S))
		{
            firearmHurtboxDown.gameObject.SetActive(true);
			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 15.5f);
			Invoke("DisableHurtBox", 0.5f);
            Invoke("ResetMovementSmoothing", 0.1f);
            sTracker.UpdateAmmoCount(-1);
            return;
		}
		else 
		{
            m_Rigidbody2D.velocity = new Vector2(-gunDirection, m_Rigidbody2D.velocity.y);
			firearmHurtboxSide.gameObject.SetActive(true);
			Invoke("DisableHurtBox", 0.5f);
            Invoke("ResetMovementSmoothing", 0.5f);
            sTracker.UpdateAmmoCount(-1);
            return;
		}
		
    }
	
    private void DisableHurtBox()
	{
		firearmHurtboxUp.gameObject.SetActive(false);
		firearmHurtboxDown.gameObject.SetActive(false);
		firearmHurtboxSide.gameObject.SetActive(false);
		attackHurtboxUp.gameObject.SetActive(false);
        attackHurtboxDown.gameObject.SetActive(false);
        attackHurtboxSide.gameObject.SetActive(false);
    }

	private void ResetMovementSmoothing()
	{
		m_MovementSmoothing = 0.05f;
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == null) return;

		if (collision.gameObject.layer == 13)
		{
			sTracker.UpdateAmmoCount(+3);
			Destroy(collision.gameObject);
			return;
		}

		if (collision.gameObject.layer == 14)
		{
			sTracker.UpdateHealthPoints(+1);
			Destroy(collision.gameObject);
			return;
		}
		if (collision.gameObject.layer == 15)
		{
			hasFirearm = true;
			Destroy(collision.gameObject); return;
		}
        if (collision.gameObject.layer == 11)
        {
            m_Rigidbody2D.velocity = new Vector2(-gunDirection, m_Rigidbody2D.velocity.y);
            sTracker.UpdateHealthPoints(-1);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 12)
		{
            m_Rigidbody2D.velocity = new Vector2(-gunDirection, m_Rigidbody2D.velocity.y);
			sTracker.UpdateHealthPoints(-1);
        }
    }
}
