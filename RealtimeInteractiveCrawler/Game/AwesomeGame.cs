﻿using SFML.Graphics;
using System;
using System.Collections.Generic;
using SFML.Window;
using NetworkLib;

namespace RealtimeInteractiveCrawler
{
    class AwesomeGame : GameLoop
    {
        public static World world;
        public static Random Rand;

        private const uint DEFAULT_WIDTH = 1280;
        private const uint DEFAULT_HEIGHT = 720;
        private const string TITLE = "OBAMA CARES";
        private InputManager inputManager = new InputManager();
        private NetworkManager networkManager = new NetworkManager();
        private Player player;
        private bool isDataReadyToInit = false;
        private Dictionary<int, Player> players = new Dictionary<int, Player>();

        private float movementSpeed = 5f;


        private bool pPressed = false;

        private int connectionId;

        public AwesomeGame() : base(DEFAULT_WIDTH, DEFAULT_HEIGHT, TITLE, Color.Black)
        {
            DebugRender.Enabled = true;


            Rand = new Random();

            //Player.Inventory = new UIInventory();
            //UIManager.AddControl(Player.Inventory);

        }

        public override void Initialize()
        {
            networkManager.Connect("localhost");
            world = new World();

            

            player = new Player(world);
            player.Spawn(650, 300);

            world.GenerateWorld(5);

            foreach (Enemy enemy in world.enemies)
            {
                behaviours.Add(new Wander(enemy, Rand));
            }


            //player = new Player();
            //player.Spawn(650, 300);

            //world.GenerateWorld(5);

        }

        public override void LoadContent()
        {
            Content.Load();
        }

        public void ProcessPacket(Packet p)
        {
            switch (p.PacketType)
            {
                case PacketType.PING:
                    Console.WriteLine("pong received from server");    //ping is received from server
                    break;
                case PacketType.INIT:
                    GetAcceptData(p);
                    break;
                case PacketType.NEW_PLAYER:
                    InitializeNewPlayer(p);
                    break;
                case PacketType.UPDATE_OTHER_POS:
                    GetOtherPlayerData(p);
                    break;
            }
        }

        public void GetAcceptData(Packet p)
        {
            PacketReader pr = new PacketReader(p);

            connectionId = pr.GetInt();
            int seed = pr.GetInt();
            int spawnX = pr.GetInt();
            int spawnY = pr.GetInt();

            Console.WriteLine("init data get processed");
            Console.WriteLine("my connection id is: " + connectionId);

            player = new Player();
            player.ClientPlayer = true;
            player.Spawn(spawnX, spawnY);
            players.Add(connectionId, player);
            world.GenerateWorld(seed);
            foreach(Enemy enemy in world.enemies)
            {
                behaviours.Add(new Wander(enemy, Rand));
            }
            isDataReadyToInit = true;
        }

        public void InitializeNewPlayer(Packet p)
        {
            PacketReader pr = new PacketReader(p);

            int connId = pr.GetInt();
            int spawnX = pr.GetInt();
            int spawnY = pr.GetInt();

            Console.WriteLine("new player data get processed");
            Player newPlayer = new Player();
            newPlayer.Spawn(spawnX, spawnY);
            players.Add(connId, newPlayer);
            Console.WriteLine("new player joined: " + connId);
        }

        public void SendPlayerUpdate()
        {
            PacketBuilder pb = new PacketBuilder(PacketType.UPDATE_MY_POS);

            pb.Add(connectionId);
            pb.Add(player.Position.X);
            pb.Add(player.Position.Y);
            Packet packet = pb.Build();
            networkManager.SendData(packet);
        }

        public void GetOtherPlayerData(Packet p)
        {
            Console.WriteLine("Client got data about other players position");
            PacketReader pr = new PacketReader(p);

            int id = pr.GetInt();
            int x = pr.GetInt();
            int y = pr.GetInt();
            Console.WriteLine("Get data from player: " + id);
            if (players.ContainsKey(id))
            {
                Player np = players[id];
                Console.WriteLine("network player has moved to x: " + np.Position.X);
                Console.WriteLine("network player has moved to y: " + np.Position.Y);
                np.UpdatePos(x, y);
            }
        }

