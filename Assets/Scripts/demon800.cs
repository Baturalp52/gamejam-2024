using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  private float mvspeed = 20.0f;
  private float dashspeed = 1.0f;
  private float dashDuration = 0.1f;
  private float dashTimeLeft;
  private bool canDash = true;
  private bool isDashing = false;

  private Rigidbody2D rb2d;
  private Vector2 movement;
  // Start is called before the first frame update
  void Start()
  {
    rb2d = GetComponent<Rigidbody2D>();
    rb2d.freezeRotation = true;

  }

  void OnCollisionEnter2D(Collision2D col)
  {
    if (col.gameObject.tag == "Floor")
    {
      canDash = true;
    }
  }

  // Update is called once per frame
  void Update()
  {

    float h = Input.GetAxisRaw("Horizontal");
    float v = Input.GetAxisRaw("Vertical");
    float dashKey = Input.GetAxisRaw("Fire3");

    Vector2 movement;

    if (dashKey != 0 && canDash)
    {
      movement = new Vector2(h, 0);
      movement = movement.normalized * dashspeed;
      canDash = false;
      isDashing = true;
      dashTimeLeft = dashDuration;
    }

    if (isDashing)
    {
      movement = new Vector2(h, 0).normalized * dashspeed;
      dashTimeLeft -= Time.deltaTime;

      if (dashTimeLeft <= 0)
      {
        isDashing = false;
      }

    }
    else
    {
      movement = new Vector2(h, v);
      movement = movement.normalized * mvspeed * Time.deltaTime;
    }

    rb2d.MovePosition(rb2d.position + movement);

  }
}
