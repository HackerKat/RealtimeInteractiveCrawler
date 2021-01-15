﻿using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using SFML.Window;
using SFML.System;
using System.IO;
using NetworkLib;
using System.Diagnostics;

namespace RealtimeInteractiveCrawler
{
    class AwesomeGame : GameLoop
    {
        private const uint DEFAULT_WIDTH = 1280;
        private const uint DEFAULT_HEIGHT = 720;
        private const string TITLE = "OBAMA CARES";
        private InputManager inputManager = new InputManager();
        private NetworkManager networkManager = new NetworkManager();
        private Player player;
        private bool isDataReadyToInit = false;
        private Dictionary<int, NetworkPlayer> players = new Dictionary<int, NetworkPlayer>();

        private float movementSpeed = 5f;
        private Sprite sprite; // player debug

        private World world;
        NpcSlime slime;

        List<NpcSlime> slimes = new List<NpcSlime>();


        private bool pPressed = false;

        private int connectionId;

        public AwesomeGame() : base(DEFAULT_WIDTH, DEFAULT_HEIGHT, TITLE, Color.Black)
        {
            DebugRender.Enabled = true;
        }

        public override void Initialize()
        {
            networkManager.Connect("localhost");
            world = new World();
            //world.GenerateWorld();

            // Create player
            //player = new Player(world);
            //player.Spawn(300, 150);
            // Create example enemy
            slime = new NpcSlime(world);
            slime.Spawn(500, 150);
            for (int i = 0; i < 1; i++)
            {
                var slime = new NpcSlime(world);        
                slime.Direction = MainClass.Rand.Next(0, 2) == 0 ? 1 : -1;
                slime.Spawn(MainClass.Rand.Next(150, 600), 150);
                slimes.Add(slime);
            }
        }

        public override void LoadContent()
        {
            Content.Load();

            sprite = new Sprite();
            sprite.Texture = Content.TexPlayer;
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
            player = new Player(world);
            player.Spawn(spawnX, spawnY);
            world.GenerateWorld(seed);
            isDataReadyToInit = true;
        }

        public void InitializeNewPlayer(Packet p)
        {
            PacketReader pr = new PacketReader(p);

            int connId = pr.GetInt();
            int spawnX = pr.GetInt();
            int spawnY = pr.GetInt();

            Console.WriteLine("new player data get processed");
            NetworkPlayer newPlayer = new NetworkPlayer(spawnX, spawnY);
            //newPlayer.Spawn(spawnX, spawnY);
            players.Add(connId, newPlayer);
        }

        public override void Update(GameTime gameTime)
        {
            if (isDataReadyToInit)
            {
                if (hasFocus)
                {
                    player.Update();
                }
            }
            
            slime.Update();

            foreach (var s in slimes)
                s.Update();

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
            if (isDataReadyToInit)
            {
                Window.Draw(world);
                Window.Draw(player);
                Window.Draw(slime);
                foreach (NetworkPlayer np in players.Values)
                {
                    Window.Draw(np);
                }
            }

            foreach (var s in slimes)
                Window.Draw(s);

            DebugRender.Draw(Window);
            //Window.Draw(sprite);
        }
    }
}
