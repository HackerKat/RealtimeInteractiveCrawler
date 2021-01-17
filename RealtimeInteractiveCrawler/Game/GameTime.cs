using System.Diagnostics;

namespace RealtimeInteractiveCrawler
{
    public class GameTime
    {
        public float TimeScale { get; set; } = 1f;

        public float deltaTime;
        public float DeltaTime
        {
            get { return deltaTime * TimeScale; }
            set { deltaTime = value; }
        }

        public float TotealTimeElapsed
        {
            get;
            private set;
        }

        public void Update(float deltaTime, float totalTimeElapsed)
        {
            this.deltaTime = deltaTime;
            TotealTimeElapsed = totalTimeElapsed;
        }
    }
}