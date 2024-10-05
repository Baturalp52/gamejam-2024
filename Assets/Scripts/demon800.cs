using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  private float mvspeed = .2f;
  private float dashspeed = .7f;
  private float dashDuration = 0.1f;
  private float dashTimeLeft;

  private bool canDash = true;
  private bool isDashing = false;

  private Rigidbody2D rb2d;
  [SerializeField]
  private Sprite _normalForm;
  [SerializeField]
  private Sprite _demonForm;
  private SpriteRenderer _spriteRenderer;
  // Start is called before the first frame update
  void Start()
  {
    rb2d = GetComponent<Rigidbody2D>();
    rb2d.freezeRotation = true;
    _spriteRenderer = GetComponent<SpriteRenderer>();
    _spriteRenderer.sprite = _normalForm;

  }

  void OnCollisionEnter2D(Collision2D col)
  {
    if (col.gameObject.tag == "Floor")
    {
      canDash = true;
    }
  }

  void OnCollisionStay2D(Collision2D col)
  {
    if (col.gameObject.tag == "Floor")
    {
      canDash = true;
    }
  }

  // Update is called once per frame
  void Update()
  {

    Dash();
    Move();
    ChangingForm();

  }


  private void ChangingForm()
  {
    if (!Input.GetKeyDown(KeyCode.C)) return;

    if (_spriteRenderer.sprite == _normalForm)
    {
      _spriteRenderer.sprite = _demonForm;
    }

    else
    {
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
