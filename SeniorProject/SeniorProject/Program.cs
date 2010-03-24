using System;

namespace SeniorProject
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            //Console.WriteLine("testing");
            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }
    }
}

