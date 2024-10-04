using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  private float mvspeed = 0.2f;
  private Rigidbody2D rb2d;
  private Vector2 movement;
  [SerializeField]
  private Sprite _normalForm;
  [SerializeField]
  private Sprite _demonForm;
  // Start is called before the first frame update
  void Start()
  {
    rb2d = GetComponent<Rigidbody2D>();
    rb2d.freezeRotation = true;

  }

  // Update is called once per frame
  void Update()
  {

    float h = Input.GetAxisRaw("Horizontal");

    Vector2 movement = new Vector2(h, 0);
    movement = movement.normalized * mvspeed;

    rb2d.MovePosition(rb2d.position + movement);
  }

}
