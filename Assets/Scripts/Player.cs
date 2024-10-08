using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class Player : MonoBehaviour
{
  private int map = 0;
  private float mvspeed = .2f;

  private float dashspeed = .7f;
  private float dashDuration = 0.1f;
  private float dashTimeLeft;

  private float jumpForce = 1f;
  private float jumpTime = 0.2f;
  private float jumpTimeLeft;

  private bool canDash = true;
  private bool canDoubleJump = false;
  private bool canJump = true;

  private bool isDashing = false;
  private bool isJumping = false;


  private float dashCooldown = 1.5f;
  private float lastUsedDashTime;

  private float screenHalfWidthInWorldUnits;
  public Camera mainCamera;

  private Vector2 startpos_0 = new Vector2(-10.66f, 4.477457f);
  private Vector2 startpos_1 = new Vector2(14.18851f, -4.522478f);
  private Vector2 startpos_2 = new Vector2(40.76052f, 5.477468f);
  private Vector2 startpos_3 = new Vector2(66.0155f, 1.477522f);
  private Vector2 startpos_4 = new Vector2(91.87942f, 5.477522f);
  private Vector2 startpos_5 = new Vector2(117.7591f, -2.522531f);


  private Rigidbody2D rb2d;
  [SerializeField]
  private Sprite _normalForm;
  [SerializeField]
  private Sprite _demonForm;
  private SpriteRenderer _spriteRenderer;
  private Animator _playerAnimator;

  private bool _canchange = true;
  private bool _isFacingRight = true;
  private bool _demonBody = false;
  private bool _isDead = false;
  private bool _isMapChanged = false;


  private bool _jumpingControl = false;

  [SerializeField]
  private AudioSource deathSound;
  [SerializeField]
  private AudioSource dashSound;
  [SerializeField]
  private AudioSource changingSound;
  [SerializeField]
  private AudioSource jumpingSound;

  void Start()
  {
    rb2d = GetComponent<Rigidbody2D>();
    rb2d.freezeRotation = true;
    _spriteRenderer = GetComponent<SpriteRenderer>();
    _spriteRenderer.sprite = _normalForm;
    _playerAnimator = GetComponent<Animator>();
    _playerAnimator.SetBool("isitnormalform", true);
    _playerAnimator.SetBool("regularwalking", false);
    _playerAnimator.SetBool("ishumanjumping", false);
    _playerAnimator.SetBool("isdemondashing", false);
    screenHalfWidthInWorldUnits = mainCamera.aspect * mainCamera.orthographicSize;

  }




  void OnCollisionStay2D(Collision2D col)
  {
    if (col.gameObject.tag == "Floor")
    {
      canDash = true;
    }
  }

  void OnCollisionEnter2D(Collision2D col)
  {
    if (col.gameObject.tag == "Diken")
    {
      deathSound.Play();
      _playerAnimator.SetBool("isitnormalform", true);

      _demonBody = false;
      _canchange = true;

      _isDead = !_isDead;
      switch (map)
      {
        case 0:
          rb2d.position = startpos_0;
          break;
        case 1:
          rb2d.position = startpos_1;
          break;
        case 2:
          rb2d.position = startpos_2;
          break;
        case 3:
          rb2d.position = startpos_3;
          break;
        case 4:
          rb2d.position = startpos_4;
          break;
        case 5:
          rb2d.position = startpos_5;
          break;
        default:
          rb2d.position = startpos_0;
          break;
      }

    }
    if (col.gameObject.tag == "Floor")
    {
      _jumpingControl = false;
      JumpingAnimation();
      foreach (ContactPoint2D contact in col.contacts)
      {
        if (contact.normal.y > 0.5f)
        {
          if (jumpTimeLeft <= 0)
          {
            isJumping = false;
            canJump = true;
          }
        }
      }

    }
  }

  // Update is called once per frame
  void Update()
  {

    Dash();
    Move();
    Jump();
    ChangingForm();
    BodyChecker();
    MoveCamera();
  }

  private void MoveCamera()
  {
    if (IsPlayerOutOfBounds())
    {
      MoveCameraHorizontally();
    }
  }


  private void ChangingForm()
  {
    if (!Input.GetKeyDown(KeyCode.C)) return;
    changingSound.Play();
    _playerAnimator.SetBool("isitnormalform", !_canchange);

    _demonBody = !_demonBody;
    _canchange = !_canchange;
  }

  private void Move()
  {
    if (!isDashing)
    {
      float h = Input.GetAxisRaw("Horizontal");
      Vector2 movement = new Vector2(h, 0);
      movement = movement.normalized * mvspeed;
      rb2d.MovePosition(rb2d.position + movement);
      if (h > 0)
      {
        if (_isFacingRight == false)
        {
          Flip();
        }
        _playerAnimator.SetBool("regularwalking", true);
      }

      else if (h < 0)
      {
        if (_isFacingRight == true)
        {
          Flip();
        }
        _playerAnimator.SetBool("regularwalking", true);
      }
      else
      {
        _playerAnimator.SetBool("regularwalking", false);
      }
    }

  }

  private void Jump()
  {

    bool jumpKey = Input.GetButtonDown("Jump");

    if (canJump || (canDoubleJump && _demonBody))
    {
      if (jumpKey)
      {
        _jumpingControl = true;
        JumpingAnimation();
        _playerAnimator.SetBool("ishumanjumping", true);
        canDoubleJump = !canDoubleJump;
        isJumping = true;
        jumpTimeLeft = jumpTime;
        canJump = false;
        jumpingSound.Play();
      }
      else
      {
        canJump = true;
      }

    }

    if (isJumping)
    {
      if (jumpTimeLeft > 0)
      {
        float h = Input.GetAxisRaw("Horizontal");

        Vector2 movement = new Vector2(h * mvspeed, jumpForce);
        jumpTimeLeft -= Time.deltaTime;
        rb2d.MovePosition(rb2d.position + movement * (jumpTimeLeft / jumpTime));
      }
    }
  }

  private void Dash()
  {
    if (!_demonBody) return;

    float dashKey = Input.GetAxisRaw("DashKey");

    if (dashKey != 0 && canDash && Time.time > lastUsedDashTime + dashCooldown)
    {
      dashSound.Play();
      _playerAnimator.SetBool("isdemondashing", true);
      canDash = false;
      isDashing = true;
      dashTimeLeft = dashDuration;
      lastUsedDashTime = Time.time;
    }

    if (isDashing)
    {
      float h = Input.GetAxisRaw("Horizontal");
      if (h > 0)
      {
        if (_isFacingRight == false)
        {
          Flip();
        }
        _playerAnimator.SetBool("regularwalking", true);
      }
      else
      {
        if (_isFacingRight == true)
        {
          Flip();
        }
        _playerAnimator.SetBool("regularwalking", true);
      }

      Vector2 movement = new Vector2(h, 0).normalized * dashspeed;
      dashTimeLeft -= Time.deltaTime;
      if (dashTimeLeft <= 0)
      {
        _playerAnimator.SetBool("isdemondashing", false);
        isDashing = false;
      }
      rb2d.MovePosition(rb2d.position + movement);
    }
  }

  private void Flip()
  {
    _isFacingRight = !_isFacingRight;
    Vector2 theScale = transform.localScale;
    theScale.x *= -1;
    transform.localScale = theScale;
  }

  private void BodyChecker()
  {
    float h = Input.GetAxisRaw("Horizontal");
    if (_demonBody == true && h != 0)
    {
      _playerAnimator.SetBool("demonwalking", true);
    }
    else
    {
      _playerAnimator.SetBool("demonwalking", false);
    }
  }

  public Animator getPlayerAnimator()
  {
    return _playerAnimator;
  }

  public void restartFromStart()
  {
    deathSound.Play();
    rb2d.position = startpos_0;
    map = 0;

    _playerAnimator.SetBool("isitnormalform", true);

    _demonBody = false;
    _canchange = true;
  }

  private void JumpingAnimation()
  {
    if (_jumpingControl == true)
    {
      if (_demonBody == true)
      {
        _playerAnimator.SetBool("isdemonjumping", true);
      }
      else
      {
        _playerAnimator.SetBool("ishumanjumping", true);
      }
    }
    else
    {
      if (_demonBody == true)
      {
        _playerAnimator.SetBool("isdemonjumping", false);
      }
      else
      {
        _playerAnimator.SetBool("ishumanjumping", false);
      }

    }
  }


  bool IsPlayerOutOfBounds()
  {
    // Get the player's position relative to the camera
    Vector3 playerViewportPosition = mainCamera.WorldToViewportPoint(transform.position);

    // Check if the player is outside the camera's view (X-axis)
    return playerViewportPosition.x < 0 || playerViewportPosition.x > 1;
  }

  void MoveCameraHorizontally()
  {
    Vector3 cameraPosition = mainCamera.transform.position;

    // Check if the player is to the left or right of the screen and move the camera accordingly
    if (transform.position.x < mainCamera.transform.position.x)
    {
      // Player is on the left, move camera one screen width to the left
      cameraPosition.x -= screenHalfWidthInWorldUnits * 2;
      map--;
      _isMapChanged = !_isMapChanged;
    }
    else if (transform.position.x > mainCamera.transform.position.x)
    {
      // Player is on the right, move camera one screen width to the right
      cameraPosition.x += screenHalfWidthInWorldUnits * 2;
      map++;
      _isMapChanged = !_isMapChanged;
    }

    // Apply the new camera position
    mainCamera.transform.position = cameraPosition;
  }


  public bool getIsDead()
  {
    return _isDead;
  }

  public bool getIsMapChanged()
  {
    return _isMapChanged;
  }

}
