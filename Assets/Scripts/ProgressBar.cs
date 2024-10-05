using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
  public Image bar;
  private int maxDuration = 10;
  private float currentDuration = 0;


  void Start()
  {
    Debug.Log(bar);
  }

  void Update()
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
