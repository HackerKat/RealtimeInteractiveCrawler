using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeInteractiveCrawler
{
    public class Seek : SteeringBehaviour
    {
        public Enemy enemy;
        public Player target;

        public Seek(Enemy enemy, Player player)
        {
            this.enemy = enemy;
            this.target = player;
        }

        public SteeringOutput GetSteering()
        {
            SteeringOutput steeringOutput = new SteeringOutput();
            steeringOutput.velocity = target.Position - enemy.Position;
            steeringOutput.velocity = Enemy.normalize(steeringOutput.velocity);  //temp code, goes to server anyways and not using SFML
            steeringOutput.velocity *= Enemy.MAX_SPEED;

            enemy.orientation = enemy.getNewOrientation(enemy.orientation, steeringOutput.velocity);

            steeringOutput.rotation = 0;
            return steeringOutput;
        }
    }
}
