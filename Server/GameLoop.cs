using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Server
{
    public class GameLoop
    {
        public const int FPS = 30;
        public const float TIME_TILL_UPDATE = 1f / FPS;
        public Stopwatch stopwatch;
        public World world;
        private Random rand;
        private NetworkManager netMan;

        public static GameTime GameTime
        {
            get;
            protected set;
        }

        public GameLoop(Random rand, int seed, World world, NetworkManager networkManager)
        {
            this.rand = rand;
            this.world = world;
            this.netMan = networkManager;

            GameTime = new GameTime();

            stopwatch = new Stopwatch();
            stopwatch.Start();
           
            world.GenerateWorld(seed);
        }

        public void Run()
        {
            Init();
            Console.WriteLine("Game loop started");
            float totalTimeBeforeUpdate = 0f;
            float previousTimeElapsed = 0f;
            float deltaTime = 0f;
            float totalTimeElapsed = 0f;
            while (true)
            {
                totalTimeElapsed = stopwatch.ElapsedMilliseconds / 1000f;
                deltaTime = totalTimeElapsed - previousTimeElapsed;
                previousTimeElapsed = totalTimeElapsed;
                totalTimeBeforeUpdate += deltaTime;

                if (totalTimeBeforeUpdate >= TIME_TILL_UPDATE)
                {
                    GameTime.Update(totalTimeBeforeUpdate, totalTimeElapsed);
                    totalTimeBeforeUpdate = 0f;
                    Update();
                }
            }
        }

        public void Init()
        {
            foreach(Entity entity in world.enemies)
            {
                entity.Behaviour = new Wander(entity, rand);
            }
        }

        public void Update()
        {
            foreach (Entity enemy in world.enemies)
            {
                foreach(Player player in netMan.Players.Values)
                {
                    if (enemy.CheckIfSeekPlayer(player))
                    {
                        enemy.Behaviour = new Seek(enemy, player);
                        break;
                    }
                    else
                    {
                        enemy.Behaviour = new Wander(enemy, rand);
                    }
                }
                enemy.Update(GameTime);
            }
            netMan.SendEnemyUpdate();
        }
    }
}
