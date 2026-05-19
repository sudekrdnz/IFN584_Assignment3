using BoardGameFramework.ConnectFour.Game;
using BoardGameFramework.TicTacToe.Game;
using BoardGameFramework.Gomoku.Game;
using BoardGameFramework.Notakto.Game;
using BoardGameFramework.NumericalTicTacToe.Game;

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
                case "4": NotaktoLauncher.Run();break;
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
        Console.WriteLine("  4. Notakto");
        Console.WriteLine("  5. Gomoku");
        Console.WriteLine("  6. Exit");
        Console.WriteLine();
        Console.Write("  Enter choice: ");
    }
}