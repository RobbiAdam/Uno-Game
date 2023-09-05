namespace UnoGame;

public class PlayerData
{
    private IPlayer _player;
    public List<ICard> PlayerHandList;

    public PlayerData(IPlayer player)
    {
        _player = player;
        PlayerHandList = new List<ICard>();
    }

    public void AddCardToHand(ICard card)
    {
        PlayerHandList.Add(card);
    }
    
    
}
