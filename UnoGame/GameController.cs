namespace UnoGame;

public class GameController
{
    private List<IPlayer> PlayerList;
    private List<PlayerData> PlayerDataList;
    public GameController()
    {
        PlayerList = new List<IPlayer>();
        PlayerDataList = new List<PlayerData>();
    }
    public void AddPlayer(IPlayer player)
    {
        PlayerList.Add(player);
        PlayerDataList.Add(new PlayerData(player));
    }
    public List<IPlayer> Players
    {
        get { return PlayerList; }
    }

    public PlayerData GetPlayerData(IPlayer player)
    {
        int _playerIndex = PlayerList.IndexOf(player);
        if (_playerIndex != -1 && _playerIndex < PlayerDataList.Count)
        {
            return PlayerDataList[_playerIndex];
        }
        return null;
    }

    public void DrawCardForPlayer(IPlayer player)
    {
        ICard _drawnCard = DrawCard();
        PlayerData playerData = GetPlayerData(player);

        if (playerData != null)
        {
            playerData.AddCardToHand(_drawnCard);
        }
    }

    public ICard DrawCard()
    {
        Random random = new Random();
        CardValue randomValue = (CardValue)random.Next(Enum.GetValues(typeof(CardValue)).Length);

        if (randomValue == CardValue.Wild || randomValue == CardValue.WildDrawFour)
        {
            return new Card { CardValue = randomValue, IsWild = true };
        }
        else
        {
            CardColor randomColor = (CardColor)random.Next(Enum.GetValues(typeof(CardColor)).Length);
            return new Card { CardColor = randomColor, CardValue = randomValue };
        }
    }

    public void DealStartingHands()
    {
        foreach (IPlayer player in PlayerList)
        {
            for (int i = 0; i < 7; i++)
            {
                DrawCardForPlayer(player);
            }
        }
    }
}

