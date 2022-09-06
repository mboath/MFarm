using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TimeUI : MonoBehaviour
{
    public RectTransform dayNightImage;
    public RectTransform clockParent;
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI timeText;
    public Image seasonImage;

    public Sprite[] seasonSprites;

    private List<GameObject> clockBlocks = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < clockParent.childCount; i++)
        {
            clockBlocks.Add(clockParent.GetChild(i).gameObject);
            clockBlocks[i].SetActive(false);
        }
    }

    private void OnEnable()
    {
        EventHandler.GameMinuteEvent += OnGameMinuteEvent;
        EventHandler.GameHourEvent += OnGameHourEvent;
    }

    private void OnDisable()
    {
        EventHandler.GameMinuteEvent -= OnGameMinuteEvent;
        EventHandler.GameHourEvent -= OnGameHourEvent;
    }

    private void OnGameMinuteEvent(int minute, int hour)
    {
        timeText.text = hour.ToString("00") + ":" + minute.ToString("00");
    }

    private void OnGameHourEvent(int hour, int day, int month, int year, Season season)
    {
        dateText.text = "第" + year + "年 " + month + "月" + day + "日";

        seasonImage.sprite = seasonSprites[(int)season];

        SwitchHourImage(hour);

        DayNightImageRotate(hour);
    }

    /// <summary>
    /// 根据小时切换时间块显示
    /// </summary>
    /// <param name="hour">小时</param>
    private void SwitchHourImage(int hour)
    {
        int index = hour / 4;

        for (int i = 0; i < clockBlocks.Count; i++)
        {
            if (i <= index)
                clockBlocks[i].SetActive(true);
            else
                clockBlocks[i].SetActive(false);
        }
    }

    /// <summary>
    /// 根据时间旋转昼夜图标
    /// </summary>
    /// <param name="hour">小时</param>
    private void DayNightImageRotate(int hour)
    {
        var target = new Vector3(0, 0, hour * 15 - 90);
        dayNightImage.DORotate(target, 1f, RotateMode.Fast);
    }
}