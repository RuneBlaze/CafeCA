using System;
using Cafeo.World;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Cafeo.UI
{
    public class TimePanel : MonoBehaviour
    {
        [SerializeField] private Text dateLabel;
        [SerializeField] private Text timeLabel;
        
        private const string DateTemplate = "{0:D3} 年 {1:D2} 月 {2:D2} 日 {3}";
        private const string TimeTemplate = "{0:D2}:{1:D2} ({2}) {3}";
        private void Awake()
        {
            Assert.IsNotNull(dateLabel);
            Assert.IsNotNull(timeLabel);
        }

        public void Start()
        {
            
        }
        
        public GameClock Clock => GameClock.Instance;

        private void Update()
        {
            dateLabel.text = string.Format(DateTemplate, Clock.YearCnt, 1 + Clock.MonthCnt, 1 + Clock.DayCnt, Clock.SeasonName);
            timeLabel.text = string.Format(TimeTemplate, Clock.HourCnt, Clock.MinuteCnt, Clock.WeekDayCnt, Clock.TimePhaseCnt);
        }
    }
}