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

  private float jumpForce = 300f;
  private float jumpTime = 0.2f;
  private float jumpTimeLeft;

  private bool canDash = true;
  private bool canDoubleJump = false;
  private bool canJump = true;

  private bool isDashing = false;
  private bool isJumping = false;

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
  }


  private void ChangingForm()
  {
    if (!Input.GetKeyDown(KeyCode.C)) return;
    //if (!_canchange) return;
    _playerAnimator.SetBool("isitnormalform", !_canchange);
    _canchange = !_canchange;
    _demonBody = !_demonBody;
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
        canDoubleJump = !canDoubleJump;
        isJumping = true;
        jumpTimeLeft = jumpTime;
        canJump = false;
        rb2d.velocity = new Vector2(rb2d.velocity.x, 0); // Reset vertical velocity
        rb2d.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse); // Apply force for the jump
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
        rb2d.AddForce(new Vector2(rb2d.velocity.x, jumpForce));
        jumpTimeLeft -= Time.deltaTime;
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
}
