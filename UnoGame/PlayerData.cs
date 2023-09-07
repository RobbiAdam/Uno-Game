namespace UnoGame;

public class PlayerData
{
    private IPlayer _player;
    private List<ICard> _playerHandList;

    public PlayerData(IPlayer player)
    {
        _player = player;
        _playerHandList = new List<ICard>();
    }

    public List<ICard> HandCard => _playerHandList;
    

    public void AddCardToHand(ICard card)
    {
        HandCard.Add(card);
    }


    
    
}
