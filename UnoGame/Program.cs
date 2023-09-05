using UnoGame;

class Program
{
    static int _numberOfPlayer;
    static GameController gameController = new();

    static void Main()
    {
            gameController.SetDiscardPile();

            ICard topDiscardCard = gameController.DiscardPileList.LastOrDefault();
            if (topDiscardCard != null)
            {
                Console.WriteLine("Top card in the discard pile: " + topDiscardCard.CardColor + " " + topDiscardCard.CardValue);
            }
            else
            {
                Console.WriteLine("Discard pile is empty.");
            }
        
    }

    static int InsertNumberOfPlayers()
    {
        int _numberOfPlayers = 0;
        bool validInput = false;

        while (!validInput)
        {
            Console.WriteLine("Enter the numbers of players: ");
            if (int.TryParse(Console.ReadLine(), out _numberOfPlayers) && _numberOfPlayers > 1 && _numberOfPlayers <= 4)
            {
                validInput = true;
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number between 2 and 4.");
            }
        }
        return _numberOfPlayers;
    }

    static void ShowPlayerList()
    {
        for (int playerId = 1; playerId <= _numberOfPlayer; playerId++)
        {
            Console.WriteLine($"Enter the name for Player {playerId}");
            string playerName = Console.ReadLine();

            gameController.AddPlayer(new Player(playerId, playerName));
        }
        foreach (IPlayer player in gameController.Players)
        {
            Console.WriteLine($"Player ID: {player.PlayerId}, Player Name: {player.PlayerName}");
        }
    }


    static void DisplayPlayerHands()
    {
        Console.WriteLine("Players and their hands:");
        foreach (IPlayer player in gameController.Players)
        {
            PlayerData playerData = gameController.GetPlayerData(player);
            if (playerData != null)
            {
                Console.WriteLine($"{player.PlayerName} (ID: {player.PlayerId}):");
                foreach (ICard card in playerData.PlayerHandList)
                {
                    if (card.CardValue == CardValue.Wild || card.CardValue == CardValue.WildDrawFour)
                    {
                        Console.WriteLine($"  {card.CardValue}");
                    }
                    else
                    {
                        Console.WriteLine($"  {card.CardColor} {card.CardValue}");
                    }
                }
            }
        }
    }




}