        public override void Update(GameTime gameTime)
        {
            //if (isDataReadyToInit)
            //{

                player.Update();
                // SendPlayerUpdate();
                for(int i = 0; i < behaviours.Count; i++)
                {

                    SteeringBehaviour wander = behaviours[i];
                    var steering = wander.GetSteering();
                    Enemy enemy = world.enemies[i];
                    enemy.UpdatePos(steering, gameTime);
                }

            }
        }

        public void UpdateEnemyHealth(Packet p)
        {
            //Console.WriteLine("Client got data about enemy position");
            PacketReader pr = new PacketReader(p);
            int id = pr.GetInt();
            int health = pr.GetInt();
            foreach (Enemy enemy in Enemies.Values)
            {
                if (enemy.id == id)
                {
                    enemy.Health = health;
                    return;
                }
            }
        }

        public void UpdatePlayerHealth(Packet p)
        {

            PacketReader pr = new PacketReader(p);
            int id = pr.GetInt();
            int health = pr.GetInt();
            int damage = Players[id].Health - health;
            Debug.WriteLine(damage + " " + health);
            //Players[id].Health = health;
            Players[id].ChangeHealth(-damage, true);

            return;

                    world.Update();
                    player.Update();
                    SendPlayerUpdate();
                }
            }
            //UIManager.UpdateOver();
            //UIManager.Update();
            // TODO revert debug change


            MessageQueue messageQueue = networkManager.MessageQueue;
            Packet p;
            while ((p = messageQueue.Pop()) != null)
            {
                ProcessPacket(p);
            }

            if (inputManager.getKeyDown(Keyboard.Key.P) && !pPressed)
            {
                pPressed = true;
                PacketBuilder pb = new PacketBuilder(0);
                pb.Add(5);
                networkManager.SendData(pb.Build());  //ping is sent
                Console.WriteLine("Ping is sent");
            }
            if (!inputManager.getKeyDown(Keyboard.Key.P))
            {
                pPressed = false;
            }
            if (inputManager.getKeyDown(Keyboard.Key.Escape))
            {
                Window.Close();
            }
        }

        public override void Draw(GameTime gameTime)
        {

            //DebugUtility.DrawPerformanceData(this, Color.White);
            //if (isDataReadyToInit)
            //{
                Window.Draw(world);
                Window.Draw(player);

                //foreach (NetworkPlayer np in players.Values)
                //{
                    //Window.Draw(np);
                //}
            //}


            // TODO remove debug
            Window.Draw(world);
            Window.Draw(player);
            foreach(Enemy enemy in world.enemies)
            {
                Window.Draw(enemy);
            }

            DebugRender.Draw(Window);
            //Window.Draw(sprite);

            //DebugUtility.DrawPerformanceData(Color.White);

            // TODO remove debug
            //Window.Draw(world);
            //Window.Draw(player);

            DebugRender.Draw(Window);
            //UIManager.Draw();

            if (isDataReadyToInit)
            {
                Window.Draw(world);
                foreach (KeyValuePair<int, Player> np in Players)
                {

                    Player np = v.Value;
                    int id = v.Key;
                    //Console.WriteLine("Player: " + id + " has health " + np.Health);
                    if (np.Health <= 0)
                        np.AnimSprite = new AnimSprite(Content.SpriteDead);
                    Window.Draw(np);
                }
                foreach (Enemy enemy in Enemies.Values)
                {
                    if (enemy.Health > 0)
                    {
                        double distanceToPlayer = Distance(enemy.Position, Player.Position);
                        if (distanceToPlayer > Tile.TILE_SIZE * Chunk.CHUNK_SIZE * 0.5f)
                            continue;
                        Window.Draw(enemy);
                    }
                        
                }
                foreach (var bar in StatusBars.Values)
                {
                    Window.Draw(bar);
                }
            }

        }
    }
}
