using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace Tetris
{
    internal class Game
    {
        public bool figureOnField = false;
        public bool updateField = false;
        Figure figure = new Figure();
        PlayField playField = new PlayField();

        /// <summary>
        /// Start the game, draw the game board, setup the timer and the loop to check for key presses.
        /// </summary>
        public void Run()
        {
            Boolean aTimerEnabled = false;
            //Setup the timer
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 500;

            //Clear anything that's on the console.
            Console.Clear();
            Array.Copy(playField.emptyField, playField.field, 504);
            //Setup the first piece and draw the game area
            figure.RandomFigure();
            playField.DrawField(figure.figureNextType);
            //Write the options for the user to start, exit or get the about details.
            Console.WriteLine();
            Console.WriteLine("Press (A) for about, Escape (Esc) to exit or any other key to start the fun...");

            //Start the loop to check for the key presses.
            do
            {
                if (Console.KeyAvailable)
                {
                    if (aTimerEnabled) aTimer.Stop();
                    HandleKey(Console.ReadKey(true /* do not display*/).Key);
                    if (aTimerEnabled) aTimer.Start();
                }
                if (updateField)
                {
                    if (!aTimerEnabled)
                    {
                        aTimer.Start();
                        playField.initialiseScore();
                        aTimerEnabled = true;
                    }
                    
                    //A key was pressed so update the game board to reflect any changes that need to be made.
                    if (!figureOnField)
                    {
                        //Have we completed any lines, if so we can remove them.
                        playField.RemoveFullLines();
                        figureOnField = true;
                        figure.figureYCoord = 1;
                        figure.figureXCoord = 12;
                        figure.RandomFigure();
                        //Make sure the game hasn't finished.
                        if (!figure.CheckSpace(playField))
                        {
                            Array.Copy(playField.gameOver, 0, playField.field, 48, 360);
                            Console.Clear();
                            aTimer.Stop();
                            aTimerEnabled = false;
                            playField.DrawField(figure.figureNextType);

                            Array.Copy(playField.emptyField, playField.field, 504);
                            Array.Copy(figure.figureTypeEmpty, figure.figureType, 32);
                            //Setup the first piece and draw the game area
                            figure.RandomFigure();

                            Console.WriteLine();
                            Console.WriteLine("Press (A) for about, Escape (Esc) to exit or any other key to re start the fun...");
                            Console.WriteLine();
                            Console.WriteLine("Your score: " + playField.CurrentScore);

                            updateField = false;
                            continue;
                        }
                    }
                    //The game is still going so re draw the console.
                    Console.Clear();
                    playField.DrawField(figure.figureNextType);
                    Console.WriteLine();
                    Console.WriteLine("Press (A) for about or Escape (Esc) to exit...");
                    Console.WriteLine();
                    Console.WriteLine("Your score: " + playField.CurrentScore);
                    updateField = false;
                }
            }
            while (true);
        }

        /// <summary>
		/// Advances the game based on the pressed key.
		/// </summary>
		/// <param name="consoleKeyInfo">The key the user pressed.</param>
		private void HandleKey(ConsoleKey consoleKey)
        {
            switch (consoleKey)
            {
                case ConsoleKey.Escape:
                    //Call to end the game
                    EndClick();
                    break;
                case ConsoleKey.A:
                    //Call to give the about message.
                    AboutClick();
                    return;
                case ConsoleKey.LeftArrow:
                    //Move current piece to the left if there is the space
                    figure.ClearFigure(playField);
                    figure.figureXCoord -= 2;
                    if (!figure.CheckSpace(playField))
                    {
                        figure.figureXCoord += 2;
                    }
                    figure.DrawFigure(playField);
                    break;
                case ConsoleKey.RightArrow:
                    //Move current piece to the right if there is the space
                    figure.ClearFigure(playField);
                    figure.figureXCoord += 2;
                    if (!figure.CheckSpace(playField))
                    {
                        figure.figureXCoord -= 2;
                    }
                    figure.DrawFigure(playField);
                    break;
                case ConsoleKey.DownArrow:
                    //Move current piece down if there is the space
                    MovePieceDown();
                    break;
                case ConsoleKey.UpArrow:
                case ConsoleKey.Spacebar:
                    //Try and rotate the piece on the screen.
                    if (figure.RotateFigure(playField))
                    {
                        figure.DrawFigure(playField);
                    }
                    break;
            }
            updateField = true;
        }

        

        /// <summary>
        /// Shows the about message with the game controls
        /// </summary>
        private void AboutClick()
        {
            //Give the user a bit of info on how to play the game.
            string msg = "This implementation of tetris was developed in C# in Visual Studio 2019." +
                "\r\n" +
                "\r\n" +
                "Controls:" +
                "\r\n" +
                "\r\n" +
                "\u2022 Left arrow \t\t - \t Move piece left" +
                "\r\n" +
                "\u2022 Right arrow \t\t - \t Move piece right" +
                "\r\n" +
                "\u2022 Up arrow or Space bar \t - \t Rotate piece" +
                "\r\n" +
                "\u2022 Down arrow \t\t - \t Drop piece";

            MessageBox.Show(msg, "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Ends the game
        /// </summary>
        private void EndClick()
        {
            //Close the application, if automactically close the console in Tools/Debugging is not set the user will still need to close this manually.
            Environment.Exit(0);
        }

        /// <summary>
        /// Moves the current piece down one row
        /// </summary>
        private void MovePieceDown()
        {
            figure.ClearFigure(playField);
            figure.figureYCoord++;
            if (!figure.CheckSpace(playField))
            {
                figure.figureYCoord--;
                figure.DrawFigure(playField);
                figureOnField = false;
                updateField = true;
            }
            else
            {
                figure.DrawFigure(playField);
                updateField = true;
            }
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            //No key presses so check the game is still running and drop the current piece down one line if it is or prepare for the nect piece if it has reached the bottom. 
            MovePieceDown();
        }
    }
}
