using UnoGame;

class Program
{
    static int _numberOfPlayer;
    static GameController gameController = new();
    static bool _gameStatus = false;

    static void Main()
    {
        SetupGame();
        PlayGame();
        EndGame();
    }

    static void SetupGame()
    {
        _numberOfPlayer = InsertNumberOfPlayers();
        ShowPlayerList();
        gameController.DealStartingHands();
        gameController.InitialDiscardPile();
        DisplayPlayerHands();
        DisplayTopCardOnDiscardPile();
    }

    static void PlayGame()
    {
        while (!gameController.IsGameOver() && !_gameStatus)
        {
            TakeAction();
            Console.WriteLine("");
            gameController.NextPlayerTurn();
        }

    }

    static void EndGame()
    {
        Console.WriteLine("Game over!");
    }
    static int InsertNumberOfPlayers()
    {
        int _numberOfPlayers = 0;
        bool _validInput = false;

        while (!_validInput)
        {
            Console.WriteLine("Enter the numbers of players: ");
            if (int.TryParse(Console.ReadLine(), out _numberOfPlayers) && _numberOfPlayers > 1 && _numberOfPlayers <= 4)
            {
                _validInput = true;
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
            string _playerName;
            bool _isNameTaken;

            do
            {
                Console.WriteLine($"Enter the name for Player {playerId}");
                _playerName = Console.ReadLine();

                _isNameTaken = gameController.IsPlayerNameTaken(_playerName);

                if (_isNameTaken)
                {
                    Console.WriteLine("Player name is already taken. Please choose a different name.");
                }

            } while (_isNameTaken);

            gameController.AddPlayer(new Player(playerId, _playerName));
        }

        foreach (IPlayer player in gameController.Players)
        {
            Console.WriteLine("");
            Console.WriteLine($"Player ID: {player.PlayerId}, Player Name: {player.PlayerName}");
        }
    }

    static void DisplayPlayerHands()
    {
        Console.WriteLine("Players and their hands:");

        foreach (IPlayer player in gameController.Players)
        {
            PlayerData playerData = gameController.GetPlayerData(player);

            if (playerData == null)
            {
                Console.WriteLine("No current player data.");
                continue;
            }

            Console.WriteLine($"{player.PlayerName} (ID: {player.PlayerId}):");

            foreach (ICard card in playerData.HandCard)
            {
                string cardDescription = GetCardDescription(card);
                Console.WriteLine($"  {cardDescription}");
            }
        }
    }
    static void DisplayTopCardOnDiscardPile()
    {
        if (gameController.DiscardedPile.Count > 0)
        {
            ICard topCard = gameController.DiscardedPile.Last();
            Console.WriteLine($"Top Card on Discard Pile: {topCard.CardColor} {topCard.CardValue}");
            Console.WriteLine("");
        }
        else
        {
            Console.WriteLine("Discard pile is empty.");
        }
    }
    static void DisplayCurrentPlayerHand(string playerName, List<ICard> HandCard)
    {
        Console.WriteLine("");
        Console.WriteLine($"{playerName}'s Hand:");

        for (int i = 0; i < HandCard.Count; i++)
        {
            ICard card = HandCard[i];
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

        if (currentPlayer == null)
        {
            Console.WriteLine("No current player.");
            return;
        }

        PlayerData currentPlayerData = gameController.GetPlayerData(currentPlayer);

        if (currentPlayerData == null)
        {
            Console.WriteLine("No current player data.");
            return;
        }

        Console.WriteLine($"{currentPlayer.PlayerName}'s turn.");
        DisplayCurrentPlayerHand(currentPlayer.PlayerName, currentPlayerData.HandCard);

        while (true)
        {
            DisplayTopCardOnDiscardPile();
            Console.WriteLine($"{currentPlayer.PlayerName} Choose an action:");
            Console.WriteLine("1. Draw a card");
            Console.WriteLine("2. Discard a card");
            Console.WriteLine("3. End turn");
            Console.WriteLine("4. End the game");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1:
                        gameController.DrawCardToPlayerHand(currentPlayer);
                        Console.WriteLine($"{currentPlayer.PlayerName} drew a card.");
                        DisplayCurrentPlayerHand(currentPlayer.PlayerName, currentPlayerData.HandCard);
                        break;
                    case 2:
                        if (ChooseCardToDiscard(currentPlayer))
                        {
                            return;
                        }
                        break;
                    case 3:
                    Console.WriteLine($"{currentPlayer.PlayerName}'s ending their turn");
                        return;
                    case 4:
                        _gameStatus = true; // End Game
                        return;
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

    static bool ChooseCardToDiscard(IPlayer player)
    {
        PlayerData playerData = gameController.GetPlayerData(player);

        if (playerData == null)
        {
            Console.WriteLine("No current player data.");
            return false;
        }

        List<ICard> handCards = playerData.HandCard;

        Console.WriteLine("");
        Console.WriteLine($"{player.PlayerName}, choose a card to discard:");
        DisplayTopCardOnDiscardPile();
        

        for (int i = 0; i < handCards.Count; i++)
        {
            ICard card = handCards[i];
            string cardDescription = GetCardDescription(card);
            Console.WriteLine($"{i + 1}. {cardDescription}");
        }

        if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= handCards.Count)
        {
            ICard selectedCard = handCards[choice - 1];

            if (gameController.IsCardValidToDiscard(selectedCard))
            {
                gameController.DiscardCard(player, selectedCard, choice);
                Console.WriteLine($"{player.PlayerName} discarded: {GetCardDescription(selectedCard)}");

                if (gameController.IsActionCard(selectedCard))
                {
                    Console.WriteLine($"{player.PlayerName} discarded an action card!");
                    DisplayActionCardMessage(selectedCard, player.PlayerName);
                }

                return true;
            }
            else
            {
                Console.WriteLine("You can't discard that card.");
            }
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a valid card number.");
        }

        return false;
    }
    static void DisplayActionCardMessage(ICard card, string playerName)
    {
        switch (card.CardValue)
        {
            case CardValue.Skip:
                Console.WriteLine($"Next Player Turn is skipped.");
                break;
            case CardValue.Reverse:
                Console.WriteLine($"Player {playerName} reverses the turn order.");
                break;
            case CardValue.DrawTwo:
                Console.WriteLine($"Next Player draws two cards and their turn is skipped.");
                break;
            default:

                Console.WriteLine($"Action card played by {playerName}: {card.CardValue}");
                break;
        }
    }


}
