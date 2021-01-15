using SFML.Graphics;
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

        private float movementSpeed = 5f;
        private Sprite sprite; // player debug


        private NpcSlime slime;

        private List<NpcSlime> slimes = new List<NpcSlime>();

        private bool pPressed = false;

        public AwesomeGame() : base(DEFAULT_WIDTH, DEFAULT_HEIGHT, TITLE, Color.Black)
        {
            DebugRender.Enabled = true;

            Rand = new Random();
        }

        public override void Draw(GameTime gameTime)
        {
            DebugUtility.DrawPerformanceData(Color.White);

            Window.Draw(world);
            Window.Draw(player);
            //Window.Draw(slime);

            foreach (var s in slimes)
                Window.Draw(s);

            DebugRender.Draw(Window);
            //Window.Draw(sprite);
        }

        public override void Initialize()
        {
            //networkManager.Connect("localhost");
            world = new World(6);
            world.GenerateWorld();

            // Create player
            player = new Player();
            player.Spawn(650, 300);
            // Create example enemy
            //slime = new NpcSlime(world);
            //slime.Spawn(500, 150);
            for (int i = 0; i < 1; i++)
            {
                return;
                var slime = new NpcSlime();        
                slime.Direction = Rand.Next(0, 2) == 0 ? 1 : -1;
                slime.Spawn(Rand.Next(150, 600), 150);
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
            player.Update();
            slime.Update();

            foreach (var s in slimes)
                s.Update();

            switch (p.Id)
            {
                case 0:
                    Console.WriteLine("pong received from server");    //ping is received from server
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            player.Update();
            //slime.Update();

            foreach (var s in slimes)
                s.Update();

            //MessageQueue messageQueue = networkManager.MessageQueue;
            //Packet p;
            //while ((p = messageQueue.Pop()) != null)
            //{
            //    ProcessPacket(p);
            //}

            //if (inputManager.getKeyDown(Keyboard.Key.P) && !pPressed)
            //{
            //    pPressed = true;
            //    PacketBuilder pb = new PacketBuilder(0);
            //    pb.Add(5);
            //    networkManager.SendData(pb.Build());  //ping is sent
            //    Console.WriteLine("Ping is sent");
            //}
            if (!inputManager.getKeyDown(Keyboard.Key.P))
            {
                pPressed = false;
            }
            if (inputManager.getKeyDown(Keyboard.Key.Escape))
            {
                Window.Close();
            }
        }
    }
}
