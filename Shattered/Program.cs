using System;

namespace Shattered
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ShatteredGame game = new ShatteredGame())
            {
                game.Run();
            }
        }
    }
}

