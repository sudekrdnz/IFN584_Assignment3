namespace BoardGameFramework.Core.Players;

public abstract class ComputerPlayer : Player
{
    protected ComputerPlayer(string name, int playerNumber)
        : base(name, playerNumber)
    {
        IsHuman = false;
    }
}
