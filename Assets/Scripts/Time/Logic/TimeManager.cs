using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private int gameSecond, gameMinute, gameHour, gameDay, gameMonth, gameYear;
    
    private Season gameSeason;
    
    private int monthInSeason;

    public bool gameClockPause;

    private float tikTime;

    private void Awake()
    {
        NewGameTime();
    }

    private void Start()
    {
        //方法在OnEnable中注册，Awake优先OnEnable执行，所以呼叫事件要在Start而非Awake中以避免方法还未注册
        EventHandler.CallGameMinuteEvent(gameMinute, gameHour);
        EventHandler.CallGameHourEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
    }

    private void Update()
    {
        if (!gameClockPause)
        {
            tikTime += Time.deltaTime;

            if (tikTime >= Settings.secondThreshold)
            {
                tikTime -= Settings.secondThreshold;
                UpdateGameTime();
            }

            //按住T键加速时间（直接跳过一分钟）
            if (Input.GetKey(KeyCode.T))
            {
                for (int i = 0; i < Settings.secondHold + 1; i++)
                {
                    UpdateGameTime();
                }
            }
        }
    }

    private void NewGameTime()
    {
        gameSecond = 0;
        gameMinute = 0;
        gameHour = 6;
        gameDay = 1;
        gameMonth = 1;
        gameYear = 1;
        gameSeason = Season.春天;
        monthInSeason = Settings.monthInSeason;
    }

    private void UpdateGameTime()
    {
        gameSecond++;

        if (gameSecond > Settings.secondHold)
        {
            gameMinute++;
            gameSecond = 0;

            if (gameMinute > Settings.minuteHold)
            {
                gameHour++;
                gameMinute = 0;

                if (gameHour > Settings.hourHold)
                {
                    gameDay++;
                    gameHour = 0;

                    if (gameDay > Settings.dayHold)
                    {
                        gameMonth++;
                        monthInSeason--;
                        gameDay = 1;
                        
                        if (monthInSeason == 0)
                        {
                            monthInSeason = Settings.monthInSeason;

                            int seasonNumber = (int)gameSeason;
                            seasonNumber++;

                            if (seasonNumber > Settings.seasonHold)
                            {
                                gameYear++;
                                seasonNumber = 0;
                                gameMonth = 1;
                            }   //年更新

                            gameSeason = (Season)seasonNumber;
                        }   //季节更新
                    }   //月更新
                }   //天更新
                EventHandler.CallGameHourEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
            }   //小时更新
            EventHandler.CallGameMinuteEvent(gameMinute, gameHour);
        }   //分钟更新

        //Debug.Log("Second: " + gameSecond + " Minute: " + gameMinute + " Hour: " + gameHour + " Day: " + gameDay + " Month: " + gameMonth + " Season: " + gameSeason + " Year: " + gameYear);
    }
}
