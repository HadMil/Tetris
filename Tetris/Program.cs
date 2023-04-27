using System;
using System.Collections.Generic;
using System.Linq;
using Tetris;

namespace Tetris
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Game game = new Game();
            //Lets go, start the application.
            game.Run();
        }
    }
}

