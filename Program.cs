using BoardGameFramework.ConnectFour.Game;
<<<<<<< Updated upstream
using BoardGameFramework.TicTacToe.Game;
=======
using BoardGameFramework.Gomoku.Game;
>>>>>>> Stashed changes

namespace BoardGameFramework;

class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            Console.Clear();
            PrintBanner();

            string? choice = Console.ReadLine()?.Trim();
            switch (choice)
            {
                case "1": ConnectFourLauncher.Run(); break;
                case "2": TicTacToeLauncher.Run(); break;
                case "3": NumericalTicTacToeLauncher.Run(); break;
                case "4": Console.WriteLine("\n  [Your game here] — not yet implemented"); Console.ReadLine(); break;
                case "5": GomokuLauncher.Run(); break;
                case "6":
                    Console.WriteLine("\nGoodbye!");
                    return;
                default:
                    Console.WriteLine("\nInvalid option. Press Enter to continue...");
                    Console.ReadLine();
                    break;
            }
        }
    }

    static void PrintBanner()
    {
        Console.WriteLine("╔══════════════════════════════════════╗");
        Console.WriteLine("║               BOARDGAME              ║");
        Console.WriteLine("╚══════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine("  Select a game:");
        Console.WriteLine();
        Console.WriteLine("  1. Connect Four");
        Console.WriteLine("  2. Tic-Tac-Toe");
        Console.WriteLine("  3. Numerical Tic-Tac-Toe");
        Console.WriteLine("  4. Notakto                  [not yet implemented]");
        Console.WriteLine("  5. Gomoku");
        Console.WriteLine("  6. Exit");
        Console.WriteLine();
        Console.Write("  Enter choice: ");
    }
}
