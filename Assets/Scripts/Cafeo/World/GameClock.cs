using Cafeo.Utils;
using UnityEngine.Events;

namespace Cafeo.World
{
    public class GameClock : Singleton<GameClock>
    {
        public int elapsed = 0; // how much time has elapsed since the start of the game
        public void Elapse(int minutes)
        {
            elapsed += minutes;
        }
        public int DayCnt => elapsed / 1440;
        public int HourCnt => (elapsed % 1440) / 60;
        public int MinuteCnt => (elapsed % 1440) % 60;
        public int MonthCnt => DayCnt / 30 % 12;
        public int YearCnt => DayCnt / (21 * 21);
        
        // each year contains 12 months, but each month contains 21 days precisely
        public static int Hours(int k) => k * 60;
        public static int Minutes(int k) => k;
        public static int Days(int k) => k * 60 * 24;
        public static int Months(int k) => 21 * Days(k);
        public static int Years(int k) => 12 * Months(k);
        
        public static readonly string[] seasonNames = {"春", "夏", "秋", "冬"};
        public static readonly string[] seasonModifier = {"早", "盛", "晚"};
        
        public string SeasonName => $"{seasonModifier[MonthCnt % 3]} {seasonNames[MonthCnt / 4]}";

        public UnityEvent onTurn;

        protected override void Setup()
        {
            base.Setup();
            onTurn = new UnityEvent();
        }

        public void ElapseTurn()
        {
            onTurn.Invoke();
        }
    }
}