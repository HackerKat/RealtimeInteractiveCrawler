using SFML.Graphics;
using System;
using System.Collections.Generic;
using SFML.Window;
using NetworkLib;
using SFML.System;
using System.Diagnostics;
using static RealtimeInteractiveCrawler.Item;

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

        public static Dictionary<ItemType, SimpleUI> StatusBars = new Dictionary<ItemType, SimpleUI>();

        private const uint DEFAULT_WIDTH = 1280;
        private const uint DEFAULT_HEIGHT = 720;
        private const string TITLE = "Realtime Interactive Crawler";
        private InputManager inputManager = new InputManager();
        public static NetworkManager networkManager = new NetworkManager();
        private int[] statusVals = new int[] { 100, 10, 50, 10 };

        private bool isDataReadyToInit = false;
        private bool pPressed = false;
        private int connectionId;

        public AwesomeGame() : base(DEFAULT_WIDTH, DEFAULT_HEIGHT, TITLE, Color.Black)
        {
            DebugRender.Enabled = true;

            StatusBars.Add(ItemType.HEALTH, new SimpleUI(Color.Red, new Vector2f(20, 20), "Health", new Vector2f(statusVals[0], 20)));
            StatusBars.Add(ItemType.ATTACK, new SimpleUI(Color.Yellow, new Vector2f(20, 50), "Attack", new Vector2f(statusVals[1], 20)));
            StatusBars.Add(ItemType.DEFENSE, new SimpleUI(Color.Blue, new Vector2f(20, 80), "Defense", new Vector2f(statusVals[2], 20)));
            StatusBars.Add(ItemType.ERASER, new SimpleUI(Color.Magenta, new Vector2f(20, 110), "Erase", new Vector2f(statusVals[3], 20)));
            //Rand = new Random();

            //Player.Inventory = new UIInventory();
            //UIManager.AddControl(Player.Inventory);

        }

        public override void Initialize()
        {
            world = new World();
            // Network
            networkManager.Connect("localhost");

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

        private void AssignStatusValues(Player player)
        {
            player.Health = statusVals[0];
            player.Attack = statusVals[1];
            player.Defense = statusVals[2];
            player.Erase = statusVals[3];
        }

        public void ProcessPacket(Packet p)
        {
            switch (p.PacketType)
            {
                case PacketType.PING:
                    //Console.WriteLine("pong received from server");    //ping is received from server
                    break;
                case PacketType.INIT:
                    GetAcceptData(p);
                    break;
                case PacketType.NEW_PLAYER:
                    InitializeNewPlayer(p);
                    break;
                case PacketType.UPDATE_OTHER_POS:
                    GetOtherPlayerPos(p);
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
                case PacketType.UPDATE_PLAYER_HEALTH:
                    if (isDataReadyToInit)
                    {
                        UpdatePlayerHealth(p);
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
            int health = pr.GetInt();
            int enemyCount = pr.GetInt();
            Rand = new Random(seed);
            world.GenerateWorld(seed);
            for (int i = 0; i < enemyCount; i++)
            {
                int id = pr.GetInt();
                float x = pr.GetFloat();
                float y = pr.GetFloat();
                int currHealth = pr.GetInt();
                if (currHealth > 0)
                {
                    Enemy enemy = new Enemy();
                    enemy.id = id;
                    enemy.Health = currHealth;
                    Enemies.Add(id, enemy);
                    enemy.Spawn(x, y);
                }
            }
            int itemsToDestroy = pr.GetInt();
            for (int i = 0; i < itemsToDestroy; i++)
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
            Player.Health = health;
            Debug.WriteLine("health server "+ health);
            Player.Spawn(pos.X, pos.Y);
            Players.Add(connectionId, Player);
            //Player.Rect.FillColor = CreateRandomColor(connectionId);
            AssignStatusValues(Player);
            isDataReadyToInit = true;
        }

        public void InitializeNewPlayer(Packet p)
        {
            PacketReader pr = new PacketReader(p);

            int connId = pr.GetInt();
            float spawnX = pr.GetFloat();
            float spawnY = pr.GetFloat();
            int health = pr.GetInt();

            Player newPlayer = new Player();
            newPlayer.Health = health;
            //Vector2f pos = world.GetChunk(0, 0).GetTile((int)spawnX, (int)spawnY).Position;
            newPlayer.Spawn(spawnX, spawnY);
            Players.Add(connId, newPlayer);
            //Player.Rect.FillColor = CreateRandomColor(connId);
            AssignStatusValues(newPlayer);
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

        public void GetOtherPlayerPos(Packet p)
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
            //Console.WriteLine("received an update on item: " + id);
            foreach (var item in World.Items.Values)
            {
                if (item.id == id)
                {
                    item.IsDestroyed = true;
                    world.Update();
                    break;
                }
            }
        }

        private Color CreateRandomColor(int seed)
        {
            Random rand = new Random(seed);
            return new Color((byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255));
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
                if (currHealth > 0)
                {
                    Enemies[id].Chunk = world.chunks[(int)chunkX][(int)chunkY];
                    Enemies[id].Health = currHealth;
                    Enemies[id].UpdatePos(x, y);
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

            //Console.WriteLine("Client got data about player: " + id + " health " + health);
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
                    //Console.WriteLine(Player.Health);
                    if (Player.Health <= 0)
                    {
                        Window.Close();
                    }
                }
            }

            //UIManager.UpdateOver();
            //UIManager.Update();
            // TODO revert debug change
            //return;
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


            // Single
            //Window.Draw(world);       
            //Window.Draw(Player);


            //foreach (var bar in StatusBars.Values)
            //{
            //    Window.Draw(bar);
            //}


            //UIManager.Draw();

            // Network
            if (isDataReadyToInit)
            {

                Window.Draw(world);
                DebugRender.Draw(Window); // mainly for green mouse rect
                foreach (KeyValuePair<int, Player> v in Players)
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

        public static double Distance(Vector2f pos, Vector2f obj, Vector2f objOrigin)
        {
            float dX = pos.X - (obj.X + objOrigin.X);
            float dY = pos.Y - (obj.Y + objOrigin.Y);
            double distance = Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));
            return distance;
        }

        public static double Distance(Vector2f pos, Vector2f obj)
        {
            float dX = pos.X - obj.X;
            float dY = pos.Y - obj.Y;
            double distance = Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));
            return distance;
        }

    }
}
