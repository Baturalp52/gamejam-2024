using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
  public Image bar;

  private int maxDuration = 60;
  private float currentDuration = 0;
  private float demonDuration = 0;

  private bool prevPlayerIsDead;
  private bool prevIsMapChanged;

  private Player player;


  void Start()
  {
    player = GameObject.Find("Player").GetComponent<Player>();
    prevPlayerIsDead = player.getIsDead();
    prevIsMapChanged = player.getIsMapChanged();
  }


  private void DrawBar()
  {
    bar.rectTransform.localScale = new Vector2((float)currentDuration / maxDuration, bar.rectTransform.localScale.y);
  }

  void Update()
  {

    if (prevIsMapChanged != player.getIsMapChanged())
    {
      prevIsMapChanged = player.getIsMapChanged();
      demonDuration = 0;
    }

    if (!player.getPlayerAnimator().GetBool("isitnormalform"))
    {
      if (currentDuration >= maxDuration)
      {
        player.restartFromStart();
        currentDuration = 0;
      }
      else
      {
        currentDuration += Time.deltaTime;
        demonDuration += Time.deltaTime;
        DrawBar();
      }
    }

    if (prevPlayerIsDead != player.getIsDead())
    {
      prevPlayerIsDead = player.getIsDead();
      currentDuration -= demonDuration;
      demonDuration = 0;
      DrawBar();
    }


  }
}
