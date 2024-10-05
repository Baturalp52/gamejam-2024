using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  private float mvspeed = .2f;

  private float dashspeed = .7f;
  private float dashDuration = 0.1f;
  private float dashTimeLeft;

  private float jumpForce = 250f;
  private float jumpTime = 0.1f;
  private float jumpTimeLeft;

  private bool canDash = true;

  private bool isDashing = false;
  private bool isJumping = false;

  private Rigidbody2D rb2d;
  [SerializeField]
  private Sprite _normalForm;
  [SerializeField]
  private Sprite _demonForm;
  private SpriteRenderer _spriteRenderer;
  private Animator _playerAnimator;
  // Start is called before the first frame update
  void Start()
  {
    rb2d = GetComponent<Rigidbody2D>();
    rb2d.freezeRotation = true;
    _spriteRenderer = GetComponent<SpriteRenderer>();
    _spriteRenderer.sprite = _normalForm;
    _playerAnimator = GetComponent<Animator>();

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
      if (jumpTimeLeft <= 0)
        isJumping = false;
    }
  }

  // Update is called once per frame
  void Update()
  {

    Dash();
    Move();
    Jump();
    ChangingForm();

  }


  private void ChangingForm()
  {
    if (!Input.GetKeyDown(KeyCode.C)) return;

    if (_spriteRenderer.sprite == _normalForm)
    {
      //_playerAnimator.SetBool("NormaltoDemon", true);
      //_playerAnimator.SetBool("DemontoNormal", false);
      _spriteRenderer.sprite = _demonForm;
    }

    else
    {
      //_playerAnimator.SetBool("NormaltoDemon", false);
      //_playerAnimator.SetBool("DemontoNormal", true);
      //_playerAnimator.SetBool("DemontoNormal", false);
      _spriteRenderer.sprite = _normalForm;
    }
  }

  private void Move()
  {
    if (!isDashing)
    {
      float h = Input.GetAxisRaw("Horizontal");
      Vector2 movement = new Vector2(h, 0);
      movement = movement.normalized * mvspeed;
      rb2d.MovePosition(rb2d.position + movement);
    }

  }

  private void Jump()
  {

    if (!isJumping)
    {
      float jumpKey = Input.GetAxisRaw("Jump");
      if (jumpKey != 0)
      {
        isJumping = true;
        jumpTimeLeft = jumpTime;
      }
    }
    else
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
}
