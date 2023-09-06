using UnoGame;

class Program
{
    static int _numberOfPlayer;
    static GameController gameController = new();
    static bool _isGameOver = false;

    static void Main()
    {
        _numberOfPlayer = InsertNumberOfPlayers();
        ShowPlayerList();
        gameController.DealStartingHands();
        gameController.SetDiscardPile();
        DisplayTopCardOnDiscardPile();
        DisplayPlayerHands();

        // Game loop
        while (!_isGameOver)
        {
            TakeAction();

            if (IsGameOver())
            {
                _isGameOver = true;
                Console.WriteLine("Game over!");

            }
            gameController.NextPlayerTurn();
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

    static void DisplayTopCardOnDiscardPile()
    {
        if (gameController.DiscardPileList.Count > 0)
        {
            ICard topCard = gameController.DiscardPileList.Last();
            Console.WriteLine($"Top Card on Discard Pile: {topCard.CardColor} {topCard.CardValue}");
        }
        else
        {
            Console.WriteLine("Discard pile is empty.");
        }
    }
    static void DisplayCurrentPlayerHand(string playerName, List<ICard> playerHandList)
    {
        Console.WriteLine($"{playerName}'s Hand:");

        for (int i = 0; i < playerHandList.Count; i++)
        {
            ICard card = playerHandList[i];
            string cardDescription = GetCardDescription(card);
            Console.WriteLine($"{i + 1}. {cardDescription}");
        }
    }


    static string GetCardDescription(ICard card)
    {
        if (card.CardValue == CardValue.Wild || card.CardValue == CardValue.WildDrawFour)
        {
            return card.CardValue.ToString();
        }
        else
        {
            return $"{card.CardColor} {card.CardValue}";
        }
    }




    static void TakeAction()
    {
        IPlayer currentPlayer = gameController.GetPlayerTurn();

        if (currentPlayer != null)
        {
            PlayerData currentPlayerData = gameController.GetPlayerData(currentPlayer);

            if (currentPlayerData != null)
            {
                Console.WriteLine($"{currentPlayer.PlayerName}'s turn.");
                DisplayCurrentPlayerHand(currentPlayer.PlayerName, currentPlayerData.PlayerHandList);

                bool _continueTakingAction = true;

                while (_continueTakingAction)
                {
                    DisplayTopCardOnDiscardPile();
                    Console.WriteLine("Choose an action:");
                    Console.WriteLine("1. Draw a card");
                    Console.WriteLine("2. Discard a card");
                    Console.WriteLine("3. Skip turn");
                    Console.WriteLine("4. End the game"); //force end

                    if (int.TryParse(Console.ReadLine(), out int choice))
                    {
                        switch (choice)
                        {
                            case 1:
                                gameController.DrawCardToPlayerHand(currentPlayer);
                                Console.WriteLine($"{currentPlayer.PlayerName} drew a card.");
                                DisplayCurrentPlayerHand(currentPlayer.PlayerName, currentPlayerData.PlayerHandList);
                                break;
                            case 2:
                                if (ChooseCardToDiscard(currentPlayer))
                                {
                                    _continueTakingAction = false;
                                }
                                break;

                            case 3:
                                _continueTakingAction = false;
                                break;
                            case 4:
                                _isGameOver = true; // End Game
                                _continueTakingAction = false;
                                break;
                            default:
                                Console.WriteLine("Invalid choice. Please choose a valid action.");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Please enter a valid action number.");
                    }
                }
            }
            else
            {
                Console.WriteLine("No current player data.");
            }
        }
        else
        {
            Console.WriteLine("No current player.");
        }
    }
    static bool ChooseCardToDiscard(IPlayer player)
    {
        Console.WriteLine($"{player.PlayerName}, choose a card to discard:");
        DisplayTopCardOnDiscardPile();

        PlayerData playerData = gameController.GetPlayerData(player);
        if (playerData != null)
        {
            List<ICard> playerHandList = playerData.PlayerHandList;

            for (int i = 0; i < playerHandList.Count; i++)
            {
                ICard card = playerHandList[i];
                string cardDescription = GetCardDescription(card);
                Console.WriteLine($"{i + 1}. {cardDescription}");
            }


            Console.Write("Enter the number of the card to discard: ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= playerHandList.Count)
            {
                ICard selectedCard = playerHandList[choice - 1];

                if (gameController.IsCardValidToDiscard(selectedCard))
                {
                    gameController.DiscardCard(player, selectedCard);
                    Console.WriteLine($"{player.PlayerName} discarded: {GetCardDescription(selectedCard)}");
                    return true;
                }
                else
                {
                    Console.WriteLine("Invalid choice. You can't discard that card.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid card number.");
            }
        }
        else
        {
            Console.WriteLine("No current player data.");
        }

        return false;
    }
    static bool IsGameOver()
    {
        IPlayer currentPlayer = gameController.GetPlayerTurn();

        if (currentPlayer != null)
        {
            PlayerData currentPlayerData = gameController.GetPlayerData(currentPlayer);

            if (currentPlayerData != null && currentPlayerData.PlayerHandList.Count == 0)
            {
                return true; // If player has no card in hand
            }
        }

        return false;
    }


}