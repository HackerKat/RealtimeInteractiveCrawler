using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeInteractiveCrawler
{
    public class Wander : SteeringBehaviour
    {
        private Enemy enemy;
        private Random rand;
        
        public Wander(Enemy enemy, Random rand)
        {
            this.enemy = enemy;
            this.rand = rand;
        }

        public SteeringOutput GetSteering()
        {
            SteeringOutput steering = new SteeringOutput();
            steering.velocity = Enemy.MAX_SPEED * enemy.OrientVector;

            steering.rotation = randomBinomial() * Enemy.MAX_ROTATION;
            return steering;
        }

        public float randomBinomial()
        {
            float val = (float)rand.NextDouble();
            return (val * 2) - 1;
        }
    }
}
