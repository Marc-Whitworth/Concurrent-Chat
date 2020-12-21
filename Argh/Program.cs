using System;
using System.Linq.Expressions;

namespace ClientClasses
{
    class Program
    {
        ///// <summary>
        ///// The main entry point for the application.
        ///// </summary>
        //[STAThread]
        //static void Main()
        //{
        //    Application.EnableVisualStyles();
        //    Application.SetCompatibleTextRenderingDefault(false);
        //    Application.Run(new Form1());
        //}

        private const string _serverIP = "127.0.0.1";
        private const int _serverPort = 4444;
        static void Main(string[] args)
        {
            Client client = new Client();
            if (client.Connect(_serverIP, _serverPort))
            {
                Console.WriteLine("Connected...");
                try {
                    client.Run();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
                Console.WriteLine("Failed connection: " + _serverIP +":"+ + _serverPort);
            Console.Read();
        }
    }
}
