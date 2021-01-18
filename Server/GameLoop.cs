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
        private NetworkManager netMan;

        public static GameTime GameTime
        {
            get;
            protected set;
        }

        public GameLoop(int seed, NetworkManager networkManager)
        {
            this.netMan = networkManager;

            GameTime = new GameTime();

            stopwatch = new Stopwatch();
            stopwatch.Start();
           
            Server.world.GenerateWorld(seed);
        }

        public void Run()
        {
            Init();
            //Console.WriteLine("Game loop started");
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

        public static async Task<bool> Wait(int milliSeconds)
        {
            await Task.Delay(milliSeconds);
            return true;
        }

        public void Init()
        {
            foreach(Entity entity in Server.world.enemies)
            {
                entity.Behaviour = new Wander(entity, Server.rand);
            }
        }

        public void Update()
        {
            foreach (Entity enemy in Server.world.enemies)
            {
                if(enemy.Health <= 0)
                {
                    continue;
                }
                foreach(Player player in netMan.Players.Values)
                {
                    Console.WriteLine("playerid " + player.ConnId + " health " + player.Health);
                    if (enemy.CheckIfSeekPlayer(player) && player.Health > 0)
                    {
                        enemy.Behaviour = new Seek(enemy, player);
                        enemy.Attack(player);
                        netMan.UpdatePlayerHealth(player.ConnId);
                        break;
                    }
                    else
                    {
                        enemy.Behaviour = new Wander(enemy, Server.rand);
                    }
                }
                enemy.Update(GameTime);
            }
            netMan.SendEnemyUpdate();
        }
    }
}
