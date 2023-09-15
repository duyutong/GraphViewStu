using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using static UnityEngine.PlayerLoop.EarlyUpdate;

public class ClockoutCountdown : EditorWindow
{
    private VisualTreeAsset visualTreeAsset = default;
    private Label timeCount;
    private string saveKey = "TodayStartTime";
    private long todayEndTime;
    private string showStr = "{0}:{1}:{2}";
    private CountdownData showCountDown;
    private float refreshTimeCount = 1;
    [MenuItem("Tools/ClockoutCountdown")]
    public static void ShowExample()
    {
        ClockoutCountdown wnd = GetWindow<ClockoutCountdown>();
        wnd.titleContent = new GUIContent("ClockoutCountdown");
        wnd.minSize = new Vector2(750, 330);
        wnd.maxSize = new Vector2(750, 330);
    }
    public void SetTodayStartTime()
    {
        DateTime nowTime = DateTime.Now;
        DateTime currentDate = DateTime.Now.Date;

        DateTime time1 = currentDate.AddHours(8).AddMinutes(30);
        DateTime time2 = currentDate.AddHours(9);
        DateTime time3 = currentDate.AddHours(9).AddMinutes(30);
        DateTime targetTime = time1;

        long saveTimestamp = (long)PlayerPrefs.GetFloat(saveKey);
        DateTime saveDateTime = ConvertTimestampToDateTime(saveTimestamp);

        if (saveDateTime.DayOfYear < nowTime.DayOfYear || saveDateTime.Year < nowTime.Year)
        {
            if (nowTime > time1 && nowTime <= time2) targetTime = time2;
            if (nowTime > time2) targetTime = time3;

            saveTimestamp = GetTimestamp(targetTime);
            PlayerPrefs.SetFloat(saveKey, saveTimestamp);
            PlayerPrefs.Save();
        }
        else if (saveDateTime.DayOfYear == nowTime.DayOfYear) 
        {
            if (saveDateTime > time1 && saveDateTime <= time2) targetTime = time2;
            if (saveDateTime > time2) targetTime = time3;
        }
        todayEndTime = GetTimestamp(targetTime.AddHours(9).AddMinutes(30));
    }
    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        visualTreeAsset = Resources.Load<VisualTreeAsset>("UIBuilder/ClockoutCountdown/ClockoutCountdown");
        visualTreeAsset.CloneTree(root);

        timeCount = root.Q<Label>("timeCount");
        SetTodayStartTime();
    }
    private void Update()
    {
        refreshTimeCount -= Time.deltaTime;
        if (refreshTimeCount <= 0)
        {
            refreshTimeCount = 1;
            showCountDown = CalculateCountdown(todayEndTime);
            timeCount.text = string.Format(showStr
                , FormatNumber(showCountDown.Hours)
                , FormatNumber(showCountDown.Minutes)
                , FormatNumber(showCountDown.Second));
        }
    }
    private DateTime ConvertTimestampToDateTime(long timestampSeconds)
    {
        TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timestampSeconds);
        DateTime beijingTime = TimeZoneInfo.ConvertTime(dateTimeOffset, cstZone).DateTime;
        return beijingTime;
    }
    private long GetTimestamp(DateTime dateTime)
    {
        return new DateTimeOffset(dateTime).ToUnixTimeSeconds();
    }
    private string FormatNumber(int input)
    {
        if (input < 10)
        {
            return "0" + input.ToString();
        }
        else
        {
            return input.ToString();
        }
    }
    private CountdownData CalculateCountdown(long timestamp)
    {
        DateTime currentTime = DateTime.Now;
        DateTime targetTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).LocalDateTime;

        if (targetTime <= currentTime) targetTime = currentTime;

        TimeSpan timeLeft = targetTime - currentTime;

        CountdownData countdownData = new CountdownData
        {
            Days = timeLeft.Days,
            Hours = timeLeft.Hours % 24,
            Minutes = timeLeft.Minutes % 60,
            Second = timeLeft.Seconds % 60,
        };

        return countdownData;
    }
}
public struct CountdownData
{
    public int Days;
    public int Hours;
    public int Minutes;
    public int Second;
}