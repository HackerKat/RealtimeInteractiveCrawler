using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Wander : SteeringBehaviour
    {
        private Entity entity;
        private Random rand;
        
        public Wander(Entity entity, Random rand)
        {
            this.entity = entity;
            this.rand = rand;
        }

        public SteeringOutput GetSteering()
        {
            SteeringOutput steering = new SteeringOutput();
            steering.velocity = Entity.MAX_SPEED * entity.OrientVector;

            steering.rotation = randomBinomial() * Entity.MAX_ROTATION;
            return steering;
        }

        public float randomBinomial()
        {
            float val = (float)rand.NextDouble();
            return (val * 2) - 1;
        }
    }
}
