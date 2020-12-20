using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System.Net;
using System.Net.Sockets;


namespace RealtimeInteractiveCrawler
{
    class Program
    {
        static RenderWindow window = new RenderWindow(new VideoMode(800, 600), "RealtimeInteractiveCrawler");
        static Texture texture = new Texture("image.png");
        static Sprite sprite = new Sprite();
        static InputManager inputManager = new InputManager();

        static void Main(string[] args)
        {
            sprite.Texture = texture;
            window.Closed += Window_Closed;
            //Connect("localhost", "Hello");

            GameLoop();
        }

        private static void GameLoop()
        {
            Init();
            while (window.IsOpen)
            {
                window.DispatchEvents();

                Update();
                Draw();
            }
        }

        private static void Update()
        {
            Console.WriteLine("UPDATE");
            if (inputManager.getKeyDown(Keyboard.Key.W))
            {
                Console.WriteLine("W IS PRESSED");
                sprite.Position += new Vector2f(0f,-1f);
            }
            if (inputManager.getKeyDown(Keyboard.Key.A))
            {
                sprite.Position += new Vector2f(-1f, 0f);
            }
            if (inputManager.getKeyDown(Keyboard.Key.S))
            {
                sprite.Position += new Vector2f(0f, 1f);
            }
            if (inputManager.getKeyDown(Keyboard.Key.D))
            {
                sprite.Position += new Vector2f(1f, 0f);
            }
        }

        private static void Draw()
        {
            window.Clear(Color.Cyan);
            window.Draw(sprite);
            
            //drawing
            window.Display();
        }

        private static void Init()
        {
            sprite.Position = new Vector2f(100f, 100f);
            sprite.Scale = new Vector2f(0.1f, 0.1f);
        }

        private static void Window_Closed(object sender, EventArgs e)
        {
            RenderWindow win = (RenderWindow)sender;
            win.Close();
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

            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }
    }
}
