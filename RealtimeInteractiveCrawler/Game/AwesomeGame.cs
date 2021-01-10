using SFML.Graphics;
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

        private float movementSpeed = 5f;
        private Sprite sprite;

        private World world;


        private bool pPressed = false;

        public AwesomeGame() : base(DEFAULT_WIDTH, DEFAULT_HEIGHT, TITLE, Color.Black)
        {
            
        }

        public override void Draw(GameTime gameTime)
        {
            DebugUtility.DrawPerformanceData(this, Color.White);

            Window.Draw(world);
            Window.Draw(sprite);
        }

        public override void Initialize()
        {
            player = new Player(sprite);
            networkManager.Connect("localhost");
            world = new World();
        }

        public override void LoadContent()
        {
            Content.Load();

            sprite = new Sprite();
            sprite.Texture = Content.TexPlay0;
        }

        public void ProcessPacket(Packet p)
        {
            switch (p.Id)
            {
                case 0:
                    Console.WriteLine("pong received from server");    //ping is received from server
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            MessageQueue messageQueue = networkManager.MessageQueue;
            Packet p;
            while ((p = messageQueue.Pop()) != null)
            {
                ProcessPacket(p);
            }

            player.Update(inputManager, gameTime);

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
    }
}
