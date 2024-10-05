using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Player : MonoBehaviour
{
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

  public Camera mainCamera;
  private float screenHalfWidthInWorldUnits;

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
  [SerializeField]
  private bool _jumpingControl = false;
  // Start is called before the first frame update
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
    _playerAnimator.SetBool("isitnormalform", !_canchange);

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

    if (canJump || canDoubleJump)
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

    float dashKey = Input.GetAxisRaw("DashKey");

    if (dashKey != 0 && canDash)
    {
      canDash = false;
      isDashing = true;
      dashTimeLeft = dashDuration;
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

  private void JumpingAnimation()
  {
    if (_jumpingControl == true)
    {
      _playerAnimator.SetBool("ishumanjumping", true);
    }
    else
    {
      _playerAnimator.SetBool("ishumanjumping", false);
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
    }
    else if (transform.position.x > mainCamera.transform.position.x)
    {
      // Player is on the right, move camera one screen width to the right
      cameraPosition.x += screenHalfWidthInWorldUnits * 2;
    }

    // Apply the new camera position
    mainCamera.transform.position = cameraPosition;
  }

}
