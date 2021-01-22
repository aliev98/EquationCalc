using System;

namespace rockpaperscissors
{
    class Program
    {
        static void Main (string[] args)
        {
            ConsoleKeyInfo cki;
            Console.TreatControlCAsInput = true;
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.White;

            int randomInt = 0, playerScore, CPUScore;
            var rnd = new Random();
            bool playing = true;

            do
            {
                 Console.Clear();
                 playerScore = 0;
                 CPUScore = 0;

                while (playerScore < 3 && CPUScore < 3)
                {
                    Console.Write("\nChoose between rock (R), paper (P) and scissors (S) ");
                    cki = Console.ReadKey(true);

                    if (cki.Key == ConsoleKey.R) { Console.Write("/ You chose Rock and");  }
                    if (cki.Key == ConsoleKey.P) { Console.Write("/ You chose Paper and"); }
                    if (cki.Key == ConsoleKey.S) { Console.Write("/ You chose scissors and"); }
                    
                    while (cki.Key != ConsoleKey.R && cki.Key != ConsoleKey.P && cki.Key != ConsoleKey.S)
                    {
                        Console.Write("\n\nInvalid input, try again ");
                        cki = Console.ReadKey(true);

                        if (cki.Key == ConsoleKey.R) { Console.Write("/ You chose Rock and"); }
                        if (cki.Key == ConsoleKey.P) { Console.Write("/ You chose Paper and");  }
                        if (cki.Key == ConsoleKey.S) { Console.Write("/ You chose scissors and");  }
                    }

                    randomInt = rnd.Next(1, 4);

                    switch (randomInt)
                    {
                        case 1:
             
                            Console.Write(" CPU chose Rock");

                            if (cki.Key == ConsoleKey.R)
                            {
                                Console.Write(" = Draw \n\n");
                            }
                            else if (cki.Key == ConsoleKey.S)
                            {
                                Console.Write(" = CPU wins \n\n");
                                CPUScore++;
                            }
                            else if (cki.Key == ConsoleKey.P)
                            {
                                Console.Write(" = Player wins \n\n");
                                playerScore++;
                            }
                            break;

                        case 2:
                           
                            Console.Write(" CPU chose Paper");

                            if (cki.Key == ConsoleKey.R)
                            {
                                Console.Write(" = CPU wins \n\n");
                                CPUScore++;
                            }

                            else if (cki.Key == ConsoleKey.S)
                            {
                                Console.Write(" = Player wins \n\n");
                                playerScore++;
                            }

                            else if (cki.Key == ConsoleKey.P)
                            {
                                Console.Write(" = Draw \n\n");
                            }
                            break;

                        case 3:

                            Console.Write(" CPU chose Scissors");

                            if (cki.Key == ConsoleKey.R)
                            {
                                Console.Write(" = Player wins \n\n");
                                playerScore++;
                            }

                            else if (cki.Key == ConsoleKey.S)
                            {
                                Console.Write(" = Draw \n\n");
                            }
                            else if (cki.Key == ConsoleKey.P)
                            {
                                Console.Write(" = CPU wins \n\n");
                                CPUScore++;
                            }
                            break;

                        default:
                            break;
                    }
                }

                    Console.ForegroundColor = ConsoleColor.Yellow;    
                    Console.WriteLine( (playerScore == 3) ? "Player won" : "CPU won" );
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\nPlay again? (y/n)");

                    cki = Console.ReadKey(true);

                    playing = (cki.Key == ConsoleKey.Y) ? true : false;
            }

            while (playing == true);
        }

    }
}