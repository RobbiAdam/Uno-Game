namespace UnoGame;

public class PlayerData
{
    private IPlayer _player;
    public List<ICard> _playerHandList;

    public PlayerData(IPlayer player)
    {
        _player = player;
        _playerHandList = new List<ICard>();
    }

    public void AddCardToHand(ICard card)
    {
        _playerHandList.Add(card);
    }
    
    
}
