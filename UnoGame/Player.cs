namespace UnoGame;

public class Player : IPlayer
{
    private int _playerId;
    private string _playerName;

    public int PlayerId
    {
        get { return _playerId; }
        set { _playerId = value; }
    } 

    public string PlayerName
    {
        get { return _playerName; }
        set { _playerName = value; }
    }

    public Player(int playerId, string playerName)
    {
        _playerId = playerId;
        _playerName = playerName;
    }
}
