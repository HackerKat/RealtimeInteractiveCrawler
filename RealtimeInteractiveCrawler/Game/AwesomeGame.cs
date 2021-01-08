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
                //case 1:
                //processing packets
                //break;
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


            if (inputManager.getKeyDown(Keyboard.Key.Escape))
            {
                Window.Close();
            }
        }
    }
}
