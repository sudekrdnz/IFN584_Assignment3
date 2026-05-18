namespace BoardGameFramework.Core.Players;

public abstract class HumanPlayer : Player
{
    protected HumanPlayer(string name, int playerNumber)
        : base(name, playerNumber)
    {
        IsHuman = true;
    }
}
