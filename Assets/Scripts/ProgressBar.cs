using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
  public Image bar;

  private int maxDuration = 60;
  private float currentDuration = 0;

  private Player player;


  void Start()
  {
    player = GameObject.Find("Player").GetComponent<Player>();
  }

  void Update()
  {

    if (!player.getPlayerAnimator().GetBool("isitnormalform"))
    {
      if (currentDuration >= maxDuration)
      {
        currentDuration = maxDuration;
      }
      else
      {
        currentDuration += Time.deltaTime;
        bar.rectTransform.localScale = new Vector2((float)currentDuration / maxDuration, 0.5f);
      }
    }

  }
}
