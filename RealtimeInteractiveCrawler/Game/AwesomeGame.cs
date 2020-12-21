using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using SFML.Window;
using SFML.System;

namespace RealtimeInteractiveCrawler
{
    class AwesomeGame : GameLoop
    {
        private const uint DEFAULT_WIDTH = 1280;
        private const uint DEFAULT_HEIGHT = 720;
        private const string TITLE = "OBAMA CARES";
        private InputManager inputManager = new InputManager();

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
            //Connect("localhost", "Hello");
            world = new World();
        }

        public override void LoadContent()
        {
            Content.Load();

            sprite = new Sprite();
            sprite.Texture = Content.TexPlay0;
        }

        public override void Update(GameTime gameTime)
        {
            if (inputManager.getKeyDown(Keyboard.Key.W) || inputManager.getKeyDown(Keyboard.Key.Up))
            {
                Console.WriteLine("W is pressed");
                sprite.Position += new Vector2f(0f, -1f) * movementSpeed * gameTime.DeltaTime;
            }
            if (inputManager.getKeyDown(Keyboard.Key.A) || inputManager.getKeyDown(Keyboard.Key.Left))
            {
                sprite.Position += new Vector2f(-1f, 0f) * movementSpeed * gameTime.DeltaTime;
            }
            if (inputManager.getKeyDown(Keyboard.Key.S) || inputManager.getKeyDown(Keyboard.Key.Down))
            {
                sprite.Position += new Vector2f(0f, 1f) * movementSpeed * gameTime.DeltaTime;
            }
            if (inputManager.getKeyDown(Keyboard.Key.D) || inputManager.getKeyDown(Keyboard.Key.Right))
            {
                sprite.Position += new Vector2f(1f, 0f) * movementSpeed * gameTime.DeltaTime;
            }
            if (inputManager.getKeyDown(Keyboard.Key.Escape))
            {
                Window.Close();
            }
        }

        static void Connect(String server, String message)
        {
            try
            {
                Int32 port = 8888;
                TcpClient client = new TcpClient(server, port);

                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer.
                stream.Write(data, 0, data.Length);

                Console.WriteLine("Sent: {0}", message);

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);

                // Close everything.
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }
    }
}
