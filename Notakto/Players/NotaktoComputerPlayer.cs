using BoardGameFramework.Core.Players;
using BoardGameFramework.Notakto.Grid;

namespace BoardGameFramework.Notakto.Players;

public class NotaktoComputerPlayer : ComputerPlayer
{
    private readonly Random _rng = new();

    public int LastBoard { get; private set; }
    public int LastRow { get; private set; }
    public int LastCol { get; private set; }

    public NotaktoComputerPlayer(string name, int playerNumber)
        : base(name, playerNumber) { }

    public void ChooseMove(NotaktoGrid grid)
    {
        var validMoves = new List<(int b, int r, int c)>();

        for (int b = 0; b < 3; b++)
        {
            if (grid.IsBoardDead(b))
                continue;

            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    if (grid.IsEmpty(b, r, c))
                        validMoves.Add((b, r, c));
                }
            }
        }

        var move = validMoves[_rng.Next(validMoves.Count)];

        LastBoard = move.b;
        LastRow = move.r;
        LastCol = move.c;
    }
}