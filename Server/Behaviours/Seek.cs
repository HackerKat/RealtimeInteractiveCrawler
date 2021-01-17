﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Server
{
    public class Seek : SteeringBehaviour
    {
        public Entity entity;
        public Player target;

        public Seek(Entity entity, Player player)
        {
            this.entity = entity;
            this.target = player;
        }

        public SteeringOutput GetSteering()
        {
            SteeringOutput steeringOutput = new SteeringOutput();
            steeringOutput.velocity = target.Position - entity.Position;
            steeringOutput.velocity = Vector2.Normalize(steeringOutput.velocity); 
            steeringOutput.velocity *= Entity.MAX_SPEED;
            

            entity.Orientation = entity.getNewOrientation(entity.Orientation, steeringOutput.velocity);

            steeringOutput.rotation = 0;
            return steeringOutput;
        }
    }
}
