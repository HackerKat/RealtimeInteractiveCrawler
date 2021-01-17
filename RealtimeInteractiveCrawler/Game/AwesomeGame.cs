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
        private bool isDataReadyToInit = false;
        private Dictionary<int, Player> players = new Dictionary<int, Player>();
        private Dictionary<int, Enemy> enemies = new Dictionary<int, Enemy>();

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
                case PacketType.UPDATE_ENEMY:
                    if (isDataReadyToInit)
                    {
                        UpdateEnemyData(p);
                    }
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
            int enemyCount = pr.GetInt();
            world.GenerateWorld(seed);
            for (int i = 0; i < enemyCount; i++)
            {
                int id = pr.GetInt();
                float x = pr.GetFloat();
                float y = pr.GetFloat();

                Enemy enemy = new Enemy();
                enemies.Add(id, enemy);
                enemy.Spawn(x, y);
            }
           
            Console.WriteLine("my connection id is: " + connectionId);

            player = new Player();
            player.ClientPlayer = true;
            player.Spawn(spawnX, spawnY);
            players.Add(connectionId, player);
            isDataReadyToInit = true;
        }

        public void InitializeNewPlayer(Packet p)
        {
            PacketReader pr = new PacketReader(p);

            int connId = pr.GetInt();
            int spawnX = pr.GetInt();
            int spawnY = pr.GetInt();

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
            PacketReader pr = new PacketReader(p);

            int id = pr.GetInt();
            int x = pr.GetInt();
            int y = pr.GetInt();
            if (players.ContainsKey(id))
            {
                Player np = players[id];
                np.UpdatePos(x, y);
            }
        }

        public void UpdateEnemyData(Packet p)
        {
            //Console.WriteLine("Client got data about enemy position");
            PacketReader pr = new PacketReader(p);
            int count = pr.GetInt();
            for (int i = 0; i < count; i++)
            {
                int id = pr.GetInt();
                int x = (int)pr.GetFloat();
                int y = (int)pr.GetFloat();
                enemies[id].UpdatePos(x, y);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (isDataReadyToInit)
            {
                if (hasFocus)
                {
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
            // TODO remove debug
          
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
                foreach (Player np in players.Values)
                {
                    Window.Draw(np);
                }
                foreach (Enemy enemy in enemies.Values)
                {
                    Window.Draw(enemy);
                }
            }

        }
    }
}
