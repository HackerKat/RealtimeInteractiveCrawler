using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;

namespace RealtimeInteractiveCrawler
{
    class GameTime
    {
        public float TimeScale { get; set; } = 1f;
        private Clock clock = new Clock();
        private float currentTime = 0f;
        private float previousTime = 0f;
        private float deltaTime = 0f;
     
        public float DeltaTime
        {
            get { return deltaTime * TimeScale; }
            set { deltaTime = value; }
        }

        public float TotalTime
        {
            get;
            private set;
        }

        public void Update()
        {
            currentTime = clock.ElapsedTime.AsSeconds();
            deltaTime = currentTime - previousTime;
            previousTime = currentTime;

            TotalTime += deltaTime;
        }
    }
}
