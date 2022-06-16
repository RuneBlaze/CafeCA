using Cafeo.Utils;
using UnityEngine.Events;

namespace Cafeo.World
{
    public class GameClock : Singleton<GameClock>
    {
        public static readonly string[] seasonNames = { "春", "夏", "秋", "冬" };
        public static readonly string[] seasonModifier = { "早", "盛", "晚" };
        public static readonly string[] weekdayNames = { "一", "二", "三", "四", "五", "六", "日" };

        public enum TimePhase
        {
            EarlyMorning,
            Morning,
            Noon,
            Afternoon,
            Evening,
            Night,
            Midnight,
        }

        public string HumanizeTimePhase(TimePhase phase)
        {
            switch (phase)
            {
                case TimePhase.EarlyMorning:
                    return "凌晨";
                case TimePhase.Morning:
                    return "上午";
                case TimePhase.Noon:
                    return "中午";
                case TimePhase.Afternoon:
                    return "下午";
                case TimePhase.Evening:
                    return "傍晚";
                case TimePhase.Night:
                    return "晚上";
                case TimePhase.Midnight:
                    return "半夜";
                default:
                    return "";
            }
        }
        
        public int elapsed; // how much time has elapsed since the start of the game

        public UnityEvent onTurn;
        public int DayCnt => elapsed / 1440;
        public string WeekDayCnt => weekdayNames[DayCnt % 7];
        public int HourCnt => elapsed % 1440 / 60;
        public int MinuteCnt => elapsed % 1440 % 60;
        public int MonthCnt => DayCnt / 30 % 12;
        public int YearCnt => 87 + DayCnt / (21 * 21);

        public string TimePhaseCnt
        {
            get
            {
                int phase = elapsed % 1440 / 60;
                if (phase < 6)
                    return "凌晨";
                else if (phase < 12)
                    return "上午";
                else if (phase < 13)
                    return "中午";
                else if (phase < 18)
                    return "下午";
                else if (phase < 21)
                    return "傍晚";
                else if (phase < 24)
                    return "晚上";
                else
                    return "半夜";
            }
        }

        public string SeasonName => $"{seasonModifier[MonthCnt % 3]}{seasonNames[MonthCnt / 4]}";

        public void Elapse(int minutes)
        {
            elapsed += minutes;
        }

        // each year contains 12 months, but each month contains 21 days precisely
        public static int Hours(int k)
        {
            return k * 60;
        }

        public static int Minutes(int k)
        {
            return k;
        }

        public static int Days(int k)
        {
            return k * 60 * 24;
        }

        public static int Months(int k)
        {
            return 21 * Days(k);
        }

        public static int Years(int k)
        {
            return 12 * Months(k);
        }

        protected override void Setup()
        {
            base.Setup();
            onTurn = new UnityEvent();
        }

        public void ElapseTurn()
        {
            onTurn.Invoke();
            Elapse(5);
        }
    }
}