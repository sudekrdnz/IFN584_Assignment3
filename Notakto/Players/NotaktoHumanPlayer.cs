using BoardGameFramework.Core.Players;

namespace BoardGameFramework.Notakto.Players;

public class NotaktoHumanPlayer : HumanPlayer
{
    public NotaktoHumanPlayer(string name, int playerNumber)
        : base(name, playerNumber) { }
}