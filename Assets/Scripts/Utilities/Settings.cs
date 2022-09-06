using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{
    //渐隐效果
    public const float fadeDuration = 0.35f;
    public const float targetAlpha = 0.45f;

    //时间: 数值越小时间越快
    public const float secondThreshold = 0.012f;
    public const int secondHold = 59;
    public const int minuteHold = 59;
    public const int hourHold = 23;
    public const int dayHold = 30;
    public const int seasonHold = 3;
    public const int monthInSeason = 3;
}
