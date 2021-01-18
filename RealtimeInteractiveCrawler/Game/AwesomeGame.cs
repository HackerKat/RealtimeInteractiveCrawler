using SFML.Graphics;
using System;
using System.Collections.Generic;
using SFML.Window;
using NetworkLib;
using SFML.System;
using System.Diagnostics;

namespace RealtimeInteractiveCrawler
{
    class AwesomeGame : GameLoop
    {
        public static World world;
        public static Random Rand;

        public static List<Tile> PlayerTiles = new List<Tile>();
        public static Dictionary<int, Player> Players { get; set; } = new Dictionary<int, Player>();
        public static Player Player;
        public static Dictionary<int, Enemy> Enemies = new Dictionary<int, Enemy>();

        private const uint DEFAULT_WIDTH = 1280;
        private const uint DEFAULT_HEIGHT = 720;
        private const string TITLE = "Realtime Interactive Crawler";
        private InputManager inputManager = new InputManager();
        public static NetworkManager networkManager = new NetworkManager();

        private bool isDataReadyToInit = false;


        private bool pPressed = false;

        private int connectionId;

        public AwesomeGame() : base(DEFAULT_WIDTH, DEFAULT_HEIGHT, TITLE, Color.Black)
        {
            DebugRender.Enabled = true;


            //Rand = new Random();

            //Player.Inventory = new UIInventory();
            //UIManager.AddControl(Player.Inventory);

        }

        public override void Initialize()
        {
            // Network
            networkManager.Connect("localhost");
            world = new World();
            // Single
            //Player = new Player();
            //Player.Spawn(650, 300);
            //Player.ClientPlayer = true;
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
                case PacketType.UPDATE_ENEMY:
                    if (isDataReadyToInit)
                    {
                        UpdateEnemyData(p);
                    }
                    break;
                case PacketType.UPDATE_ITEM:
                    if (isDataReadyToInit)
                    {
                        UpdateItems(p);
                    }
                    break;
                case PacketType.UPDATE_ENEMY_HEALTH:
                    if (isDataReadyToInit)
                    {
                        UpdateEnemyHealth(p);
                    }
                    break;
            }
        }

        public void GetAcceptData(Packet p)
        {
            PacketReader pr = new PacketReader(p);

            connectionId = pr.GetInt();
            int seed = pr.GetInt();
            float spawnX = pr.GetFloat();
            float spawnY = pr.GetFloat();
            int enemyCount = pr.GetInt();
            Rand = new Random(seed);
            world.GenerateWorld(seed);
            for (int i = 0; i < enemyCount; i++)
            {
                int id = pr.GetInt();
                float x = pr.GetFloat();
                float y = pr.GetFloat();
                int currHealth = pr.GetInt();
                Enemy enemy = new Enemy();
                enemy.id = id;
                enemy.Health = currHealth;
                Enemies.Add(id, enemy);
                enemy.Spawn(x, y);
            }
            int itemsToDestroy = pr.GetInt();
            for(int i = 0; i < itemsToDestroy; i++)
            {
                int itemId = pr.GetInt();
                foreach (var item in World.Items.Values)
                {
                    if (item.id == itemId)
                    {
                        item.IsDestroyed = true;
                    }
                }
            }

            Console.WriteLine("my connection id is: " + connectionId);
            world.Update();
            Player = new Player();
            Player.ClientPlayer = true;
            Vector2f pos = world.GetChunk(0, 0).GetTile((int)spawnX, (int)spawnY).Position;
            Player.Spawn(pos.X, pos.Y);
            Players.Add(connectionId, Player);
            isDataReadyToInit = true;
        }

        public void InitializeNewPlayer(Packet p)
        {
            PacketReader pr = new PacketReader(p);

            int connId = pr.GetInt();
            float spawnX = pr.GetFloat();
            float spawnY = pr.GetFloat();

            Player newPlayer = new Player();

            //Vector2f pos = world.GetChunk(0, 0).GetTile((int)spawnX, (int)spawnY).Position;
            newPlayer.Spawn(spawnX, spawnY);
            Players.Add(connId, newPlayer);
            Console.WriteLine("new player joined: " + connId);
        }

        public void SendPlayerUpdate()
        {
            PacketBuilder pb = new PacketBuilder(PacketType.UPDATE_MY_POS);

            pb.Add(connectionId);
            pb.Add(Player.Position.X);
            pb.Add(Player.Position.Y);
            Packet packet = pb.Build();
            networkManager.SendData(packet);
        }

        public void GetOtherPlayerData(Packet p)
        {
            PacketReader pr = new PacketReader(p);

            int id = pr.GetInt();
            int x = (int)pr.GetFloat();
            int y = (int)pr.GetFloat();
            if (Players.ContainsKey(id))
            {
                Player np = Players[id];
                np.UpdatePos(x, y);
            }
        }

        public void UpdateItems(Packet p)
        {
            PacketReader pr = new PacketReader(p);

            int id = pr.GetInt();
            //int x = (int)pr.GetFloat();
            //int y = (int)pr.GetFloat();
            Console.WriteLine("received an update on item: " + id);
            foreach (var item in World.Items.Values)
            {
                if(item.id == id)
                {
                    item.IsDestroyed = true;
                    world.Update();
                    break;
                }
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
                int currHealth = pr.GetInt();
                float chunkX = pr.GetFloat();
                float chunkY = pr.GetFloat();
                Enemies[id].Chunk = world.chunks[(int)chunkX][(int)chunkY];
                Enemies[id].Health = currHealth;
                Enemies[id].UpdatePos(x, y);
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
                if(enemy.id == id)
                {
                    enemy.Health = health;
                    return;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Network
            if (isDataReadyToInit)
            {
                if (hasFocus)
                {
                    world.Update();
                    Player.Update();
                    GameView.Center = Player.Position;
                    // Network
                    SendPlayerUpdate();
                }
            }
            //UIManager.UpdateOver();
            //UIManager.Update();
            // TODO revert debug change

            // Network
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
            //Window.Draw(Player);

            DebugRender.Draw(Window);
            //UIManager.Draw();

            // Network
            if (isDataReadyToInit)
            {
                Window.Draw(world);
                foreach (Player np in Players.Values)
                {
                    Window.Draw(np);
                }
                foreach (Enemy enemy in Enemies.Values)
                {
                    Window.Draw(enemy);
                }
            }

        }

        public static double Distance(Vector2f pos, Vector2f obj, Vector2f objOrigin)
        {
            float dX = pos.X - (obj.X + objOrigin.X);
            float dY = pos.Y - (obj.Y + objOrigin.Y);
            double distance = Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));
            return distance;
        }

    }
}
